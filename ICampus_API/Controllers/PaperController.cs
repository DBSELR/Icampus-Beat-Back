using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.DTOs;
using ICampus_Models.Requests;
using ICampus_Models.Common;
using Microsoft.AspNetCore.Mvc;

namespace ICampus_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaperController : BaseApiController
    {
        private readonly IPaperService _svc;
        public PaperController(IPaperService svc) => _svc = svc;

        [HttpGet("regulations")]
        public async Task<IActionResult> GetRegulations()
        {
            var result = await _svc.LoadRegulationsAsync();
            return Ok(new ApiResponse<object>
            {
                Success = result != null && result.Any(),
                Message = result != null && result.Any() ? "Regulations loaded" : "No regulations found",
                Data = result
            });
        }

        [HttpGet("courses")]
        public async Task<IActionResult> GetCourses()
        {
            var result = await _svc.LoadCoursesAsync();
            return Ok(new ApiResponse<object>
            {
                Success = result != null && result.Any(),
                Message = result != null && result.Any() ? "Courses loaded" : "No courses found",
                Data = result
            });
        }

        [HttpGet("batches")]
        public async Task<IActionResult> GetBatches([FromQuery] string course)
        {
            var result = await _svc.LoadBatchesAsync(course);
            return Ok(new ApiResponse<object>
            {
                Success = result != null && result.Any(),
                Message = result != null && result.Any() ? "Batches loaded" : "No batches found",
                Data = result
            });
        }

        [HttpGet("branches")]
        public async Task<IActionResult> GetBranches([FromQuery] string course, [FromQuery] string regu)
        {
            var result = await _svc.LoadBranchesAsync(course, regu);
            return Ok(new ApiResponse<object>
            {
                Success = result != null && result.Any(),
                Message = result != null && result.Any() ? "Branches loaded" : "No branches found",
                Data = result
            });
        }

        [HttpGet("sems")]
        public async Task<IActionResult> GetSems([FromQuery] string course, [FromQuery] string batch, [FromQuery] string branch)
        {
            var result = await _svc.LoadSemsAsync(course, batch, branch);
            return Ok(new ApiResponse<object>
            {
                Success = result != null && result.Any(),
                Message = result != null && result.Any() ? "Sems loaded" : "No sems found",
                Data = result
            });
        }

        [HttpGet("streams")]
        public async Task<IActionResult> GetStreams([FromQuery] string course, [FromQuery] string batch, [FromQuery] string branch, [FromQuery] int sem)
        {
            var result = await _svc.LoadStreamsAsync(course, batch, branch, sem);
            return Ok(new ApiResponse<object>
            {
                Success = result != null && result.Any(),
                Message = result != null && result.Any() ? "Streams loaded" : "No streams found",
                Data = result
            });
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetPaperList([FromQuery] string course, [FromQuery] string regu, [FromQuery] string branch, [FromQuery] int sem, [FromQuery] string stream)
        {
            var result = await _svc.LoadPaperListAsync(course, regu, branch, sem, stream);
            return Ok(new ApiResponse<object>
            {
                Success = result != null && result.Any(),
                Message = result != null && result.Any() ? "Paper list loaded" : "No papers found",
                Data = result
            });
        }

        [HttpGet("details")]
        public async Task<IActionResult> GetPaperDetails([FromQuery] string course, [FromQuery] string regu, [FromQuery] int sem, [FromQuery] string pcode, [FromQuery] string branch)
        {
            var result = await _svc.GetPaperDetailsAsync(course, regu, sem, pcode, branch);
            return Ok(new ApiResponse<object>
            {
                Success = result != null && result.Any(),
                Message = result != null && result.Any() ? "Paper details loaded" : "No details found",
                Data = result
            });
        }

        [HttpPost("save")]
        public async Task<IActionResult> SavePaper([FromBody] PaperSaveRequest request)
        {
            // set user from token
           // request.UserId = UserId ?? request.UserId;

            var rows = await _svc.SavePaperAsync(request);
            return Ok(new ApiResponse<object>
            {
                Success = rows > 0,
                Message = rows > 0 ? "Paper saved successfully" : "Save failed"
            });
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeletePaper([FromQuery] PaperDeleteRequest request)
        {
            var rows = await _svc.DeletePaperAsync(request);
            return Ok(new ApiResponse<object>
            {
                Success = rows > 0,
                Message = rows > 0 ? "Paper deleted" : "Delete failed"
            });
        }

        [HttpPost("reorder")]
        public async Task<IActionResult> ReorderPapers([FromBody] PaperReorderRequest request)
        {
            var ok = await _svc.ReorderPapersAsync(request);
            return Ok(new ApiResponse<object>
            {
                Success = ok,
                Message = ok ? "Reorder completed" : "Reorder failed"
            });
        }

        [HttpPost("copy")]
        public async Task<IActionResult> CopyPapers([FromBody] PaperCopyRequest request)
        {
            request.UserId = UserId ?? request.UserId;
            var rows = await _svc.CopyPapersAsync(request);
            return Ok(new ApiResponse<object>
            {
                Success = rows > 0,
                Message = rows > 0 ? "Papers copied" : "Copy failed"
            });
        }

        [HttpGet("isregular")]
        public async Task<IActionResult> IsRegular([FromQuery] IsRegularRequest request)
        {
            var ok = await _svc.IsRegularBatchAsync(request);
            return Ok(new ApiResponse<object>
            {
                Success = ok,
                Message = ok ? "Regular" : "Not regular"
            });
        }
    }
}
