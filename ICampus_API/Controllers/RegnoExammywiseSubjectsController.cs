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
    /// Regno Exammywise Subjects — Post-Exams module
    ///
    /// Mirrors the reference project's Regno_Exammywise_Subjets.aspx flow:
    ///   1. GET /load  → load all subject rows for a student by RegNo
    ///                   (PROC_OMRNUM_UPDATE — @Regno, @Course, @Regulation, @ExamMy)
    ///   2. POST /save → save edited marks for all paper rows in bulk
    ///                   (PROC_marksupdate_regnowise — once per paper row)
    ///
    /// All SP params confirmed from DLL IL analysis (method loadOmrNumUpdate and btnOmrNo_Click)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class RegnoExammywiseSubjectsController : BaseApiController
    {
        private readonly IRegnoExammywiseSubjectsService _svc;

        public RegnoExammywiseSubjectsController(IRegnoExammywiseSubjectsService svc) => _svc = svc;

        /// <summary>
        /// Load all subject rows for a student by Registration Number
        /// Corresponds to txtregno AutoPostBack → txtregno_TextChanged in reference project
        ///
        /// SP: PROC_OMRNUM_UPDATE
        /// Params confirmed from DLL IL (4 positional, all varchar): @Regno, @Course, @Regulation, @ExamMy
        /// Returns: aSHID, PTYPE, Exammy, sem, PCode, TempCode, PName, smarks, TMARKS, RVMARKS, V3, MRK_FIN, pmarks
        ///
        /// GET /api/regnoexammywisesubjects/load?regNo=20671A0162&course=B.TECH&regulation=R19&examMY=Nov-2024
        /// </summary>
        [HttpGet("load")]
        public async Task<IActionResult> LoadSubjects(
            [FromQuery] string regNo,
            [FromQuery] string course,
            [FromQuery] string regulation,
            [FromQuery] string examMY)
        {
            if (string.IsNullOrWhiteSpace(regNo))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "RegNo is required" });
            if (string.IsNullOrWhiteSpace(course))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Course is required" });
            if (string.IsNullOrWhiteSpace(regulation))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Regulation is required" });
            if (string.IsNullOrWhiteSpace(examMY))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "ExamMY is required" });

            var request = new RegnoExammywiseLoadRequest
            {
                RegNo      = regNo.Trim().ToUpper(),
                Course     = course,
                Regulation = regulation,
                ExamMY     = examMY
            };

            var data = await _svc.LoadSubjectsAsync(request);
            var list = data?.ToList();

            return Ok(new ApiResponse<object>
            {
                Success = list != null && list.Any(),
                Message = list != null && list.Any() ? "Subjects loaded" : "No subjects found",
                Data    = list
            });
        }

        /// <summary>
        /// Load subject rows filtered by Reg/Sup type — triggered by btnGetData with ddlSemType
        /// Corresponds to Get_LoadOmrGrid → Get_loadOmrNumUpdate in the compiled code-behind
        ///
        /// SP: PROC_OMRNUM_UPDATE_Get
        /// Params confirmed from DLL IL (5 positional, all varchar): @Regno, @Course, @Regulation, @ExamMy, @RegSup
        ///
        /// GET /api/regnoexammywisesubjects/load-get?regNo=20671A0162&course=B.TECH&regulation=R19&examMY=Nov-2024&regSup=Reg
        /// GET /api/regnoexammywisesubjects/load-get?regNo=20671A0162&course=B.TECH&regulation=R19&examMY=Nov-2024&regSup=Sup
        /// </summary>
        [HttpGet("load-get")]
        public async Task<IActionResult> LoadSubjectsByRegSup(
            [FromQuery] string regNo,
            [FromQuery] string course,
            [FromQuery] string regulation,
            [FromQuery] string examMY,
            [FromQuery] string regSup)
        {
            if (string.IsNullOrWhiteSpace(regNo))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "RegNo is required" });
            if (string.IsNullOrWhiteSpace(course))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Course is required" });
            if (string.IsNullOrWhiteSpace(regulation))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Regulation is required" });
            if (string.IsNullOrWhiteSpace(examMY))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "ExamMY is required" });
            if (string.IsNullOrWhiteSpace(regSup))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "RegSup is required (Reg or Sup)" });

            var request = new RegnoExammywiseGetLoadRequest
            {
                RegNo      = regNo.Trim().ToUpper(),
                Course     = course,
                Regulation = regulation,
                ExamMY     = examMY,
                RegSup     = regSup
            };

            var data = await _svc.LoadSubjectsByRegSupAsync(request);
            var list = data?.ToList();

            return Ok(new ApiResponse<object>
            {
                Success = list != null && list.Any(),
                Message = list != null && list.Any() ? "Subjects loaded" : "No subjects found",
                Data    = list
            });
        }

        /// <summary>
        /// Save marks for all paper rows in bulk
        /// Corresponds to btnOmrNo "UPDATE MARKS" click in reference project
        ///
        /// SP: PROC_marksupdate_regnowise — called once per paper row
        /// Params confirmed from DLL IL (7 positional): @AshId, @Smarks, @Pmarks, @Tmarks, @Rvmarks, @v3marks, @Tmrk_final
        ///
        /// POST /api/regnoexammywisesubjects/save
        /// Body:
        /// {
        ///   "RegNo": "20671A0162",
        ///   "Papers": [
        ///     { "ASHID": 123456, "SMARKS": "18", "TMARKS": "45", "PMARKS": null, "RVMARKS": null, "V3": null, "MRK_FIN": null }
        ///   ]
        /// }
        /// </summary>
        [HttpPost("save")]
        public async Task<IActionResult> SaveMarks([FromBody] RegnoExammywiseSaveRequest request)
        {
            if (request == null)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Request body is required" });
            if (string.IsNullOrWhiteSpace(request.RegNo))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "RegNo is required" });
            if (request.Papers == null || request.Papers.Count == 0)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "At least one paper row is required" });

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
