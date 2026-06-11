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
    public class PracticalMarksService : IPracticalMarksService
    {
        private readonly IGenericRepository<object> _repo;

        public PracticalMarksService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Load papers dropdown for Practical Marks Entry
        /// SP: PROC_LOADPAPERS_MRKENTRY
        /// Parameters: @Regulation, @EXAMMY, @Sem, @Course, @GRP, @TYPE='P'
        /// Returns: PCODE, PName
        /// Only returns papers where PMAX > 0 (has practical marks)
        /// </summary>
        public async Task<IEnumerable<PracticalMarksPaperDto>> LoadPapersAsync(PracticalMarksPapersRequest request)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.PROC_LOADPAPERS_MRKENTRY,
                "@Regulation", "@EXAMMY", "@Sem", "@Course", "@GRP", "@TYPE");

            var parameters = new[]
            {
                new SqlParameter("@Regulation", SqlDbType.VarChar, 10)  { Value = request.Regulation ?? string.Empty },
                new SqlParameter("@EXAMMY",     SqlDbType.VarChar, 20)  { Value = request.ExamMY     ?? string.Empty },
                new SqlParameter("@Sem",        SqlDbType.VarChar, 2)   { Value = request.Sem        ?? string.Empty },
                new SqlParameter("@Course",     SqlDbType.VarChar, 30)  { Value = request.Course     ?? string.Empty },
                new SqlParameter("@GRP",        SqlDbType.VarChar, 30)  { Value = request.GRP        ?? string.Empty },
                new SqlParameter("@TYPE",       SqlDbType.Char,    1)   { Value = "P" }   // P = Practical (PMAX > 0)
            };

            var raw = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            if (raw == null) return Enumerable.Empty<PracticalMarksPaperDto>();

            return raw.Select(row =>
            {
                var d = row as IDictionary<string, object>;
                return new PracticalMarksPaperDto
                {
                    PCODE = d?["PCODE"]?.ToString() ?? string.Empty,
                    PName = d?["PName"]?.ToString() ?? string.Empty
                };
            });
        }

        /// <summary>
        /// Load student marks grid for Practical Marks Entry
        /// SP: PROC_LOADMARKS_MRKENTRY
        /// Parameters: @Regulation, @EXAMMY, @Sem, @Course, @GRP, @PCode, @TYPE='P'
        /// Returns: aSHID, RegNo, grp (GRP), PCODE, PMARKS, pMax
        /// </summary>
        public async Task<IEnumerable<PracticalMarksStudentDto>> LoadStudentsAsync(PracticalMarksStudentsRequest request)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.PROC_LOADMARKS_MRKENTRY,
                "@Regulation", "@EXAMMY", "@Sem", "@Course", "@GRP", "@PCode", "@TYPE");

            var parameters = new[]
            {
                new SqlParameter("@Regulation", SqlDbType.VarChar, 10)  { Value = request.Regulation ?? string.Empty },
                new SqlParameter("@EXAMMY",     SqlDbType.VarChar, 20)  { Value = request.ExamMY     ?? string.Empty },
                new SqlParameter("@Sem",        SqlDbType.Int)          { Value = int.TryParse(request.Sem, out var s) ? s : (object)DBNull.Value },
                new SqlParameter("@Course",     SqlDbType.VarChar, 30)  { Value = request.Course     ?? string.Empty },
                new SqlParameter("@GRP",        SqlDbType.VarChar, 20)  { Value = string.IsNullOrWhiteSpace(request.GRP)   ? (object)DBNull.Value : request.GRP },
                new SqlParameter("@PCode",      SqlDbType.VarChar, 20)  { Value = string.IsNullOrWhiteSpace(request.PCode) ? (object)DBNull.Value : request.PCode },
                new SqlParameter("@TYPE",       SqlDbType.Char,    1)   { Value = "P" }   // P = Practical (PMARKS + pMax)
            };

            var raw = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            if (raw == null) return Enumerable.Empty<PracticalMarksStudentDto>();

            return raw.Select(row =>
            {
                var d = row as IDictionary<string, object>;
                return new PracticalMarksStudentDto
                {
                    aSHID  = d?["aSHID"]  != null && d["aSHID"]  != DBNull.Value ? Convert.ToInt64(d["aSHID"])  : 0,
                    RegNo  = d?["RegNo"]?.ToString()  ?? string.Empty,
                    GRP    = d?["grp"]?.ToString()    ?? string.Empty,
                    PCODE  = d?["PCODE"]?.ToString()  ?? string.Empty,
                    PMARKS = d?["PMARKS"] != null && d["PMARKS"] != DBNull.Value ? (int?)Convert.ToInt32(d["PMARKS"]) : null,
                    pMax   = d?["pMax"]   != null && d["pMax"]   != DBNull.Value ? Convert.ToInt32(d["pMax"])   : 0
                };
            });
        }

        /// <summary>
        /// Save a single student's practical mark
        /// SP: PROC_UPDATE_MARKS_INT_S_T
        /// Parameters: @MARKS, @ASHID, @TYPE='P'
        /// TYPE='P' updates tbl_SH.PMARKS
        /// Marks value: numeric string (e.g. "18") or "AB" (Absent)
        /// </summary>
        public async Task<int> SaveMarkAsync(PracticalMarksSaveRequest request)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.PROC_UPDATE_MARKS_INT_S_T,
                "@MARKS", "@ASHID", "@TYPE");

            var parameters = new[]
            {
                new SqlParameter("@MARKS", SqlDbType.VarChar, 10) { Value = request.Marks ?? string.Empty },
                new SqlParameter("@ASHID", SqlDbType.BigInt)      { Value = request.ASHID },
                new SqlParameter("@TYPE",  SqlDbType.Char,    1)  { Value = "P" }   // P = PMARKS (practical)
            };

            return await _repo.ExecuteStoredProcAsync(sql, (object[])parameters);
        }
    }
}
