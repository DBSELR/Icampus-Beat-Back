using ICampus_Api.Controllers;
using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace ICampus_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SupplyLabRegisteredController : BaseApiController
    {
        private readonly ISupplyLabRegisteredService _svc;
        public SupplyLabRegisteredController(ISupplyLabRegisteredService svc) => _svc = svc;

        // GET api/supplylabregistered/semesters?course=B.Tech
        [HttpGet("semesters")]
        public async Task<IActionResult> GetSemesters([FromQuery] string course)
        {
            if (string.IsNullOrWhiteSpace(course))
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Course parameter is required"
                });

            var data = await _svc.GetSemestersAsync(course);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Semesters loaded" : "No semesters found",
                Data = data
            });
        }

        // GET api/supplylabregistered/batches
        [HttpGet("batches")]
        public async Task<IActionResult> GetBatches()
        {
            var data = await _svc.GetBatchesAsync();
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Batches loaded" : "No batches found",
                Data = data
            });
        }

        // GET api/supplylabregistered/data?course=B.Tech&examMY=Jul-2021&sem=1&regu=2018
        [HttpGet("data")]
        public async Task<IActionResult> GetSupplyLabData([FromQuery] string course, [FromQuery] string examMY, [FromQuery] string sem, [FromQuery] string regu)
        {
            if (string.IsNullOrWhiteSpace(course))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Course parameter is required" });
            if (string.IsNullOrWhiteSpace(examMY))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "ExamMY parameter is required" });
            if (string.IsNullOrWhiteSpace(sem))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Sem parameter is required" });
            if (string.IsNullOrWhiteSpace(regu))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Regu parameter is required" });

            var data = await _svc.GetSupplyLabDataAsync(examMY, course, regu, sem);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Supply lab registered data loaded" : "No data found",
                Data = data
            });
        }
    }
}

