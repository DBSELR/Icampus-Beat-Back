using ICampus_Models.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IOMRSheetService
    {
        // Get semester list for dropdown
        Task<IEnumerable<object>> GetSemestersAsync(string course, string examMY);

        // Get exam date list for dropdown (depends on semester)
        Task<IEnumerable<object>> GetExamDatesAsync(string course, string examMY, string sem);

        // Get room list for dropdown (depends on exam date)
        Task<IEnumerable<object>> GetRoomsAsync(string course, string examMY, string sem, string edate);

        // Get OMR sheet data
        Task<IEnumerable<object>> GetOMRSheetDataAsync(OMRSheetRequest request);

        // Generate OMR numbers (call sp_SH_Omrnumber 8 times)
        Task<List<int>> GenerateOMRNumbersAsync(string examMY, string course, string regulation);

        // Get OMR data for export
        Task<IEnumerable<object>> GetOMRDataForExportAsync(string examMY, string course, string regulation);
    }
}

