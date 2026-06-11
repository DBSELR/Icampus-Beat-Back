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
    public class MidHallTicketController : BaseApiController
    {
        private readonly IMidHallTicketService _svc;

        public MidHallTicketController(IMidHallTicketService svc)
        {
            _svc = svc;
        }

        /// <summary>
        /// Prepare/generate mid hall ticket data
        /// Stored Procedure: SPM_HallTicket_Mid
        /// </summary>
        [HttpPost("prepare")]
        public async Task<IActionResult> PrepareMidHallTickets([FromBody] MidHallTicketRequest request)
        {
            // Validate required parameters
            if (string.IsNullOrWhiteSpace(request.ExamMY) || string.IsNullOrWhiteSpace(request.Course) ||
                string.IsNullOrWhiteSpace(request.Regulation) || string.IsNullOrWhiteSpace(request.ExamType))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "examMY, course, regulation, and examType are required parameters"
                });
            }

            // Validate examType
            if (!request.ExamType.Equals("MID-I", System.StringComparison.OrdinalIgnoreCase) &&
                !request.ExamType.Equals("MID-II", System.StringComparison.OrdinalIgnoreCase) &&
                request.ExamType != "1" && request.ExamType != "2")
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "examType must be 'MID-I' or 'MID-II' (or '1' for MID-I, '2' for MID-II)"
                });
            }

            var rowsAffected = await _svc.PrepareMidHallTicketsAsync(request);

            return Ok(new ApiResponse<object>
            {
                Success = rowsAffected >= 0,
                Message = rowsAffected >= 0 ? "Mid hall tickets prepared successfully" : "Failed to prepare mid hall tickets",
                Data = new { rowsAffected }
            });
        }

        /// <summary>
        /// Get mid hall ticket data after preparation
        /// Queries the tbl_hallticket table populated by SPM_HallTicket_Mid
        /// </summary>
        [HttpGet("data")]
        public async Task<IActionResult> GetMidHallTicketData(
            [FromQuery] string examMY,
            [FromQuery] string course,
            [FromQuery] string regulation,
            [FromQuery] string sem = null,
            [FromQuery] string batch = null,
            [FromQuery] string branch = null,
            [FromQuery] string regno = null,
            [FromQuery] string examType = null)
        {
            // Validate required parameters
            if (string.IsNullOrWhiteSpace(examMY) || string.IsNullOrWhiteSpace(course) ||
                string.IsNullOrWhiteSpace(regulation))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "examMY, course, and regulation are required parameters"
                });
            }

            var request = new MidHallTicketRequest
            {
                ExamMY = examMY,
                Course = course,
                Regulation = regulation,
                Sem = sem ?? string.Empty,
                Batch = batch ?? string.Empty,
                Branch = branch ?? string.Empty,
                Regno = regno ?? string.Empty,
                ExamType = examType ?? string.Empty
            };

            var data = await _svc.GetMidHallTicketDataAsync(request);

            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Mid hall ticket data loaded successfully" : "No mid hall ticket data found",
                Data = data
            });
        }
    }
}

