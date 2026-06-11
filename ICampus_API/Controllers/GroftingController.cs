using ICampus_Api.Controllers;
using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GroftingController : BaseApiController
    {
        private readonly IGroftingService _svc;

        public GroftingController(IGroftingService svc)
        {
            _svc = svc;
        }

        // GET api/grofting/exammy?course=B.Tech&regulation=R20
        // SP: SPM_EXAMS_ExamMY_Load(@Regulation, @Course)
        // Confirmed: 23 rows for B.Tech/R20
        [HttpGet("exammy")]
        public async Task<IActionResult> GetExamMy([FromQuery] string course, [FromQuery] string regulation)
        {
            var data = await _svc.LoadExamMyAsync(course, regulation);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "ExamMY list loaded" : "No ExamMY found",
                Data = data
            });
        }

        // GET api/grofting/sems?course=B.Tech&examMy=May-2024&regulation=R20
        // SP: SP_SEMS_RP(@REGULATION, @COURSE, @EXAMMY)
        // Confirmed: 4 rows (3,5,7,8) for B.Tech/May-2024/R20
        [HttpGet("sems")]
        public async Task<IActionResult> GetSems(
            [FromQuery] string course, [FromQuery] string examMy, [FromQuery] string regulation)
        {
            var data = await _svc.LoadSemsAsync(course, examMy, regulation);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Semesters loaded" : "No semesters found",
                Data = data
            });
        }

        // POST api/grofting/run
        // SP: SP_GRACING_GROFTING(@EXAMMY, @COURSE) — no semester/pcode params
        // Body: { "course": "B.Tech", "examMy": "May-2024" }
        [HttpPost("run")]
        public async Task<IActionResult> RunGrofting([FromBody] GroftingRunRequest request)
        {
            var result = await _svc.RunGroftingAsync(request.Course, request.ExamMy);

            return Ok(new ApiResponse<object>
            {
                Success = result >= 0,
                Message = result >= 0 ? "Grofting process completed" : "Grofting process failed",
                Data = result
            });
        }
    }

    public class GroftingRunRequest
    {
        public string Course  { get; set; }
        public string ExamMy  { get; set; }
    }
}
