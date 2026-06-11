using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IResultSheetService
    {
        Task<IEnumerable<object>> LoadSemsAsync(string course, string examMY, string regu);
        Task<IEnumerable<object>> GetDataAsync(
            string course, string examMY, string regu, string sem,
            bool isReadmit, string readmitRegu);
    }
}
