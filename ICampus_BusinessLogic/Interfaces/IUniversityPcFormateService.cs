using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IUniversityPcFormateService
    {
        Task<IEnumerable<object>> LoadBatchAsync(string course);
        Task<IEnumerable<object>> LoadBranchAsync(string course, string batch);
        Task<IEnumerable<object>> GetSpParamsAsync(string spName);
        Task<IEnumerable<object>> GetDataAsync(
            string course, string examMY, string regu,
            string batch, string branch,
            bool isGracing, bool isLateral);
    }
}
