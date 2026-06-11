using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Requests;
using ICampus_Models.Common;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ICampus_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubjectGradeController : BaseApiController
    {
        private readonly ISubjectGradeService _svc;
        public SubjectGradeController(ISubjectGradeService svc) => _svc = svc;

        // GET: api/subjectgrade/batches?course=B.TECH
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

        // GET: api/subjectgrade/grid?course=B.TECH&regu=15
        [HttpGet("grid")]
        public async Task<IActionResult> GetGrid([FromQuery] string course, [FromQuery] string regu)
        {
            var data = await _svc.LoadSubGradeGridAsync(course, regu);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Grades loaded" : "No grades found",
                Data = data
            });
        }

        // POST: api/subjectgrade/save
        [HttpPost("save")]
        public async Task<IActionResult> Save([FromBody] SubjectGradeSaveRequest request)
        {
            var rows = await _svc.SaveSubGradeAsync(request);
            return Ok(new ApiResponse<object>
            {
                Success = rows > 0,
                Message = rows > 0 ? "Grade saved successfully" : "Save failed"
            });
        }

        // POST: api/subjectgrade/delete
        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody] DeleteGradeRequest request)
        {
            var rows = await _svc.DeleteSubGradeAsync(request.Id);
            return Ok(new ApiResponse<object>
            {
                Success = rows > 0,
                Message = rows > 0 ? "Grade deleted successfully" : "Delete failed"
            });
        }

        // POST: api/subjectgrade/copy
        [HttpPost("copy")]
        public async Task<IActionResult> Copy([FromBody] CopyGradeRequest request)
        {
            var rows = await _svc.CopyGradeAsync(request);
            return Ok(new ApiResponse<object>
            {
                Success = rows > 0,
                Message = rows > 0 ? "Grade data copied successfully" : "Copy failed"
            });
        }

        // GET: api/subjectgrade/regus?course=B.TECH&procType=TBL_GRADE
        [HttpGet("regus")]
        public async Task<IActionResult> GetRegus([FromQuery] string course, [FromQuery] string procType = "TBL_GRADE")
        {
            var data = await _svc.LoadReguListAsync(course, procType);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Regu list loaded" : "No regu data found",
                Data = data
            });
        }
    }
}
