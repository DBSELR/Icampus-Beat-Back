using ICampus_Api.Controllers;
using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using ICampus_Models.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ICampus_Api.Controllers
{
    /// <summary>
    /// Reg. No. Wise Marks Entry (Sessional and Practical) — Post-Exams module
    ///
    /// Mirrors the reference project's MarksEntry_Regnowise.aspx flow:
    ///   1. GET /student?regNo= → load student name + SEM + all paper rows
    ///                            (PROC_GETSUBJTS_REGNO_WISE)
    ///   2. POST /save          → save all edited mark rows in bulk
    ///                            (PROC_marksupdate_regnowise — one call per paper)
    ///
    /// Mark fields per paper: SMARKS, TMARKS, RVMARKS, V3, MRK_FIN, PMARKS
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class RegNoMarksEntryController : BaseApiController
    {
        private readonly IRegNoMarksEntryService _svc;

        public RegNoMarksEntryController(IRegNoMarksEntryService svc) => _svc = svc;

        /// <summary>
        /// Load student details and all paper rows by Registration Number
        /// Corresponds to txtregno AutoPostBack → txtregno_TextChanged in reference project
        ///
        /// Returns: RegNo, Name, Sem + list of papers with all mark fields
        /// SP: PROC_GETSUBJTS_REGNO_WISE (@Course, @ExamMy, @Batch, @Sem, @Regno — confirmed from DLL IL)
        ///
        /// GET /api/regnomarsentry/student?regNo=20B91A0501&examMY=NOV2023&batch=2020&sem=3&course=B.TECH
        /// </summary>
        [HttpGet("student")]
        public async Task<IActionResult> GetStudent(
            [FromQuery] string regNo,
            [FromQuery] string? examMY = null,
            [FromQuery] string? batch = null,
            [FromQuery] string? sem = null,
            [FromQuery] string? course = null)
        {
            if (string.IsNullOrWhiteSpace(regNo))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "RegNo is required" });

            var data = await _svc.LoadByRegNoAsync(regNo, examMY, batch, sem, course);

            if (data == null)
                return Ok(new ApiResponse<object> { Success = false, Message = "No data found for the given RegNo" });

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Student data loaded",
                Data    = data
            });
        }

        /// <summary>
        /// Save marks for all paper rows in bulk
        /// Corresponds to btnOmrNo "UPDATE MARKS" click in reference project
        ///
        /// Calls PROC_marksupdate_regnowise once per paper with:
        ///   ASHID, PTYPE, SMARKS, TMARKS, RVMARKS, V3, MRK_FIN, PMARKS
        ///
        /// POST /api/regnomarsentry/save
        /// Body:
        /// {
        ///   "RegNo": "20B91A0501",
        ///   "Papers": [
        ///     { "ASHID": 123, "PTYPE": "T", "SMARKS": "18", "TMARKS": "45", "RVMARKS": null, "V3": null, "MRK_FIN": null, "PMARKS": null },
        ///     { "ASHID": 124, "PTYPE": "P", "SMARKS": null, "TMARKS": null, "RVMARKS": null, "V3": null, "MRK_FIN": null, "PMARKS": "22" }
        ///   ]
        /// }
        /// </summary>
        [HttpPost("save")]
        public async Task<IActionResult> SaveMarks([FromBody] RegNoMarksSaveRequest request)
        {
            if (request == null)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Request body is required" });
            if (string.IsNullOrWhiteSpace(request.RegNo))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "RegNo is required" });
            if (request.Papers == null || request.Papers.Count == 0)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "At least one paper is required" });

            var totalUpdated = await _svc.SaveMarksAsync(request);

            return Ok(new ApiResponse<object>
            {
                Success = totalUpdated >= 0,
                Message = $"Marks updated for {totalUpdated} paper(s)",
                Data    = new { RowsAffected = totalUpdated }
            });
        }
    }
}
