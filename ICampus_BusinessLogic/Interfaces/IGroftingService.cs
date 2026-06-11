using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IGroftingService
    {
        /// <summary>
        /// Load available ExamMY list
        /// SP: SPM_EXAMS_ExamMY_Load(@Regulation, @Course) — @Regulation must NOT be empty
        /// </summary>
        Task<IEnumerable<object>> LoadExamMyAsync(string course, string regulation);

        /// <summary>
        /// Load semester list for selected course+examMy
        /// SP: SP_SEMS_RP(@REGULATION, @COURSE, @EXAMMY)
        /// </summary>
        Task<IEnumerable<object>> LoadSemsAsync(string course, string examMy, string regulation);

        /// <summary>
        /// Run Grofting/Grafting process
        /// SP: SP_GRACING_GROFTING(@EXAMMY, @COURSE) — no Semester or PaperCode params
        /// Processes all REG failed students for the given course+exammy
        /// </summary>
        Task<int> RunGroftingAsync(string course, string examMy);
    }
}
