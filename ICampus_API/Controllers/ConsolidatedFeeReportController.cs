using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using ICampus_Models.Requests;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConsolidatedFeeReportController : BaseApiController
    {
        private readonly IConsolidatedFeeReportService _svc;

        public ConsolidatedFeeReportController(IConsolidatedFeeReportService svc)
        {
            _svc = svc;
        }

        /// <summary>
        /// Get consolidated fee report data (Legacy endpoint - kept for backward compatibility)
        /// Stored Procedure: proc_feee_list_overall
        /// </summary>
        [HttpGet("report")]
        public async Task<IActionResult> GetConsolidatedFeeReport(
            [FromQuery] string regulation,
            [FromQuery] string course,
            [FromQuery] string examMy,
            [FromQuery] string fDate,
            [FromQuery] string tDate)
        {
            var request = new ConsolidatedFeeReportRequest
            {
                Regulation = regulation,
                Course = course,
                ExamMy = examMy,
                FDate = fDate,
                TDate = tDate
            };

            var data = await _svc.LoadConsolidatedFeeReportAsync(request);

            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Consolidated fee report loaded" : "No fee data found for the specified criteria",
                Data = data
            });
        }

        /// <summary>
        /// Get consolidated fee report data
        /// Stored Procedure: proc_feee_list_overall
        /// </summary>
        [HttpGet("data")]
        public async Task<IActionResult> GetConsolidatedFeeReportData(
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
            if (DateTime.TryParse(fDate, out var fromDate) &&
                DateTime.TryParse(tDate, out var toDate))
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

            var request = new ConsolidatedFeeReportRequest
            {
                Regulation = regulation,
                Course = course,
                ExamMy = examMY,
                FDate = fDate,
                TDate = tDate
            };

            var data = await _svc.GetConsolidatedFeeReportDataAsync(request);

            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Consolidated fee report data loaded successfully" : "No consolidated fee data found",
                Data = data
            });
        }

        /// <summary>
        /// Get consolidated fee report data for Excel export
        /// Stored Procedure: proc_feee_list_overall
        /// </summary>
        [HttpGet("export")]
        public async Task<IActionResult> GetConsolidatedFeeReportExport(
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
            if (DateTime.TryParse(fDate, out var fromDate) &&
                DateTime.TryParse(tDate, out var toDate))
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

            var request = new ConsolidatedFeeReportRequest
            {
                Regulation = regulation,
                Course = course,
                ExamMy = examMY,
                FDate = fDate,
                TDate = tDate
            };

            var data = await _svc.GetConsolidatedFeeReportExportDataAsync(request);

            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Consolidated fee report export data loaded successfully" : "No consolidated fee data found for export",
                Data = data
            });
        }
    }
}

