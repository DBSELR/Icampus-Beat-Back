// IStudentMasterService.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using ICampus_Models.Requests;

public interface IStudentMasterService
{
    Task<IEnumerable<object>> LoadBranchAsync(string course, string examMy, string regulation);
    Task<IEnumerable<object>> LoadSemsAsync(string course, string examMy, string regulation);
    Task<IEnumerable<object>> LoadStdMasterAsync(string course, string examMy, string regu, string sem, string regno);
    Task<int> GetCountAsync(string examMy, string regu, string regno);
    Task<int> CreateMasterAsync(SaveStudentMasterRequest request);
    Task<IEnumerable<object>> LoadOmrNumUpdateAsync(string regno, string course, string regulation, string examMy);
    Task<IEnumerable<object>> GetLoadOmrNumUpdateAsync(string regno, string course, string regulation, string examMy, string regsup);
    Task<int> UpdateOmrNumAsync(UpdateOmrRequest req);
    Task<IEnumerable<object>> LoadStdUpdateAsync(string regulation, string course, string regu, string grp, string sem);
    Task<int> StdUpdateAsync(StdUpdateRequest req);
    Task<int> MarksUpdateRegnoWiseAsync(MarksUpdateRequest req);
    Task<int> DeleteAshIdAsync(DeleteAshIdRequest req);
}
