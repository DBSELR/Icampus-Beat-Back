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
    public class OMRSheetController : BaseApiController
    {
        private readonly IOMRSheetService _svc;
        public OMRSheetController(IOMRSheetService svc) => _svc = svc;

        // GET api/omrsheet/semesters?course=B.Tech&examMY=MAY-2024
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

        // GET api/omrsheet/examdates?course=B.Tech&examMY=MAY-2024&sem=1
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

        // GET api/omrsheet/rooms?course=B.Tech&examMY=MAY-2024&sem=1&edate=15-05-2024
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

        // GET api/omrsheet/data?regulation=R18&course=B.Tech&examMY=MAY-2024&sem=1&edate=15-05-2024&room=101
        [HttpGet("data")]
        public async Task<IActionResult> GetOMRSheetData([FromQuery] OMRSheetRequest request)
        {
            if (request == null)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Request parameters are required" });

            if (string.IsNullOrWhiteSpace(request.Regulation))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Regulation parameter is required" });

            if (string.IsNullOrWhiteSpace(request.Course))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Course parameter is required" });

            if (string.IsNullOrWhiteSpace(request.ExamMY))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "ExamMY parameter is required" });

            if (string.IsNullOrWhiteSpace(request.Sem))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Sem parameter is required" });

            if (!int.TryParse(request.Sem, out _))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Sem parameter must be a valid integer" });

            var data = await _svc.GetOMRSheetDataAsync(request);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "OMR sheet data loaded" : "No OMR sheet data found",
                Data = data
            });
        }

        // POST api/omrsheet/generateomrnumbers
        [HttpPost("generateomrnumbers")]
        public async Task<IActionResult> GenerateOMRNumbers([FromBody] OMRSheetRequest request)
        {
            if (request == null)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Request body is required" });

            if (string.IsNullOrWhiteSpace(request.ExamMY))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "ExamMY is required" });

            if (string.IsNullOrWhiteSpace(request.Course))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Course is required" });

            if (string.IsNullOrWhiteSpace(request.Regulation))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Regulation is required" });

            var results = await _svc.GenerateOMRNumbersAsync(request.ExamMY, request.Course, request.Regulation);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "OMR numbers generated successfully",
                Data = new { RowsAffected = results, TotalRanges = results.Count }
            });
        }

        // GET api/omrsheet/export?examMY=MAY-2024&course=B.Tech&regulation=R18
        [HttpGet("export")]
        public async Task<IActionResult> ExportOMRData([FromQuery] string examMY, [FromQuery] string course, [FromQuery] string regulation)
        {
            if (string.IsNullOrWhiteSpace(examMY))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "ExamMY parameter is required" });

            if (string.IsNullOrWhiteSpace(course))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Course parameter is required" });

            if (string.IsNullOrWhiteSpace(regulation))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Regulation parameter is required" });

            // First generate OMR numbers
            await _svc.GenerateOMRNumbersAsync(examMY, course, regulation);

            // Then get data for export
            var data = await _svc.GetOMRDataForExportAsync(examMY, course, regulation);
            
            // Note: Excel export should be handled by a separate service or library
            // For now, return JSON data. Frontend can use libraries like ExcelJS, xlsx, etc. to generate Excel
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "OMR data ready for export" : "No OMR data found",
                Data = data
            });
        }
    }
}

