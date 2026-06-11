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
    /// Theory Absentees List — Post Exam-Reports module (AbsenteesList.aspx)
    ///
    /// Mirrors the reference project's AbsenteesList.aspx flow:
    ///   1. Page_Load     → GET /semesters  — populates ddlSemester
    ///   2. Select Sem    → GET /edates     — cascades ddlEdate (Get_Exams_Edates)
    ///   3. Click View    → GET /report     — calls SP_REP_ABLIST
    ///
    /// SP params confirmed from DLL IL (App_Web_gp3pforx.dll offset 0x000128e7):
    ///   Regulation(1,varchar), Course(2,varchar), ExamMY(3,varchar),
    ///   SEM(4,INT unquoted optional), EDate(5,varchar yyyy-MM-dd optional)
    ///
    /// Note: ddlBranch is display:none in ASPX — no branches endpoint needed for theory page.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AbsenteesListController : BaseApiController
    {
        private readonly IAbsenteesListService _svc;

        public AbsenteesListController(IAbsenteesListService svc) => _svc = svc;

        /// <summary>
        /// Load semester list (populated on Page_Load)
        /// Inline SQL on tbl_sh: SELECT DISTINCT sem WHERE course=@Course
        ///
        /// GET /api/absenteeslist/semesters?course=B.TECH
        /// </summary>
        [HttpGet("semesters")]
        public async Task<IActionResult> GetSemesters([FromQuery] string course)
        {
            if (string.IsNullOrWhiteSpace(course))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Course is required" });

            var data = await _svc.GetSemestersAsync(course);
            var list = data?.ToList();

            return Ok(new ApiResponse<object>
            {
                Success = list != null && list.Count > 0,
                Message = list != null && list.Count > 0 ? "Semesters loaded" : "No semesters found",
                Data    = list
            });
        }

        /// <summary>
        /// Load exam dates — cascade triggered by ddlSemester_SelectedIndexChanged
        /// Inline SQL via Get_Exams_Edates: SELECT DISTINCT EDate FROM tbl_sh WHERE COURSE=@Course AND sem=@Sem AND ExamMY=@ExamMY
        /// Returns edate1 (dd-MM-yyyy display) and EDATE (raw)
        /// Use the raw EDATE formatted as yyyy-MM-dd when calling /report
        ///
        /// GET /api/absenteeslist/edates?course=B.TECH&examMY=Nov-2024&sem=3
        /// </summary>
        [HttpGet("edates")]
        public async Task<IActionResult> GetEdates(
            [FromQuery] string course,
            [FromQuery] string examMY,
            [FromQuery] string sem)
        {
            if (string.IsNullOrWhiteSpace(course))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Course is required" });
            if (string.IsNullOrWhiteSpace(examMY))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "ExamMY is required" });
            if (string.IsNullOrWhiteSpace(sem))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Sem is required" });

            var data = await _svc.GetEdatesAsync(course, examMY, sem);
            var list = data?.ToList();

            return Ok(new ApiResponse<object>
            {
                Success = list != null && list.Count > 0,
                Message = list != null && list.Count > 0 ? "Exam dates loaded" : "No exam dates found",
                Data    = list
            });
        }

        /// <summary>
        /// Generate Theory Absentees List report
        /// Calls SP_REP_ABLIST — 5 params in order: Regulation, Course, ExamMY, SEM (optional INT), EDate (optional yyyy-MM-dd)
        ///
        /// GET /api/absenteeslist/report?regulation=R19&course=B.TECH&examMY=Nov-2024&sem=3&eDate=2024-11-15
        /// GET /api/absenteeslist/report?regulation=R19&course=B.TECH&examMY=Nov-2024   (all sems, all dates)
        /// </summary>
        [HttpGet("report")]
        public async Task<IActionResult> GetReport(
            [FromQuery] string regulation,
            [FromQuery] string course,
            [FromQuery] string examMY,
            [FromQuery] int? sem,
            [FromQuery] string? eDate)
        {
            if (string.IsNullOrWhiteSpace(regulation))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Regulation is required" });
            if (string.IsNullOrWhiteSpace(course))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Course is required" });
            if (string.IsNullOrWhiteSpace(examMY))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "ExamMY is required" });

            var request = new AbsenteesListRequest
            {
                Regulation = regulation,
                Course     = course,
                ExamMY     = examMY,
                SEM        = sem,
                EDate      = eDate
            };

            var data = await _svc.GetReportDataAsync(request);
            var list = data?.ToList();

            return Ok(new ApiResponse<object>
            {
                Success = list != null && list.Count > 0,
                Message = list != null && list.Count > 0 ? "Absentees list loaded" : "No data found",
                Data    = list
            });
        }
    }
}
