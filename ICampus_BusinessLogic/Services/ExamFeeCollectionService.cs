using ICampus_BusinessLogic.Interfaces;
using ICampus_DataAccessLayer.Helpers;
using ICampus_DataAccessLayer.Interfaces;
using ICampus_Models.Requests;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Services
{
    public class ExamFeeCollectionService : IExamFeeCollectionService
    {
        private readonly IGenericRepository<object> _repo;

        public ExamFeeCollectionService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Get exam fee collection data for report display
        /// Stored Procedure: SPM_EXAMFEE_COLLECTION
        /// Parameters: @ExamMy, @Course, @fDate, @tDate, @UserID (optional)
        /// If UserID is empty: call without @UserID parameter
        /// If UserID is provided: include @UserID parameter
        /// </summary>
        public async Task<IEnumerable<object>> GetExamFeeCollectionDataAsync(ExamFeeCollectionRequest request)
        {
            // Convert date format if needed (accept dd-MM-yyyy or yyyy-MM-dd, convert to dd-MM-yyyy for SP)
            string fDateFormatted = FormatDateForSP(request.FDate);
            string tDateFormatted = FormatDateForSP(request.TDate);

            // Build parameters list - UserID is optional
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@ExamMy", SqlDbType.VarChar) { Value = request.ExamMY ?? string.Empty },
                new SqlParameter("@Course", SqlDbType.VarChar) { Value = request.Course ?? string.Empty },
                new SqlParameter("@fDate", SqlDbType.VarChar) { Value = fDateFormatted },
                new SqlParameter("@tDate", SqlDbType.VarChar) { Value = tDateFormatted }
            };

            var paramNames = new List<string> { "@ExamMy", "@Course", "@fDate", "@tDate" };

            // Add UserID parameter only if provided
            if (!string.IsNullOrWhiteSpace(request.UserID))
            {
                parameters.Add(new SqlParameter("@UserID", SqlDbType.VarChar) { Value = request.UserID });
                paramNames.Add("@UserID");
            }

            var sql = StoredProcSql.Exec(StoredProcedures.SPM_EXAMFEE_COLLECTION, paramNames.ToArray());
            var raw = await _repo.QueryFromStoredProcAsync(sql, parameters.ToArray());
            return raw ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Get exam fee collection data for Excel export
        /// Stored Procedure: proc_feee_list_overall
        /// Parameters: @Regulation, @Course, @ExamMy, @fDate, @tDate
        /// </summary>
        public async Task<IEnumerable<object>> GetExamFeeCollectionExportDataAsync(ExamFeeCollectionRequest request)
        {
            // Convert date format if needed (accept dd-MM-yyyy or yyyy-MM-dd, convert to dd-MM-yyyy for SP)
            string fDateFormatted = FormatDateForSP(request.FDate);
            string tDateFormatted = FormatDateForSP(request.TDate);

            var parameters = new[]
            {
                new SqlParameter("@regulation", SqlDbType.VarChar, 20) { Value = request.Regulation ?? string.Empty },
                new SqlParameter("@course", SqlDbType.VarChar, 20) { Value = request.Course ?? string.Empty },
                new SqlParameter("@exammy", SqlDbType.VarChar, 20) { Value = request.ExamMY ?? string.Empty },
                new SqlParameter("@Fdate", SqlDbType.VarChar, 20) { Value = fDateFormatted },
                new SqlParameter("@Tdate", SqlDbType.VarChar, 20) { Value = tDateFormatted }
            };

            var sql = StoredProcSql.Exec(StoredProcedures.PROC_FEEE_LIST_OVERALL, "@regulation", "@course", "@exammy", "@Fdate", "@Tdate");
            var raw = await _repo.QueryFromStoredProcAsync(sql, parameters);
            return raw ?? Enumerable.Empty<object>();
        }

        // =====================================================================
        // Reports/ExamFeeCollection.aspx — Sem + Branch based
        // =====================================================================

        /// <summary>
        /// Load Semester dropdown (Page_Load — Reports/ExamFeeCollection.aspx)
        /// SQL: SELECT DISTINCT cast(SEM as varchar(250)) SEM FROM tbl_sh
        ///      WHERE COURSE=@Course AND EXAMMY=@ExamMY ORDER BY SEM
        /// Confirmed: DataAccessLayer.dll UTF-16LE US heap pattern (same as other Sem-load SQLs)
        /// </summary>
        public async Task<IEnumerable<object>> LoadReportSemsAsync(string course, string examMY)
        {
            var sql = "SELECT DISTINCT cast(SEM as varchar(250)) SEM FROM tbl_sh" +
                      " WHERE COURSE=@Course AND EXAMMY=@ExamMY ORDER BY SEM";

            var parameters = new[]
            {
                new SqlParameter("@Course", SqlDbType.VarChar, 30) { Value = course ?? string.Empty },
                new SqlParameter("@ExamMY", SqlDbType.VarChar, 20) { Value = examMY ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, parameters.Cast<object>().ToArray());
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Load Branch dropdown (Page_Load — Reports/ExamFeeCollection.aspx)
        /// SQL: SELECT DISTINCT GRP FROM TBL_COURSE WHERE COURSE=@Course ORDER BY GRP
        /// Note: Branch validation is commented out in ASPX — branch filter is optional (@Branch='')
        /// </summary>
        public async Task<IEnumerable<object>> LoadReportBranchesAsync(string course)
        {
            var sql = "SELECT DISTINCT GRP FROM TBL_COURSE WHERE COURSE=@Course ORDER BY GRP";

            var parameters = new[]
            {
                new SqlParameter("@Course", SqlDbType.VarChar, 30) { Value = course ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, parameters.Cast<object>().ToArray());
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Get Exam Fee Collection Report data — Reports/ExamFeeCollection.aspx (btnview_Click)
        /// SP: SPR_ExamFee_Collection  params: @Course, @ExamMY, @Regu, @Sem, @Branch
        /// @Branch: empty string = all branches (validation commented out in ASPX)
        /// Note: Distinct from Pre-Exams/ExamFeeCollection.aspx (date-range, SPM_EXAMFEE_COLLECTION)
        /// Confirmed: DataAccessLayer.dll UTF-16LE US heap offset 162019
        /// </summary>
        public async Task<IEnumerable<object>> GetReportDataAsync(
            string course, string examMY, string regu, string sem, string branch)
        {
            // SP param order: @COURSE, @BRANCH, @SEM INT, @EXAMMY (no @Regu param in SP)
            var sql = StoredProcSql.Exec(StoredProcedures.SPR_ExamFee_Collection,
                "@Course", "@Branch", "@Sem", "@ExamMY");

            var parameters = new object[]
            {
                new SqlParameter("@Course", SqlDbType.VarChar, 10) { Value = course ?? string.Empty },
                new SqlParameter("@Branch", SqlDbType.VarChar, 50) { Value = string.IsNullOrWhiteSpace(branch) ? (object)DBNull.Value : branch },
                new SqlParameter("@Sem",    SqlDbType.Int)         { Value = int.TryParse(sem, out var s) ? s : (object)DBNull.Value },
                new SqlParameter("@ExamMY", SqlDbType.VarChar, 15) { Value = examMY ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, parameters.Cast<object>().ToArray());
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Format date for stored procedure (convert to dd-MM-yyyy format)
        /// Accepts: dd-MM-yyyy, yyyy-MM-dd, or other formats
        /// Returns: dd-MM-yyyy format string
        /// </summary>
        private string FormatDateForSP(string dateStr)
        {
            if (string.IsNullOrWhiteSpace(dateStr))
                return string.Empty;

            // Try parsing the date
            if (DateTime.TryParse(dateStr, out var dateValue))
            {
                // Return in dd-MM-yyyy format (as expected by SP)
                return dateValue.ToString("dd-MM-yyyy");
            }

            // If parsing fails, return as-is (might already be in correct format)
            return dateStr;
        }
    }
}

