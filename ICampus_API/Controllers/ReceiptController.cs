using ICampus_Api.Controllers;
using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using ICampus_Models.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class ReceiptController : BaseApiController
{
    private readonly IReceiptService _svc;
    public ReceiptController(IReceiptService svc) => _svc = svc;

    // GET api/receipt/list?course=B.Tech&examMy=Dec-2018&fDate=11-12-2018&tDate=20-12-2018&userId=COE
    [HttpGet("list")]
    public async Task<IActionResult> GetList([FromQuery] ReceiptCollectionRequest req)
    {
        var data = await _svc.LoadReceiptCollectionAsync(req);
        return Ok(new ApiResponse<object>
        {
            Success = data != null && data.Any(),
            Message = data != null && data.Any() ? "Receipt list loaded" : "No receipt data",
            Data = data
        });
    }

    // POST api/receipt/list (body variant)
    [HttpPost("list")]
    public async Task<IActionResult> PostList([FromBody] ReceiptCollectionRequest req)
    {
        var data = await _svc.LoadReceiptCollectionAsync(req);
        return Ok(new ApiResponse<object>
        {
            Success = data != null && data.Any(),
            Message = data != null && data.Any() ? "Receipt list loaded" : "No receipt data",
            Data = data
        });
    }

    // GET api/receipt/{receiptNo}?course=B.Tech&examMy=Dec-2018
    [HttpGet("{receiptNo}")]
    public async Task<IActionResult> GetReceipt([FromRoute] string receiptNo, [FromQuery] string course, [FromQuery] string examMy)
    {
        var req = new ReceiptDetailRequest { ReceiptNo = receiptNo, Course = course, ExamMy = examMy };
        var data = await _svc.LoadReceiptDetailAsync(req);
        return Ok(new ApiResponse<object>
        {
            Success = data != null,
            Message = data != null ? "Receipt loaded" : "Receipt not found",
            Data = data
        });
    }

    // POST api/receipt/detail (body variant)
    [HttpPost("detail")]
    public async Task<IActionResult> PostReceiptDetail([FromBody] ReceiptDetailRequest req)
    {
        var data = await _svc.LoadReceiptDetailAsync(req);
        return Ok(new ApiResponse<object>
        {
            Success = data != null,
            Message = data != null ? "Receipt loaded" : "Receipt not found",
            Data = data
        });
    }

    // GET api/receipt/search?regno=18671A05C0
    [HttpGet("search")]
    public async Task<IActionResult> SearchByRegno([FromQuery] string regno)
    {
        var data = await _svc.SearchByRegnoAsync(regno);
        return Ok(new ApiResponse<object>
        {
            Success = data != null && data.Any(),
            Message = data != null && data.Any() ? "Search results loaded" : "No records found",
            Data = data
        });
    }

    // POST api/receipt/search (body variant)
    [HttpPost("search")]
    public async Task<IActionResult> PostSearch([FromBody] SearchRegnoRequest req)
    {
        var data = await _svc.SearchByRegnoAsync(req.Regno);
        return Ok(new ApiResponse<object>
        {
            Success = data != null && data.Any(),
            Message = data != null && data.Any() ? "Search results loaded" : "No records found",
            Data = data
        });
    }
}
