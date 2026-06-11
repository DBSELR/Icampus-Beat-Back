using ICampus_Models.DTOs;
using ICampus_Models.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface ICgpService
    {
        Task<IEnumerable<CourseDto>> LoadGridAsync(string type, string searchString, string regulation);
        Task<IEnumerable<CourseDto>> SearchCgAsync(string regu, string course, string grp);
        Task<IEnumerable<string>> SearchReguAsync(string prefix);
        Task<IEnumerable<string>> SearchBatchAsync(string prefix);
        Task<IEnumerable<string>> SearchCourseAsync(string prefix);
        Task<IEnumerable<string>> SearchCourseNameAsync(string prefix);
        Task<IEnumerable<string>> SearchGrpAsync(string prefix);
        Task<IEnumerable<string>> SearchGrpNameAsync(string prefix);

        Task<int> SaveCourseAsync(SaveCourseRequest request);      // returns affected rows / result code
        Task<int> DeleteCourseAsync(DeleteCourseRequest request);
        Task<int> CopyGroupAsync(CopyGroupRequest request);
        Task<bool> CheckAndCopyAsync(CopyGroupRequest request);    // replicates check + popup flow
    }
}
