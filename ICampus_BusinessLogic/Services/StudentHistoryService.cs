using ICampus_BusinessLogic.Interfaces;
using ICampus_DataAccessLayer.Helpers;
using ICampus_DataAccessLayer.Interfaces;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Services
{
    public class StudentHistoryService : IStudentHistoryService
    {
        private readonly IGenericRepository<object> _repo;

        public StudentHistoryService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Load student personal details (Name, Programme/Course, Branch)
        /// SP: SPM_STUDENT_DETAILS
        /// params: @RegNo (varchar)
        /// Confirmed from: DataAccessLayer.dll UTF-16LE "SPM_STUDENT_DETAILS '"
        ///   BAL method: GetStudetails / displaystdpersonaldata
        ///   UI: txtStudentName, txtCourse, txtGRP (read-only)
        /// </summary>
        public async Task<IEnumerable<object>> LoadStudentDetailsAsync(string regNo)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.SPM_STUDENT_DETAILS, "@RegNo");

            var parameters = new[]
            {
                new SqlParameter("@RegNo", SqlDbType.VarChar, 20) { Value = regNo ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Load the full subject-wise history grid for a student (all sems)
        /// SPM_STUDENTHISTORY needs @REGNO + @SEM (no default) — use inline SQL to get all sems
        /// </summary>
        public async Task<IEnumerable<object>> LoadHistoryAsync(string regNo)
        {
            var sql = "SELECT ASHID, REGNO, SEM, PNO, PCODE, TEMPCODE, PNAME, CR, MMARKS, " +
                      "dbo.val(MRK_FIN) + dbo.val(PMarks) AS MRK_FIN, dbo.val(SMARKS) AS SMARKS, " +
                      "PMARKS, MARKS, PAPRES, REGSUP, " +
                      "CASE WHEN RVMARKS IS NULL THEN 'NA' ELSE CAST(RVMARKS AS VARCHAR) END AS RVMARKS, " +
                      "GR, GRPTS, EXAMMY " +
                      "FROM TBL_SH WHERE REGNO = @REGNO ORDER BY SEM, PNO, ASHID";

            var parameters = new[]
            {
                new SqlParameter("@REGNO", SqlDbType.VarChar, 15) { Value = regNo ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Load SGPA/CGPA per-semester summary for the student
        /// SP: SP_STUDENT_SGPA_CGPA — params: @REGNO only
        /// Returns: SEM, TCR, SCR, REGNO, SGPA, CGPA, RNO, BACKLOGS
        /// </summary>
        public async Task<IEnumerable<object>> LoadSgpaCgpaAsync(string regNo)
        {
            var sql = "EXEC dbo.SP_STUDENT_SGPA_CGPA @REGNO";

            var parameters = new[]
            {
                new SqlParameter("@REGNO", SqlDbType.VarChar, 15) { Value = regNo ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Load a single TBL_SH record for the edit modal (by ASHID)
        /// Raw SQL: SELECT PCODE, PNAME, TMARKS, MMARKS, RVMARKS, V3, MRK_FIN, SMARKS, PMARKS
        ///            FROM TBL_SH WHERE ASHID = @ASHID
        /// Confirmed from: DataAccessLayer.dll UTF-16LE offset 83386
        ///   BAL method: getStudentMarks → populates modal (txtPCode, txtPName, txtSMARKS,
        ///     txtTMarks, txtMMarks, txtRVMarks, TXTV3, TXTMRK_FIN, txtPMarks)
        /// </summary>
        public async Task<IEnumerable<object>> GetMarksByAshIdAsync(string ashId)
        {
            var sql = "SELECT PCODE, PNAME, TMARKS, MMARKS, RVMARKS, V3, MRK_FIN, SMARKS, PMARKS FROM TBL_SH WHERE ASHID = @ASHID";

            var parameters = new[]
            {
                new SqlParameter("@ASHID", SqlDbType.VarChar, 20) { Value = ashId ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Update marks for a TBL_SH record (SetStudentMarks, btnSave_Click)
        /// Raw SQL: UPDATE TBL_SH SET PNAME=@PName, SMARKS=@SMarks, TMARKS=@TMarks,
        ///            MMARKS=@MMarks, RVMARKS=@RVMarks,
        ///            V3 = CASE WHEN IS_V3='Y' THEN @V3 ELSE NULL END, PMARKS=@PMarks
        ///          WHERE ASHID = @ASHID
        /// Confirmed from: DataAccessLayer.dll UTF-16LE fragment (offset 83437)
        ///   "UPDATE TBL_SH SET PNAME = '?' ... V3 = CASE WHEN IS_V3 = 'Y' THEN '?' ELSE NULL END
        ///    ... SMARKS = '?' WHERE ASHID = ?"
        ///   BAL method: SetStudentMarks
        ///   UI: btnSave_Click in modal (txtPName, txtSMARKS, txtTMarks, txtMMarks,
        ///       txtRVMarks, TXTV3, txtPMarks)
        /// Note: MRK_FIN (Final SEE) is NOT updated here — it is computed by Result Process.
        ///       V3 is only set if IS_V3='Y' on the existing TBL_SH record (column flag).
        /// </summary>
        public async Task<int> UpdateMarksAsync(string ashId, string pName, string sMarks, string tMarks,
            string mMarks, string rvMarks, string v3, string pMarks)
        {
            var sql = @"UPDATE TBL_SH SET
                PNAME   = @PName,
                SMARKS  = @SMarks,
                TMARKS  = @TMarks,
                MMARKS  = @MMarks,
                RVMARKS = @RVMarks,
                V3      = @V3,
                PMARKS  = @PMarks
            WHERE ASHID = @ASHID";

            var parameters = new[]
            {
                new SqlParameter("@ASHID",   SqlDbType.VarChar, 20)  { Value = ashId   ?? string.Empty },
                new SqlParameter("@PName",   SqlDbType.VarChar, 100) { Value = pName   ?? string.Empty },
                new SqlParameter("@SMarks",  SqlDbType.VarChar, 10)  { Value = sMarks  ?? string.Empty },
                new SqlParameter("@TMarks",  SqlDbType.VarChar, 10)  { Value = tMarks  ?? string.Empty },
                new SqlParameter("@MMarks",  SqlDbType.VarChar, 10)  { Value = mMarks  ?? string.Empty },
                new SqlParameter("@RVMarks", SqlDbType.VarChar, 10)  { Value = rvMarks ?? string.Empty },
                new SqlParameter("@V3",      SqlDbType.VarChar, 10)  { Value = v3      ?? string.Empty },
                new SqlParameter("@PMarks",  SqlDbType.VarChar, 10)  { Value = pMarks  ?? string.Empty }
            };

            return await _repo.ExecuteStoredProcAsync(sql, parameters);
        }

        /// <summary>
        /// Delete a TBL_SH record by ASHID
        /// SP: PROC_DEL_ASHID
        /// params: @ASHID (varchar)
        /// Confirmed from: DataAccessLayer.dll UTF-16LE "PROC_DEL_ASHID '"
        ///   BAL method: Delete_ashid
        ///   UI: Delete button per row in dgvStudentHistory (RowDeleting event)
        /// </summary>
        public async Task<int> DeleteByAshIdAsync(string ashId)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.PROC_DEL_ASHID, "@ASHID");

            var parameters = new[]
            {
                new SqlParameter("@ASHID", SqlDbType.VarChar, 20) { Value = ashId ?? string.Empty }
            };

            return await _repo.ExecuteStoredProcAsync(sql, parameters);
        }

        /// <summary>
        /// Get the latest ExamMY for a student
        /// SP: SPM_Student_MaxExamMY
        /// params: @RegNo (varchar)
        /// Returns: MaxExamMY (latest exam period string, e.g. "NOV2023")
        /// Confirmed from: DataAccessLayer.dll UTF-16LE "SPM_Student_MaxExamMY '"
        ///   BAL method: getStudentMaxExamMY (called after SetStudentMarks → then ResultProcess)
        ///   UI: btnResultProcess auto-trigger after marks save
        /// </summary>
        public async Task<string> GetMaxExamMyAsync(string regNo)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.SPM_Student_MaxExamMY, "@RegNo");

            var parameters = new[]
            {
                new SqlParameter("@RegNo", SqlDbType.VarChar, 20) { Value = regNo ?? string.Empty }
            };

            var raw = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            try
            {
                var first = raw?.Cast<System.Collections.Generic.IDictionary<string, object>>().FirstOrDefault();
                if (first != null && first.Values.Any())
                    return first.Values.First()?.ToString() ?? "";
            }
            catch { }
            return "";
        }
    }
}
