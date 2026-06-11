using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface ITcIssueService
    {
        Task<IEnumerable<object>> GetStudentInfoAsync(string regNo);
        Task<IEnumerable<object>> GetSampleRegnosAsync();
        Task<int> IssueTcAsync(
            string regNo, string regulation, string section,
            string sName, string fName, string mName,
            string dob, string gender, string caste,
            string email, string mobile, string aadhaarNo,
            string mole1, string mole2, string religion,
            string dateofAdmitted, string scholar,
            string courseComplete, string higherEdu, string dateofLeave);
    }
}
