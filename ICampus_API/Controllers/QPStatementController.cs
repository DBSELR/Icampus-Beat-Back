using ICampus_Api.Controllers;
using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using ICampus_Models.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QPStatementController : BaseApiController
    {
        private readonly IQPStatementService _svc;
        public QPStatementController(IQPStatementService svc) => _svc = svc;

        // GET api/qpstatement/semesters?course=B.Tech&regulation=R18&examMY=MAY-2024
        [HttpGet("semesters")]
        public async Task<IActionResult> GetSemesters([FromQuery] string course, [FromQuery] string regulation, [FromQuery] string examMY)
        {
            if (string.IsNullOrWhiteSpace(course))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Course parameter is required" });

            if (string.IsNullOrWhiteSpace(regulation))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Regulation parameter is required" });

            if (string.IsNullOrWhiteSpace(examMY))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "ExamMY parameter is required" });

            var data = await _svc.GetSemestersAsync(course, regulation, examMY);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Semesters loaded" : "No semesters found",
                Data = data
            });
        }

        // GET api/qpstatement/data?course=B.Tech&examMY=MAY-2024&sem=1
        [HttpGet("data")]
        public async Task<IActionResult> GetQPStatementData([FromQuery] QPStatementRequest request)
        {
            if (request == null)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Request parameters are required" });

            if (string.IsNullOrWhiteSpace(request.Course))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Course parameter is required" });

            if (string.IsNullOrWhiteSpace(request.ExamMY))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "ExamMY parameter is required" });

            var data = await _svc.GetQPStatementDataAsync(request);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Question paper statement data loaded" : "No question paper statement data found",
                Data = data
            });
        }
    }
}

