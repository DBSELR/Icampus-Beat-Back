using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IResultProcessService
    {
        /// <summary>
        /// Load ExamMY dropdown for result process page
        /// SP: SPM_EXAMS_ExamMY_Load(@regulation, @COURSE)
        /// </summary>
        Task<IEnumerable<object>> LoadExamMyAsync(string course, string regulation);

        /// <summary>
        /// Run result process for a student (regno-wise)
        /// SP: SP_RESULT_PROCESS_REGNOWISE(@Regulation, @COURSE, @EXAMMY1, @SEM, @GRP, @REGNO, @flag)
        /// </summary>
        Task<int> RunResultProcessAsync(string regNo, string examMy, string regulation, string course, string sem, string grp);

        /// <summary>
        /// Run readmit result process for a student
        /// SP: SP_RESULT_PROCESS_readmit(@Regulation, @COURSE, @EXAMMY1, @SEM, @flag, @GRP, @READMIT_REGULATION)
        /// </summary>
        Task<int> RunReadmitResultProcessAsync(string regNo, string examMy, string regulation, string course, string sem, string grp, string readmitRegulation);
    }
}
