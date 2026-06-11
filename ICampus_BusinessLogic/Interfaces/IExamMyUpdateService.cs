using ICampus_Models.DTOs;
using ICampus_Models.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IExamMyUpdateService
    {
        /// <summary>
        /// Load student grid filtered by Regulation, Course, Batch, Branch and Sem
        /// Calls sp_loadstdexammy_update (@Regulation, @Course, @Batch, @Branch, @Sem)
        /// Returns: ASHID, REGULATION, REGU, COURSE, GRP, SEM, STREAM, REGNO, EXAMMY, ACT_EXAMMY
        /// </summary>
        Task<IEnumerable<ExamMyUpdateDto>> LoadAsync(ExamMyLoadRequest request);

        /// <summary>
        /// Bulk update ExamMY — one SP call per row (btnUpdateExammy click)
        /// Calls sp_Exammy_update (@ACT_EXAMMY, @Regulation, @Batch, @Course, @Branch, @Sem, @REGNO, @EXAMMY)
        /// SP identifies row by REGNO + full exam context (not by ASHID)
        /// Returns total rows affected
        /// </summary>
        Task<int> UpdateExamMyAsync(ExamMyUpdateRequest request);

        /// <summary>
        /// Delete a single student record by ASHID (Delete button per row)
        /// Calls PROC_DEL_ASHID (@ASHID)
        /// </summary>
        Task<int> DeleteAsync(long ashid);
    }
}
