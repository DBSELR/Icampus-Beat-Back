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
    public class ConsolidatedFeeReportService : IConsolidatedFeeReportService
    {
        private readonly IGenericRepository<object> _repo;

        public ConsolidatedFeeReportService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Get consolidated fee report data (Legacy method - kept for backward compatibility)
        /// Stored Procedure: proc_feee_list_overall
        /// Parameters: @Regulation, @Course, @ExamMy, @fDate, @tDate
        /// Date Format: Converts dd-MM-yyyy to yyyy-MM-dd for database
        /// </summary>
        public async Task<IEnumerable<object>> LoadConsolidatedFeeReportAsync(ConsolidatedFeeReportRequest request)
        {
            // Call the new method to maintain consistency
            return await GetConsolidatedFeeReportDataAsync(request);
        }

        /// <summary>
        /// Get consolidated fee report data
        /// Stored Procedure: proc_feee_list_overall
        /// Parameters: @Regulation, @Course, @ExamMy, @fDate, @tDate
        /// Date Format: Converts dd-MM-yyyy to yyyy-MM-dd for database
        /// </summary>
        public async Task<IEnumerable<object>> GetConsolidatedFeeReportDataAsync(ConsolidatedFeeReportRequest request)
        {
            // Convert date format from dd-MM-yyyy (UI) to yyyy-MM-dd (database)
            string fDateFormatted = FormatDateForSP(request.FDate);
            string tDateFormatted = FormatDateForSP(request.TDate);

            var parameters = new[]
            {
                new SqlParameter("@regulation", SqlDbType.VarChar, 20) { Value = request.Regulation ?? string.Empty },
                new SqlParameter("@course", SqlDbType.VarChar, 20) { Value = request.Course ?? string.Empty },
                new SqlParameter("@exammy", SqlDbType.VarChar, 20) { Value = request.ExamMy ?? string.Empty },
                new SqlParameter("@Fdate", SqlDbType.VarChar, 20) { Value = fDateFormatted },
                new SqlParameter("@Tdate", SqlDbType.VarChar, 20) { Value = tDateFormatted }
            };

            var sql = StoredProcSql.Exec(StoredProcedures.PROC_FEEE_LIST_OVERALL, "@regulation", "@course", "@exammy", "@Fdate", "@Tdate");
            var raw = await _repo.QueryFromStoredProcAsync(sql, parameters);
            return raw ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Get consolidated fee report data for Excel export
        /// Stored Procedure: proc_feee_list_overall (same as data endpoint)
        /// Parameters: @Regulation, @Course, @ExamMy, @fDate, @tDate
        /// Date Format: Converts dd-MM-yyyy to yyyy-MM-dd for database
        /// </summary>
        public async Task<IEnumerable<object>> GetConsolidatedFeeReportExportDataAsync(ConsolidatedFeeReportRequest request)
        {
            // Same as GetConsolidatedFeeReportDataAsync - returns data for export
            return await GetConsolidatedFeeReportDataAsync(request);
        }

        /// <summary>
        /// Format date for stored procedure (convert to yyyy-MM-dd format)
        /// Accepts: dd-MM-yyyy, yyyy-MM-dd, or other formats
        /// Returns: yyyy-MM-dd format string (as expected by SP)
        /// </summary>
        private string FormatDateForSP(string dateStr)
        {
            if (string.IsNullOrWhiteSpace(dateStr))
                return string.Empty;

            // Try parsing the date
            if (DateTime.TryParse(dateStr, out var dateValue))
            {
                // Return in yyyy-MM-dd format (as expected by SP)
                return dateValue.ToString("yyyy-MM-dd");
            }

            // If parsing fails, return as-is (might already be in correct format)
            return dateStr;
        }
    }
}

