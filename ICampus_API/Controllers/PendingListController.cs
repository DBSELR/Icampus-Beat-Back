using ICampus_Api.Controllers;
using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PendingListController : BaseApiController
    {
        private readonly IPendingListService _svc;

        public PendingListController(IPendingListService svc)
        {
            _svc = svc;
        }

        // GET api/pendinglist/internal
        // Returns internal marks pending list
        // SP: SPS_Get_InternalPendingList
        [HttpGet("internal")]
        public async Task<IActionResult> GetInternalPendingList(
            [FromQuery] string examMy, [FromQuery] string course, [FromQuery] string regulation)
        {
            var data = await _svc.GetInternalPendingListAsync(examMy, course, regulation);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Internal pending list loaded" : "No pending data",
                Data = data
            });
        }

        // GET api/pendinglist/practical
        // Returns practical marks pending list
        // SP: SPS_Get_PracticalPendingList
        [HttpGet("practical")]
        public async Task<IActionResult> GetPracticalPendingList(
            [FromQuery] string examMy, [FromQuery] string course, [FromQuery] string regulation)
        {
            var data = await _svc.GetPracticalPendingListAsync(examMy, course, regulation);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Practical pending list loaded" : "No pending data",
                Data = data
            });
        }

        // GET api/pendinglist/theory
        // Returns theory marks pending list
        // SP: SPS_Get_TheoryPendingList
        [HttpGet("theory")]
        public async Task<IActionResult> GetTheoryPendingList(
            [FromQuery] string examMy, [FromQuery] string course, [FromQuery] string regulation)
        {
            var data = await _svc.GetTheoryPendingListAsync(examMy, course, regulation);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Theory pending list loaded" : "No pending data",
                Data = data
            });
        }

        // GET api/pendinglist/rv
        // Returns revaluation marks pending list
        // SP: SPS_Get_RVPendingList
        [HttpGet("rv")]
        public async Task<IActionResult> GetRVPendingList(
            [FromQuery] string examMy, [FromQuery] string course, [FromQuery] string regulation)
        {
            var data = await _svc.GetRVPendingListAsync(examMy, course, regulation);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "RV pending list loaded" : "No pending data",
                Data = data
            });
        }
    }
}
