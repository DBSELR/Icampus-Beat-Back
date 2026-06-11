using ICampus_Models.DTOs;
using ICampus_Models.Requests;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IRegNoMarksEntryService
    {
        /// <summary>
        /// Load student name, SEM and all paper rows by RegNo
        /// Calls PROC_GETSUBJTS_REGNO_WISE (@Course, @ExamMy, @Batch, @Sem, @Regno — confirmed from DLL IL)
        /// Returns all papers with current marks: SMARKS, TMARKS, RVMARKS, V3, MRK_FIN, PMARKS
        /// </summary>
        Task<RegNoMarksStudentDto?> LoadByRegNoAsync(string regNo, string? examMY = null, string? batch = null, string? sem = null, string? course = null);

        /// <summary>
        /// Save all edited mark rows in bulk (btnOmrNo "UPDATE MARKS")
        /// Calls PROC_marksupdate_regnowise once per paper row
        /// Parameters per row: @ASHID, @PTYPE, @SMARKS, @TMARKS, @RVMARKS, @V3, @MRK_FIN, @PMARKS
        /// </summary>
        Task<int> SaveMarksAsync(RegNoMarksSaveRequest request);
    }
}
