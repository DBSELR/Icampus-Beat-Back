using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IBtechCmmService
    {
        // Load Batch dropdown — Page_Load
        Task<IEnumerable<object>> LoadBatchAsync(string course);

        // Load Branch dropdown — ddlBatch_SelectedIndexChanged
        Task<IEnumerable<object>> LoadBranchAsync(string course, string batch);

        // Get CGC data — btnView_Click
        // SP selection by isGracing + regu:
        //   isGracing=true  + regu contains "R16" → SP_CMM_AddGracing_R16  (6 params)
        //   isGracing=true                        → SP_CMM_AddGracing       (6 params)
        //   regu contains "R18"                   → SP_CMM_R18              (3 params)
        //   regu contains "R11" (PG)              → SP_CMM_R11_PG           (3 params)
        //   default                               → SP_CMM                  (3 params)
        Task<IEnumerable<object>> GetDataAsync(
            string course, string examMY, string regu,
            string batch, string branch, string regNo,
            bool isGracing, bool isLateral);
    }
}
