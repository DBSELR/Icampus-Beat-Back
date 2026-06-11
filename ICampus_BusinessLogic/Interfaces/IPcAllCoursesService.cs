using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IPcAllCoursesService
    {
        // Load Batch dropdown — Page_Load
        Task<IEnumerable<object>> LoadBatchAsync(string course);

        // Load Branch dropdown — ddlBatch_SelectedIndexChanged
        Task<IEnumerable<object>> LoadBranchAsync(string course, string batch);

        // Get PC data — btnView_Click
        // SP selection by isGracing + regu:
        //   isGracing=true  + regu contains "R16" → proc_pc_rep_AddGracing_R16  (6 params)
        //   isGracing=true                        → proc_pc_rep_AddGracing       (6 params)
        //   regu contains "R18"                   → proc_pc_rep_R18              (3 params)
        //   default                               → proc_pc_rep_AllCourse        (3 params)
        Task<IEnumerable<object>> GetDataAsync(
            string course, string examMY, string regu,
            string batch, string branch, string regNo,
            bool isGracing);
    }
}
