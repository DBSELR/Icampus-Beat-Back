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
    /// Practical Check List — Post Exam-Reports module
    ///
    /// Confirmed from DLL IL (App_Web_gp3pforx.dll + DataAccessLayer.dll):
    ///   - Old project has NO PracticalCheckList_data DAL method
    ///   - Uses same InternalCheckList_data DAL method as InternalChecklist
    ///   - Same SP: SP_REP_INTERNAL_CHKLIST (6 params)
    ///   - Same table: TBL_REPORT_PERIODICAL
    ///   - SP_REP_PRAC_CHKLIST does NOT appear in any old project DLL
    ///
    /// SP params: Course(1), ExamMY(2), Regulation(3), REGU(4), GRP(5 optional), SEM(6 optional)
    /// Dropdowns use same TBL_GPAP / TBL_COURSE queries as InternalChecklist
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PracticalChecklistController : BaseApiController
    {
        private readonly IPracticalChecklistService _svc;

        public PracticalChecklistController(IPracticalChecklistService svc) => _svc = svc;

        /// <summary>
        /// Generate Practical Check List report data
        ///
        /// GET /api/practicalchecklist/report?course=B.Tech&examMY=Sep-2021&regulation=R20&regu=20&grp=CE&sem=2
        /// GET /api/practicalchecklist/report?course=B.Tech&examMY=Sep-2021&regulation=R20&regu=20   (all branches, all sems)
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

            var request = new PracticalChecklistRequest
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
                Message = list != null && list.Count > 0 ? "Practical checklist loaded" : "No data found",
                Data    = list
            });
        }

        /// <summary>
        /// Load semester list for PracticalChecklist dropdown
        /// NOTE: regulation param = 2-char REGU value (e.g. "20"), NOT full "R20"
        ///
        /// GET /api/practicalchecklist/semesters?course=B.Tech&regulation=20
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
        /// Load branch list for PracticalChecklist dropdown
        /// NOTE: regulation param = 2-char REGU value (e.g. "20"), NOT full "R20"
        ///
        /// GET /api/practicalchecklist/branches?course=B.Tech&regulation=20
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
