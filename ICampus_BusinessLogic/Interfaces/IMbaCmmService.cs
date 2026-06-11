using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IMbaCmmService
    {
        // Get MBA CMM data — Page_Load (no user controls)
        // SP: SP_CMM @Course, @ExamMY, @Regu  (same SP as MTECHCMM, Course=MBA)
        Task<IEnumerable<object>> GetDataAsync(string course, string examMY, string regu, string grp);
    }
}
