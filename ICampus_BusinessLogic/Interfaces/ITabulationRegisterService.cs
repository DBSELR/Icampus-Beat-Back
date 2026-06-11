using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface ITabulationRegisterService
    {
        Task<IEnumerable<object>> LoadBatchesAsync(string course);
        Task<IEnumerable<object>> LoadBranchesAsync(string course, string regu);
        Task<IEnumerable<object>> LoadExamMYsAsync(string course, string regu);
        Task<IEnumerable<object>> GetDataAsync(
            string course, string examMY, string regu, string branch, string regNo,
            bool isReadmit, string readmitRegu);
    }
}
