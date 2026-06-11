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
    /// Marks Entry (OMR Wise) — Post-Exams module
    ///
    /// Mirrors the reference project's MarksEntry_OMRwise.aspx flow:
    ///   1. GET /load?omrNumber=&course=&regulation=&examMY= → load student record by OMR number
    ///                                                          (PROC_OMRNUM_UPDATE_Get)
    ///   2. POST /save                                        → save marks for the OMR number
    ///                                                          (SP_OMRSAVEMARKSENTRY)
    ///
    /// Entry modes (from radio buttons in reference project):
    ///   Type = "RV" → Revaluation marks (RbtnRv)
    ///   Type = "V3" → V3 marks (Rbtnv3)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class OmrMarksEntryController : BaseApiController
    {
        private readonly IOmrMarksEntryService _svc;

        public OmrMarksEntryController(IOmrMarksEntryService svc) => _svc = svc;

        /// <summary>
        /// Load student record by OMR Number in exam context
        /// Corresponds to txtomrNumber AutoPostBack (txtomrNumber_TextChanged1) in reference project
        /// Calls PROC_OMRNUM_UPDATE_Get (@Regno, @Course, @Regulation, @ExamMy, @RegSup — confirmed from DLL IL)
        ///
        /// Returns: BATCH, GRP, SEM, TEMPCODE, PCODE, PNAME, OMRNUMBER, TMARKS, RVMARKS, V3MARKS
        ///
        /// GET /api/omrmarksentry/load?omrNumber=A0001&course=B.Tech&regulation=R18&examMY=NOV2023&regSup=R
        /// </summary>
        [HttpGet("load")]
        public async Task<IActionResult> Load(
            [FromQuery] string omrNumber,
            [FromQuery] string course,
            [FromQuery] string regulation,
            [FromQuery] string examMY,
            [FromQuery] string? regSup = null)
        {
            if (string.IsNullOrWhiteSpace(omrNumber))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "OmrNumber is required" });
            if (string.IsNullOrWhiteSpace(course))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Course is required" });
            if (string.IsNullOrWhiteSpace(regulation))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Regulation is required" });
            if (string.IsNullOrWhiteSpace(examMY))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "ExamMY is required" });

            var data = await _svc.LoadByOmrNumberAsync(new OmrMarksLoadRequest
            {
                OmrNumber  = omrNumber,
                Course     = course,
                Regulation = regulation,
                ExamMY     = examMY,
                RegSup     = regSup
            });

            var list = data?.ToList();
            return Ok(new ApiResponse<object>
            {
                Success = list != null && list.Any(),
                Message = list != null && list.Any() ? "Record loaded" : "No record found for given OMR number",
                Data    = list
            });
        }

        /// <summary>
        /// Save marks for the entered OMR number
        /// Corresponds to btnOmrMarksEntry_Click in reference project
        /// Calls SP_OMRSAVEMARKSENTRY (@Regulation, @ExamMy, @Course, @OmrNumber, @Marks, @Type, @Sem)
        ///
        /// Type must be "RV" (Revaluation) or "V3"
        ///
        /// POST /api/omrmarksentry/save
        /// Body: { "Regulation": "R18", "ExamMY": "NOV2023", "Course": "B.Tech", "OmrNumber": "A0001", "Marks": "45", "Type": "RV", "Sem": "3" }
        /// </summary>
        [HttpPost("save")]
        public async Task<IActionResult> Save([FromBody] OmrMarksSaveRequest request)
        {
            if (request == null)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Request body is required" });
            if (request.OmrNumber <= 0)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "OmrNumber is required (use the numeric OMRNUMBER from the load result)" });
            if (string.IsNullOrWhiteSpace(request.Marks))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Marks is required" });

            var type = request.Type?.Trim().ToUpper();
            if (type != "RV" && type != "V3")
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Type must be 'RV' (Revaluation) or 'V3'" });

            var result = await _svc.SaveMarksAsync(request);
            return Ok(new ApiResponse<object>
            {
                Success = result >= 0,
                Message = result >= 0 ? "Marks saved successfully" : "Failed to save marks",
                Data    = new { RowsAffected = result }
            });
        }
    }
}
