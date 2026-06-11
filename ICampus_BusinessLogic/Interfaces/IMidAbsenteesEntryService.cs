using ICampus_Models.DTOs;
using ICampus_Models.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IMidAbsenteesEntryService
    {
        /// <summary>
        /// Load papers dropdown for Mid Absentees Entry
        /// Calls PROC_LOADPAPERS_MRKENTRY (same SP as regular AbsenteesEntry)
        /// 5 positional params + TYPE='T' hardcoded: @Regulation, @ExamMy, @Sem[INT], @Course, @GRP
        /// ExamType is NOT passed to this SP — only to the students and save SPs
        /// </summary>
        Task<IEnumerable<AbsenteesPaperDto>> LoadPapersAsync(MidAbsenteesPapersRequest request);

        /// <summary>
        /// Load student list for Mid Absentees Entry grid
        /// Calls PROC_LOADMARKS_MRKENTRY_MID (Mid-specific SP)
        /// 8 positional params: @Regulation, @ExamMy, @Sem, @Course, @Branch, @PaperCode, @TYPE='T', @ExamType
        /// Confirmed from DLL IL: method Load_Mid_Marks_Grid_Absentees
        /// Returns: aSHID, RegNo, grp, PCODE, MIDCODE (current AB/MP status)
        /// ExamType: "1" = MID-I, "2" = MID-II
        /// </summary>
        Task<IEnumerable<MidAbsenteesStudentDto>> LoadStudentsAsync(MidAbsenteesStudentsRequest request);

        /// <summary>
        /// Save a single student's Mid absentee code (AB or MP)
        /// Calls PROC_UPDATE_MID_MARKS_INT_S_T (Mid-specific SP)
        /// 4 positional params: @Marks, @AshId, @TYPE='T', @ExamType
        /// Confirmed from DLL IL: method Update_Mid_ABSENTEES_Marks
        /// ExamType: "1" = MID-I, "2" = MID-II
        /// </summary>
        Task<int> SaveCodeAsync(MidAbsenteesSaveRequest request);
    }
}
