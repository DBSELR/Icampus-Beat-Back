using ICampus_Models.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IMidHallTicketService
    {
        /// <summary>
        /// Prepare/generate mid hall ticket data
        /// Stored Procedure: SPM_HallTicket_Mid
        /// </summary>
        Task<int> PrepareMidHallTicketsAsync(MidHallTicketRequest request);

        /// <summary>
        /// Get mid hall ticket data after preparation
        /// Queries the tbl_hallticket table populated by SPM_HallTicket_Mid
        /// </summary>
        Task<IEnumerable<object>> GetMidHallTicketDataAsync(MidHallTicketRequest request);
    }
}

