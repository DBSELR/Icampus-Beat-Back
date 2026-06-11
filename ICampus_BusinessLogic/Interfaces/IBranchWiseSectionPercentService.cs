using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IBranchWiseSectionPercentService
    {
        Task<IEnumerable<object>> LoadSemsAsync(string course, string examMY);
        Task<IEnumerable<object>> GetDataAsync(
            string course, string examMY, string regu, string sem);
    }
}
