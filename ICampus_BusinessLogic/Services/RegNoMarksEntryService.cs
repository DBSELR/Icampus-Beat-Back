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
    public class RegNoMarksEntryService : IRegNoMarksEntryService
    {
        private readonly IGenericRepository<object> _repo;

        public RegNoMarksEntryService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Load student info + all paper rows for the given RegNo
        /// SP: PROC_GETSUBJTS_REGNO_WISE
        /// Parameters confirmed from DLL IL (5 positional params, method loadStdMaster):
        ///   @Course, @ExamMy, @Batch, @Sem, @Regno
        /// Returns: Name, SEM (from first row), then per-paper: aSHID, PTYPE, Exammy, sem,
        ///          PCode, TempCode, PName, smarks, TMARKS, RVMARKS, V3, MRK_FIN, pmarks
        /// </summary>
        public async Task<RegNoMarksStudentDto?> LoadByRegNoAsync(string regNo, string? examMY = null, string? batch = null, string? sem = null, string? course = null)
        {
            if (string.IsNullOrWhiteSpace(regNo)) return null;

            // Step 1: Get paper list from PROC_GETSUBJTS_REGNO_WISE (returns PCODE, PNAME, max marks etc.)
            var sql = StoredProcSql.Exec(StoredProcedures.PROC_GETSUBJTS_REGNO_WISE, "@Course", "@ExamMy", "@Batch", "@Sem", "@Regno");

            var parameters = new[]
            {
                new SqlParameter("@Course", SqlDbType.VarChar, 30) { Value = string.IsNullOrWhiteSpace(course) ? (object)DBNull.Value : course.Trim() },
                new SqlParameter("@ExamMy", SqlDbType.VarChar, 20) { Value = string.IsNullOrWhiteSpace(examMY) ? (object)DBNull.Value : examMY.Trim() },
                new SqlParameter("@Batch",  SqlDbType.VarChar, 10) { Value = string.IsNullOrWhiteSpace(batch)  ? (object)DBNull.Value : batch.Trim() },
                new SqlParameter("@Sem",    SqlDbType.VarChar, 5)  { Value = string.IsNullOrWhiteSpace(sem)    ? (object)DBNull.Value : sem.Trim() },
                new SqlParameter("@Regno",  SqlDbType.VarChar, 20) { Value = regNo.Trim().ToUpper() }
            };

            var raw = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            if (raw == null || !raw.Any()) return null;

            // Step 2: Get aSHID + actual marks from tbl_sh directly (SP does not return these)
            // tbl_sh columns confirmed: aSHID, PCODE, TEMPCODE, TMARKS, MMARKS, RVMARKS, V3, MRK_FIN
            // NOTE: PTYPE does NOT exist in tbl_sh — paper type is derived from TMAX/PMAX in the SP result
            var shSql = @"SELECT aSHID, PCODE, TEMPCODE,
                                 SMARKS, PMARKS, TMARKS, RVMARKS, V3, MRK_FIN
                          FROM tbl_sh
                          WHERE REGNO  = @REGNO
                            AND Exammy = @ExamMY
                            AND SEM    = @SEM";

            var shParams = new[]
            {
                new SqlParameter("@REGNO",  SqlDbType.VarChar, 20) { Value = regNo.Trim().ToUpper() },
                new SqlParameter("@ExamMY", SqlDbType.VarChar, 20) { Value = examMY?.Trim() ?? string.Empty },
                new SqlParameter("@SEM",    SqlDbType.Int)         { Value = int.TryParse(sem, out var semInt) ? semInt : 0 }
            };

            var shRaw = await _repo.QueryFromStoredProcAsync(shSql, (object[])shParams);

            // Build PCODE → tbl_sh row lookup (case-insensitive)
            var shLookup = new Dictionary<string, Dictionary<string, object>>(StringComparer.OrdinalIgnoreCase);
            if (shRaw != null)
            {
                foreach (var shRow in shRaw)
                {
                    if (shRow is IDictionary<string, object> shd)
                    {
                        var shci = new Dictionary<string, object>(shd, StringComparer.OrdinalIgnoreCase);
                        if (shci.TryGetValue("PCODE", out var pc) && pc != null && pc != DBNull.Value)
                            shLookup[pc.ToString()!] = shci;
                    }
                }
            }

            // Step 3: Merge SP paper list with tbl_sh marks data by PCODE
            var papers = raw.Select(row =>
            {
                var ci = row is IDictionary<string, object> raw_d
                    ? new Dictionary<string, object>(raw_d, StringComparer.OrdinalIgnoreCase)
                    : new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

                string? Str(string key) => ci.TryGetValue(key, out var v) && v != null && v != DBNull.Value ? v.ToString() : null;

                // PCODE from SP result (column appears twice in SP — same value both times)
                var pcode = Str("PCODE") ?? string.Empty;

                // Look up matching tbl_sh row for this paper
                shLookup.TryGetValue(pcode, out var sh);
                string? ShStr(string key) => sh != null && sh.TryGetValue(key, out var v) && v != null && v != DBNull.Value ? v.ToString() : null;
                long    ShLng(string key) => sh != null && sh.TryGetValue(key, out var v) && v != null && v != DBNull.Value ? Convert.ToInt64(v) : 0;

                // Derive PTYPE from TMAX/PMAX returned by SP (from TBL_PAP)
                // TMAX > 0 → theory paper (T), PMAX > 0 and TMAX = 0 → practical paper (P)
                var tmax   = Str("TMAX");
                var pmax   = Str("PMAX");
                var isTheory = !string.IsNullOrEmpty(tmax) && tmax != "0" && tmax != "0.00";
                var ptype  = isTheory ? "T" : "P";

                return new RegNoMarksPaperDto
                {
                    aSHID    = ShLng("aSHID"),
                    PTYPE    = ptype,
                    Exammy   = examMY ?? string.Empty,
                    sem      = Str("SEM") ?? sem ?? string.Empty,
                    PCode    = pcode,
                    TempCode = ShStr("TEMPCODE") ?? pcode,
                    PName    = Str("PNAME") ?? string.Empty,
                    smarks   = ShStr("SMARKS"),    // Direct column from tbl_sh
                    TMARKS   = ShStr("TMARKS"),
                    RVMARKS  = ShStr("RVMARKS"),
                    V3       = ShStr("V3"),
                    MRK_FIN  = ShStr("MRK_FIN"),
                    pmarks   = ShStr("PMARKS")     // Direct column from tbl_sh
                };
            }).ToList();

            var firstCi = raw.FirstOrDefault() is IDictionary<string, object> first_d
                ? new Dictionary<string, object>(first_d, StringComparer.OrdinalIgnoreCase)
                : new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            return new RegNoMarksStudentDto
            {
                RegNo  = regNo.Trim().ToUpper(),
                Name   = firstCi.TryGetValue("Name", out var n) && n != null && n != DBNull.Value ? n.ToString()! : string.Empty,
                Sem    = firstCi.TryGetValue("SEM",  out var s) && s != null && s != DBNull.Value ? s.ToString()! : sem ?? string.Empty,
                Papers = papers
            };
        }

        /// <summary>
        /// Save all edited mark rows in bulk — one SP call per paper row
        /// SP: PROC_marksupdate_regnowise
        /// Parameters confirmed from DLL IL (7 params, no @PTYPE):
        ///   @ASHID, @SMARKS, @PMARKS, @TMARKS, @RVMARKS, @V3, @MRK_FIN
        /// Mirrors btnOmrNo_Click which loops over all grid rows and saves each
        /// </summary>
        public async Task<int> SaveMarksAsync(RegNoMarksSaveRequest request)
        {
            if (request?.Papers == null || !request.Papers.Any()) return 0;

            // SP: PROC_marksupdate_regnowise — 7 params (no @PTYPE)
            // Order confirmed from DLL IL: AshId, Smarks, Pmarks, Tmarks, Rvmarks, v3marks, Tmrk_final
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
