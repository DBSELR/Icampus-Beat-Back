using ICampus_Models.DTOs;
using ICampus_Models.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IInternalMarksService
    {
        /// <summary>
        /// Load papers dropdown for Internal Marks Entry screen
        /// Calls PROC_LOADPAPERS_MRKENTRY with TYPE='I' (SMAX > 0)
        /// </summary>
        Task<IEnumerable<InternalMarksPaperDto>> LoadPapersAsync(InternalMarksPapersRequest request);

        /// <summary>
        /// Load student marks grid for the selected paper
        /// Calls PROC_LOADMARKS_MRKENTRY with TYPE='I'
        /// Returns: aSHID, RegNo, PCODE, SMARKS, SMAX, GRP
        /// </summary>
        Task<IEnumerable<InternalMarksStudentDto>> LoadStudentsAsync(InternalMarksStudentsRequest request);

        /// <summary>
        /// Save a single student's internal mark (SMARKS in tbl_SH)
        /// Calls PROC_UPDATE_MARKS_INT_S_T with TYPE='S'
        /// Marks can be numeric (0 to SMAX) or "AB" (Absent)
        /// </summary>
        Task<int> SaveMarkAsync(InternalMarksSaveRequest request);
    }
}
