using ICampus_Models.Requests;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface ICondonationService
    {
        Task<IEnumerable<object>> LoadSemsAsync(string course, string regulation, string examMy, string regsup, string regno);
        Task<object> GetStudentDetailsAsync(string regno);
        Task<IEnumerable<object>> GetCondonationGridAsync(string regno, string examMy, string course, string sem);
        Task<IEnumerable<object>> CheckCondonationDatesAsync(string regno, string examMy, string course, string regulation, string sem);
        Task<int> SaveCondonationAsync(CondonationSaveRequest req);
        Task<int> DeleteCondonationAsync(int id);
        Task<object> GetCondonationFormatAsync();
        Task<IEnumerable<object>> ExportCondonationAsync(string examMy, string regulation);
    }
}
