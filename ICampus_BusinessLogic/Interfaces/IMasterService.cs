using System.Collections.Generic;
using System.Threading.Tasks;
using ICampus_Models.Requests;

public interface IMasterService
{
    Task<IEnumerable<object>> GetRegularDataAsync(string course, string examMy, string regulation);
    Task<int> UpdatePapDataAsync(UpdatePapRequest req);
    Task<IEnumerable<object>> LoadMasterDataAsync(string course, string examMy, string regulation);
    Task<int> MasterExistsAsync(string course, string examMy, string batch, string sem);
    Task<int> CreateMasterAsync(CreateMasterRequest req);
    Task<byte[]> ExportPapDataExcelAsync(string course, string examMy, string regulation);
}
