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
    /// Lab Absentees List — Post Exam-Reports module (LabAbsenteesList.aspx)
    ///
    /// Mirrors the reference project's LabAbsenteesList.aspx flow:
    ///   1. Page_Load       → GET /semesters — populates ddlSemester
    ///   2. Select Sem      → GET /branches  — cascades ddlBranch
    ///   3. Select Branch   → GET /subjects  — cascades ddlSubject (loadingSubjects_Absent)
    ///   4. Click View      → GET /report    — inline SQL (no SP): PMARKS IN ('ab','sm')
    ///
    /// No stored procedure for report — uses Crystal Reports embedded SQL / selection formula:
    ///   tbl_SH WHERE Course=@Course AND ExamMY=@ExamMY AND PMARKS IN ('ab','sm')
    ///   [AND PCode=@PCode] [AND grp=@GRP]
    /// Confirmed from DLL IL: App_Web_gp3pforx.dll string pool (Crystal selection formula fragments)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class LabAbsenteesListController : BaseApiController
    {
        private readonly ILabAbsenteesListService _svc;

        public LabAbsenteesListController(ILabAbsenteesListService svc) => _svc = svc;

        /// <summary>
        /// Load semester list (populated on Page_Load)
        /// Inline SQL on tbl_sh: SELECT DISTINCT sem WHERE course=@Course
        ///
        /// GET /api/lababsenteeslist/semesters?course=B.TECH
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
        /// Load branch list — cascade triggered by ddlSemester_SelectedIndexChanged
        /// Inline SQL on TBL_COURSE: SELECT DISTINCT GSUB GRP, GRP ID WHERE COURSE=@Course AND REGU=@Regulation
        ///
        /// GET /api/lababsenteeslist/branches?course=B.TECH&regulation=R19
        /// </summary>
        [HttpGet("branches")]
        public async Task<IActionResult> GetBranches(
            [FromQuery] string course,
            [FromQuery] string regulation)
        {
            if (string.IsNullOrWhiteSpace(course))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Course is required" });
            if (string.IsNullOrWhiteSpace(regulation))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Regulation is required" });

            var data = await _svc.GetBranchesAsync(course, regulation);
            var list = data?.ToList();

            return Ok(new ApiResponse<object>
            {
                Success = list != null && list.Count > 0,
                Message = list != null && list.Count > 0 ? "Branches loaded" : "No branches found",
                Data    = list
            });
        }

        /// <summary>
        /// Load subject list — cascade triggered by ddlBranch_SelectedIndexChanged
        /// Inline SQL confirmed from DLL IL (DataAccessLayer.dll offset 0x25AF7):
        ///   SELECT DISTINCT pno, pcode, pcode+'-'+pname FROM tbl_sh WHERE Course=@Course AND Regu=@Regulation AND grp=@GRP AND sem=@SEM
        ///
        /// GET /api/lababsenteeslist/subjects?course=B.TECH&regulation=R19&grp=CSE&sem=3
        /// </summary>
        [HttpGet("subjects")]
        public async Task<IActionResult> GetSubjects(
            [FromQuery] string course,
            [FromQuery] string regulation,
            [FromQuery] string grp,
            [FromQuery] string sem)
        {
            if (string.IsNullOrWhiteSpace(course))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Course is required" });
            if (string.IsNullOrWhiteSpace(regulation))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Regulation is required" });
            if (string.IsNullOrWhiteSpace(grp))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "GRP (Branch) is required" });
            if (string.IsNullOrWhiteSpace(sem))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Sem is required" });

            var data = await _svc.GetSubjectsAsync(course, regulation, grp, sem);
            var list = data?.ToList();

            return Ok(new ApiResponse<object>
            {
                Success = list != null && list.Count > 0,
                Message = list != null && list.Count > 0 ? "Subjects loaded" : "No subjects found",
                Data    = list
            });
        }

        /// <summary>
        /// Generate Lab Absentees List report (no SP — inline SQL)
        /// Filters: Course, ExamMY (required), PCode and GRP (optional)
        /// PMARKS IN ('ab','sm') is always applied — 'ab'=Absent, 'sm'=Supplementary-Malpractice
        ///
        /// GET /api/lababsenteeslist/report?course=B.TECH&examMY=Nov-2024&pCode=CS301&grp=CSE
        /// GET /api/lababsenteeslist/report?course=B.TECH&examMY=Nov-2024   (all subjects, all branches)
        /// </summary>
        [HttpGet("report")]
        public async Task<IActionResult> GetReport(
            [FromQuery] string course,
            [FromQuery] string examMY,
            [FromQuery] string? pCode,
            [FromQuery] string? grp)
        {
            if (string.IsNullOrWhiteSpace(course))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Course is required" });
            if (string.IsNullOrWhiteSpace(examMY))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "ExamMY is required" });

            var request = new LabAbsenteesListRequest
            {
                Course  = course,
                ExamMY  = examMY,
                PCode   = pCode,
                GRP     = grp
            };

            var data = await _svc.GetReportDataAsync(request);
            var list = data?.ToList();

            return Ok(new ApiResponse<object>
            {
                Success = list != null && list.Count > 0,
                Message = list != null && list.Count > 0 ? "Lab absentees list loaded" : "No data found",
                Data    = list
            });
        }
    }
}
