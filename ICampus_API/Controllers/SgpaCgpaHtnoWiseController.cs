using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SgpaCgpaHtnoWiseController : BaseApiController
    {
        private readonly ISgpaCgpaHtnoWiseService _svc;

        public SgpaCgpaHtnoWiseController(ISgpaCgpaHtnoWiseService svc)
        {
            _svc = svc;
        }

        // GET api/sgpacgpahtnwise/batches?course=BTECH
        // ddlBatch AutoPostBack — populate batch/regulation dropdown
        // SQL: SELECT DISTINCT CAST(REGU AS VARCHAR) AS REGU,
        //      '20'+CAST(REGU AS VARCHAR)+'-'+CAST(REGU+MAXSEM/2 AS VARCHAR) BATCH
        //      FROM TBL_COURSE WHERE COURSE=@Course ORDER BY REGU
        [HttpGet("batches")]
        public async Task<IActionResult> LoadBatches([FromQuery] string course)
        {
            var data = await _svc.LoadBatchesAsync(course);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Batches loaded." : "No batches found.",
                Data    = data
            });
        }

        // GET api/sgpacgpahtnwise/branches?course=BTECH
        // ddlBranch — loaded on ddlBatch_SelectedIndexChanged
        // SQL: SELECT DISTINCT GRP FROM tbl_SH WHERE COURSE=@Course Order by GRP
        // Note: uses tbl_SH (not TBL_COURSE) — branch list from actual marks records
        [HttpGet("branches")]
        public async Task<IActionResult> LoadBranches([FromQuery] string course)
        {
            var data = await _svc.LoadBranchesAsync(course);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Branches loaded." : "No branches found.",
                Data    = data
            });
        }

        // GET api/sgpacgpahtnwise/examMYs?course=BTECH&regu=20
        // cmbExamMY — exam month/year dropdown
        // SQL: SELECT DISTINCT EXAMMY, AEXAMID FROM TBL_EXAMS WHERE COURSE=@Course
        //      and REGULATION=@Regu ORDER BY AEXAMID DESC
        [HttpGet("examMYs")]
        public async Task<IActionResult> LoadExamMYs(
            [FromQuery] string course,
            [FromQuery] string regu)
        {
            var data = await _svc.LoadExamMYsAsync(course, regu);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "ExamMYs loaded." : "No ExamMYs found.",
                Data    = data
            });
        }

        // GET api/sgpacgpahtnwise/data?course=BTECH&examMY=NOV2024&regu=20&branch=CSE&withRv=false
        // btnView_Click / btnDownLoad_Click — load SGPA & CGPA data H.T.No. wise
        // SP: withRv=true  → PROC_SGPA_AVERAGE          (@Course,@ExamMY,@Regu,@Branch)
        //     withRv=false → PROC_SGPA_AVERAGE_OnlyRegular (@Course,@ExamMY,@Regu,@Branch)
        // Crystal Report: SGPA.rpt
        //   Subtitle: "(With Revaluation)" / "(Without Revaluation)" — frontend responsibility
        // Note: Distinct from /api/regnoWiseSgpaCgpa which is Sem-based (RegnoWiseSgpaCgpaList.aspx)
        [HttpGet("data")]
        public async Task<IActionResult> GetData(
            [FromQuery] string course,
            [FromQuery] string examMY,
            [FromQuery] string regu,
            [FromQuery] string branch,
            [FromQuery] bool   withRv = false)
        {
            if (string.IsNullOrWhiteSpace(course) || string.IsNullOrWhiteSpace(examMY) ||
                string.IsNullOrWhiteSpace(regu))
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Course, ExamMY, and Regu are required.",
                    Data    = null
                });

            var data = await _svc.GetDataAsync(course, examMY, regu, branch, withRv);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "SGPA & CGPA data loaded." : "No data found.",
                Data    = data
            });
        }
    }
}
