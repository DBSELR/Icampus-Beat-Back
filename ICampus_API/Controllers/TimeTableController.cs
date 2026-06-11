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
    public class TimeTableController : BaseApiController
    {
        private readonly ITimeTableService _svc;
        public TimeTableController(ITimeTableService svc) => _svc = svc;

        // GET api/timetable/semesters?course=B.Tech&examMY=MAY-2024
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

        // GET api/timetable/data?course=B.Tech&examMY=MAY-2024&sem=1
        [HttpGet("data")]
        public async Task<IActionResult> GetTimeTableData([FromQuery] TimeTableRequest request)
        {
            if (request == null)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Request parameters are required" });

            if (string.IsNullOrWhiteSpace(request.Course))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Course parameter is required" });

            if (string.IsNullOrWhiteSpace(request.ExamMY))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "ExamMY parameter is required" });

            var data = await _svc.GetTimeTableDataAsync(request);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Timetable data loaded" : "No timetable data found",
                Data = data
            });
        }
    }
}

