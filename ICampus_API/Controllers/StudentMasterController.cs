using ICampus_Api.Controllers;
using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using ICampus_Models.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class StudentMasterController : BaseApiController
{
    private readonly IStudentMasterService _svc;
    public StudentMasterController(IStudentMasterService svc) => _svc = svc;

    // dropdowns: batches/branches
    [HttpGet("branches")]
    public async Task<IActionResult> GetBranches([FromQuery] string course, [FromQuery] string examMy, [FromQuery] string regulation)
    {
        var data = await _svc.LoadBranchAsync(course, examMy, regulation);
        return Ok(new ApiResponse<object>
        {
            Success = data != null && data.Any(),
            Message = (data != null && data.Any()) ? "Branches loaded" : "No branches found",
            Data = data
        });
    }

    [HttpGet("sems")]
    public async Task<IActionResult> GetSems([FromQuery] string course, [FromQuery] string examMy, [FromQuery] string regulation)
    {
        var data = await _svc.LoadSemsAsync(course, examMy, regulation);
        return Ok(new ApiResponse<object>
        {
            Success = data != null && data.Any(),
            Message = (data != null && data.Any()) ? "Sems loaded" : "No sems found",
            Data = data
        });
    }

    // get subjects / student master rows for a regno
    [HttpGet("student/subjects")]
    public async Task<IActionResult> GetStudentSubjects([FromQuery] string course, [FromQuery] string examMy, [FromQuery] string regu, [FromQuery] string sem, [FromQuery] string regno)
    {
        var data = await _svc.LoadStdMasterAsync(course, examMy, regu, sem, regno);
        return Ok(new ApiResponse<object>
        {
            Success = data != null && data.Any(),
            Message = data != null && data.Any() ? "Student subjects loaded" : "No subjects found",
            Data = data
        });
    }

    // check if master exists (getCount)
    [HttpGet("check-exists")]
    public async Task<IActionResult> CheckExists([FromQuery] string examMy, [FromQuery] string regu, [FromQuery] string regno)
    {
        var exists = await _svc.GetCountAsync(examMy, regu, regno);
        return Ok(new ApiResponse<object>
        {
            Success = exists > 0,
            Message = exists > 0 ? "Student data exists" : "Not exists",
            Data = exists
        });
    }

    // create master (StdRegistration -> SP_MASTER_CREATE_REGNO)
    [HttpPost("create-master")]
    public async Task<IActionResult> CreateMaster([FromBody] SaveStudentMasterRequest request)
    {
        var rows = await _svc.CreateMasterAsync(request);
        return Ok(new ApiResponse<object>
        {
            Success = rows > 0,
            Message = rows > 0 ? "Master created" : "Create failed"
        });
    }

    // OMR numbers / marks read
    [HttpGet("omr/update-list")]
    public async Task<IActionResult> GetOmrUpdateList([FromQuery] string regno, [FromQuery] string course, [FromQuery] string regulation, [FromQuery] string examMy)
    {
        var data = await _svc.LoadOmrNumUpdateAsync(regno, course, regulation, examMy);
        return Ok(new ApiResponse<object>
        {
            Success = data != null && data.Any(),
            Message = data != null && data.Any() ? "OMR update list loaded" : "No records",
            Data = data
        });
    }

    [HttpGet("omr/update-get")]
    public async Task<IActionResult> GetOmrGet([FromQuery] string regno, [FromQuery] string course, [FromQuery] string regulation, [FromQuery] string examMy, [FromQuery] string regsup)
    {
        var data = await _svc.GetLoadOmrNumUpdateAsync(regno, course, regulation, examMy, regsup);
        return Ok(new ApiResponse<object>
        {
            Success = data != null && data.Any(),
            Message = data != null && data.Any() ? "OMR get loaded" : "No records",
            Data = data
        });
    }

    [HttpPost("omr/update")]
    public async Task<IActionResult> UpdateOmr([FromBody] UpdateOmrRequest request)
    {
        var rows = await _svc.UpdateOmrNumAsync(request);
        return Ok(new ApiResponse<object>
        {
            Success = rows > 0,
            Message = rows > 0 ? "OMR updated" : "Update failed"
        });
    }

    // load std update (sp_loadstdexammy_update)
    [HttpGet("student/exammy-update-list")]
    public async Task<IActionResult> LoadStdUpdate([FromQuery] string regulation, [FromQuery] string course, [FromQuery] string regu, [FromQuery] string grp, [FromQuery] string sem)
    {
        var data = await _svc.LoadStdUpdateAsync(regulation, course, regu, grp, sem);
        return Ok(new ApiResponse<object>
        {
            Success = data != null && data.Any(),
            Message = data != null && data.Any() ? "Student exammy update rows loaded" : "No rows",
            Data = data
        });
    }

    [HttpPost("student/exammy-update")]
    public async Task<IActionResult> StdExammyUpdate([FromBody] StdUpdateRequest request)
    {
        var rows = await _svc.StdUpdateAsync(request);
        return Ok(new ApiResponse<object>
        {
            Success = rows > 0,
            Message = rows > 0 ? "ExamMy updated" : "Update failed"
        });
    }

    // marks update by ashid
    [HttpPost("marks/update")]
    public async Task<IActionResult> MarksUpdate([FromBody] MarksUpdateRequest request)
    {
        var rows = await _svc.MarksUpdateRegnoWiseAsync(request);
        return Ok(new ApiResponse<object>
        {
            Success = rows > 0,
            Message = rows > 0 ? "Marks updated" : "Update failed"
        });
    }

    // delete ashid
    [HttpPost("delete/ashid")]
    public async Task<IActionResult> DeleteAshId([FromBody] DeleteAshIdRequest request)
    {
        var rows = await _svc.DeleteAshIdAsync(request);
        return Ok(new ApiResponse<object>
        {
            Success = rows > 0,
            Message = rows > 0 ? "Deleted" : "Delete failed"
        });
    }
}
