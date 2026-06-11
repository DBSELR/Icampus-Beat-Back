using ICampus_Api.Controllers;
using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using ICampus_Models.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_Api.Controllers
{
    /// <summary>
    /// Mid Absentees Entry — Post-Exams module
    ///
    /// Mirrors the reference project's AbsenteesEntry_Mid.aspx flow:
    ///   1. GET /papers   → populate papers dropdown (PROC_LOADPAPERS_MRKENTRY TYPE='T')
    ///   2. GET /students → load student list for selected paper + ExamType (PROC_LOADMARKS_MRKENTRY_MID TYPE='T')
    ///   3. POST /save    → save one student's AB/MP code + ExamType (PROC_UPDATE_MID_MARKS_INT_S_T TYPE='T')
    ///
    /// TYPE='T' confirmed from DLL IL for all three SPs
    /// Key differences from AbsenteesEntry:
    ///   - Students SP is PROC_LOADMARKS_MRKENTRY_MID (not PROC_LOADMARKS_MRKENTRY)
    ///   - Save SP is PROC_UPDATE_MID_MARKS_INT_S_T (not PROC_UPDATE_MARKS_INT_S_T)
    ///   - Both students and save SPs take an additional ExamType param ("1"=MID-I, "2"=MID-II)
    ///   - Student grid returns MIDCODE column (not CODE)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class MidAbsenteesEntryController : BaseApiController
    {
        private readonly IMidAbsenteesEntryService _svc;

        public MidAbsenteesEntryController(IMidAbsenteesEntryService svc) => _svc = svc;

        /// <summary>
        /// Load papers dropdown for Mid Absentees Entry
        /// Calls PROC_LOADPAPERS_MRKENTRY with TYPE='T' (same SP as regular AbsenteesEntry)
        /// SP params (confirmed from DLL IL): @Regulation, @ExamMy, @Sem[INT], @Course, @GRP
        ///
        /// GET /api/midabsenteesentry/papers?regulation=R19&examMY=Nov-2024&sem=3&course=B.TECH&grp=CSE
        /// </summary>
        [HttpGet("papers")]
        public async Task<IActionResult> GetPapers(
            [FromQuery] string regulation,
            [FromQuery] string examMY,
            [FromQuery] string sem,
            [FromQuery] string course,
            [FromQuery] string grp)
        {
            if (string.IsNullOrWhiteSpace(regulation))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Regulation is required" });
            if (string.IsNullOrWhiteSpace(examMY))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "ExamMY is required" });
            if (string.IsNullOrWhiteSpace(sem))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Sem is required" });
            if (string.IsNullOrWhiteSpace(course))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Course is required" });
            if (string.IsNullOrWhiteSpace(grp))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "GRP (Branch) is required" });

            var request = new MidAbsenteesPapersRequest
            {
                Regulation = regulation,
                ExamMY     = examMY,
                Sem        = sem,
                Course     = course,
                GRP        = grp
            };

            var data = await _svc.LoadPapersAsync(request);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Papers loaded" : "No papers found",
                Data    = data
            });
        }

        /// <summary>
        /// Load student list for selected Paper and Mid Exam Type
        /// Calls PROC_LOADMARKS_MRKENTRY_MID with TYPE='T' (confirmed from DLL IL)
        /// Returns: aSHID, RegNo, GRP, PCODE, MIDCODE (current AB/MP status or null)
        ///
        /// GET /api/midabsenteesentry/students?regulation=R19&examMY=Nov-2024&sem=3&course=B.TECH&grp=CSE&pCode=CS301&examType=1
        /// ExamType: 1 = MID-I, 2 = MID-II
        /// </summary>
        [HttpGet("students")]
        public async Task<IActionResult> GetStudents(
            [FromQuery] string regulation,
            [FromQuery] string examMY,
            [FromQuery] string sem,
            [FromQuery] string course,
            [FromQuery] string grp,
            [FromQuery] string? pCode,
            [FromQuery] string examType)
        {
            if (string.IsNullOrWhiteSpace(regulation))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Regulation is required" });
            if (string.IsNullOrWhiteSpace(examMY))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "ExamMY is required" });
            if (string.IsNullOrWhiteSpace(sem))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Sem is required" });
            if (string.IsNullOrWhiteSpace(course))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Course is required" });
            if (string.IsNullOrWhiteSpace(grp))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "GRP (Branch) is required" });
            if (string.IsNullOrWhiteSpace(examType))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "ExamType is required (1=MID-I, 2=MID-II)" });

            var request = new MidAbsenteesStudentsRequest
            {
                Regulation = regulation,
                ExamMY     = examMY,
                Sem        = sem,
                Course     = course,
                GRP        = grp,
                PCode      = pCode,
                ExamType   = examType
            };

            var data = await _svc.LoadStudentsAsync(request);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Students loaded" : "No students found",
                Data    = data
            });
        }

        /// <summary>
        /// Save absentee code for a single student in a Mid exam
        /// Calls PROC_UPDATE_MID_MARKS_INT_S_T with TYPE='T' (confirmed from DLL IL)
        /// Params: @Marks, @AshId, @TYPE='T', @ExamType
        ///
        /// Valid Code values: "AB" (Absent) or "MP" (Malpractice)
        /// ExamType: "1" = MID-I, "2" = MID-II
        ///
        /// POST /api/midabsenteesentry/save
        /// Body: { "ASHID": 123456, "Code": "AB", "ExamType": "1" }
        ///    or { "ASHID": 123456, "Code": "MP", "ExamType": "2" }
        /// </summary>
        [HttpPost("save")]
        public async Task<IActionResult> SaveCode([FromBody] MidAbsenteesSaveRequest request)
        {
            if (request == null)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Request body is required" });
            if (request.ASHID <= 0)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "ASHID is required" });
            if (string.IsNullOrWhiteSpace(request.Code))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Code is required" });
            if (string.IsNullOrWhiteSpace(request.ExamType))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "ExamType is required (1=MID-I, 2=MID-II)" });

            var code = request.Code.Trim().ToUpper();
            if (code != "AB" && code != "MP")
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Code must be 'AB' (Absent) or 'MP' (Malpractice)" });

            var result = await _svc.SaveCodeAsync(request);
            return Ok(new ApiResponse<object>
            {
                Success = result >= 0,
                Message = result >= 0 ? "Code saved successfully" : "Failed to save code",
                Data    = new { RowsAffected = result }
            });
        }
    }
}
