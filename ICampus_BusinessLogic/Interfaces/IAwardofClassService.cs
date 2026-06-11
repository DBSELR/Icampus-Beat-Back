using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IAwardofClassService
    {
        Task<IEnumerable<object>> LoadBatchesAsync(string course);
        Task<IEnumerable<object>> LoadSemsAsync(string course, string examMY);
        Task<IEnumerable<object>> GetViewDataAsync(string course, string examMY, string regu, string sem);
        Task<IEnumerable<object>> GetExcelDataAsync(string course, string examMY, string regu, string sem);
    }
}
