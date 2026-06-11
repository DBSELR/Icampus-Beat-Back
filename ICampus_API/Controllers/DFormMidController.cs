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
    /// D Form Mid — Post Exam-Reports module (DFORM_MID.aspx)
    ///
    /// Mirrors the reference project's DFORM_MID.aspx flow:
    ///   1. Page_Load    → GET /semesters — populates ddlSemester
    ///                     (inline SQL on tbl_sh, 3 params: Course + ExamMY + Regulation)
    ///   2. Select ExamType (MID-I or MID-II — static ddlExamType, no cascade)
    ///   3. Select Semester → GET /edates — cascades ddlEdate via proc_LOAD_MID_DATES
    ///                     (ExamType + Sem both needed for date loading)
    ///   4. Select Edate + optionally tick ChkIsreadmitresult
    ///   5. Click View   → GET /report — calls sp_rep_deform_MID or sp_rep_deform_Readmit_MID
    ///
    /// Report SP params confirmed from DLL IL (App_Web_gp3pforx.dll, 6-phase concat at offset 0xc4f1):
    ///   Regulation(1,varchar), Course(2,varchar), ExamMY(3,varchar),
    ///   SEM(4,INT unquoted optional), EDate(5,varchar yyyy-MM-dd optional), ExamType(6,varchar optional)
    ///
    /// NOTE: ddlBranch exists in ASPX but is hidden (display:none) — no branches endpoint needed.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class DFormMidController : BaseApiController
    {
        private readonly IDFormMidService _svc;

        public DFormMidController(IDFormMidService svc) => _svc = svc;

        /// <summary>
        /// Load semester list (populated on Page_Load)
        /// Inline SQL on tbl_sh: SELECT DISTINCT CAST(SEM AS VARCHAR(250)) SEM
        ///   WHERE COURSE=@Course AND Exammy=@ExamMY AND Regulation=@Regulation ORDER BY SEM
        /// All 3 params required (ExamMY included — unlike regular DFORM which omits it)
        ///
        /// GET /api/dformmid/semesters?course=B.TECH&amp;regulation=R19&amp;examMY=Nov-2024
        /// </summary>
        [HttpGet("semesters")]
        public async Task<IActionResult> GetSemesters(
            [FromQuery] string course,
            [FromQuery] string regulation,
            [FromQuery] string examMY)
        {
            if (string.IsNullOrWhiteSpace(course))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Course is required" });
            if (string.IsNullOrWhiteSpace(regulation))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Regulation is required" });
            if (string.IsNullOrWhiteSpace(examMY))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "ExamMY is required" });

            var data = await _svc.GetSemestersAsync(course, regulation, examMY);
            var list = data?.ToList();

            return Ok(new ApiResponse<object>
            {
                Success = list != null && list.Count > 0,
                Message = list != null && list.Count > 0 ? "Semesters loaded" : "No semesters found",
                Data    = list
            });
        }

        /// <summary>
        /// Load exam date list — cascade for DFORM_MID (ddlSemester_SelectedIndexChanged)
        /// Calls proc_LOAD_MID_DATES — 5 params all varchar: Regulation, Course, ExamMY, Sem, ExamType
        /// ExamType must be selected BEFORE triggering this (ddlExamType is filled first in the UI)
        /// Returns column MDATE (used as both value and display)
        ///
        /// GET /api/dformmid/edates?course=B.TECH&amp;regulation=R19&amp;examMY=Nov-2024&amp;sem=3&amp;examType=1
        /// GET /api/dformmid/edates?course=B.TECH&amp;regulation=R19&amp;examMY=Nov-2024&amp;sem=3&amp;examType=2
        /// </summary>
        [HttpGet("edates")]
        public async Task<IActionResult> GetEdates(
            [FromQuery] string course,
            [FromQuery] string regulation,
            [FromQuery] string examMY,
            [FromQuery] string sem,
            [FromQuery] string examType)
        {
            if (string.IsNullOrWhiteSpace(course))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Course is required" });
            if (string.IsNullOrWhiteSpace(regulation))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Regulation is required" });
            if (string.IsNullOrWhiteSpace(examMY))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "ExamMY is required" });
            if (string.IsNullOrWhiteSpace(sem))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Sem is required" });
            if (string.IsNullOrWhiteSpace(examType))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "ExamType is required (1=MID-I, 2=MID-II)" });

            var data = await _svc.GetEdatesAsync(course, regulation, examMY, sem, examType);
            var list = data?.ToList();

            return Ok(new ApiResponse<object>
            {
                Success = list != null && list.Count > 0,
                Message = list != null && list.Count > 0 ? "Exam dates loaded" : "No exam dates found",
                Data    = list
            });
        }

        /// <summary>
        /// Generate D Form Mid report data
        /// Calls sp_rep_deform_MID (isReadmit=false) or sp_rep_deform_Readmit_MID (isReadmit=true)
        /// SP params in order: Regulation, Course, ExamMY, SEM (optional INT), EDate (optional yyyy-MM-dd), ExamType (optional)
        ///
        /// GET /api/dformmid/report?regulation=R19&amp;course=B.TECH&amp;examMY=Nov-2024&amp;sem=3&amp;eDate=2024-11-15&amp;examType=1
        /// GET /api/dformmid/report?regulation=R19&amp;course=B.TECH&amp;examMY=Nov-2024&amp;sem=3&amp;eDate=2024-11-15&amp;examType=2&amp;isReadmit=true
        /// GET /api/dformmid/report?regulation=R19&amp;course=B.TECH&amp;examMY=Nov-2024  (all sems, all dates, all types)
        ///
        /// SEM is INT — do not quote. EDate format: yyyy-MM-dd. ExamType: "1"=MID-I, "2"=MID-II.
        /// </summary>
        [HttpGet("report")]
        public async Task<IActionResult> GetReport(
            [FromQuery] string regulation,
            [FromQuery] string course,
            [FromQuery] string examMY,
            [FromQuery] int? sem,
            [FromQuery] string? eDate,
            [FromQuery] string? examType,
            [FromQuery] bool isReadmit = false)
        {
            if (string.IsNullOrWhiteSpace(regulation))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Regulation is required" });
            if (string.IsNullOrWhiteSpace(course))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Course is required" });
            if (string.IsNullOrWhiteSpace(examMY))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "ExamMY is required" });

            var request = new DFormMidRequest
            {
                Regulation = regulation,
                Course     = course,
                ExamMY     = examMY,
                SEM        = sem,
                EDate      = eDate,
                ExamType   = examType,
                IsReadmit  = isReadmit
            };

            var data = await _svc.GetReportDataAsync(request);
            var list = data?.ToList();

            return Ok(new ApiResponse<object>
            {
                Success = list != null && list.Count > 0,
                Message = list != null && list.Count > 0
                    ? $"D Form Mid{(isReadmit ? " (Readmission)" : "")} loaded"
                    : "No data found",
                Data = list
            });
        }
    }
}
