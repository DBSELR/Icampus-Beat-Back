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
    /// Absentees Entry — Post-Exams module
    ///
    /// Mirrors the reference project's AbsenteesEntry.aspx flow:
    ///   1. GET /papers   → populate papers dropdown (PROC_LOADPAPERS_MRKENTRY TYPE='T')
    ///   2. GET /students → load student list for selected paper (PROC_LOADMARKS_MRKENTRY TYPE='T')
    ///   3. POST /save    → save one student's AB/MP code (PROC_UPDATE_MARKS_INT_S_T TYPE='T')
    ///
    /// TYPE='T' confirmed from DLL IL — Theory/External variant
    /// Valid code values: "AB" (Absent) or "MP" (Malpractice)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AbsenteesEntryController : BaseApiController
    {
        private readonly IAbsenteesEntryService _svc;

        public AbsenteesEntryController(IAbsenteesEntryService svc) => _svc = svc;

        /// <summary>
        /// Load papers dropdown for selected Regulation, ExamMY, Semester and Branch
        /// Calls PROC_LOADPAPERS_MRKENTRY with TYPE='T'
        /// SP params (confirmed from DLL IL): @Regulation, @ExamMy, @Sem[INT], @Course, @GRP
        ///
        /// GET /api/absenteesentry/papers?regulation=R19&examMY=Nov-2024&sem=3&course=B.TECH&grp=CSE
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

            var request = new AbsenteesPapersRequest
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
        /// Load student list for selected Paper
        /// Returns: aSHID, RegNo, GRP, PCODE, CODE (current AB/MP status or null)
        ///
        /// GET /api/absenteesentry/students?regulation=R18&examMY=NOV-2024&sem=3&course=B.TECH&grp=CSE&pCode=CS301
        /// </summary>
        [HttpGet("students")]
        public async Task<IActionResult> GetStudents(
            [FromQuery] string regulation,
            [FromQuery] string examMY,
            [FromQuery] string sem,
            [FromQuery] string course,
            [FromQuery] string grp,
            [FromQuery] string? pCode)
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

            var request = new AbsenteesStudentsRequest
            {
                Regulation = regulation,
                ExamMY     = examMY,
                Sem        = sem,
                Course     = course,
                GRP        = grp,
                PCode      = pCode
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
        /// Save absentee code for a single student
        /// Calls PROC_UPDATE_MARKS_INT_S_T with TYPE='T' (confirmed from DLL IL)
        ///
        /// Valid Code values: "AB" (Absent) or "MP" (Malpractice)
        ///
        /// POST /api/absenteesentry/save
        /// Body: { "ASHID": 123456, "Code": "AB" }
        ///    or { "ASHID": 123456, "Code": "MP" }
        /// </summary>
        [HttpPost("save")]
        public async Task<IActionResult> SaveCode([FromBody] AbsenteesSaveRequest request)
        {
            if (request == null)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Request body is required" });
            if (request.ASHID <= 0)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "ASHID is required" });
            if (string.IsNullOrWhiteSpace(request.Code))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Code is required" });

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
