using System.Collections.Generic;
using System.Threading.Tasks;
using ICampus_Models.Requests;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IMiscFeeService
    {
        Task<IEnumerable<object>> LoadFeeDataAsync(string regno);
        Task<object> LoadReceiptNoAsync();
        Task<IEnumerable<object>> SaveMiscFeeAsync(MiscFeeSaveRequest request);
        Task<IEnumerable<object>> GetMiscReceiptAsync(string recptno);
        Task<int> DeleteReceiptAsync(string recptno);
        Task<IEnumerable<object>> ExportDataAsync();
    }
}
