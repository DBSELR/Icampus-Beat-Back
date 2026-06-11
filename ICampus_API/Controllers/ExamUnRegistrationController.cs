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
    public class ExamUnRegistrationController : BaseApiController
    {
        private readonly IExamUnRegistrationService _svc;
        public ExamUnRegistrationController(IExamUnRegistrationService svc) => _svc = svc;

        // GET api/examunregistration/loaddata?regulation=R18&course=B.Tech&examMY=Jul-2021&regno=18671A05C0
        [HttpGet("loaddata")]
        public async Task<IActionResult> LoadData([FromQuery] string regulation, [FromQuery] string course, 
            [FromQuery] string examMY, [FromQuery] string regno)
        {
            if (string.IsNullOrWhiteSpace(regulation))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Regulation parameter is required" });

            if (string.IsNullOrWhiteSpace(course))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Course parameter is required" });

            if (string.IsNullOrWhiteSpace(examMY))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "ExamMY parameter is required" });

            if (string.IsNullOrWhiteSpace(regno))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Regno parameter is required" });

            if (regno.Trim().Length < 10)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Registration number must be at least 10 characters" });

            var data = await _svc.LoadDataAsync(regulation, course, examMY, regno);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Registration data loaded" : "No registration data found",
                Data = data
            });
        }

        // POST api/examunregistration/unregister
        [HttpPost("unregister")]
        public async Task<IActionResult> UnRegister([FromBody] UnRegistrationRequest request)
        {
            if (request == null)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Request body is required" });

            if (string.IsNullOrWhiteSpace(request.Regulation))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Regulation is required" });

            if (string.IsNullOrWhiteSpace(request.Course))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Course is required" });

            if (string.IsNullOrWhiteSpace(request.ExamMY))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "ExamMY is required" });

            if (string.IsNullOrWhiteSpace(request.Regno))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Regno is required" });

            if (request.Regno.Trim().Length < 10)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Registration number must be at least 10 characters" });

            var rows = await _svc.UnRegisterAsync(request);
            return Ok(new ApiResponse<object>
            {
                Success = rows > 0,
                Message = rows > 0 ? "Unregistered successfully" : "Unregistration failed",
                Data = new { RowsAffected = rows }
            });
        }
    }
}

