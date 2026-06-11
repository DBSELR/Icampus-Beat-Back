using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RvSummeryReportController : BaseApiController
    {
        private readonly IRvSummeryReportService _svc;

        public RvSummeryReportController(IRvSummeryReportService svc)
        {
            _svc = svc;
        }

        // GET api/rvsummeryreport/rv-summary?regulation=R20&examMY=NOV2024
        // btnExport_Click     → "Summary of RV Report"  (Crystal Report)
        // btnRVExportExcel_Click → "Summary of RV Excel" (same data, different export format)
        // SP: Proc_RV_Summary (@Regulation, @ExamMY)
        // Session in original: Session["Regulation"] and Session["ExamMY"] (set from previous page)
        // Source: RV_Summery_Report.aspx / App_Web_zjsy5om4.dll
        // Confirmed: DataAccessLayer.dll UTF-16LE 0x2699e: "[Proc_RV_Summary] '⼁"
        [HttpGet("rv-summary")]
        public async Task<IActionResult> GetRvSummary(
            [FromQuery] string regulation,
            [FromQuery] string course,
            [FromQuery] string examMY)
        {
            if (string.IsNullOrWhiteSpace(regulation) || string.IsNullOrWhiteSpace(examMY))
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "regulation and examMY are required.",
                    Data    = null
                });

            var data = await _svc.GetRvSummaryAsync(regulation, course ?? string.Empty, examMY);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "RV summary loaded." : "No data found.",
                Data    = data
            });
        }

        // GET api/rvsummeryreport/supply-summary?regulation=R20&examMY=NOV2024
        // ExcelExport_Click → "Summary of Supply Excel"
        // SP: Proc_Supply_Summary (@Regulation, @ExamMY)
        // Source: RV_Summery_Report.aspx / App_Web_zjsy5om4.dll → Supply_Summary()
        // Confirmed: DataAccessLayer.dll UTF-16LE 0x2699e: "[Proc_Supply_Summary] '㔁"
        [HttpGet("supply-summary")]
        public async Task<IActionResult> GetSupplySummary(
            [FromQuery] string regulation,
            [FromQuery] string course,
            [FromQuery] string examMY)
        {
            if (string.IsNullOrWhiteSpace(regulation) || string.IsNullOrWhiteSpace(examMY))
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "regulation and examMY are required.",
                    Data    = null
                });

            var data = await _svc.GetSupplySummaryAsync(regulation, course ?? string.Empty, examMY);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Supply summary loaded." : "No data found.",
                Data    = data
            });
        }
    }
}
