using ICampus_Models.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface ISeatingArrangementService
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
        /// Get list of rooms for dropdown
        /// Stored Procedure: SPR_LOAD_ROOM
        /// </summary>
        Task<IEnumerable<object>> GetRoomsAsync(string course, string session);

        /// <summary>
        /// Get seating arrangement data for report
        /// Stored Procedure: Sp_temproom_Dump
        /// </summary>
        Task<IEnumerable<object>> GetSeatingArrangementDataAsync(SeatingArrangementRequest request);
    }
}

