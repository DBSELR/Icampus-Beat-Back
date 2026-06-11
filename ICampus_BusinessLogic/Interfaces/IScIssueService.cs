using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IScIssueService
    {
        Task<IEnumerable<object>> GetStudentInfoAsync(string regNo);
        Task<int> IssueScAsync(
            string regNo, string regulation, string section,
            string sName, string fName, string mName,
            string conduct, string dob, string gender, string caste,
            string email, string mobile, string aadhaarNo, string religion);
    }
}
