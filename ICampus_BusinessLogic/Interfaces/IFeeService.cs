using ICampus_Models.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IFeeService
    {
        // Fee structure: dynamic rows (each row is a Dictionary<string, object> boxed as object)
        Task<IEnumerable<object>> LoadFeeStructureGridAsync(string course, string examMy, string regulation);

        // Filtered fee structure (inline SQL)
        Task<IEnumerable<object>> LoadFeeStructureFilterAsync(string course, string examMy, string regulation, string regu, string grp);

        // Load sems/branches/fine rows (PROC_LOAD_SEMS_FOR_FEE)
        Task<IEnumerable<object>> LoadBatchBranchFineAsync(string course, string examMy, string regulation, string type);

        // Supply grid (PROC_SUPPLY_FEE_GRIDLOAD)
        Task<IEnumerable<object>> LoadSupplyGridAsync(string course, string examMy, string regulation, string type);

        // Save operations
        Task<int> SaveFeeRegularAsync(FeeSaveRegularRequest request);
        Task<int> SaveSupplyFeeAsync(SupplyFeeSaveRequest request);

        // Failed-subjects count (scalar)
        Task<int> GetFailedSubjectsCountAsync(string examMy, string sems);

        // Fine list (dynamic)
        Task<IEnumerable<object>> LoadFineListAsync(string course, string examMy, string regulation);

        // Check and save fine
        Task<IEnumerable<object>> CheckFineAsync(FineSaveRequest request); // returns overlapping rows if any (dynamic)
        Task<int> SaveFineAsync(FineSaveRequest request);      // S_TYPE = SAVE_FEE
        Task<int> DeleteFineAsync(FineSaveRequest request);    // S_TYPE = DEL_FEE

        // Condination (dynamic)
        Task<IEnumerable<object>> LoadCondinationSemsAsync(string regulation, string examMy, string course);
        Task<IEnumerable<object>> LoadCondinationDatesAsync(string regulation, string examMy, string course);
        Task<int> SaveCondinationDatesAsync(CondinationDateSaveRequest request);
        Task<int> DeleteCondinationDateAsync(int fid);
    }
}
