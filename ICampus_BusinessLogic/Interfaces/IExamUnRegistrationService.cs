using ICampus_Models.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IExamUnRegistrationService
    {
        // Load student registration data for display
        Task<IEnumerable<object>> LoadDataAsync(string regulation, string course, string examMY, string regno);

        // Perform unregistration (update REGD flag)
        Task<int> UnRegisterAsync(UnRegistrationRequest request);
    }
}

