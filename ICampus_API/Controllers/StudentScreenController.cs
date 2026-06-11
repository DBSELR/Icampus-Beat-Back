using ICampus_Api.Controllers;
using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using ICampus_Models.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class StudentScreenController : BaseApiController
{
    private readonly IStudentScreenService _svc;
    public StudentScreenController(IStudentScreenService svc) => _svc = svc;

    // GET api/StudentScreen/{regno}
    [HttpGet("{regno}")]
    public async Task<IActionResult> GetStudentBasic(string regno)
    {
        if (string.IsNullOrWhiteSpace(regno))
            return BadRequest(new ApiResponse<object> { Success = false, Message = "Regno required" });

        var data = await _svc.GetStudentScreenStudentDataAsync(regno);
        return Ok(new ApiResponse<object>
        {
            Success = data != null && data.Any(),
            Message = data != null && data.Any() ? "Student loaded" : "Student not found",
            Data = data
        });
    }

    // GET api/StudentScreen/{regno}/maxsems
    [HttpGet("{regno}/maxsems")]
    public async Task<IActionResult> GetMaxSemesters(string regno)
    {
        if (string.IsNullOrWhiteSpace(regno))
            return BadRequest(new ApiResponse<object> { Success = false, Message = "Regno required" });

        var data = await _svc.GetMaxSemestersAsync(regno);
        return Ok(new ApiResponse<object>
        {
            Success = data != null && data.Any(),
            Data = data
        });
    }

    // GET api/StudentScreen/{regno}/grades?examMy=DEC-2024
    [HttpGet("{regno}/grades")]
    public async Task<IActionResult> GetGrades([FromRoute] string regno, [FromQuery] string examMy = "")
    {
        if (string.IsNullOrWhiteSpace(regno))
            return BadRequest(new ApiResponse<object> { Success = false, Message = "Regno required" });

        var data = await _svc.GetStudentGradesAsync(regno, examMy ?? string.Empty);
        return Ok(new ApiResponse<object>
        {
            Success = data != null && data.Any(),
            Data = data
        });
    }
}
