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
    public class ResultGradeSheetController : BaseApiController
    {
        private readonly IResultGradeSheetService _svc;

        public ResultGradeSheetController(IResultGradeSheetService svc)
        {
            _svc = svc;
        }

        // GET api/resultgradesheet/sems?course=BTECH&examMY=NOV2024
        // Page_Load — populate ddlSemester
        // Inline SQL: SELECT DISTINCT cast(SEM as varchar(250)) SEM
        //   FROM tbl_sh WHERE COURSE=@Course AND EXAMMY=@ExamMY ORDER BY SEM
        [HttpGet("sems")]
        public async Task<IActionResult> LoadSems(
            [FromQuery] string course,
            [FromQuery] string examMY)
        {
            var data = await _svc.LoadSemsAsync(course, examMY);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Semesters loaded." : "No semesters found.",
                Data    = data
            });
        }

        // GET api/resultgradesheet/available-sems?course=B.TECH&examMY=May-2024&regu=R20
        // Diagnostic: scans each sem from TBL_SH and checks SP_REP_GRADE_CHKLIST for data.
        // Returns list of { sem, hasData, count } so you know which params to use for testing.
        [HttpGet("available-sems")]
        public async Task<IActionResult> GetAvailableSems(
            [FromQuery] string course,
            [FromQuery] string examMY,
            [FromQuery] string regu)
        {
            if (string.IsNullOrWhiteSpace(course) || string.IsNullOrWhiteSpace(examMY) || string.IsNullOrWhiteSpace(regu))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "course, examMY, and regu are required.", Data = null });

            var semsRaw = await _svc.LoadSemsAsync(course, examMY);
            var sems = semsRaw?.Select(r =>
            {
                if (r is System.Collections.Generic.IDictionary<string, object> d)
                {
                    if (d.TryGetValue("SEM", out var v)) return v?.ToString();
                }
                return r?.ToString();
            }).Where(s => !string.IsNullOrEmpty(s)).ToList() ?? new List<string>();

            var results    = new List<object>();
            int foundCount = 0;
            foreach (var sem in sems)
            {
                var data  = await _svc.GetDataAsync(course, examMY, regu, sem, false, false, "");
                var count = data?.Count() ?? 0;
                if (count > 0) foundCount++;
                results.Add(new { sem, hasData = count > 0, count });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = foundCount > 0
                    ? $"Found grade sheet data in {foundCount} sem(s)."
                    : $"No grade sheet data in any of {sems.Count} sem(s) for course={course} examMY={examMY} regu={regu}.",
                Data = results
            });
        }

        // GET api/resultgradesheet/data?course=BTECH&examMY=NOV2024&regu=R20&sem=3&isRv=false&isReadmit=false&readmitRegu=
        // btnView_Click — regular or RV mode
        // btnreadmitok_Click — readmit mode (readmitRegu from modal popup txtreadmitReulation)
        //
        // SP selection by flags:
        //   isRv=false, isReadmit=false → SP_REP_GRADE_CHKLIST         "Results Sheet"
        //   isRv=true,  isReadmit=false → SP_REP_GRADE_CHKLIST_RV      "Revaluation Results Sheet"
        //   isRv=false, isReadmit=true  → SP_REP_GRADE_CHKLIST_Readmit "Results Sheet (Re-admitted)"
        //   isRv=true,  isReadmit=true  → SP_REP_GRADE_CHKLIST_RV_Readmit "Revaluation Results Sheet (Re-admitted)"
        [HttpGet("data")]
        public async Task<IActionResult> GetData(
            [FromQuery] string course,
            [FromQuery] string examMY,
            [FromQuery] string regu,
            [FromQuery] string sem,
            [FromQuery] bool   isRv       = false,
            [FromQuery] bool   isReadmit  = false,
            [FromQuery] string readmitRegu = "")
        {
            if (string.IsNullOrWhiteSpace(sem))
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Please select Semester.",
                    Data    = null
                });

            if (isReadmit && string.IsNullOrWhiteSpace(readmitRegu))
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Please enter Readmit Regulation.",
                    Data    = null
                });

            var data = await _svc.GetDataAsync(course, examMY, regu, sem, isRv, isReadmit, readmitRegu);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Grade sheet data loaded." : "No data found.",
                Data    = data
            });
        }
    }
}
