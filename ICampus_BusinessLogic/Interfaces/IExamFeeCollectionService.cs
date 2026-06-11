using ICampus_Models.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IExamFeeCollectionService
    {
        /// <summary>
        /// Get exam fee collection data for report display
        /// Stored Procedure: SPM_EXAMFEE_COLLECTION
        /// </summary>
        Task<IEnumerable<object>> GetExamFeeCollectionDataAsync(ExamFeeCollectionRequest request);

        /// <summary>
        /// Get exam fee collection data for Excel export
        /// Stored Procedure: proc_feee_list_overall
        /// </summary>
        Task<IEnumerable<object>> GetExamFeeCollectionExportDataAsync(ExamFeeCollectionRequest request);

        // Reports/ExamFeeCollection.aspx (Sem + Branch based — different from Pre-Exams date-range version)

        Task<IEnumerable<object>> LoadReportSemsAsync(string course, string examMY);
        Task<IEnumerable<object>> LoadReportBranchesAsync(string course);
        Task<IEnumerable<object>> GetReportDataAsync(
            string course, string examMY, string regu, string sem, string branch);
    }
}

