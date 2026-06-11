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
    public class CancelReceiptListController : BaseApiController
    {
        private readonly ICancelReceiptListService _svc;

        public CancelReceiptListController(ICancelReceiptListService svc)
        {
            _svc = svc;
        }

        /// <summary>
        /// Get list of courses for dropdown
        /// Stored Procedure: SPM_COURSE_LIST
        /// </summary>
        [HttpGet("courses")]
        public async Task<IActionResult> GetCourses([FromQuery] string regulation)
        {
            if (string.IsNullOrWhiteSpace(regulation))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "regulation is a required parameter"
                });
            }

            var data = await _svc.GetCoursesAsync(regulation);

            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Courses loaded successfully" : "No courses found",
                Data = data
            });
        }

        /// <summary>
        /// Get list of exam month-years for dropdown
        /// Stored Procedure: SPM_EXAMS_ExamMY_Load
        /// </summary>
        [HttpGet("exammys")]
        public async Task<IActionResult> GetExamMYs([FromQuery] string regulation, [FromQuery] string course)
        {
            if (string.IsNullOrWhiteSpace(regulation) || string.IsNullOrWhiteSpace(course))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "regulation and course are required parameters"
                });
            }

            var data = await _svc.GetExamMYsAsync(regulation, course);

            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Exam month-years loaded successfully" : "No exam month-years found",
                Data = data
            });
        }

        /// <summary>
        /// Get cancel receipt list data
        /// Stored Procedure: SP_Cancel_Receipt
        /// </summary>
        [HttpGet("data")]
        public async Task<IActionResult> GetCancelReceiptListData(
            [FromQuery] string course,
            [FromQuery] string examMY)
        {
            if (string.IsNullOrWhiteSpace(course) || string.IsNullOrWhiteSpace(examMY))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "course and examMY are required parameters"
                });
            }

            var request = new CancelReceiptListRequest
            {
                Course = course,
                ExamMY = examMY
            };

            var data = await _svc.GetCancelReceiptListDataAsync(request);

            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Cancel receipt list data loaded successfully" : "No cancel receipt data found",
                Data = data
            });
        }

        /// <summary>
        /// Get cancel receipt list data for Excel export
        /// Stored Procedure: SP_Cancel_Receipt
        /// </summary>
        [HttpGet("export")]
        public async Task<IActionResult> GetCancelReceiptListExport(
            [FromQuery] string course,
            [FromQuery] string examMY)
        {
            if (string.IsNullOrWhiteSpace(course) || string.IsNullOrWhiteSpace(examMY))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "course and examMY are required parameters"
                });
            }

            var request = new CancelReceiptListRequest
            {
                Course = course,
                ExamMY = examMY
            };

            var data = await _svc.GetCancelReceiptListExportDataAsync(request);

            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Cancel receipt list export data loaded successfully" : "No cancel receipt data found for export",
                Data = data
            });
        }
    }
}

