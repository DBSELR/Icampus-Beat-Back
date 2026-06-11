using ICampus_Api.Controllers;
using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using ICampus_Models.Requests;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class CourseGradeReportController : BaseApiController
{
    private readonly ICourseGradeReportService _svc;
    public CourseGradeReportController(ICourseGradeReportService svc) => _svc = svc;

    // GET api/coursegradereport/list
    [HttpGet("list")]
    public async Task<IActionResult> GetSubjectList()
    {
        var data = await _svc.LoadSubjectListAsync();
        return Ok(new ApiResponse<object>
        {
            Success = data != null && data.Any(),
            Message = data != null && data.Any() ? "Subject list loaded" : "No subjects found",
            Data = data
        });
    }

    // GET api/coursegradereport/batches?course=B.TECH&regulation=R24
    [HttpGet("batches")]
    public async Task<IActionResult> GetBatches([FromQuery] string course, [FromQuery] string regulation)
    {
        var data = await _svc.LoadBatchesAsync(course, regulation);
        return Ok(new ApiResponse<object>
        {
            Success = data != null && data.Any(),
            Message = data != null && data.Any() ? "Batches loaded" : "No batches found",
            Data = data
        });
    }

    // GET api/coursegradereport/branches?course=B.TECH&regulation=R24&batch=24
    [HttpGet("branches")]
    public async Task<IActionResult> GetBranches([FromQuery] string course, [FromQuery] string regulation, [FromQuery] string batch)
    {
        var data = await _svc.LoadBranchesAsync(course, regulation, batch);
        return Ok(new ApiResponse<object>
        {
            Success = data != null && data.Any(),
            Message = data != null && data.Any() ? "Branches loaded" : "No branches found",
            Data = data
        });
    }

    // GET api/coursegradereport/sems?course=B.TECH&regulation=R24&batch=24
    [HttpGet("sems")]
    public async Task<IActionResult> GetSems([FromQuery] string course, [FromQuery] string regulation, [FromQuery] string batch)
    {
        var data = await _svc.LoadSemsAsync(course, regulation, batch);
        return Ok(new ApiResponse<object>
        {
            Success = data != null && data.Any(),
            Message = data != null && data.Any() ? "Sems loaded" : "No sems found",
            Data = data
        });
    }

    // GET api/coursegradereport/exammy?course=B.TECH&regulation=R24
    [HttpGet("exammy")]
    public async Task<IActionResult> GetExammy([FromQuery] string course, [FromQuery] string regulation)
    {
        var data = await _svc.LoadExammyAsync(course, regulation);
        return Ok(new ApiResponse<object>
        {
            Success = data != null && data.Any(),
            Message = data != null && data.Any() ? "ExamMy loaded" : "No exammy found",
            Data = data
        });
    }

    // POST api/coursegradereport/papers  (body: CourseGradePapersRequest)
    [HttpPost("papers")]
    public async Task<IActionResult> GetPapers([FromBody] CourseGradePapersRequest request)
    {
        var data = await _svc.LoadPapersListAsync(request);
        return Ok(new ApiResponse<object>
        {
            Success = data != null && data.Any(),
            Message = data != null && data.Any() ? "Papers loaded" : "No papers found",
            Data = data
        });
    }

    // POST api/coursegradereport/run/ias
    //[HttpPost("run/ias")]
    //public async Task<IActionResult> RunIasReport()
    //{
    //    var res = await _svc.RunIasReportAsync();
    //    return Ok(new ApiResponse<object> { Success = res > 0, Message = res > 0 ? "IAS run" : "IAS failed" });
    //}

    // POST api/coursegradereport/run/resultprocess
    //[HttpPost("run/resultprocess")]
    //public async Task<IActionResult> RunResultProcess([FromBody] ResultProcessRequest req)
    //{
    //    var res = await _svc.RunRegnoResultProcessAsync(req);
    //    return Ok(new ApiResponse<object> { Success = res > 0, Message = res > 0 ? "Result process triggered" : "Process failed" });
    //}
}
