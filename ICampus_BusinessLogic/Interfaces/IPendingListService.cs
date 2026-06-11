using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IPendingListService
    {
        Task<IEnumerable<object>> GetInternalPendingListAsync(string examMy, string course, string regulation);
        Task<IEnumerable<object>> GetPracticalPendingListAsync(string examMy, string course, string regulation);
        Task<IEnumerable<object>> GetTheoryPendingListAsync(string examMy, string course, string regulation);
        Task<IEnumerable<object>> GetRVPendingListAsync(string examMy, string course, string regulation);
    }
}
