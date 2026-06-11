using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Requests;
using ICampus_Models.Common;
using Microsoft.AspNetCore.Mvc;

namespace ICampus_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RevaluationController : BaseApiController
    {
        private readonly IRevaluationService _svc;
        public RevaluationController(IRevaluationService svc) => _svc = svc;

        // GET api/revaluation/sems?course=...&examMy=...&regulation=...
        [HttpGet("sems")]
        public async Task<IActionResult> GetSems([FromQuery] string course, [FromQuery] string examMy, [FromQuery] string regulation)
        {
            var data = await _svc.LoadSemsAsync(course, examMy, regulation);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Sems loaded" : "No sems found",
                Data = data
            });
        }

        // GET api/revaluation/papers?examMy=...&regno=...&sem=...
        [HttpGet("papers")]
        public async Task<IActionResult> GetPapers([FromQuery] string examMy, [FromQuery] string regno, [FromQuery] string sem)
        {
            var data = await _svc.GetPapersForRvAsync(examMy, regno, sem);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Papers loaded" : "No papers found",
                Data = data
            });
        }

        // GET api/revaluation/opted?examMy=...&regno=...&sem=...
        [HttpGet("opted")]
        public async Task<IActionResult> GetOptedPapers([FromQuery] string examMy, [FromQuery] string regno, [FromQuery] string sem)
        {
            var data = await _svc.GetOptedPapersAsync(examMy, regno, sem);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Opted papers loaded" : "No opted papers",
                Data = data
            });
        }

        // GET api/revaluation/check-close?regulation=...&course=...&examMy=...&sem=...&regno=...
        [HttpGet("check-close")]
        public async Task<IActionResult> CheckRvClose([FromQuery] string regulation, [FromQuery] string course, [FromQuery] string examMy, [FromQuery] string sem, [FromQuery] string regno)
        {
            var data = await _svc.CheckRvCloseDateAsync(regulation, course, examMy, sem, regno);
            return Ok(new ApiResponse<object>
            {
                Success = data != null,
                Message = data != null ? "Check returned" : "No data",
                Data = data
            });
        }

        // GET api/revaluation/fee?regulation=...&course=...&examMy=...&sem=...&rvType=...
        [HttpGet("fee")]
        public async Task<IActionResult> GetRvFee([FromQuery] string regulation, [FromQuery] string course, [FromQuery] string examMy, [FromQuery] string sem, [FromQuery] string rvType)
        {
            var data = await _svc.GetRvFeeAsync(regulation, course, examMy, sem, rvType);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Rv fee loaded" : "No fee data",
                Data = data
            });
        }

        // POST api/revaluation/register-paper
        [HttpPost("register-paper")]
        public async Task<IActionResult> RegisterPaper([FromBody] RegisterRvPaperRequest request)
        {
            var rows = await _svc.RegisterRvPaperAsync(request);
            return Ok(new ApiResponse<object>
            {
                Success = rows > 0,
                Message = rows > 0 ? "Paper registered" : "Register failed"
            });
        }

        // POST api/revaluation/reset-papers
        [HttpPost("reset-papers")]
        public async Task<IActionResult> ResetPapers([FromBody] ResetRvPaperRequest request)
        {
            var rows = await _svc.ResetRvPapersAsync(request);
            return Ok(new ApiResponse<object>
            {
                Success = rows > 0,
                Message = rows > 0 ? "RV papers reset" : "Reset failed"
            });
        }

        // POST api/revaluation/fee-pay
        [HttpPost("fee-pay")]
        public async Task<IActionResult> PayRvFee([FromBody] RvFeePayRequest request)
        {
            var dt = await _svc.RvExamFeePayAsync(request);
            return Ok(new ApiResponse<object>
            {
                Success = dt != null,
                Message = dt != null ? "Fee pay processed" : "Failed",
                Data = dt
            });
        }

        // GET api/revaluation/bundle?regulation=...&examMy=...&course=...&userId=...
        [HttpGet("bundle")]
        public async Task<IActionResult> GetBundle([FromQuery] string regulation, [FromQuery] string examMy, [FromQuery] string course, [FromQuery] string userId)
        {
            var data = await _svc.GetRvBundleScriptsAsync(regulation, examMy, course, userId);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Bundle loaded" : "No bundle data",
                Data = data
            });
        }

        // GET api/revaluation/receipt?receiptNo=...
        [HttpGet("receipt")]
        public async Task<IActionResult> GetReceipt([FromQuery] string receiptNo)
        {
            var data = await _svc.GetReceiptDataAsync(receiptNo);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Receipt data loaded" : "No receipt found",
                Data = data
            });
        }
    }
}
