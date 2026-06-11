using ICampus_Models.DTOs;
using ICampus_Models.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IPracticalMarksService
    {
        /// <summary>
        /// Load papers dropdown for Practical Marks Entry screen
        /// Calls PROC_LOADPAPERS_MRKENTRY with TYPE='P' (PMAX > 0)
        /// </summary>
        Task<IEnumerable<PracticalMarksPaperDto>> LoadPapersAsync(PracticalMarksPapersRequest request);

        /// <summary>
        /// Load student marks grid for the selected paper
        /// Calls PROC_LOADMARKS_MRKENTRY with TYPE='P'
        /// Returns: aSHID, RegNo, GRP, PCODE, PMARKS, pMax
        /// </summary>
        Task<IEnumerable<PracticalMarksStudentDto>> LoadStudentsAsync(PracticalMarksStudentsRequest request);

        /// <summary>
        /// Save a single student's practical mark (PMARKS in tbl_SH)
        /// Calls PROC_UPDATE_MARKS_INT_S_T with TYPE='P'
        /// Marks can be numeric (0 to PMAX) or "AB" (Absent)
        /// </summary>
        Task<int> SaveMarkAsync(PracticalMarksSaveRequest request);
    }
}
