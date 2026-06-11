using ICampus_Models.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IQPStatementService
    {
        // Get semester list for dropdown
        Task<IEnumerable<object>> GetSemestersAsync(string course, string regulation, string examMY);

        // Get question paper statement data
        Task<IEnumerable<object>> GetQPStatementDataAsync(QPStatementRequest request);
    }
}

