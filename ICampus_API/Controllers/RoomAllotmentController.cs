using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Requests;
using ICampus_Models.Common;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ICampus_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomAllotmentController : BaseApiController
    {
        private readonly IRoomAllotmentService _svc;
        public RoomAllotmentController(IRoomAllotmentService svc) => _svc = svc;

        // 1. GET sems — only sems in the current exam notification (EXAMMY)
        [HttpGet("sems")]
        public async Task<IActionResult> GetSems([FromQuery] string course, [FromQuery] string examMy, [FromQuery] string regulation)
        {
            var data = await _svc.LoadSemsAsync(course, examMy, regulation);
            return Ok(new ApiResponse<object>
            {
                Success = data != null,
                Message = data != null ? "Sems loaded" : "No sems found",
                Data = data
            });
        }

        // 2. GET sessions
        [HttpGet("sessions")]
        public async Task<IActionResult> GetSessions([FromQuery] string course, [FromQuery] string sem, [FromQuery] string examType)
        {
            var data = await _svc.LoadSessionsAsync(course, sem, examType);
            return Ok(new ApiResponse<object>
            {
                Success = data != null,
                Message = data != null ? "Sessions loaded" : "No sessions found",
                Data = data
            });
        }

        // 3. GET exam dates
        [HttpGet("examdates")]
        public async Task<IActionResult> GetExamDates([FromQuery] string course, [FromQuery] string sem, [FromQuery] string examMy, [FromQuery] string section, [FromQuery] string regulation, [FromQuery] string examType)
        {
            var req = new RoomAllotSemanticRequest
            {
                Course = course,
                Sem = sem,
                ExamMy = examMy,
                DaySession = section,
                Regulation = regulation,
                ExamType = examType
            };

            var data = await _svc.LoadExamDatesAsync(req);
            return Ok(new ApiResponse<object>
            {
                Success = data != null,
                Message = data != null ? "Exam dates loaded" : "No exam dates",
                Data = data
            });
        }

        // 4. GET rooms
        [HttpGet("rooms")]
        public async Task<IActionResult> GetRooms([FromQuery] string course, [FromQuery] string session)
        {
            var data = await _svc.LoadRoomsAsync(course, session);
            return Ok(new ApiResponse<object>
            {
                Success = data != null,
                Message = data != null ? "Rooms loaded" : "No rooms found",
                Data = data
            });
        }

        // 5. GET pcodes
        [HttpGet("pcodes")]
        public async Task<IActionResult> GetPcodes([FromQuery] string course, [FromQuery] string regulation, [FromQuery] string examMy, [FromQuery] string examDate, [FromQuery] int sem, [FromQuery] string regsup = "REG")
        {
            var req = new RoomAllotSemanticRequest
            {
                Course = course,
                Regulation = regulation,
                ExamMy = examMy,
                EDate = examDate,
                Sem = sem.ToString(),
                Regsup = regsup
            };
            var data = await _svc.LoadPcodesAsync(req);
            return Ok(new ApiResponse<object>
            {
                Success = data != null,
                Message = data != null ? "Pcodes loaded" : "No pcodes",
                Data = data
            });
        }

        // 6. GET regnos
        [HttpGet("regnos")]
        public async Task<IActionResult> GetRegnos([FromQuery] string course, [FromQuery] string regulation, [FromQuery] string examMy, [FromQuery] string examDate,
            [FromQuery] int sem, [FromQuery] string roomNo, [FromQuery] string regsup = "REG", [FromQuery] string examType = "External", [FromQuery] string session = "", [FromQuery] string pcode = null, [FromQuery] int maxnum = 10)
        {
            var req = new RoomAllotSemanticRequest
            {
                Course = course,
                Regulation = regulation,
                ExamMy = examMy,
                EDate = examDate,
                Sem = sem.ToString(),
                RoomNo = roomNo,
                Regsup = regsup,
                ExamType = examType,
                DaySession = session,
                Pcode = pcode,
                MaxNum = maxnum
            };

            var data = await _svc.LoadRegnosAsync(req);
            return Ok(new ApiResponse<object>
            {
                Success = data != null,
                Message = data != null ? "Regnos loaded" : "No regnos",
                Data = data
            });
        }

        // 7. GET already alloted data
        [HttpGet("alloted")]
        public async Task<IActionResult> GetAlloted([FromQuery] string course, [FromQuery] string examMy, [FromQuery] string sem, [FromQuery] string roomNo,
            [FromQuery] string examDate, [FromQuery] string daySession, [FromQuery] string regsup = "REG", [FromQuery] string examType = "External")
        {
            var req = new RoomAllotSemanticRequest
            {
                Course = course,
                ExamMy = examMy,
                Sem = sem,
                RoomNo = roomNo,
                EDate = examDate,
                DaySession = daySession,
                Regsup = regsup,
                ExamType = examType
            };

            var data = await _svc.LoadAllotedDataAsync(req);
            return Ok(new ApiResponse<object>
            {
                Success = data != null,
                Message = data != null ? "Allotted data loaded" : "No data",
                Data = data
            });
        }

        // 8. POST save room allotment
        [HttpPost("save")]
        public async Task<IActionResult> Save([FromBody] RoomAllotmentSaveRequest request)
        {
            var rows = await _svc.SaveRoomAllotmentAsync(request);
            return Ok(new ApiResponse<object>
            {
                Success = rows > 0,
                Message = rows > 0 ? "Room allotted successfully" : "Save failed",
                Data = rows
            });
        }

        // 9. POST reset room
        [HttpPost("reset")]
        public async Task<IActionResult> Reset([FromBody] RoomResetRequest request)
        {
            var res = await _svc.ResetRoomAsync(request);
            return Ok(new ApiResponse<object>
            {
                Success = res > 0,
                Message = res > 0 ? "Room reset successfully" : "Reset failed",
                Data = res
            });
        }

        // 10. GET export (returns raw data to be converted to Excel by client or server)
        [HttpGet("export")]
        public async Task<IActionResult> Export([FromQuery] string course, [FromQuery] string examMy, [FromQuery] string sem, [FromQuery] string examDate, [FromQuery] string daySession, [FromQuery] string roomNo)
        {
            var req = new ExcelExportRequest
            {
                Course = course,
                ExamMy = examMy,
                Sem = sem,
                ExamDate = examDate,
                DaySession = daySession ?? string.Empty,
                RoomNo = roomNo
            };

            var data = await _svc.ExportExcelAsync(req);
            return Ok(new ApiResponse<object>
            {
                Success = data != null,
                Message = data != null ? "Export data loaded" : "No export data",
                Data = data
            });
        }

        // 11. POST apply all dates
        [HttpPost("applyalldates")]
        public async Task<IActionResult> ApplyAllDates([FromBody] ApplyAllDatesRequest request)
        {
            var res = await _svc.ApplyAllDatesAsync(request);
            return Ok(new ApiResponse<object>
            {
                Success = res > 0,
                Message = res > 0 ? "Applied seating across dates" : "Apply failed",
                Data = res
            });
        }

        // 12. GET room master configuration (NOOFROWS, NOOFCOLUMNS)
        [HttpGet("roomconfig")]
        public async Task<IActionResult> GetRoomConfig([FromQuery] string roomNo, [FromQuery] string daySession)
        {
            var data = await _svc.GetRoomMasterConfigAsync(roomNo, daySession);
            return Ok(new ApiResponse<object>
            {
                Success = data != null,
                Message = data != null ? "Room config loaded" : "No room config found",
                Data = data
            });
        }

        // 13. GET course groups (for column ListBoxes)
        [HttpGet("coursegroups")]
        public async Task<IActionResult> GetCourseGroups([FromQuery] string course, [FromQuery] string regulation)
        {
            var data = await _svc.LoadCourseGroupsAsync(course, regulation);
            return Ok(new ApiResponse<object>
            {
                Success = data != null,
                Message = data != null ? "Course groups loaded" : "No course groups found",
                Data = data
            });
        }

        // 14. GET student data with row order
        [HttpGet("students/roworder")]
        public async Task<IActionResult> GetStudentsWithRowOrder([FromQuery] int rowCount, [FromQuery] string groups)
        {
            var data = await _svc.LoadStudentDataWithRowOrderAsync(rowCount, groups);
            return Ok(new ApiResponse<object>
            {
                Success = data != null,
                Message = data != null ? "Student data loaded" : "No student data found",
                Data = data
            });
        }

        // 15. GET students from RoomAllotment table
        [HttpGet("students/roomallotment")]
        public async Task<IActionResult> GetStudentsFromRoomAllotment([FromQuery] int count, [FromQuery] string groups)
        {
            var data = await _svc.LoadStudentsFromRoomAllotmentAsync(count, groups);
            return Ok(new ApiResponse<object>
            {
                Success = data != null,
                Message = data != null ? "Student data loaded" : "No student data found",
                Data = data
            });
        }

        // 16. POST reset student status (mark as available)
        [HttpPost("students/resetstatus")]
        public async Task<IActionResult> ResetStudentStatus([FromQuery] string regno)
        {
            var res = await _svc.ResetStudentStatusAsync(regno);
            return Ok(new ApiResponse<object>
            {
                Success = res > 0,
                Message = res > 0 ? "Student status reset" : "Reset failed",
                Data = res
            });
        }

        // 17. POST mark student as allocated
        [HttpPost("students/markallocated")]
        public async Task<IActionResult> MarkStudentAllocated([FromQuery] string regno)
        {
            var res = await _svc.MarkStudentAsAllocatedAsync(regno);
            return Ok(new ApiResponse<object>
            {
                Success = res > 0,
                Message = res > 0 ? "Student marked as allocated" : "Mark failed",
                Data = res
            });
        }

        // 18. GET unallocated students count
        [HttpGet("students/unallocated/count")]
        public async Task<IActionResult> GetUnallocatedCount()
        {
            var count = await _svc.GetUnallocatedStudentsCountAsync();
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Unallocated count retrieved",
                Data = count
            });
        }
    }
}
