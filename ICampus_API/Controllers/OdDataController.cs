using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OdDataController : BaseApiController
    {
        private readonly IOdDataService _svc;

        public OdDataController(IOdDataService svc)
        {
            _svc = svc;
        }

        // GET api/oddata/batch?course=
        // Page_Load — ddlBatch population (shared by both OD pages)
        // SQL: SELECT DISTINCT REGU, '20'+REGU+'-'+CAST(REGU+MAXSEM/2 AS VARCHAR) BATCH
        //      FROM TBL_COURSE WHERE COURSE=@Course ORDER BY REGU
        [HttpGet("batch")]
        public async Task<IActionResult> LoadBatch([FromQuery] string course = "")
        {
            var data = await _svc.LoadBatchAsync(course);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Batches loaded." : "No batches found.",
                Data = data
            });
        }

        // GET api/oddata/branch?course=&regu=
        // ddlBatch AutoPostBack → populate ddlBranch
        // SQL: SELECT DISTINCT GRP, GRP+' - '+GSUB BRANCH FROM TBL_COURSE
        //      WHERE COURSE=@Course AND REGU=@Regu ORDER BY GRP
        [HttpGet("branch")]
        public async Task<IActionResult> LoadBranch([FromQuery] string course, [FromQuery] string regu)
        {
            var data = await _svc.LoadBranchAsync(course, regu);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Branches loaded." : "No branches found.",
                Data = data
            });
        }

        // GET api/oddata/list?course=&regu=&branch=&regno=&isLateral=false
        // btnExcel_Click — ODLIST_JBIET.aspx + OD_RegnoWise_subjectData.aspx
        // DAL: ODLIST_REGNO_SUBJECTS (DAL_MRF) via Load_OD_Data (BAL_MRF / BAL_StudentHistory)
        // CODE = 'OD' (On Duty) filter on TBL_SH
        // Params:
        //   course    — required
        //   regu      — required (batch, e.g. "19")
        //   branch    — optional; empty = all branches
        //   regno     — optional; empty = all students in batch+branch
        //   isLateral — optional (ChkLateral); when true → D.LATERAL='Y' filter
        [HttpGet("list")]
        public async Task<IActionResult> GetOdList(
            [FromQuery] string course,
            [FromQuery] string regu,
            [FromQuery] string branch   = "",
            [FromQuery] string regno    = "",
            [FromQuery] bool isLateral  = false)
        {
            if (string.IsNullOrWhiteSpace(course))
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Course is required.",
                    Data = null
                });

            if (string.IsNullOrWhiteSpace(regu))
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Please select Batch.",
                    Data = null
                });

            var data = await _svc.GetOdListAsync(course, regu, branch, regno, isLateral);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "OD list loaded." : "No OD data found.",
                Data = data
            });
        }

        // POST api/oddata/gracing
        // chkgracing / btnExcel_Click → Load_Btech_OD_AddGracing_R16 (BAL_MRF)
        // Runs 3 SPs: PROC_GRACING_GRAFTING → PROC_GRACING_GRAFTING_MRK_UPDATE →
        //             PROC_GRACING_GRAFTING_SH_UPDATE
        // ODLIST_JBIET.aspx only (not OD_RegnoWise_subjectData.aspx)
        // Body: { "course", "regu", "branch", "examMy", "userId", "regLetrl" }
        //   regLetrl: 'R' = Regular, 'L' = Lateral (ChkLateral selection)
        [HttpPost("gracing")]
        public async Task<IActionResult> RunGracing([FromBody] OdGracingRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.Course) ||
                string.IsNullOrWhiteSpace(request.Regu)    ||
                string.IsNullOrWhiteSpace(request.Branch)  ||
                string.IsNullOrWhiteSpace(request.ExamMy))
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Course, Regu, Branch, and ExamMy are required.",
                    Data = null
                });

            var result = await _svc.RunGracingAsync(
                request.Course, request.Regu, request.Branch,
                request.ExamMy, request.UserId ?? string.Empty,
                request.RegLetrl ?? "R");

            return Ok(new ApiResponse<object>
            {
                Success = result >= 0,
                Message = result >= 0 ? "Gracing applied successfully." : "Gracing process failed.",
                Data = result
            });
        }
    }

    public class OdGracingRequest
    {
        public string Course   { get; set; }
        public string Regu     { get; set; }
        public string Branch   { get; set; }
        public string ExamMy   { get; set; }
        public string UserId   { get; set; }
        public string RegLetrl { get; set; }   // 'R' = Regular, 'L' = Lateral
    }
}
