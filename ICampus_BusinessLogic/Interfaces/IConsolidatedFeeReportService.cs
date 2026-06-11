using ICampus_Models.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IConsolidatedFeeReportService
    {
        /// <summary>
        /// Get consolidated fee report data (Legacy endpoint - kept for backward compatibility)
        /// Stored Procedure: proc_feee_list_overall
        /// </summary>
        Task<IEnumerable<object>> LoadConsolidatedFeeReportAsync(ConsolidatedFeeReportRequest request);

        /// <summary>
        /// Get consolidated fee report data
        /// Stored Procedure: proc_feee_list_overall
        /// </summary>
        Task<IEnumerable<object>> GetConsolidatedFeeReportDataAsync(ConsolidatedFeeReportRequest request);

        /// <summary>
        /// Get consolidated fee report data for Excel export
        /// Stored Procedure: proc_feee_list_overall (same as data endpoint)
        /// </summary>
        Task<IEnumerable<object>> GetConsolidatedFeeReportExportDataAsync(ConsolidatedFeeReportRequest request);
    }
}

