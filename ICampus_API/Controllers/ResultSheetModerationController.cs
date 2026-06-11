using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResultSheetModerationController : BaseApiController
    {
        private readonly IResultSheetModerationService _svc;

        public ResultSheetModerationController(IResultSheetModerationService svc)
        {
            _svc = svc;
        }

        // GET api/resultsheetmoderation/sems?course=BTECH&examMY=NOV2024&regu=20
        // Page_Load — populate ddlSem
        // SQL: SELECT DISTINCT SEM FROM TBL_SH WHERE REGULATION=@Regu AND EXAMMY=@ExamMY AND COURSE=@Course ORDER BY SEM
        [HttpGet("sems")]
        public async Task<IActionResult> LoadSems(
            [FromQuery] string course,
            [FromQuery] string examMY,
            [FromQuery] string regu)
        {
            var data = await _svc.LoadSemsAsync(course, examMY, regu);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Semesters loaded." : "No semesters found.",
                Data    = data
            });
        }

        // GET api/resultsheetmoderation/data?course=BTECH&examMY=NOV2024&regu=20&sem=3&isReadmit=false&readmitRegu=
        // btnview_Click — load Subject Moderation result sheet
        // SP: isReadmit=true  → SP_REP_MRK_CHKLIST_Readmit_SMFLAG (@Course,@ExamMY,@Regu,@Sem,@ReadmitRegu)
        //                else → SP_REP_MRK_CHKLIST_SMFLAG (@Course,@ExamMY,@Regu,@Sem)
        // Note: chkRv is HIDDEN in ResultSheet_Moderation.aspx (display:none) — not a SP param.
        //       SMFLAG = Subject Moderation Flag. Crystal report: ResTR_SMFLAG.rpt
        // GET api/resultsheetmoderation/available-sems?course=B.TECH&examMY=May-2024&regu=R20
        // Diagnostic: scans each sem from TBL_SH and checks SP_REP_MRK_CHKLIST_SMFLAG for data.
        // Returns list of { sem, hasData, count } so you know which params to use for testing.
        [HttpGet("available-sems")]
        public async Task<IActionResult> GetAvailableSems(
            [FromQuery] string course,
            [FromQuery] string examMY,
            [FromQuery] string regu)
        {
            if (string.IsNullOrWhiteSpace(course) || string.IsNullOrWhiteSpace(examMY) || string.IsNullOrWhiteSpace(regu))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "course, examMY, and regu are required.", Data = null });

            var semsRaw = await _svc.LoadSemsAsync(course, examMY, regu);
            var sems = semsRaw?.Select(r =>
            {
                if (r is System.Collections.Generic.IDictionary<string, object> d)
                {
                    if (d.TryGetValue("SEM", out var v)) return v?.ToString();
                }
                return r?.ToString();
            }).Where(s => !string.IsNullOrEmpty(s)).ToList() ?? new List<string>();

            var results   = new List<object>();
            int foundCount = 0;
            foreach (var sem in sems)
            {
                var data  = await _svc.GetDataAsync(course, examMY, regu, sem, false, "");
                var count = data?.Count() ?? 0;
                if (count > 0) foundCount++;
                results.Add(new { sem, hasData = count > 0, count });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = foundCount > 0
                    ? $"Found SMFLAG data in {foundCount} sem(s)."
                    : $"No SMFLAG data in any of {sems.Count} sem(s) for course={course} examMY={examMY} regu={regu}.",
                Data = results
            });
        }

        [HttpGet("data")]
        public async Task<IActionResult> GetData(
            [FromQuery] string course,
            [FromQuery] string examMY,
            [FromQuery] string regu,
            [FromQuery] string sem,
            [FromQuery] bool   isReadmit   = false,
            [FromQuery] string readmitRegu = "")
        {
            if (string.IsNullOrWhiteSpace(course) || string.IsNullOrWhiteSpace(examMY) ||
                string.IsNullOrWhiteSpace(sem))
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "course, examMY, and sem are required.",
                    Data    = null
                });

            var data = await _svc.GetDataAsync(course, examMY, regu, sem, isReadmit, readmitRegu);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Moderation result sheet data loaded." : "No data found.",
                Data    = data
            });
        }
    }
}
