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
    public class NominalRollsController : BaseApiController
    {
        private readonly INominalRollsService _svc;
        public NominalRollsController(INominalRollsService svc) => _svc = svc;

        // GET api/nominalrolls/semesters?course=B.Tech&examMY=MAY-2024
        [HttpGet("semesters")]
        public async Task<IActionResult> GetSemesters([FromQuery] string course, [FromQuery] string examMY)
        {
            if (string.IsNullOrWhiteSpace(course))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Course parameter is required" });

            if (string.IsNullOrWhiteSpace(examMY))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "ExamMY parameter is required" });

            var data = await _svc.GetSemestersAsync(course, examMY);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Semesters loaded" : "No semesters found",
                Data = data
            });
        }

        // GET api/nominalrolls/examdates?course=B.Tech&examMY=MAY-2024&sem=1
        [HttpGet("examdates")]
        public async Task<IActionResult> GetExamDates([FromQuery] string course, [FromQuery] string examMY, [FromQuery] string sem)
        {
            if (string.IsNullOrWhiteSpace(course))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Course parameter is required" });

            if (string.IsNullOrWhiteSpace(examMY))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "ExamMY parameter is required" });

            if (string.IsNullOrWhiteSpace(sem))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Sem parameter is required" });

            var data = await _svc.GetExamDatesAsync(course, examMY, sem);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Exam dates loaded" : "No exam dates found",
                Data = data
            });
        }

        // GET api/nominalrolls/rooms?course=B.Tech&examMY=MAY-2024&sem=1&edate=15-05-2024
        [HttpGet("rooms")]
        public async Task<IActionResult> GetRooms([FromQuery] string course, [FromQuery] string examMY, [FromQuery] string sem, [FromQuery] string edate)
        {
            if (string.IsNullOrWhiteSpace(course))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Course parameter is required" });

            if (string.IsNullOrWhiteSpace(examMY))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "ExamMY parameter is required" });

            if (string.IsNullOrWhiteSpace(sem))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Sem parameter is required" });

            if (string.IsNullOrWhiteSpace(edate))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Edate parameter is required" });

            var data = await _svc.GetRoomsAsync(course, examMY, sem, edate);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Rooms loaded" : "No rooms found",
                Data = data
            });
        }

        // GET api/nominalrolls/data?course=B.Tech&examMY=MAY-2024&regulation=R18&sem=1&edate=2024-05-15&room=101&isReadmit=false
        [HttpGet("data")]
        public async Task<IActionResult> GetNominalRollsData([FromQuery] NominalRollsRequest request)
        {
            if (request == null)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Request parameters are required" });

            if (string.IsNullOrWhiteSpace(request.Course))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Course parameter is required" });

            if (string.IsNullOrWhiteSpace(request.ExamMY))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "ExamMY parameter is required" });

            if (string.IsNullOrWhiteSpace(request.Regulation))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Regulation parameter is required" });

            var data = await _svc.GetNominalRollsDataAsync(request);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() 
                    ? $"Nominal rolls data loaded ({(request.IsReadmit ? "Readmit" : "Regular")})" 
                    : $"No nominal rolls data found ({(request.IsReadmit ? "Readmit" : "Regular")})",
                Data = data
            });
        }
    }
}

