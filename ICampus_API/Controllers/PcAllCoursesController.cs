using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PcAllCoursesController : BaseApiController
    {
        private readonly IPcAllCoursesService _svc;

        public PcAllCoursesController(IPcAllCoursesService svc)
        {
            _svc = svc;
        }

        // GET api/pcallcourses/batches?course=BTECH
        // Page_Load — populate ddlBatch
        // SQL: SELECT DISTINCT CAST(REGU AS VARCHAR) AS REGU, '20'+... BATCH
        //      FROM TBL_COURSE WHERE COURSE=@Course ORDER BY REGU
        [HttpGet("batches")]
        public async Task<IActionResult> LoadBatches([FromQuery] string course)
        {
            var data = await _svc.LoadBatchAsync(course);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Batches loaded." : "No batches found.",
                Data    = data
            });
        }

        // GET api/pcallcourses/branches?course=BTECH&batch=R20
        // ddlBatch_SelectedIndexChanged — populate ddlBranch
        // SQL: SELECT DISTINCT GRP FROM TBL_COURSE WHERE COURSE=@Course AND REGU=@Batch ORDER BY GRP
        [HttpGet("branches")]
        public async Task<IActionResult> LoadBranches(
            [FromQuery] string course,
            [FromQuery] string batch)
        {
            var data = await _svc.LoadBranchAsync(course, batch);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Branches loaded." : "No branches found.",
                Data    = data
            });
        }

        // GET api/pcallcourses/data?course=BTECH&examMY=NOV2024&regu=R20&batch=R20&branch=CSE&regNo=&isGracing=false
        // btnView_Click — load PC data
        // SP selection: isGracing+R16→proc_pc_rep_AddGracing_R16, isGracing→proc_pc_rep_AddGracing,
        //               R18→proc_pc_rep_R18, default→proc_pc_rep_AllCourse
        // Note: batch/branch/regNo filter Crystal Report selection formula in original ASPX
        [HttpGet("data")]
        public async Task<IActionResult> GetData(
            [FromQuery] string course,
            [FromQuery] string examMY,
            [FromQuery] string regu,
            [FromQuery] string batch     = "",
            [FromQuery] string branch    = "",
            [FromQuery] string regNo     = "",
            [FromQuery] bool   isGracing = false)
        {
            if (string.IsNullOrWhiteSpace(course) || string.IsNullOrWhiteSpace(examMY))
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Course and ExamMY are required.",
                    Data    = null
                });

            var data = await _svc.GetDataAsync(course, examMY, regu, batch, branch, regNo, isGracing);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "PC data loaded." : "No data found.",
                Data    = data
            });
        }
    }
}
