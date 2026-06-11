using ICampus_Models.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface ITimeTableAndSeatingService
    {
        Task<IEnumerable<object>> GetSemsExamMyAsync(string examMy, string course, string regulation);
        Task<IEnumerable<object>> GetExamTimeTableDataAsync(string examMy, string course, int sem, string regulation);
        Task<IEnumerable<object>> GetPapersWithCodeAsync(string examMy, string course, int sem, string eDate, string regulation);
        Task<IEnumerable<object>> GetPapersDataAsync(string examMy, string course, int sem, string pcode, string regulation, string examType);
        Task<IEnumerable<object>> GetExamDatesAsync(string examMy, string course, int sem, string regulation);
        Task<IEnumerable<object>> GetRAPapersListAsync(string examMy, string course, int sem, string eDate, string regulation);
        Task<IEnumerable<object>> GetRAPapersDataAsync(string examMy, string course, int sem, string pcode, string eDate, string regulation);
        Task<IEnumerable<object>> GetExamBranchAsync(string examMy, string course, int sem, string regulation);
        Task<int> UpdateExamSessionAsync(UpdateExamSessionRequest req);
        Task<int> UpdateExamDateAsync(UpdateExamDateRequest req);
        Task<int> UpdateRoomNumbersAsync(UpdateRoomNumbersRequest req);
        Task<IEnumerable<object>> RoomsSearchAsync(string prefixText);
        Task<IEnumerable<object>> ExamDatesFormatAsync(string regulation, string course, string examMy);
    }
}
