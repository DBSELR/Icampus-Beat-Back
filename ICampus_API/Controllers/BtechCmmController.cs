using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BtechCmmController : BaseApiController
    {
        private readonly IBtechCmmService _svc;

        public BtechCmmController(IBtechCmmService svc)
        {
            _svc = svc;
        }

        // GET api/btechcmm/batches?course=BTECH
        // Page_Load — populate ddlBatch
        // SQL: SELECT DISTINCT CAST(REGU AS VARCHAR) AS REGU, '20'+CAST(REGU AS VARCHAR)+'-'+... BATCH
        //      FROM TBL_COURSE WHERE COURSE=@Course ORDER BY REGU
        // Note: BTECHCMM.aspx title = "CGC Report" (CGC = Consolidated Grade Card)
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

        // GET api/btechcmm/branches?course=BTECH&batch=R20
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

        // GET api/btechcmm/data?course=BTECH&examMY=NOV2024&regu=R20&batch=R20&branch=CSE&regNo=&isGracing=false&isLateral=false
        // btnView_Click — load CGC data
        // SP selection: isGracing+R16→SP_CMM_AddGracing_R16, isGracing→SP_CMM_AddGracing,
        //               R18→SP_CMM_R18, R11→SP_CMM_R11_PG, default→SP_CMM
        // ChkLateral: isLateral flag — passed through but SP filtering is done in Crystal Report
        //   (batch/branch/regNo/isLateral filter the Crystal Report selection formula in original)
        [HttpGet("data")]
        public async Task<IActionResult> GetData(
            [FromQuery] string course,
            [FromQuery] string examMY,
            [FromQuery] string regu,
            [FromQuery] string batch    = "",
            [FromQuery] string branch   = "",
            [FromQuery] string regNo    = "",
            [FromQuery] bool   isGracing  = false,
            [FromQuery] bool   isLateral  = false)
        {
            if (string.IsNullOrWhiteSpace(course) || string.IsNullOrWhiteSpace(examMY))
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Course and ExamMY are required.",
                    Data    = null
                });

            var data = await _svc.GetDataAsync(course, examMY, regu, batch, branch, regNo, isGracing, isLateral);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "CGC data loaded." : "No data found.",
                Data    = data
            });
        }
    }
}
