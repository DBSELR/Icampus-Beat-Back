using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IIndividualMarksMemoService
    {
        Task<IEnumerable<object>> LoadSemsAsync(string course, string examMY, string regu);
        Task<IEnumerable<object>> LoadBranchesAsync(string course);
        Task<IEnumerable<object>> GetStudentInfoAsync(string regNo);
        Task<IEnumerable<object>> GetDataAsync(
            string regulation, string examMY, string course, string semester,
            string branch, string regNo);
        Task<IEnumerable<object>> GetValidRegnosAsync(
            string course, string examMY, string regulation, string semester, string branch);
    }
}
