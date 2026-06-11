using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Requests;
using ICampus_Models.Common;
using Microsoft.AspNetCore.Mvc;

namespace ICampus_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DropdownController : BaseApiController
    {
        private readonly IDropdownService _svc;
        public DropdownController(IDropdownService svc) => _svc = svc;

        [HttpGet("regulations")]
        public async Task<IActionResult> GetRegulations()
        {
            var data = await _svc.GetRegulationsAsync();
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Regulations loaded",
                Data = data
            });
        }

        [HttpGet("courses")]
        public async Task<IActionResult> GetCourses([FromQuery] string regulation)
        {
            var data = await _svc.GetCoursesAsync(regulation);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Courses loaded",
                Data = data
            });
        }

        [HttpGet("exammy")]
        public async Task<IActionResult> GetExamMY([FromQuery] string regulation, [FromQuery] string course)
        {
            var data = await _svc.GetExamMYAsync(regulation, course);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "ExamMY loaded",
                Data = data
            });
        }

        [HttpPost("save-selection")]
        public async Task<IActionResult> SaveUserSelection([FromBody] SaveUserSelectionRequest request)
        {
            var rows = await _svc.SaveUserSelectionAsync(request, UserId);

            return Ok(new ApiResponse<object>
            {
                Success = rows > 0,
                Message = rows > 0 ? "Selection saved successfully" : "Failed to save selection"
            });
        }
    }
}
