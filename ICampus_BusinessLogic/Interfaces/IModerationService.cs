using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IModerationService
    {
        /// <summary>Load batch/regu dropdown for moderation (PROC_RESULT_LOAD_REGU_COURSE_GRP)</summary>
        Task<IEnumerable<object>> LoadBatchAsync(string course);

        /// <summary>Load branch (GRP) dropdown for moderation</summary>
        Task<IEnumerable<object>> LoadBranchAsync(string course, string examMy, string sem);

        /// <summary>Load paper list for moderation dropdown</summary>
        Task<IEnumerable<object>> LoadPapersAsync(string course, string sem);

        /// <summary>Load moderation marks grid (PROC_MODERATION_REG_SEM_GRP_PAP)</summary>
        Task<IEnumerable<object>> LoadModerationDataAsync(string course, string examMy, string regu, string sem, string grp, string papCode);

        /// <summary>Save moderation marks (PROC_MODERATION_AND_MCNT)</summary>
        Task<int> SaveModerationAsync(string course, string examMy, string regu, string sem, string grp, string papCode, string modMarks);
    }
}
