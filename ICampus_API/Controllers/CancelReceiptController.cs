using ICampus_Api.Controllers;
using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using ICampus_Models.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

[ApiController]
[Route("api/[controller]")]
public class CancelReceiptController : BaseApiController
{
    private readonly ICancelReceiptService _svc;
    public CancelReceiptController(ICancelReceiptService svc) => _svc = svc;

    // GET api/cancelreceipt/student?regno=18671A05C0
    [HttpGet("student")]
    public async Task<IActionResult> GetStudent([FromQuery] string regno)
    {
        var data = await _svc.GetStudentDetailsAsync(regno);
        return Ok(new ApiResponse<object>
        {
            Success = data != null,
            Message = data != null ? "Student loaded" : "Student not found",
            Data = data
        });
    }

    // GET api/cancelreceipt/subjects?regno=18671A05C0&examMy=Dec-2018
    [HttpGet("subjects")]
    public async Task<IActionResult> GetReceiptSubjects([FromQuery] string regno, [FromQuery] string examMy)
    {
        var data = await _svc.LoadReceiptSubjectsAsync(regno, examMy);
        return Ok(new ApiResponse<object>
        {
            Success = data != null && data.Any(),
            Message = data != null && data.Any() ? "Receipt subjects loaded" : "No subjects found",
            Data = data
        });
    }

    // POST api/cancelreceipt/cancel
    // body: { "ReceiptNo":"12345", "RegNo":"18671A05C0", "UserId":"COE" }
    [HttpPost("cancel")]
    public async Task<IActionResult> CancelReceipt([FromBody] CancelReceiptRequest req)
    {
        var rows = await _svc.CancelReceiptAsync(req);
        return Ok(new ApiResponse<object>
        {
            Success = rows > 0,
            Message = rows > 0 ? "Receipt deleted (canceled) successfully" : "Cancel failed"
        });
    }

    // Optional: delete using DELETE
    [HttpDelete("{receiptNo}")]
    public async Task<IActionResult> CancelReceiptByNo(string receiptNo, [FromQuery] string regno, [FromQuery] string userId)
    {
        var req = new CancelReceiptRequest { ReceiptNo = receiptNo, RegNo = regno, UserId = userId };
        var rows = await _svc.CancelReceiptAsync(req);
        return Ok(new ApiResponse<object>
        {
            Success = rows > 0,
            Message = rows > 0 ? "Receipt deleted (canceled) successfully" : "Cancel failed"
        });
    }
}
