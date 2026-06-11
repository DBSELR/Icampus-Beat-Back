using System.Collections.Generic;
using System.Threading.Tasks;
using ICampus_Models.Requests;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface ISubjectGradeService
    {
        Task<IEnumerable<object>> LoadBatchAsync(string course);
        Task<IEnumerable<object>> LoadSubGradeGridAsync(string course, string regu);
        Task<int> SaveSubGradeAsync(SubjectGradeSaveRequest req);
        Task<int> DeleteSubGradeAsync(string id);
        Task<int> CopyGradeAsync(CopyGradeRequest req);
        Task<IEnumerable<object>> LoadReguListAsync(string course, string procType); // used for copy modal
    }
}
