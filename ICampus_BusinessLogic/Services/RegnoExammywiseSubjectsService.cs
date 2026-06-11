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
    public class RegnoExammywiseSubjectsService : IRegnoExammywiseSubjectsService
    {
        private readonly IGenericRepository<object> _repo;

        public RegnoExammywiseSubjectsService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Load subject rows by RegNo for the Regno Exammywise Subjects screen
        /// SP: PROC_OMRNUM_UPDATE
        /// Parameters confirmed from DLL IL (4 positional params, method loadOmrNumUpdate, all varchar quoted):
        ///   @Regno(1, varchar), @Course(2, varchar), @Regulation(3, varchar), @ExamMy(4, varchar)
        /// </summary>
        public async Task<IEnumerable<RegnoExammywiseSubjectRowDto>> LoadSubjectsAsync(RegnoExammywiseLoadRequest request)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.PROC_OMRNUM_UPDATE,
                "@Regno", "@Course", "@Regulation", "@ExamMy");

            var parameters = new[]
            {
                new SqlParameter("@Regno",      SqlDbType.VarChar, 20) { Value = request.RegNo      ?? string.Empty },
                new SqlParameter("@Course",     SqlDbType.VarChar, 30) { Value = request.Course     ?? string.Empty },
                new SqlParameter("@Regulation", SqlDbType.VarChar, 10) { Value = request.Regulation ?? string.Empty },
                new SqlParameter("@ExamMy",     SqlDbType.VarChar, 20) { Value = request.ExamMY     ?? string.Empty }
            };

            var raw = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            if (raw == null) return Enumerable.Empty<RegnoExammywiseSubjectRowDto>();

            return MapRows(raw);
        }

        /// <summary>
        /// Load subject rows filtered by Reg/Sup type (btnGetData_Click with ddlSemType)
        /// SP: PROC_OMRNUM_UPDATE_Get
        /// Parameters confirmed from DLL IL (5 positional params, method Get_loadOmrNumUpdate, all varchar quoted):
        ///   @Regno(1, varchar), @Course(2, varchar), @Regulation(3, varchar), @ExamMy(4, varchar), @RegSup(5, varchar)
        /// RegSup: "Reg" = Regular, "Sup" = Supplementary
        /// </summary>
        public async Task<IEnumerable<RegnoExammywiseSubjectRowDto>> LoadSubjectsByRegSupAsync(RegnoExammywiseGetLoadRequest request)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.PROC_OMRNUM_UPDATE_Get,
                "@Regno", "@Course", "@Regulation", "@ExamMy", "@RegSup");

            var parameters = new[]
            {
                new SqlParameter("@Regno",      SqlDbType.VarChar, 20) { Value = request.RegNo      ?? string.Empty },
                new SqlParameter("@Course",     SqlDbType.VarChar, 30) { Value = request.Course     ?? string.Empty },
                new SqlParameter("@Regulation", SqlDbType.VarChar, 10) { Value = request.Regulation ?? string.Empty },
                new SqlParameter("@ExamMy",     SqlDbType.VarChar, 20) { Value = request.ExamMY     ?? string.Empty },
                new SqlParameter("@RegSup",     SqlDbType.VarChar, 5)  { Value = request.RegSup     ?? string.Empty }
            };

            var raw = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            if (raw == null) return Enumerable.Empty<RegnoExammywiseSubjectRowDto>();

            return MapRows(raw);
        }

        private static IEnumerable<RegnoExammywiseSubjectRowDto> MapRows(IEnumerable<object> raw)
        {
            return raw.Select(row =>
            {
                var ci = row is IDictionary<string, object> rd
                    ? new Dictionary<string, object>(rd, StringComparer.OrdinalIgnoreCase)
                    : new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

                string? Str(string key) => ci.TryGetValue(key, out var v) && v != null && v != DBNull.Value ? v.ToString() : null;
                long    Lng(string key) => ci.TryGetValue(key, out var v) && v != null && v != DBNull.Value ? Convert.ToInt64(v) : 0;

                return new RegnoExammywiseSubjectRowDto
                {
                    aSHID    = Lng("aSHID"),
                    PTYPE    = Str("PTYPE")    ?? string.Empty,
                    Exammy   = Str("Exammy")   ?? string.Empty,
                    sem      = Str("sem")      ?? string.Empty,
                    PCode    = Str("PCode")    ?? string.Empty,
                    TempCode = Str("TempCode") ?? string.Empty,
                    PName    = Str("PName")    ?? string.Empty,
                    smarks   = Str("smarks"),
                    TMARKS   = Str("TMARKS"),
                    RVMARKS  = Str("RVMARKS"),
                    V3       = Str("V3"),
                    MRK_FIN  = Str("MRK_FIN"),
                    pmarks   = Str("pmarks")
                };
            });
        }

        /// <summary>
        /// Save marks for all grid rows — one SP call per paper row
        /// SP: PROC_marksupdate_regnowise
        /// Parameters confirmed from DLL IL (7 positional params):
        ///   @AshId(1), @Smarks(2), @Pmarks(3), @Tmarks(4), @Rvmarks(5), @v3marks(6), @Tmrk_final(7)
        /// </summary>
        public async Task<int> SaveMarksAsync(RegnoExammywiseSaveRequest request)
        {
            if (request?.Papers == null || !request.Papers.Any()) return 0;

            var sql = StoredProcSql.Exec(StoredProcedures.PROC_marksupdate_regnowise,
                "@ASHID", "@SMARKS", "@PMARKS", "@TMARKS", "@RVMARKS", "@V3", "@MRK_FIN");

            int totalUpdated = 0;

            foreach (var paper in request.Papers)
            {
                var parameters = new[]
                {
                    new SqlParameter("@ASHID",   SqlDbType.BigInt)       { Value = paper.ASHID },
                    new SqlParameter("@SMARKS",  SqlDbType.VarChar, 10)  { Value = string.IsNullOrWhiteSpace(paper.SMARKS)  ? (object)DBNull.Value : paper.SMARKS },
                    new SqlParameter("@PMARKS",  SqlDbType.VarChar, 10)  { Value = string.IsNullOrWhiteSpace(paper.PMARKS)  ? (object)DBNull.Value : paper.PMARKS },
                    new SqlParameter("@TMARKS",  SqlDbType.VarChar, 10)  { Value = string.IsNullOrWhiteSpace(paper.TMARKS)  ? (object)DBNull.Value : paper.TMARKS },
                    new SqlParameter("@RVMARKS", SqlDbType.VarChar, 10)  { Value = string.IsNullOrWhiteSpace(paper.RVMARKS) ? (object)DBNull.Value : paper.RVMARKS },
                    new SqlParameter("@V3",      SqlDbType.VarChar, 10)  { Value = string.IsNullOrWhiteSpace(paper.V3)      ? (object)DBNull.Value : paper.V3 },
                    new SqlParameter("@MRK_FIN", SqlDbType.VarChar, 10)  { Value = string.IsNullOrWhiteSpace(paper.MRK_FIN) ? (object)DBNull.Value : paper.MRK_FIN }
                };

                totalUpdated += await _repo.ExecuteStoredProcAsync(sql, (object[])parameters);
            }

            return totalUpdated;
        }
    }
}
