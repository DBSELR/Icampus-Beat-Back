using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UniversityPcFormateController : BaseApiController
    {
        private readonly IUniversityPcFormateService _svc;

        public UniversityPcFormateController(IUniversityPcFormateService svc)
        {
            _svc = svc;
        }

        // GET api/universitypcrformate/batch?course=BTECH
        // Page_Load → ddlBatch
        // SQL: SELECT DISTINCT CAST(REGU AS VARCHAR) AS REGU,
        //      '20'+CAST(REGU AS VARCHAR)+'-'+CAST(REGU+MAXSEM/2 AS VARCHAR) BATCH
        //      FROM TBL_COURSE WHERE COURSE=@Course ORDER BY REGU
        [HttpGet("batch")]
        public async Task<IActionResult> GetBatch([FromQuery] string course)
        {
            if (string.IsNullOrWhiteSpace(course))
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "course is required.",
                    Data    = null
                });

            var data = await _svc.LoadBatchAsync(course);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Batch list loaded.",
                Data    = data
            });
        }

        // GET api/universitypcrformate/branch?course=BTECH&batch=17
        // ddlBatch_SelectedIndexChanged → ddlBranch
        // SQL: SELECT DISTINCT GRP FROM TBL_COURSE WHERE COURSE=@Course AND REGU=@Batch ORDER BY GRP
        [HttpGet("branch")]
        public async Task<IActionResult> GetBranch([FromQuery] string course, [FromQuery] string batch)
        {
            if (string.IsNullOrWhiteSpace(course) || string.IsNullOrWhiteSpace(batch))
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "course and batch are required.",
                    Data    = null
                });

            var data = await _svc.LoadBranchAsync(course, batch);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Branch list loaded.",
                Data    = data
            });
        }

        // GET api/universitypcrformate/sp-info?spName=SP_Jntu_Award_JBIET
        // Diagnostic: returns actual parameter list of an SP from sys.parameters
        [HttpGet("sp-info")]
        public async Task<IActionResult> GetSpInfo([FromQuery] string spName = "")
        {
            var data = await _svc.GetSpParamsAsync(spName);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = $"Parameters for [{spName}]",
                Data    = data
            });
        }

        // GET api/universitypcrformate/data?course=BTECH&examMY=NOV2024&regu=17&batch=17&branch=CSE&isGracing=false&isLateral=false
        // btnDownLoad_Click → Get_Univeristy_Formate / Get_Univeristy_Formate_R18 / Get_Univeristy_Formate_Gracing
        //
        // SP selection (matches App_Web_gp3pforx.dll logic):
        //   isGracing=true + regu contains "R16" → SP_Jntu_Award_JBIET_AddGracing_R16  (@Course,@ExamMY,@Regu,@Batch,@Branch)
        //   isGracing=true                        → SP_Jntu_Award_JBIET_AddGracing       (@Course,@ExamMY,@Regu,@Batch,@Branch)
        //   default (incl. R18)                   → SP_Jntu_Award_JBIET                  (@Course,@ExamMY,@Regu)
        //
        // isLateral: ChkLateral (Lateral Entry students) — frontend indicator, handled by SP internally
        // Crystal: Btech_Pc.rpt / MCA_Pc.rpt / MBA_Pc.rpt / Mtech_Pc.rpt (chosen by frontend based on course)
        // QR path: ~/QRimages/PC/
        [HttpGet("data")]
        public async Task<IActionResult> GetData(
            [FromQuery] string course,
            [FromQuery] string examMY,
            [FromQuery] string regu,
            [FromQuery] string batch   = "",
            [FromQuery] string branch  = "",
            [FromQuery] bool isGracing = false,
            [FromQuery] bool isLateral = false)
        {
            if (string.IsNullOrWhiteSpace(course) || string.IsNullOrWhiteSpace(examMY) || string.IsNullOrWhiteSpace(regu))
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "course, examMY, and regu are required.",
                    Data    = null
                });

            var data = await _svc.GetDataAsync(course, examMY, regu, batch, branch, isGracing, isLateral);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Data loaded." : "No data found.",
                Data    = data
            });
        }
    }
}
