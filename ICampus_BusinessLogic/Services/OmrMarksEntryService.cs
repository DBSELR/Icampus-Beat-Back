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
    public class OmrMarksEntryService : IOmrMarksEntryService
    {
        private readonly IGenericRepository<object> _repo;

        public OmrMarksEntryService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Load student record by OMR Number in exam context
        /// SP: PROC_OMRNUM_UPDATE_Get
        /// Parameters confirmed from DLL IL (method Get_loadOmrNumUpdate, 5 positional params):
        ///   @Regno (= OmrNumber), @Course, @Regulation, @ExamMy, @RegSup
        /// Returns rows with: BATCH, GRP, SEM, TEMPCODE, PCODE, PNAME, OMRNUMBER, TMARKS, RVMARKS, V3MARKS
        /// </summary>
        public async Task<IEnumerable<OmrMarksEntryDto>> LoadByOmrNumberAsync(OmrMarksLoadRequest request)
        {
            // Auto-detect REGSUP from tbl_sh — frontend doesn't need to pass it
            var regSup = request.RegSup?.Trim();
            if (string.IsNullOrWhiteSpace(regSup))
            {
                var detectSql = "SELECT TOP 1 REGSUP FROM tbl_sh WHERE REGNO = @R AND COURSE = @C AND REGULATION = @G AND EXAMMY = @E";
                var detectParams = new[]
                {
                    new SqlParameter("@R", SqlDbType.VarChar, 30) { Value = request.OmrNumber?.Trim() ?? string.Empty },
                    new SqlParameter("@C", SqlDbType.VarChar, 30) { Value = request.Course?.Trim()    ?? string.Empty },
                    new SqlParameter("@G", SqlDbType.VarChar, 10) { Value = request.Regulation?.Trim() ?? string.Empty },
                    new SqlParameter("@E", SqlDbType.VarChar, 20) { Value = request.ExamMY?.Trim()    ?? string.Empty }
                };
                var detected = await _repo.QueryFromStoredProcAsync(detectSql, (object[])detectParams);
                var first = detected?.FirstOrDefault() as IDictionary<string, object>;
                regSup = first?["REGSUP"]?.ToString() ?? "Reg";
            }

            var sql = StoredProcSql.Exec(StoredProcedures.PROC_OMRNUM_UPDATE_Get,
                "@Regno", "@Course", "@Regulation", "@ExamMy", "@RegSup");

            var parameters = new[]
            {
                new SqlParameter("@Regno",      SqlDbType.VarChar, 30) { Value = request.OmrNumber?.Trim()  ?? string.Empty },
                new SqlParameter("@Course",     SqlDbType.VarChar, 30) { Value = request.Course?.Trim()     ?? string.Empty },
                new SqlParameter("@Regulation", SqlDbType.VarChar, 10) { Value = request.Regulation?.Trim() ?? string.Empty },
                new SqlParameter("@ExamMy",     SqlDbType.VarChar, 20) { Value = request.ExamMY?.Trim()     ?? string.Empty },
                new SqlParameter("@RegSup",     SqlDbType.VarChar, 10) { Value = regSup }
            };

            var raw = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            if (raw == null) return Enumerable.Empty<OmrMarksEntryDto>();

            return raw.Select(row =>
            {
                // Case-insensitive — SP column casing may vary
                var ci = row is IDictionary<string, object> rd
                    ? new Dictionary<string, object>(rd, StringComparer.OrdinalIgnoreCase)
                    : new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

                string? Str(string key) => ci.TryGetValue(key, out var v) && v != null && v != DBNull.Value ? v.ToString() : null;
                long    Lng(string key) => ci.TryGetValue(key, out var v) && v != null && v != DBNull.Value ? Convert.ToInt64(v) : 0;
                int     Int(string key) => ci.TryGetValue(key, out var v) && v != null && v != DBNull.Value ? Convert.ToInt32(v) : 0;

                return new OmrMarksEntryDto
                {
                    ASHID     = Lng("ASHID"),
                    REGNO     = Str("REGNO")     ?? string.Empty,
                    SNAME     = Str("SNAME")     ?? string.Empty,
                    BATCH     = Str("BATCH")     ?? string.Empty,
                    SEM       = Str("SEM")       ?? string.Empty,
                    TEMPCODE  = Str("TEMPCODE")  ?? string.Empty,
                    PCODE     = Str("PCODE")     ?? string.Empty,
                    PNAME     = Str("PNAME")     ?? string.Empty,
                    OMRNUMBER = Str("OMRNUMBER"),
                    exammy    = Str("exammy"),
                    PTYPE     = Int("PTYPE"),     // SP returns 1 (TI) or 0 (other)
                    smarks    = Str("smarks"),
                    TMARKS    = Str("TMARKS"),
                    pmarks    = Str("pmarks"),
                    RVMARKS   = Str("RVMARKS"),
                    V3        = Str("V3MARKS"),   // SP returns "V3MARKS" — confirmed from old ASPX Eval("V3MARKS")
                    MRK_FIN   = Str("MRK_FIN")
                };
            });
        }

        /// <summary>
        /// Save marks for the given OMR number
        /// SP: SP_OMRSAVEMARKSENTRY
        /// Parameters confirmed from DLL IL (method SaveMarksEntry, 7 positional params):
        ///   @Regulation, @ExamMy, @Course, @OmrNumber (INT — no quotes in IL template), @Marks, @Type, @Sem
        /// Type: "RV" = saves to RVMARKS, "V3" = saves to V3MARKS
        /// </summary>
        public async Task<int> SaveMarksAsync(OmrMarksSaveRequest request)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.SP_OMRSAVEMARKSENTRY,
                "@Regulation", "@ExamMy", "@Course", "@OmrNumber", "@Marks", "@Type", "@Sem");

            var parameters = new[]
            {
                new SqlParameter("@Regulation", SqlDbType.VarChar, 10) { Value = request.Regulation?.Trim() ?? string.Empty },
                new SqlParameter("@ExamMy",     SqlDbType.VarChar, 20) { Value = request.ExamMY?.Trim()     ?? string.Empty },
                new SqlParameter("@Course",     SqlDbType.VarChar, 30) { Value = request.Course?.Trim()     ?? string.Empty },
                new SqlParameter("@OmrNumber",  SqlDbType.Int)         { Value = request.OmrNumber },   // INT — passed without quotes in DLL IL
                new SqlParameter("@Marks",      SqlDbType.VarChar, 10) { Value = request.Marks?.Trim()      ?? string.Empty },
                new SqlParameter("@Type",       SqlDbType.VarChar, 5)  { Value = request.Type?.Trim().ToUpper() ?? string.Empty },
                new SqlParameter("@Sem",        SqlDbType.VarChar, 5)  { Value = request.Sem?.Trim()        ?? string.Empty }
            };

            return await _repo.ExecuteStoredProcAsync(sql, (object[])parameters);
        }
    }
}
