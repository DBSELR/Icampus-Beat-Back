using ICampus_Models.Requests;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface ICancelReceiptService
    {
        // 1) Load student info
        Task<object> GetStudentDetailsAsync(string regno);

        // 2) Load receipt subjects by RegNo + ExamMy
        Task<IEnumerable<object>> LoadReceiptSubjectsAsync(string regno, string examMy);

        // 3) Cancel a receipt (delete/cancel entry)
        Task<int> CancelReceiptAsync(CancelReceiptRequest req);
    }
}
