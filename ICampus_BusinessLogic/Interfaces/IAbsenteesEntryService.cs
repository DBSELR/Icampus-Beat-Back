using ICampus_Models.DTOs;
using ICampus_Models.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IAbsenteesEntryService
    {
        /// <summary>
        /// Load papers dropdown for Absentees Entry screen
        /// Calls PROC_LOADPAPERS_MRKENTRY with TYPE='T'
        /// Params (confirmed from DLL IL): @Regulation, @ExamMy, @Sem[INT], @Course, @GRP
        /// </summary>
        Task<IEnumerable<AbsenteesPaperDto>> LoadPapersAsync(AbsenteesPapersRequest request);

        /// <summary>
        /// Load student list for the selected paper
        /// Calls PROC_LOADMARKS_MRKENTRY with TYPE='T'
        /// Params (confirmed from DLL IL, in this order): @Regulation, @ExamMy, @Sem, @Course, @Branch, @PaperCode
        /// Returns: aSHID, RegNo, GRP, PCODE, CODE (AB/MP/null)
        /// </summary>
        Task<IEnumerable<AbsenteesStudentDto>> LoadStudentsAsync(AbsenteesStudentsRequest request);

        /// <summary>
        /// Save a single student's absentee code (AB or MP)
        /// Calls PROC_UPDATE_MARKS_INT_S_T with TYPE='T' (@Marks, @AshId — confirmed from DLL IL)
        /// Code must be "AB" (Absent) or "MP" (Malpractice)
        /// </summary>
        Task<int> SaveCodeAsync(AbsenteesSaveRequest request);
    }
}
