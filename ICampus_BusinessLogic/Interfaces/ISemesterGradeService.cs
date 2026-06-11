using System.Collections.Generic;
using System.Threading.Tasks;
using ICampus_Models.Requests;

public interface ISemesterGradeService
{
    Task<IEnumerable<object>> LoadBatchesAsync(string course);
    Task<IEnumerable<object>> LoadSemGradeGridAsync(string course, string regu);
    Task<int> SaveSemGradeAsync(SemGradeSaveRequest req);
    Task<int> DeleteSemGradeAsync(DeleteRequest req);
    Task<IEnumerable<object>> CheckSemGradeAsync(string course, string toBatch, string type);
    Task<int> CopyGradeAsync(CopySemesterGradeRequest req);
    Task<IEnumerable<object>> LoadReguAsync(string course, string type);
}
