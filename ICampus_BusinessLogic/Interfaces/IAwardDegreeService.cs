using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IAwardDegreeService
    {
        Task<IEnumerable<object>> LoadBatchesAsync(string course);
        Task<IEnumerable<object>> LoadExamMYsAsync(string course, string regu);
        Task<IEnumerable<object>> GetDataAsync(string regu, string examMY, string course);
    }
}
