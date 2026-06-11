using ICampus_Api.Controllers;
using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using Microsoft.AspNetCore.Mvc;

namespace ICampus_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DupCertificateController : BaseApiController
    {
        private readonly IDupCertificateService _svc;
        public DupCertificateController(IDupCertificateService svc) => _svc = svc;

        [HttpGet("receipt/{receiptNo}")]
        public async Task<IActionResult> GetReceiptData(string receiptNo)
        {
            var data = await _svc.LoadReceiptStudentDataAsync(receiptNo);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Receipt student data loaded" : "No data found for receipt",
                Data = data
            });
        }

        [HttpGet("marks-memo")]
        public async Task<IActionResult> GetMarksMemo([FromQuery] MarksMemoRequest req)
        {
            var data = await _svc.LoadMarksMemoAsync(req);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Marks memo loaded" : "No marks memo data",
                Data = data
            });
        }

        [HttpGet("hallticket")]
        public async Task<IActionResult> GetHallTicket([FromQuery] string examMy, [FromQuery] string course, [FromQuery] string regNo, [FromQuery] string regulation)
        {
            var data = await _svc.LoadHallTicketAsync(examMy, course, regNo, regulation);
            return Ok(new ApiResponse<object> { Success = data != null && data.Any(), Message = data != null && data.Any() ? "Hall ticket loaded" : "No hall ticket data", Data = data });
        }

        [HttpGet("check/regcount")]
        public async Task<IActionResult> CheckRegCount([FromQuery] string regNo, [FromQuery] int sem, [FromQuery] string examMy, [FromQuery] string certificateName)
        {
            var cnt = await _svc.CheckRegWiseDupCountAsync(regNo, sem, examMy, certificateName);
            return Ok(new ApiResponse<object> { Success = cnt > 0, Message = cnt > 0 ? "Duplicate(s) found" : "No duplicate", Data = new { Count = cnt } });
        }

        [HttpGet("check/receiptcount")]
        public async Task<IActionResult> CheckReceiptCount([FromQuery] string receiptNo, [FromQuery] string regNo, [FromQuery] int sem, [FromQuery] string examMy, [FromQuery] string certificateName)
        {
            var cnt = await _svc.CheckReceiptWiseDupCountAsync(receiptNo, regNo, sem, examMy, certificateName);
            return Ok(new ApiResponse<object> { Success = cnt > 0, Message = cnt > 0 ? "Duplicate(s) found for receipt" : "No duplicate for receipt", Data = new { Count = cnt } });
        }

        [HttpPost("save")]
        public async Task<IActionResult> Save([FromBody] DupCertificateSaveRequest request)
        {
            // you can set request.CrId = UserId ?? request.CrId if you have auth
            var rows = await _svc.SaveDupCertificateAsync(request);
            return Ok(new ApiResponse<object> { Success = rows > 0, Message = rows > 0 ? "Saved successfully" : "Save failed" });
        }
    }

}
