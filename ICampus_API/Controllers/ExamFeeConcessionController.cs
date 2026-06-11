using ICampus_Api.Controllers;
using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using ICampus_Models.Requests;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ExamFeeConcessionController : BaseApiController
{
    private readonly IExamFeeConcessionService _svc;
    public ExamFeeConcessionController(IExamFeeConcessionService svc) => _svc = svc;

    // Load sems (maps to bal_ME.Load_sems -> DAL Load_Sems SQL in file)
    [HttpGet("sems")]
    public async Task<IActionResult> GetSems([FromQuery] string course, [FromQuery] string regulation)
    {
        var data = await _svc.LoadSemsAsync(course, regulation);
        return Ok(new ApiResponse<object>
        {
            Success = data != null && data.Any(),
            Message = data != null && data.Any() ? "Sems loaded" : "No sems found",
            Data = data
        });
    }

    // Get student/details by regno (SP_GETDETAILS)
    [HttpGet("student")]
    public async Task<IActionResult> GetStudentByRegno([FromQuery] string regno)
    {
        var data = await _svc.GetDetailsByRegnoAsync(regno);
        return Ok(new ApiResponse<object>
        {
            Success = data != null && data.Any(),
            Message = data != null && data.Any() ? "Details loaded" : "No details found",
            Data = data
        });
    }

    // Fee concession grid
    [HttpGet("grid")]
    public async Task<IActionResult> GetGrid([FromQuery] string course, [FromQuery] string examMy, [FromQuery] string regno, [FromQuery] int sem = 0)
    {
        var data = await _svc.LoadFeeConcessionGridAsync(course, examMy, regno, sem);
        return Ok(new ApiResponse<object>
        {
            Success = data != null && data.Any(),
            Message = data != null && data.Any() ? "Grid loaded" : "No records found",
            Data = data
        });
    }

    // Check if semester is Regular or Supply
    [HttpGet("checkregsup")]
    public async Task<IActionResult> CheckRegSup([FromQuery] string regu, [FromQuery] int sem, [FromQuery] string course, [FromQuery] string examMy)
    {
        var regsup = await _svc.CheckRegSupAsync(regu, sem, course, examMy);
        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = $"Semester type: {regsup}",
            Data = new { RegSup = regsup }
        });
    }

    // Get fee after selecting semester (checks Regular/Supply and returns appropriate fee)
    // For Regular: requires regu, sem (string), course, grp, examMy, regulation, regno
    // For Supply: requires course, examMy, regulation, regno, sem (int)
    [HttpGet("fee")]
    public async Task<IActionResult> GetFee(
        [FromQuery] string course, 
        [FromQuery] string examMy, 
        [FromQuery] string regulation, 
        [FromQuery] string regu, 
        [FromQuery] string sem,  // Changed to string for Regular SP
        [FromQuery] string grp,   // Required for Regular
        [FromQuery] string regno) // Required for both
    {
        // Validate required parameters
        if (string.IsNullOrWhiteSpace(course) || string.IsNullOrWhiteSpace(examMy) || 
            string.IsNullOrWhiteSpace(regulation) || string.IsNullOrWhiteSpace(regno))
        {
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = "course, examMy, regulation, and regno are required parameters"
            });
        }

        // First check if semester is Regular or Supply
        int semInt = 0;
        if (!string.IsNullOrWhiteSpace(sem) && int.TryParse(sem, out var parsedSem))
        {
            semInt = parsedSem;
        }
        else
        {
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = "sem must be a valid integer"
            });
        }

        var regsup = await _svc.CheckRegSupAsync(regu, semInt, course, examMy);
        
        IEnumerable<object> data;
        if (regsup == "SUP" || regsup == "SUPPLY")
        {
            // Use Supply stored procedure: @COURSE, @EXAMMY, @REGULATION, @Regno, @sem
            data = await _svc.GetFeeConcessionSupAsync(course, examMy, regulation, regno, semInt);
        }
        else
        {
            // Use Regular stored procedure: @REGU, @SEM, @COURSE, @GRP, @EXAMMY, @REGULATION, @REGNO
            if (string.IsNullOrWhiteSpace(regu) || string.IsNullOrWhiteSpace(grp))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "regu and grp are required for Regular semester"
                });
            }
            data = await _svc.GetFeeConcessionRegAsync(regu, sem, course, grp, examMy, regulation, regno);
        }
        
        return Ok(new ApiResponse<object>
        {
            Success = data != null && data.Any(),
            Message = data != null && data.Any() ? $"Fee loaded ({regsup})" : "No fee data found",
            Data = data
        });
    }

    // Save concession (insert or update). Body maps to BOL_ExamFeeConcession fields
    [HttpPost("save")]
    public async Task<IActionResult> Save([FromBody] FeeConcessionSaveRequest request)
    {
        var rows = await _svc.SaveFeeConcessionAsync(request);
        return Ok(new ApiResponse<object>
        {
            Success = rows > 0,
            Message = rows > 0 ? "Concession saved successfully" : "Save failed"
        });
    }

    // Delete concession by id
    [HttpPost("delete")]
    public async Task<IActionResult> Delete([FromBody] FeeConcessionDeleteRequest request)
    {
        var rows = await _svc.DeleteFeeConcessionAsync(request.Id);
        return Ok(new ApiResponse<object>
        {
            Success = rows > 0,
            Message = rows > 0 ? "Deleted successfully" : "Delete failed"
        });
    }
}
