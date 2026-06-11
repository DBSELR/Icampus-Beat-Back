using ICampus_Models.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface ICancelReceiptListService
    {
        /// <summary>
        /// Get list of courses for dropdown
        /// Stored Procedure: SPM_COURSE_LIST
        /// </summary>
        Task<IEnumerable<object>> GetCoursesAsync(string regulation);

        /// <summary>
        /// Get list of exam month-years for dropdown
        /// Stored Procedure: SPM_EXAMS_ExamMY_Load
        /// </summary>
        Task<IEnumerable<object>> GetExamMYsAsync(string regulation, string course);

        /// <summary>
        /// Get cancel receipt list data
        /// Stored Procedure: SP_Cancel_Receipt
        /// </summary>
        Task<IEnumerable<object>> GetCancelReceiptListDataAsync(CancelReceiptListRequest request);

        /// <summary>
        /// Get cancel receipt list data for Excel export
        /// Stored Procedure: SP_Cancel_Receipt (same as data endpoint)
        /// </summary>
        Task<IEnumerable<object>> GetCancelReceiptListExportDataAsync(CancelReceiptListRequest request);
    }
}

