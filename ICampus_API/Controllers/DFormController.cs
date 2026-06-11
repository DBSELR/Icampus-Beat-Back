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
    /// D Form — Post Exam-Reports module (DFORM.aspx)
    ///
    /// Mirrors the reference project's DFORM.aspx flow:
    ///   1. User selects Semester from ddlSemester → exam dates cascade into ddlEdate
    ///   2. User selects Exam Date from ddlEdate
    ///   3. User optionally checks ChkIsreadmitresult (readmission variant)
    ///   4. Clicks View → GET /report → calls sp_rep_deform or sp_rep_deform_Readmit
    ///
    /// SP params confirmed from DLL IL (App_Web_gp3pforx.dll loadingdform, IL offset 0x0000c3be):
    ///   Regulation(1), Course(2), ExamMY(3), SEM(4 optional), EDate(5 optional, yyyy-MM-dd)
    ///
    /// SP selection: IsReadmit=false → sp_rep_deform, IsReadmit=true → sp_rep_deform_Readmit
    /// EDate format: yyyy-MM-dd (how the original code formats ddlEdate.SelectedValue via .ToString("yyyy-MM-dd"))
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class DFormController : BaseApiController
    {
        private readonly IDFormService _svc;

        public DFormController(IDFormService svc) => _svc = svc;

        /// <summary>
        /// Generate D Form report data
        /// Calls sp_rep_deform (isReadmit=false) or sp_rep_deform_Readmit (isReadmit=true)
        /// SP params in order: Regulation, Course, ExamMY, SEM (optional), EDate (optional)
        ///
        /// GET /api/dform/report?regulation=R19&amp;course=B.TECH&amp;examMY=Nov-2024&amp;sem=3&amp;eDate=2024-11-15
        /// GET /api/dform/report?regulation=R19&amp;course=B.TECH&amp;examMY=Nov-2024&amp;sem=3&amp;eDate=2024-11-15&amp;isReadmit=true
        /// GET /api/dform/report?regulation=R19&amp;course=B.TECH&amp;examMY=Nov-2024  (all sems, all dates)
        ///
        /// EDate format: yyyy-MM-dd (e.g. 2024-11-15)
        /// </summary>
        [HttpGet("report")]
        public async Task<IActionResult> GetReport(
            [FromQuery] string regulation,
            [FromQuery] string course,
            [FromQuery] string examMY,
            [FromQuery] string? sem,
            [FromQuery] string? eDate,
            [FromQuery] bool isReadmit = false)
        {
            if (string.IsNullOrWhiteSpace(regulation))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Regulation is required" });
            if (string.IsNullOrWhiteSpace(course))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Course is required" });
            if (string.IsNullOrWhiteSpace(examMY))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "ExamMY is required" });

            var request = new DFormRequest
            {
                Regulation = regulation,
                Course     = course,
                ExamMY     = examMY,
                SEM        = sem,
                EDate      = eDate,
                IsReadmit  = isReadmit
            };

            var data = await _svc.GetReportDataAsync(request);
            var list = data?.ToList();

            return Ok(new ApiResponse<object>
            {
                Success = list != null && list.Count > 0,
                Message = list != null && list.Count > 0
                    ? $"D Form{(isReadmit ? " (Readmission)" : "")} loaded"
                    : "No data found",
                Data = list
            });
        }

        /// <summary>
        /// Load semester list for DForm (populated on Page_Load)
        /// Uses TBL_SH — different from InternalChecklist which uses TBL_GPAP
        /// Confirmed from DLL IL: DataAccessLayer.dll method DForm_Semester
        ///
        /// GET /api/dform/semesters?course=B.TECH&regulation=R19
        /// </summary>
        [HttpGet("semesters")]
        public async Task<IActionResult> GetSemesters(
            [FromQuery] string course,
            [FromQuery] string regulation)
        {
            if (string.IsNullOrWhiteSpace(course))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Course is required" });
            if (string.IsNullOrWhiteSpace(regulation))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Regulation is required" });

            var data = await _svc.GetSemestersAsync(course, regulation);
            var list = data?.ToList();

            return Ok(new ApiResponse<object>
            {
                Success = list != null && list.Count > 0,
                Message = list != null && list.Count > 0 ? "Semesters loaded" : "No semesters found",
                Data    = list
            });
        }

        /// <summary>
        /// Load exam date list — critical cascade for DForm (ddlSemester_SelectedIndexChanged)
        /// Without this, the user cannot select an EDate to pass to GET /report
        /// Confirmed from DLL IL: DataAccessLayer.dll method DForm_Edate
        /// Returns: edate1 (dd-MM-yyyy for display) and EDATE (raw value)
        /// When calling /report, use the raw EDATE value reformatted as yyyy-MM-dd
        ///
        /// GET /api/dform/edates?course=B.TECH&examMY=Nov-2024&sem=3
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
    }
}
