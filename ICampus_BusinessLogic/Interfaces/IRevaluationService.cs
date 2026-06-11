// IRevaluationService.cs
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IRevaluationService
{
    Task<IEnumerable<object>> LoadSemsAsync(string course, string examMy, string regulation);
    Task<IEnumerable<object>> GetPapersForRvAsync(string examMy, string regno, string sem);
    Task<IEnumerable<object>> GetOptedPapersAsync(string examMy, string regno, string sem);
    Task<IEnumerable<object>> CheckRvCloseDateAsync(string regulation, string course, string examMy, string sem, string regno);
    Task<IEnumerable<object>> GetRvFeeAsync(string regulation, string course, string examMy, string sem, string rvType);
    Task<int> RegisterRvPaperAsync(RegisterRvPaperRequest request);
    Task<int> ResetRvPapersAsync(ResetRvPaperRequest request);
    Task<IEnumerable<object>> RvExamFeePayAsync(RvFeePayRequest request);
    Task<IEnumerable<object>> GetRvBundleScriptsAsync(string regulation, string examMy, string course, string userId);
    Task<IEnumerable<object>> GetReceiptDataAsync(string receiptNo);
}
