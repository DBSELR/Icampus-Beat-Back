using ICampus_Models.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IRoomAbstractService
    {
        /// <summary>
        /// Get list of semesters for dropdown
        /// Query: select distinct sem from tbl_sh where course='{Course}' order by sem
        /// </summary>
        Task<IEnumerable<object>> GetSemestersAsync(string course);

        /// <summary>
        /// Get list of sessions for dropdown
        /// Stored Procedure: Spr_Load_Session
        /// </summary>
        Task<IEnumerable<object>> GetSessionsAsync(string course, string sem, string examType);

        /// <summary>
        /// Get list of exam dates for dropdown
        /// Stored Procedure: Proc_Load_Edate
        /// </summary>
        Task<IEnumerable<object>> GetExamDatesAsync(string course, string sem, string session, string examMY, string examType);

        /// <summary>
        /// Get room abstract data for report
        /// Stored Procedures: SPR_LOAD_EXAMDATES (Regular) or SPR_LOAD_EXAMDATES_Supple (Supply)
        /// </summary>
        Task<IEnumerable<object>> GetRoomAbstractDataAsync(RoomAbstractRequest request);
    }
}

