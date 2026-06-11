using ICampus_Models.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IClassGradeService
{
    Task<IEnumerable<object>> LoadBatchAsync(string course);
    Task<IEnumerable<object>> LoadClassGradeGridAsync(string course, string regu);
    Task<int> SaveClassGradeAsync(ClassGradeSaveRequest request);
    Task<int> DeleteClassGradeAsync(IdDeleteRequest request);
    Task<int> CopyClassGradeFromPrevReguAsync(CopyClassGradeRequest request);
}
