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
    /// Internal Check List — Post Exam-Reports module
    ///
    /// Mirrors the reference project's InternalChecklist.aspx flow:
    ///   1. User selects Batch (Regulation), Semester, Branch from dropdowns
    ///   2. Clicks View → GET /report → calls SP_REP_INTERNAL_CHKLIST
    ///
    /// SP params confirmed from DLL IL (App_Web_oxqewfcs.dll, method loadingInternalCheckList):
    ///   Course(1), ExamMY(2), Regulation(3), REGU(4, 2-char batch year), GRP(5, optional), SEM(6, optional)
    ///
    /// Dropdowns (Batch/ExamMY, Semester, Branch) are served by existing DropdownController endpoints.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class InternalChecklistController : BaseApiController
    {
        private readonly IInternalChecklistService _svc;

        public InternalChecklistController(IInternalChecklistService svc) => _svc = svc;

        /// <summary>
        /// Generate Internal Check List report data
        /// Calls SP_REP_INTERNAL_CHKLIST (confirmed from DLL IL)
        /// SP params in order: Course, ExamMY, Regulation, GRP (optional), SEM (optional)
        ///
        /// GET /api/internalchecklist/report?course=B.TECH&examMY=Nov-2024&regulation=R20&regu=20&grp=CSE&sem=3
        /// GET /api/internalchecklist/report?course=B.TECH&examMY=Nov-2024&regulation=R20&regu=20          (all branches, all sems)
        /// GET /api/internalchecklist/report?course=B.TECH&examMY=Nov-2024&regulation=R20&regu=20&grp=CSE  (all sems for CSE)
        /// </summary>
        [HttpGet("report")]
        public async Task<IActionResult> GetReport(
            [FromQuery] string course,
            [FromQuery] string examMY,
            [FromQuery] string regulation,
            [FromQuery] string regu,
            [FromQuery] string? grp,
            [FromQuery] string? sem)
        {
            if (string.IsNullOrWhiteSpace(course))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Course is required" });
            if (string.IsNullOrWhiteSpace(examMY))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "ExamMY is required" });
            if (string.IsNullOrWhiteSpace(regulation))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Regulation is required" });
            if (string.IsNullOrWhiteSpace(regu))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Regu (2-char batch year e.g. '20') is required" });

            var request = new InternalChecklistRequest
            {
                Course     = course,
                ExamMY     = examMY,
                Regulation = regulation,
                Regu       = regu,
                GRP        = grp,
                SEM        = sem
            };

            var data = await _svc.GetReportDataAsync(request);
            var list = data?.ToList();

            return Ok(new ApiResponse<object>
            {
                Success = list != null && list.Count > 0,
                Message = list != null && list.Count > 0 ? "Internal checklist loaded" : "No data found",
                Data    = list
            });
        }

        /// <summary>
        /// Load semester list for InternalChecklist dropdown (ddlBatch_SelectedIndexChanged cascade)
        /// Inline SQL on TBL_GPAP confirmed from DLL IL: DataAccessLayer.dll InternalCheckList_Semester
        ///
        /// GET /api/internalchecklist/semesters?course=B.TECH&regulation=20   ← pass 2-char REGU, NOT full "R20"
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
        /// Load branch list for InternalChecklist dropdown (ddlSemester_SelectedIndexChanged cascade)
        /// Inline SQL on TBL_COURSE confirmed from DLL IL: DataAccessLayer.dll InternalCheckList_Branch
        ///
        /// GET /api/internalchecklist/branches?course=B.TECH&regulation=20   ← pass 2-char REGU, NOT full "R20"
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
    }
}
