using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BackLogsListController : BaseApiController
    {
        private readonly IBackLogsListService _svc;

        public BackLogsListController(IBackLogsListService svc)
        {
            _svc = svc;
        }

        // ── BackLogsList.aspx ──────────────────────────────────────────────

        // GET api/backlogslist/exammy?regulation=R20&course=CE
        // Load ExamMY dropdown (ddlexammy) on page load
        // SP: SPM_EXAMS_ExamMY_Load (@Regulation, @Course)
        [HttpGet("exammy")]
        public async Task<IActionResult> GetExamMY(
            [FromQuery] string regulation,
            [FromQuery] string course)
        {
            var data = await _svc.LoadExamMYAsync(regulation, course);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "ExamMY list loaded" : "No exam periods found",
                Data = data
            });
        }

        // GET api/backlogslist/batch?course=CE
        // Load Batch dropdown (DDLBatch) — also reloaded on DDLBatch_SelectedIndexChanged
        // Raw SQL: SELECT DISTINCT REGU, BATCH FROM TBL_COURSE WHERE COURSE = @Course
        [HttpGet("batch")]
        public async Task<IActionResult> GetBatch([FromQuery] string course)
        {
            var data = await _svc.LoadBatchAsync(course);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Batch list loaded" : "No batches found",
                Data = data
            });
        }

        // GET api/backlogslist/list?course=CE&regu=R20&examMY=NOV-2024&semFrom=1&semTo=8&noOfBackLogs=3&op=%3D
        // Get BackLogs list (GVBackLogsList) — btnBackLogsList_Click
        // SP: SP_BACKLOGS_LIST (@Course, @REGU, @ExamMY, @SemFrom, @SemTo, @NoOfBackLogs, @Op)
        // Returns: REGNO, SNAME, SECTION, GRP, SEM, PCODE, PNAME, LAST ATTEMPT
        // op: '=' | '<=' | '>=' (cycled by = button; URL-encode as %3D, %3C%3D, %3E%3D)
        [HttpGet("list")]
        public async Task<IActionResult> GetList(
            [FromQuery] string course,
            [FromQuery] string regu,
            [FromQuery] string examMY,
            [FromQuery] string semFrom,
            [FromQuery] string semTo,
            [FromQuery] string noOfBackLogs,
            [FromQuery] string op = "=")
        {
            var data = await _svc.GetBackLogsListAsync(
                course, regu, examMY, semFrom, semTo, noOfBackLogs, op);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "BackLogs list loaded" : "No data found",
                Data = data
            });
        }

        // ── Backlogs_Regno.aspx ────────────────────────────────────────────

        // GET api/backlogslist/regno/data?course=B.TECH&examMY=Nov-2024&regu=20&regulation=R20
        // SP: PROC_Backlogs_RegNowise (@regulation, @course, @exammy, @regu)
        [HttpGet("regno/data")]
        public async Task<IActionResult> GetRegnoData(
            [FromQuery] string course,
            [FromQuery] string examMY,
            [FromQuery] string regu,
            [FromQuery] string? regulation = null)
        {
            var data = await _svc.GetBackLogsRegnoDataAsync(course, examMY, regu, regulation ?? string.Empty);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Backlogs data loaded" : "No data found",
                Data = data
            });
        }

        // GET api/backlogslist/regno/count?course=B.TECH&examMY=Nov-2024&regu=20&regulation=R20
        // SP: Sp_BacklogsData (@Regulation, @Course, @Exammy, @Regu)
        [HttpGet("regno/count")]
        public async Task<IActionResult> GetRegnoCount(
            [FromQuery] string course,
            [FromQuery] string examMY,
            [FromQuery] string regu,
            [FromQuery] string? regulation = null)
        {
            var data = await _svc.GetBackLogsCountAsync(course, examMY, regu, regulation ?? string.Empty);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Backlogs count loaded" : "No data found",
                Data = data
            });
        }
    }
}
