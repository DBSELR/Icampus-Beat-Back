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
    public class UnblockRegistrationsController : BaseApiController
    {
        private readonly IUnblockRegistrationsService _svc;
        public UnblockRegistrationsController(IUnblockRegistrationsService svc) => _svc = svc;

        // GET api/unblockregistrations/exammy
        [HttpGet("exammy")]
        public async Task<IActionResult> GetExamMy()
        {
            var data = await _svc.LoadExamMyAsync();
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Exam month-years loaded" : "No exam month-years found",
                Data = data
            });
        }

        // GET api/unblockregistrations/blockedstudents?exammy=MAY-2024
        [HttpGet("blockedstudents")]
        public async Task<IActionResult> GetBlockedStudents([FromQuery] string exammy)
        {
            if (string.IsNullOrWhiteSpace(exammy))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Exammy parameter is required" });

            var data = await _svc.LoadBlockedStudentsAsync(exammy);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Blocked students loaded" : "No blocked students found",
                Data = data
            });
        }

        // POST api/unblockregistrations/unblock
        [HttpPost("unblock")]
        public async Task<IActionResult> Unblock([FromBody] UnblockRegistrationsRequest request)
        {
            if (request == null)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Request body is required" });

            if (string.IsNullOrWhiteSpace(request.Exammy))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Exammy is required" });

            if (request.Regnos == null || !request.Regnos.Any())
                return BadRequest(new ApiResponse<object> { Success = false, Message = "At least one registration number is required" });

            // If single regno, use single unblock method
            if (request.Regnos.Count == 1)
            {
                var rows = await _svc.UnblockStudentAsync(request.Exammy, request.Regnos.First());
                return Ok(new ApiResponse<object>
                {
                    Success = rows > 0,
                    Message = rows > 0 ? "Student unblocked successfully" : "Unblock failed",
                    Data = new { RowsAffected = rows }
                });
            }

            // Multiple regnos, use batch method
            var result = await _svc.UnblockMultipleAsync(request.Exammy, request.Regnos);
            return Ok(new ApiResponse<object>
            {
                Success = result.SuccessfullyUnblocked > 0,
                Message = result.SuccessfullyUnblocked > 0 
                    ? $"Successfully unblocked {result.SuccessfullyUnblocked} out of {result.TotalSelected} students"
                    : "Unblock operation failed for all students",
                Data = result
            });
        }
    }
}

