using ICampus_Models.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IRoomWiseNominalRollsService
    {
        /// <summary>
        /// Get list of semesters for dropdown
        /// Query: SELECT DISTINCT cast( SEM as varchar(250)) SEM,cast(sem as int )sem1 
        ///        FROM tbl_sh WHERE COURSE = '{Course}' and ExamMY = '{ExamMy}' ORDER BY sem1
        /// </summary>
        Task<IEnumerable<object>> GetSemestersAsync(string course, string examMY);

        /// <summary>
        /// Get list of exam dates for dropdown (depends on Semester and ExamType)
        /// Stored Procedure: Sp_REP_Nominal_LoadEdate
        /// </summary>
        Task<IEnumerable<object>> GetExamDatesAsync(string course, string sem, string examMY, string regulation, string examType);

        /// <summary>
        /// Get list of branches for dropdown (depends on Exam Date)
        /// Stored Procedure: Sp_REP_Nominal_LoadBranch
        /// </summary>
        Task<IEnumerable<object>> GetBranchesAsync(string course, string sem, string examMY, string regulation, string edate, string examType);

        /// <summary>
        /// Get room-wise nominal rolls data
        /// Stored Procedure: Sp_REP_NominalRolls_ROOMWISE
        /// Parameters: @Course, @ExamMY, @Regulation, @ExamType, @Sem (optional), @Edate (optional), @Branch (optional)
        /// </summary>
        Task<IEnumerable<object>> GetRoomWiseNominalRollsDataAsync(RoomWiseNominalRollsRequest request);
    }
}

