using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FailedInResultPassedInSubController : BaseApiController
    {
        private readonly IFailedInResultPassedInSubService _svc;

        public FailedInResultPassedInSubController(IFailedInResultPassedInSubService svc)
        {
            _svc = svc;
        }

        // GET api/failedinresultpassedinsub/data?regulation=R20&course=B.TECH&examMY=May-2024
        // Page_Load — loads on page open (no View button)
        // SP: SP_SubJ_FAILEDLIST_NEW (@regulation, @course, @EXAMMY, @sem='')
        // Returns: students who failed overall semester result but passed in individual subjects
        // Crystal Report: FailedResult.rpt ("Passed_List Subjectwise")
        [HttpGet("data")]
        public async Task<IActionResult> GetData(
            [FromQuery] string regulation,
            [FromQuery] string course,
            [FromQuery] string examMY)
        {
            if (string.IsNullOrWhiteSpace(course) || string.IsNullOrWhiteSpace(examMY))
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Course and ExamMY are required.",
                    Data    = null
                });

            var data = await _svc.GetDataAsync(regulation ?? string.Empty, course, examMY);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Data loaded." : "No data found.",
                Data    = data
            });
        }
    }
}
