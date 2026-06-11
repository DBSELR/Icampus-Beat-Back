// Controllers/FeeHeadsController.cs
using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Requests;
using ICampus_Models.Common;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ICampus_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeeHeadsController : BaseApiController
    {
        private readonly IFeeHeadService _svc;
        public FeeHeadsController(IFeeHeadService svc) => _svc = svc;

        [HttpGet]
        public async Task<IActionResult> GetHeads([FromQuery] string course = "B.TECH")
        {
            var data = await _svc.GetHeadsAsync(course);
            return Ok(new ApiResponse<object>
            {
                Success = data != null,
                Message = data != null ? "Fee heads loaded" : "No fee heads found",
                Data = data
            });
        }

        [HttpPost("save")]
        public async Task<IActionResult> Save([FromBody] FeeHeadRequest request)
        {
            var rows = await _svc.SaveHeadAsync(request);
            return Ok(new ApiResponse<object>
            {
                Success = rows > 0,
                Message = rows > 0 ? "Fee head saved successfully" : "Save failed"
            });
        }

        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody] FeeHeadRequest request)
        {
            if (!request.ID.HasValue || request.ID.Value <= 0)
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = "ID is required for delete" });
            }

            var rows = await _svc.DeleteHeadAsync(request.ID.Value);
            return Ok(new ApiResponse<object>
            {
                Success = rows > 0,
                Message = rows > 0 ? "Fee head deleted successfully" : "Delete failed"
            });
        }
    }
}
