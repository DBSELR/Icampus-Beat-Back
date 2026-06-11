using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using ICampus_Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace ICampus_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TimeTableAndSeatingController : BaseApiController
    {
        private readonly ITimeTableAndSeatingService _svc;
        public TimeTableAndSeatingController(ITimeTableAndSeatingService svc) => _svc = svc;

        // 1. sems dropdown
        [HttpGet("sems")]
        public async Task<IActionResult> GetSems([FromQuery] string course, [FromQuery] string examMy, [FromQuery] string regulation)
        {
            var data = await _svc.GetSemsExamMyAsync(examMy, course, regulation);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Sems loaded" : "No sems found",
                Data = data
            });
        }

        // 2. exam timetable data (sessions grid)
        [HttpGet("timetable")]
        public async Task<IActionResult> GetTimeTable([FromQuery] string course, [FromQuery] string examMy, [FromQuery] int sem, [FromQuery] string regulation)
        {
            var data = await _svc.GetExamTimeTableDataAsync(examMy, course, sem, regulation);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Timetable loaded" : "No timetable data found",
                Data = data
            });
        }

        // 3. papers list for a sem (optionally by EDATE)
        // For Exam Dates tab: eDate should be null or empty string
        // For Room Allotment tab: eDate is required
        [HttpGet("papers")]
        public async Task<IActionResult> GetPapers([FromQuery] string course, [FromQuery] string examMy, [FromQuery] int sem, [FromQuery] string? eDate, [FromQuery] string regulation)
        {
            // Make eDate optional - if not provided, pass empty string (matches original behavior)
            var data = await _svc.GetPapersWithCodeAsync(examMy, course, sem, eDate ?? string.Empty, regulation);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Papers loaded" : "No papers found",
                Data = data
            });
        }

        // 3b. branches list for a sem (loads after paper selection in Exam Dates tab)
        [HttpGet("branches")]
        public async Task<IActionResult> GetBranches([FromQuery] string course, [FromQuery] string examMy, [FromQuery] int sem, [FromQuery] string regulation)
        {
            var data = await _svc.GetExamBranchAsync(examMy, course, sem, regulation);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Branches loaded" : "No branches found",
                Data = data
            });
        }

        // 4. paper data (dates/rooms etc.) — includes examType to pick EDATE/ESESS for mid/external
        [HttpGet("paper-data")]
        public async Task<IActionResult> GetPaperData([FromQuery] string course, [FromQuery] string examMy, [FromQuery] int sem, [FromQuery] string pcode, [FromQuery] string regulation, [FromQuery] string examType)
        {
            var data = await _svc.GetPapersDataAsync(examMy, course, sem, pcode, regulation, examType);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Paper data loaded" : "No paper data found",
                Data = data
            });
        }

        // 5. exam dates (distinct dates)
        [HttpGet("dates")]
        public async Task<IActionResult> GetExamDates([FromQuery] string course, [FromQuery] string examMy, [FromQuery] int sem, [FromQuery] string regulation)
        {
            var data = await _svc.GetExamDatesAsync(examMy, course, sem, regulation);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Dates loaded" : "No dates found",
                Data = data
            });
        }

        // 6. RA papers list (for room allotment date-based)
        [HttpGet("ra/papers")]
        public async Task<IActionResult> GetRAPapers([FromQuery] string course, [FromQuery] string examMy, [FromQuery] int sem, [FromQuery] string eDate, [FromQuery] string regulation)
        {
            var data = await _svc.GetRAPapersListAsync(examMy, course, sem, eDate, regulation);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "RA papers loaded" : "No RA papers found",
                Data = data
            });
        }

        // 7. RA paper data (room allotment data)
        [HttpGet("ra/paper-data")]
        public async Task<IActionResult> GetRAPaperData([FromQuery] string course, [FromQuery] string examMy, [FromQuery] int sem, [FromQuery] string pcode, [FromQuery] string eDate, [FromQuery] string regulation)
        {
            var data = await _svc.GetRAPapersDataAsync(examMy, course, sem, pcode, eDate, regulation);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "RA paper data loaded" : "No RA paper data found",
                Data = data
            });
        }

        // 8. Update exam session (saves ESESS/ETIME for a sem)
        [HttpPost("save/session")]
        public async Task<IActionResult> SaveExamSession([FromBody] UpdateExamSessionRequest request)
        {
            var rows = await _svc.UpdateExamSessionAsync(request);
            return Ok(new ApiResponse<object>
            {
                Success = rows > 0,
                Message = rows > 0 ? "Exam session saved successfully" : "Save failed"
            });
        }

        // 9. Update exam date (saves EDATE/ESESS/ETIME for a paper)
        [HttpPost("save/date")]
        public async Task<IActionResult> SaveExamDate([FromBody] UpdateExamDateRequest request)
        {
            var rows = await _svc.UpdateExamDateAsync(request);
            return Ok(new ApiResponse<object>
            {
                Success = rows > 0,
                Message = rows > 0 ? "Exam date saved successfully" : "Save failed"
            });
        }

        // 10. Update room numbers (room allotment between a range of regnos)
        [HttpPost("save/rooms")]
        public async Task<IActionResult> SaveRoomAllocation([FromBody] UpdateRoomNumbersRequest request)
        {
            var rows = await _svc.UpdateRoomNumbersAsync(request);
            return Ok(new ApiResponse<object>
            {
                Success = rows > 0,
                Message = rows > 0 ? "Room allocation saved successfully" : "Save failed"
            });
        }

        // 11. Room search/autocomplete
        [HttpGet("rooms/search")]
        public async Task<IActionResult> SearchRooms([FromQuery] string prefixText)
        {
            var data = await _svc.RoomsSearchAsync(prefixText);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Rooms found" : "No rooms found",
                Data = data
            });
        }

        // 12. examdates format (used to produce excel format)
        [HttpGet("dates/format")]
        public async Task<IActionResult> ExamDatesFormat([FromQuery] string regulation, [FromQuery] string course, [FromQuery] string examMy)
        {
            var data = await _svc.ExamDatesFormatAsync(regulation, course, examMy);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Format loaded" : "No data found",
                Data = data
            });
        }
    }
}
