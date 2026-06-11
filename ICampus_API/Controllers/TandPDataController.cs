using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TandPDataController : BaseApiController
    {
        private readonly ITandPDataService _svc;

        public TandPDataController(ITandPDataService svc)
        {
            _svc = svc;
        }

        // GET api/tandpdata/batch?course=
        // Page_Load — populate ddlbatch
        [HttpGet("batch")]
        public async Task<IActionResult> LoadBatch([FromQuery] string course = "")
        {
            var data = await _svc.LoadBatchAsync(course);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Batches loaded." : "No batches found.",
                Data = data
            });
        }

        // GET api/tandpdata/download?course=&regu=&exammy=
        // btnDownLoad_Click — sp_t_and_p_data(@Regulation,@Course,@batch,@EXAMMY)
        [HttpGet("download")]
        public async Task<IActionResult> GetTandPData(
            [FromQuery] string course,
            [FromQuery] string regu,
            [FromQuery] string exammy = "")
        {
            if (string.IsNullOrWhiteSpace(regu))
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Please select Batch.",
                    Data = null
                });

            var data = await _svc.GetTandPDataAsync(course, regu, exammy);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "T&P data loaded." : "No T&P data found.",
                Data = data
            });
        }
    }
}
