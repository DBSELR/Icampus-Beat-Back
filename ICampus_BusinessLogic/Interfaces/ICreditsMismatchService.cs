using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface ICreditsMismatchService
    {
        // Get batches for dropdown
        Task<IEnumerable<object>> GetBatchesAsync(string regulation);

        // Get exam month-years for dropdown
        Task<IEnumerable<object>> GetExamMyAsync(string course, string regulation);

        // Get semesters for dropdown
        Task<IEnumerable<object>> GetSemestersAsync(string regulation, string examMy);

        // Get credits mismatch data
        Task<IEnumerable<object>> GetMismatchCreditsAsync(string regulation, string examMy, string batch, string course, string sem);
    }
}

