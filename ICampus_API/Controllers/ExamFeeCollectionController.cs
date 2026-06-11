using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using ICampus_Models.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExamFeeCollectionController : BaseApiController
    {
        private readonly IExamFeeCollectionService _svc;

        public ExamFeeCollectionController(IExamFeeCollectionService svc)
        {
            _svc = svc;
        }

        /// <summary>
        /// Get exam fee collection data for report display
        /// Stored Procedure: SPM_EXAMFEE_COLLECTION
        /// </summary>
        [HttpGet("data")]
        public async Task<IActionResult> GetExamFeeCollectionData(
            [FromQuery] string examMY,
            [FromQuery] string course,
            [FromQuery] string fDate,
            [FromQuery] string tDate,
            [FromQuery] string userID = null)
        {
            // Validate required parameters
            if (string.IsNullOrWhiteSpace(examMY) || string.IsNullOrWhiteSpace(course) ||
                string.IsNullOrWhiteSpace(fDate) || string.IsNullOrWhiteSpace(tDate))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "examMY, course, fDate, and tDate are required parameters"
                });
            }

            // Validate date range
            if (System.DateTime.TryParse(fDate, out var fromDate) &&
                System.DateTime.TryParse(tDate, out var toDate))
            {
                if (fromDate > toDate)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "From date must be less than or equal to To date"
                    });
                }
            }

            var request = new ExamFeeCollectionRequest
            {
                ExamMY = examMY,
                Course = course,
                FDate = fDate,
                TDate = tDate,
                UserID = userID ?? string.Empty
            };

            var data = await _svc.GetExamFeeCollectionDataAsync(request);

            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Exam fee collection data loaded successfully" : "No exam fee collection data found",
                Data = data
            });
        }

        /// <summary>
        /// Get exam fee collection data for Excel export
        /// Stored Procedure: proc_feee_list_overall
        /// </summary>
        [HttpGet("export")]
        public async Task<IActionResult> GetExamFeeCollectionExport(
            [FromQuery] string regulation,
            [FromQuery] string course,
            [FromQuery] string examMY,
            [FromQuery] string fDate,
            [FromQuery] string tDate)
        {
            // Validate required parameters
            if (string.IsNullOrWhiteSpace(regulation) || string.IsNullOrWhiteSpace(course) ||
                string.IsNullOrWhiteSpace(examMY) || string.IsNullOrWhiteSpace(fDate) ||
                string.IsNullOrWhiteSpace(tDate))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "regulation, course, examMY, fDate, and tDate are required parameters"
                });
            }

            // Validate date range
            if (System.DateTime.TryParse(fDate, out var fromDate) &&
                System.DateTime.TryParse(tDate, out var toDate))
            {
                if (fromDate > toDate)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "From date must be less than or equal to To date"
                    });
                }
            }

            var request = new ExamFeeCollectionRequest
            {
                Regulation = regulation,
                Course = course,
                ExamMY = examMY,
                FDate = fDate,
                TDate = tDate
            };

            var data = await _svc.GetExamFeeCollectionExportDataAsync(request);

            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Exam fee collection export data loaded successfully" : "No exam fee collection data found for export",
                Data = data
            });
        }

        // =====================================================================
        // Reports/ExamFeeCollection.aspx — Sem + Branch based
        // =====================================================================

        // GET api/examfeecollection/report/sems?course=BTECH&examMY=NOV2024
        // ddlSemester — Page_Load
        // SQL: SELECT DISTINCT cast(SEM as varchar(250)) SEM FROM tbl_sh
        //      WHERE COURSE=@Course AND EXAMMY=@ExamMY ORDER BY SEM
        [HttpGet("report/sems")]
        public async Task<IActionResult> LoadReportSems(
            [FromQuery] string course,
            [FromQuery] string examMY)
        {
            var data = await _svc.LoadReportSemsAsync(course, examMY);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Semesters loaded." : "No semesters found.",
                Data    = data
            });
        }

        // GET api/examfeecollection/report/branches?course=BTECH
        // ddlBranch — Page_Load
        // SQL: SELECT DISTINCT GRP FROM TBL_COURSE WHERE COURSE=@Course ORDER BY GRP
        // Branch is optional filter (@Branch='' = all branches)
        [HttpGet("report/branches")]
        public async Task<IActionResult> LoadReportBranches([FromQuery] string course)
        {
            var data = await _svc.LoadReportBranchesAsync(course);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Branches loaded." : "No branches found.",
                Data    = data
            });
        }

        // GET api/examfeecollection/report/data?course=BTECH&examMY=NOV2024&regu=20&sem=3&branch=CSE
        // btnview_Click
        // SP: SPR_ExamFee_Collection (@Course, @ExamMY, @Regu, @Sem, @Branch)
        // branch="" = all branches (validation commented out in ASPX)
        [HttpGet("report/data")]
        public async Task<IActionResult> GetReportData(
            [FromQuery] string course,
            [FromQuery] string examMY,
            [FromQuery] string regu,
            [FromQuery] string sem,
            [FromQuery] string branch = "")
        {
            if (string.IsNullOrWhiteSpace(course) || string.IsNullOrWhiteSpace(examMY) ||
                string.IsNullOrWhiteSpace(sem))
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "course, examMY, and sem are required.",
                    Data    = null
                });

            var data = await _svc.GetReportDataAsync(course, examMY, regu, sem, branch ?? string.Empty);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Exam fee collection report data loaded." : "No data found.",
                Data    = data
            });
        }
    }
}

