using ICampus_BusinessLogic.Interfaces;
using ICampus_DataAccessLayer.Helpers;
using ICampus_DataAccessLayer.Interfaces;
using ICampus_Models.DTOs;
using ICampus_Models.Requests;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Services
{
    public class ExamMyUpdateService : IExamMyUpdateService
    {
        private readonly IGenericRepository<object> _repo;

        public ExamMyUpdateService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Load student grid for ExamMY Update
        /// SP: sp_loadstdexammy_update
        /// Parameters confirmed from DLL IL (BOL_StudentWiseMasterCreation, 5 positional params):
        ///   @Regulation, @Course, @Batch, @Branch, @Sem
        /// Returns: ASHID, REGULATION, REGU, COURSE, GRP, SEM, STREAM, REGNO, EXAMMY, ACT_EXAMMY
        /// </summary>
        public async Task<IEnumerable<ExamMyUpdateDto>> LoadAsync(ExamMyLoadRequest request)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.sp_loadstdexammy_update,
                "@Regulation", "@Course", "@Batch", "@Branch", "@Sem");

            var parameters = new[]
            {
                new SqlParameter("@Regulation", SqlDbType.VarChar, 10) { Value = request.Regulation ?? string.Empty },
                new SqlParameter("@Course",     SqlDbType.VarChar, 30) { Value = request.Course     ?? string.Empty },
                new SqlParameter("@Batch",      SqlDbType.VarChar, 10) { Value = request.Batch      ?? string.Empty },
                new SqlParameter("@Branch",     SqlDbType.VarChar, 20) { Value = request.Branch     ?? string.Empty },
                new SqlParameter("@Sem",        SqlDbType.VarChar, 2)  { Value = request.Sem        ?? string.Empty }
            };

            var raw = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            if (raw == null) return Enumerable.Empty<ExamMyUpdateDto>();

            return raw.Select(row =>
            {
                var ci = row is IDictionary<string, object> rd
                    ? new Dictionary<string, object>(rd, StringComparer.OrdinalIgnoreCase)
                    : new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

                string? Str(string key) => ci.TryGetValue(key, out var v) && v != null && v != DBNull.Value ? v.ToString() : null;
                long    Lng(string key) => ci.TryGetValue(key, out var v) && v != null && v != DBNull.Value ? Convert.ToInt64(v) : 0;

                return new ExamMyUpdateDto
                {
                    ASHID      = Lng("ASHID"),
                    REGULATION = Str("REGULATION") ?? string.Empty,
                    REGU       = Str("REGU")       ?? string.Empty,
                    COURSE     = Str("COURSE")     ?? string.Empty,
                    GRP        = Str("GRP")        ?? string.Empty,
                    SEM        = Str("SEM")        ?? string.Empty,
                    STREAM     = Str("STREAM")     ?? string.Empty,
                    REGNO      = Str("REGNO")      ?? string.Empty,
                    EXAMMY     = Str("EXAMMY")     ?? string.Empty,
                    ACT_EXAMMY = Str("ACT_EXAMMY")
                };
            });
        }

        /// <summary>
        /// Bulk update ExamMY for all edited rows
        /// SP: sp_Exammy_update
        /// Parameters confirmed from DLL IL (BOL_StudentWiseMasterCreation, 8 positional params):
        ///   @ACT_EXAMMY, @Regulation, @Batch, @Course, @Branch, @Sem, @REGNO, @EXAMMY
        /// SP identifies the row by REGNO + full exam context (not ASHID)
        /// Mirrors btnUpdateExammy_Click which loops over all grid rows
        /// </summary>
        public async Task<int> UpdateExamMyAsync(ExamMyUpdateRequest request)
        {
            if (request?.Rows == null || !request.Rows.Any()) return 0;

            var sql = StoredProcSql.Exec(StoredProcedures.sp_Exammy_update,
                "@ACT_EXAMMY", "@Regulation", "@Batch", "@Course", "@Branch", "@Sem", "@REGNO", "@EXAMMY");

            int total = 0;
            foreach (var row in request.Rows)
            {
                if (string.IsNullOrWhiteSpace(row.REGNO) || string.IsNullOrWhiteSpace(row.ACT_EXAMMY))
                    continue;

                var parameters = new[]
                {
                    new SqlParameter("@ACT_EXAMMY",  SqlDbType.VarChar, 20) { Value = row.ACT_EXAMMY.Trim() },
                    new SqlParameter("@Regulation",  SqlDbType.VarChar, 10) { Value = row.Regulation ?? string.Empty },
                    new SqlParameter("@Batch",       SqlDbType.VarChar, 10) { Value = row.Batch      ?? string.Empty },
                    new SqlParameter("@Course",      SqlDbType.VarChar, 30) { Value = row.Course     ?? string.Empty },
                    new SqlParameter("@Branch",      SqlDbType.VarChar, 20) { Value = row.Branch     ?? string.Empty },
                    new SqlParameter("@Sem",         SqlDbType.VarChar, 2)  { Value = row.Sem        ?? string.Empty },
                    new SqlParameter("@REGNO",       SqlDbType.VarChar, 20) { Value = row.REGNO      ?? string.Empty },
                    new SqlParameter("@EXAMMY",      SqlDbType.VarChar, 20) { Value = row.EXAMMY     ?? string.Empty }
                };

                total += await _repo.ExecuteStoredProcAsync(sql, (object[])parameters);
            }

            return total;
        }

        /// <summary>
        /// Delete a single student record by ASHID
        /// SP: PROC_DEL_ASHID
        /// Parameter: @ASHID
        /// Mirrors Delete ImageButton per grid row → GrdOmrNum_RowCommand (CommandName="Delete")
        /// </summary>
        public async Task<int> DeleteAsync(long ashid)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.PROC_DEL_ASHID, "@ASHID");
            var p = new SqlParameter("@ASHID", SqlDbType.Int) { Value = (int)ashid };
            return await _repo.ExecuteStoredProcAsync(sql, p);
        }
    }
}
