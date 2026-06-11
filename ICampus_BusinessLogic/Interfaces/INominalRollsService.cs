using ICampus_Models.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface INominalRollsService
    {
        // Get semester list for dropdown
        Task<IEnumerable<object>> GetSemestersAsync(string course, string examMY);

        // Get exam date list for dropdown (depends on semester)
        Task<IEnumerable<object>> GetExamDatesAsync(string course, string examMY, string sem);

        // Get room list for dropdown (depends on exam date)
        Task<IEnumerable<object>> GetRoomsAsync(string course, string examMY, string sem, string edate);

        // Get nominal rolls data (regular or readmit)
        Task<IEnumerable<object>> GetNominalRollsDataAsync(NominalRollsRequest request);
    }
}

