using ICampus_Api.Controllers;
using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using ICampus_Models.Requests;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class SemesterGradeController : BaseApiController
{
    private readonly ISemesterGradeService _svc;
    public SemesterGradeController(ISemesterGradeService svc) => _svc = svc;

    // GET api/semestergrade/batches?course=B.TECH
    [HttpGet("batches")]
    public async Task<IActionResult> GetBatches([FromQuery] string course)
    {
        var data = await _svc.LoadBatchesAsync(course);
        return Ok(new ApiResponse<object>
        {
            Success = data != null && data.Any(),
            Message = data != null && data.Any() ? "Batches loaded" : "No batches found",
            Data = data
        });
    }

    // GET api/semestergrade/grid?course=B.TECH&regu=15
    [HttpGet("grid")]
    public async Task<IActionResult> GetGrid([FromQuery] string course, [FromQuery] string regu = "")
    {
        var data = await _svc.LoadSemGradeGridAsync(course, regu);
        return Ok(new ApiResponse<object>
        {
            Success = data != null && data.Any(),
            Message = data != null && data.Any() ? "Sem grade grid loaded" : "No sem grade rows found",
            Data = data
        });
    }

    // POST api/semestergrade/save
    [HttpPost("save")]
    public async Task<IActionResult> Save([FromBody] SemGradeSaveRequest request)
    {
        var rows = await _svc.SaveSemGradeAsync(request);
        return Ok(new ApiResponse<object>
        {
            Success = rows > 0,
            Message = rows > 0 ? "Grade saved successfully" : "Save failed"
        });
    }

    // POST api/semestergrade/delete
    [HttpPost("delete")]
    public async Task<IActionResult> Delete([FromBody] DeleteRequest request)
    {
        var rows = await _svc.DeleteSemGradeAsync(request);
        return Ok(new ApiResponse<object>
        {
            Success = rows > 0,
            Message = rows > 0 ? "Grade deleted successfully" : "Delete failed"
        });
    }

    // GET api/semestergrade/check?course=B.TECH&toBatch=17&type=TBL_SEMGRADE
    [HttpGet("check")]
    public async Task<IActionResult> Check([FromQuery] string course, [FromQuery] string toBatch, [FromQuery] string type = "TBL_SEMGRADE")
    {
        var data = await _svc.CheckSemGradeAsync(course, toBatch, type);
        var exists = data != null && data.Any();
        return Ok(new ApiResponse<object>
        {
            Success = exists,
            Message = exists ? "Records exist" : "No records",
            Data = data
        });
    }

    // POST api/semestergrade/copy
    [HttpPost("copy")]
    public async Task<IActionResult> Copy([FromBody] CopySemesterGradeRequest request)
    {
        var rows = await _svc.CopyGradeAsync(request);
        return Ok(new ApiResponse<object>
        {
            Success = rows > 0,
            Message = rows > 0 ? "Grades copied successfully" : "Copy failed"
        });
    }

    // GET api/semestergrade/regu?course=B.TECH&type=TBL_SEMGRADE
    [HttpGet("regu")]
    public async Task<IActionResult> LoadRegu([FromQuery] string course, [FromQuery] string type = "TBL_SEMGRADE")
    {
        var data = await _svc.LoadReguAsync(course, type);
        return Ok(new ApiResponse<object>
        {
            Success = data != null && data.Any(),
            Message = data != null && data.Any() ? "Regu list loaded" : "No regu rows found",
            Data = data
        });
    }
}
