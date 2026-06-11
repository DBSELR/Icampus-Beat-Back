using ICampus_Api.Controllers;
using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace ICampus_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CreditsMismatchController : BaseApiController
    {
        private readonly ICreditsMismatchService _svc;
        public CreditsMismatchController(ICreditsMismatchService svc) => _svc = svc;

        // GET api/creditsmismatch/batches?regulation=R18
        [HttpGet("batches")]
        public async Task<IActionResult> GetBatches([FromQuery] string regulation)
        {
            if (string.IsNullOrWhiteSpace(regulation))
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Regulation parameter is required"
                });

            var data = await _svc.GetBatchesAsync(regulation);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Batches loaded" : "No batches found",
                Data = data
            });
        }

        // GET api/creditsmismatch/exammy?course=B.Tech&regulation=R18
        [HttpGet("exammy")]
        public async Task<IActionResult> GetExamMy([FromQuery] string course, [FromQuery] string regulation)
        {
            if (string.IsNullOrWhiteSpace(course))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Course parameter is required" });

            if (string.IsNullOrWhiteSpace(regulation))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Regulation parameter is required" });

            var data = await _svc.GetExamMyAsync(course, regulation);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Exam month-years loaded" : "No exam month-years found",
                Data = data
            });
        }

        // GET api/creditsmismatch/semesters?regulation=R18&examMy=Jul-2021
        [HttpGet("semesters")]
        public async Task<IActionResult> GetSemesters([FromQuery] string regulation, [FromQuery] string examMy)
        {
            if (string.IsNullOrWhiteSpace(regulation))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Regulation parameter is required" });

            if (string.IsNullOrWhiteSpace(examMy))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "ExamMy parameter is required" });

            var data = await _svc.GetSemestersAsync(regulation, examMy);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Semesters loaded" : "No semesters found",
                Data = data
            });
        }

        // GET api/creditsmismatch/data?regulation=R18&examMy=Jul-2021&batch=2018&course=B.Tech&sem=1
        [HttpGet("data")]
        public async Task<IActionResult> GetMismatchCredits([FromQuery] string regulation, [FromQuery] string examMy, 
            [FromQuery] string batch, [FromQuery] string course, [FromQuery] string sem)
        {
            if (string.IsNullOrWhiteSpace(regulation))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Regulation parameter is required" });

            if (string.IsNullOrWhiteSpace(examMy))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "ExamMy parameter is required" });

            if (string.IsNullOrWhiteSpace(batch))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Batch parameter is required" });

            if (string.IsNullOrWhiteSpace(course))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Course parameter is required" });

            if (string.IsNullOrWhiteSpace(sem))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Sem parameter is required" });

            var data = await _svc.GetMismatchCreditsAsync(regulation, examMy, batch, course, sem);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Credits mismatch data loaded" : "No data found",
                Data = data
            });
        }
    }
}

