using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IRvSummeryReportService
    {
        Task<IEnumerable<object>> GetRvSummaryAsync(string regulation, string course, string examMY);
        Task<IEnumerable<object>> GetSupplySummaryAsync(string regulation, string course, string examMY);
    }
}
