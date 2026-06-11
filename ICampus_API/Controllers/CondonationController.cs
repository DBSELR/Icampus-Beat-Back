using Microsoft.AspNetCore.Mvc;
using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Requests;
using ICampus_Models.Common;

namespace ICampus_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CondonationController : ControllerBase
    {
        private readonly ICondonationService _svc;

        public CondonationController(ICondonationService svc)
        {
            _svc = svc;
        }

        // 1) Load Semesters
        [HttpGet("sems")]
        public async Task<IActionResult> GetSems([FromQuery] string course,
                                                 [FromQuery] string regulation,
                                                 [FromQuery] string examMy,
                                                 [FromQuery] string regsup,
                                                 [FromQuery] string regno)
        {
            var data = await _svc.LoadSemsAsync(course, regulation, examMy, regsup, regno);

            return Ok(new ApiResponse<object>
            {
                Success = data.Any(),
                Message = data.Any() ? "Sems loaded" : "No sems found",
                Data = data
            });
        }

        // 2) Load Student Details
        [HttpGet("student")]
        public async Task<IActionResult> GetStudent([FromQuery] string regno)
        {
            var data = await _svc.GetStudentDetailsAsync(regno);

            return Ok(new ApiResponse<object>
            {
                Success = data != null,
                Message = data != null ? "Student details loaded" : "Student not found",
                Data = data
            });
        }

        // 3) Load Grid
        [HttpGet("grid")]
        public async Task<IActionResult> GetGrid([FromQuery] string regno,
                                                 [FromQuery] string examMy,
                                                 [FromQuery] string course,
                                                 [FromQuery] string sem)
        {
            var data = await _svc.GetCondonationGridAsync(regno, examMy, course, sem);

            return Ok(new ApiResponse<object>
            {
                Success = data.Any(),
                Message = data.Any() ? "Grid loaded" : "No records found",
                Data = data
            });
        }

        // 4) Check Dates
        [HttpGet("checkdates")]
        public async Task<IActionResult> CheckDates([FromQuery] string regno,
                                                    [FromQuery] string examMy,
                                                    [FromQuery] string course,
                                                    [FromQuery] string regulation,
                                                    [FromQuery] string sem)
        {
            var data = await _svc.CheckCondonationDatesAsync(regno, examMy, course, regulation, sem);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Check completed",
                Data = data
            });
        }

        // 5) Save
        [HttpPost("save")]
        public async Task<IActionResult> Save([FromBody] CondonationSaveRequest req)
        {
            var rows = await _svc.SaveCondonationAsync(req);

            return Ok(new ApiResponse<object>
            {
                Success = rows > 0,
                Message = rows > 0 ? "Saved successfully" : "Save failed",
                Data = rows
            });
        }

        // 6) Delete
        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody] CondonationDeleteRequest req)
        {
            var rows = await _svc.DeleteCondonationAsync(req.Id);

            return Ok(new ApiResponse<object>
            {
                Success = rows > 0,
                Message = rows > 0 ? "Deleted" : "Delete failed",
                Data = rows
            });
        }

        // 7) Format Export (sample row)
        [HttpGet("format")]
        public async Task<IActionResult> Format()
        {
            var data = await _svc.GetCondonationFormatAsync();

            return Ok(new ApiResponse<object>
            {
                Success = data != null,
                Message = "Format loaded",
                Data = data
            });
        }

        // 8) Export
        [HttpGet("export")]
        public async Task<IActionResult> Export([FromQuery] string examMy,
                                                [FromQuery] string regulation)
        {
            var data = await _svc.ExportCondonationAsync(examMy, regulation);

            return Ok(new ApiResponse<object>
            {
                Success = data.Any(),
                Message = "Export loaded",
                Data = data
            });
        }
    }
}
