using ICampus_Models.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IHallTicketService
    {
        // Get batch list for dropdown
        Task<IEnumerable<object>> GetBatchesAsync(string course, string regulation);

        // Get branch list for dropdown (depends on batch)
        Task<IEnumerable<object>> GetBranchesAsync(string course, string regulation, string batch);

        // Get semester list for dropdown
        Task<IEnumerable<object>> GetSemestersAsync(string course, string regulation, string examMY);

        // Prepare hall ticket data (call SPM_HT_LBRCE)
        // Returns: Tuple with (rowsAffected, recordsCountInTable)
        Task<(int RowsAffected, int RecordsCount)> PrepareHallTicketsAsync(string examMY, string course, string regulation, string selectionFormula);

        // Get hall ticket data (after preparation)
        Task<IEnumerable<object>> GetHallTicketDataAsync(HallTicketRequest request);

        // Diagnostic: Check if source data exists for the given criteria
        Task<object> CheckSourceDataAsync(string examMY, string course, string regulation, string selectionFormula);
    }
}

