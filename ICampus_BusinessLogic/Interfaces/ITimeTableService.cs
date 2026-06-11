using ICampus_Models.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface ITimeTableService
    {
        // Get semester list for dropdown
        Task<IEnumerable<object>> GetSemestersAsync(string course, string examMY);

        // Get timetable data (executes UPDATE query first, then calls stored procedure)
        Task<IEnumerable<object>> GetTimeTableDataAsync(TimeTableRequest request);
    }
}

