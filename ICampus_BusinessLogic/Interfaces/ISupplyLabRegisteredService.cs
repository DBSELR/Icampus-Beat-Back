using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface ISupplyLabRegisteredService
    {
        // Get semesters for dropdown
        Task<IEnumerable<object>> GetSemestersAsync(string course);

        // Get batches/regulations for dropdown
        Task<IEnumerable<object>> GetBatchesAsync();

        // Get supply lab registered data for export
        Task<IEnumerable<object>> GetSupplyLabDataAsync(string examMy, string course, string regu, string sem);
    }
}

