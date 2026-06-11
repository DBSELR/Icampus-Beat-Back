using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IUnblockRegistrationsService
    {
        // Load exam month-year list for dropdown
        Task<IEnumerable<object>> LoadExamMyAsync();

        // Load blocked registrations data for grid
        Task<IEnumerable<object>> LoadBlockedStudentsAsync(string exammy);

        // Save/Perform unblock operation for a single student
        Task<int> UnblockStudentAsync(string exammy, string regno);

        // Save/Perform unblock operation for multiple students
        Task<UnblockBatchResult> UnblockMultipleAsync(string exammy, List<string> regnos);
    }

    public class UnblockBatchResult
    {
        public int TotalSelected { get; set; }
        public int SuccessfullyUnblocked { get; set; }
        public int Failed { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
}

