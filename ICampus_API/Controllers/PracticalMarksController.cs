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
    /// Practical Marks Entry — Post-Exams module
    ///
    /// Mirrors the reference project's PracticalMarksEntry.aspx flow:
    ///   1. GET /papers   → populate papers dropdown (PROC_LOADPAPERS_MRKENTRY TYPE='P')
    ///   2. GET /students → load student marks grid  (PROC_LOADMARKS_MRKENTRY  TYPE='P')
    ///   3. POST /save    → save one student's mark  (PROC_UPDATE_MARKS_INT_S_T TYPE='P')
    ///
    /// Marks field stores to tbl_SH.PMARKS.
    /// Valid mark values: numeric 0–PMAX  OR  "AB" (Absent).
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PracticalMarksController : BaseApiController
    {
        private readonly IPracticalMarksService _svc;

        public PracticalMarksController(IPracticalMarksService svc) => _svc = svc;

        /// <summary>
        /// Load papers dropdown for selected Semester + Branch
        /// Only returns papers that have PMAX > 0 (practical marks applicable)
        ///
        /// GET /api/practicalmarks/papers?regulation=R18&examMY=NOV-2024&sem=3&course=B.TECH&grp=CSE
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

            var request = new PracticalMarksPapersRequest
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
        /// Load student marks grid for selected Paper
        /// Returns: aSHID, RegNo, GRP, PCODE, PMARKS (current practical marks), pMax
        ///
        /// GET /api/practicalmarks/students?regulation=R18&examMY=NOV-2024&sem=3&course=B.TECH&grp=CSE&pCode=CS301
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

            var request = new PracticalMarksStudentsRequest
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
        /// Save practical mark for a single student
        /// Updates tbl_SH.PMARKS via PROC_UPDATE_MARKS_INT_S_T (TYPE='P')
        ///
        /// Valid Marks: "0"–"PMAX" (numeric) OR "AB" (Absent)
        ///
        /// POST /api/practicalmarks/save
        /// Body: { "ASHID": 123456, "Marks": "18" }
        ///    or { "ASHID": 123456, "Marks": "AB" }
        /// </summary>
        [HttpPost("save")]
        public async Task<IActionResult> SaveMark([FromBody] PracticalMarksSaveRequest request)
        {
            if (request == null)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Request body is required" });
            if (request.ASHID <= 0)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "ASHID is required" });
            if (string.IsNullOrWhiteSpace(request.Marks))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Marks value is required" });

            var result = await _svc.SaveMarkAsync(request);
            return Ok(new ApiResponse<object>
            {
                Success = result >= 0,
                Message = result >= 0 ? "Mark saved successfully" : "Failed to save mark",
                Data    = new { RowsAffected = result }
            });
        }
    }
}
