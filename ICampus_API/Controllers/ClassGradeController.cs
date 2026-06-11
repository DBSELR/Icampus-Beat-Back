using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Requests;
using ICampus_Models.Common;
using Microsoft.AspNetCore.Mvc;

namespace ICampus_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClassGradeController : BaseApiController
    {
        private readonly IClassGradeService _svc;
        public ClassGradeController(IClassGradeService svc) => _svc = svc;

        // GET: api/classgrade/batches?course=B.TECH
        [HttpGet("batches")]
        public async Task<IActionResult> GetBatches([FromQuery] string course)
        {
            var data = await _svc.LoadBatchAsync(course);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Batches loaded" : "No batches found",
                Data = data
            });
        }

        // GET: api/classgrade/grid?course=B.TECH&regu=15
        [HttpGet("grid")]
        public async Task<IActionResult> GetGrid([FromQuery] string course, [FromQuery] string regu)
        {
            var data = await _svc.LoadClassGradeGridAsync(course, regu);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Class grades loaded" : "No class grades found",
                Data = data
            });
        }

        // POST: api/classgrade/save
        // body -> ClassGradeSaveRequest
        [HttpPost("save")]
        public async Task<IActionResult> Save([FromBody] ClassGradeSaveRequest request)
        {
            var rows = await _svc.SaveClassGradeAsync(request);
            return Ok(new ApiResponse<object>
            {
                Success = rows > 0,
                Message = rows > 0 ? "Class grade saved successfully" : "Save failed"
            });
        }

        // POST: api/classgrade/delete
        // body -> IdDeleteRequest
        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody] IdDeleteRequest request)
        {
            var rows = await _svc.DeleteClassGradeAsync(request);
            return Ok(new ApiResponse<object>
            {
                Success = rows > 0,
                Message = rows > 0 ? "Deleted successfully" : "Delete failed"
            });
        }

        // POST: api/classgrade/copy
        // body -> CopyClassGradeRequest
        [HttpPost("copy")]
        public async Task<IActionResult> Copy([FromBody] CopyClassGradeRequest request)
        {
            var rows = await _svc.CopyClassGradeFromPrevReguAsync(request);
            return Ok(new ApiResponse<object>
            {
                Success = rows > 0,
                Message = rows > 0 ? "Class grades copied successfully" : "Copy failed"
            });
        }
    }
}
