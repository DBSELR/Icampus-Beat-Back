using Microsoft.AspNetCore.Mvc;
using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using ICampus_Models.Requests; // we'll add requests here
using System.Threading.Tasks;

namespace ICampus_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MiscFeeController : BaseApiController
    {
        private readonly IMiscFeeService _svc;
        public MiscFeeController(IMiscFeeService svc) => _svc = svc;

        // GET: api/miscfee/load?regno=...
        [HttpGet("load")]
        public async Task<IActionResult> LoadFeeData([FromQuery] string regno)
        {
            var data = await _svc.LoadFeeDataAsync(regno);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Fee items loaded" : "No fee items found",
                Data = data
            });
        }

        // GET: api/miscfee/receiptno
        [HttpGet("receiptno")]
        public async Task<IActionResult> GetReceiptNo()
        {
            var data = await _svc.LoadReceiptNoAsync();
            return Ok(new ApiResponse<object>
            {
                Success = data != null,
                Message = data != null ? "Receipt no loaded" : "Failed to load receipt no",
                Data = data
            });
        }

        // POST: api/miscfee/save
        [HttpPost("save")]
        public async Task<IActionResult> SaveMiscFee([FromBody] MiscFeeSaveRequest request)
        {
            var dt = await _svc.SaveMiscFeeAsync(request);
            // dt can be null or an IEnumerable<object> depending on repo
            var ok = dt != null;
            return Ok(new ApiResponse<object>
            {
                Success = ok,
                Message = ok ? "Misc fee saved" : "Save failed",
                Data = dt
            });
        }

        // GET: api/miscfee/getreceipt?recptno=XXX
        [HttpGet("getreceipt")]
        public async Task<IActionResult> GetMiscReceipt([FromQuery] string recptno)
        {
            var data = await _svc.GetMiscReceiptAsync(recptno);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Receipt loaded" : "Receipt not found",
                Data = data
            });
        }

        // POST: api/miscfee/delete
        [HttpPost("delete")]
        public async Task<IActionResult> DeleteReceipt([FromBody] MiscFeeDeleteRequest request)
        {
            var rows = await _svc.DeleteReceiptAsync(request.ReceiptNo);
            return Ok(new ApiResponse<object>
            {
                Success = rows > 0,
                Message = rows > 0 ? "Receipt deleted successfully" : "Delete failed"
            });
        }

        // GET: api/miscfee/export (optional: could return file stream)
        [HttpGet("export")]
        public async Task<IActionResult> Export()
        {
            var data = await _svc.ExportDataAsync();
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Export data loaded" : "No data for export",
                Data = data
            });
        }
    }
}
