using DocumentFormat.OpenXml.Office2010.Excel;
using ICampus_Api.Controllers;
using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using ICampus_Models.DTOs;
using ICampus_Models.Requests;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ExamController : BaseApiController
{
    private readonly IExamService _svc;
    public ExamController(IExamService svc) => _svc = svc;

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
    public async Task<IActionResult> GetCourses([FromQuery] string regulation)
    {
        var result = await _svc.LoadCoursesByRegulationAsync(regulation);
        return Ok(new ApiResponse<object>
        {
            Success = result != null && result.Any(),
            Message = result != null && result.Any() ? "Courses loaded" : "No courses found",
            Data = result
        });
    }

    [HttpGet("existing")]
    public async Task<IActionResult> GetExisting([FromQuery] string regulation, [FromQuery] string examMy, [FromQuery] string course)
    {
        var result = await _svc.LoadExistingExamsAsync(regulation, examMy, course);
        return Ok(new ApiResponse<object> { Success = result != null && result.Any(), Data = result });
    }

    //[HttpPost("save")]
    //public async Task<IActionResult> SaveExams([FromBody] ExamsSaveRequest request)
    //{
    //    var rows = await _svc.SaveExamsAsync(request);
    //    return Ok(new ApiResponse<object> { Success = true, Message = "Exams saved" });
    //}

    [HttpGet("notifications")]
    public async Task<IActionResult> GetNotifications([FromQuery] string examMy, [FromQuery] string course, [FromQuery] string regulation)
    {
        var rows = await _svc.LoadNotificationsAsync(examMy, course, regulation);
        return Ok(new ApiResponse<object> { Success = rows != null, Data = rows });
    }

    //[HttpPost("notifications/save")]
    //public async Task<IActionResult> SaveNotification([FromBody] ExamNotificationSaveRequest request)
    //{
    //    var rows = await _svc.SaveNotificationAsync(request);
    //    return Ok(new ApiResponse<object> { Success = rows > 0, Message = rows > 0 ? "Saved" : "Save failed" });
    //}

    [HttpPost("save-with-notification")]
    public async Task<IActionResult> SaveExamWithNotification([FromBody] ExamWithNotificationSaveRequest request)
    {
        try
        {
            // 1️⃣ Save Exam
            var examRequest = new ExamsSaveRequest
            {
                ExamMY = request.ExamMY,
                EType = request.EType,
                Remarks = request.Remarks,
                Sems = request.Sems,
                ExSems = request.ExSems,
                Course = request.Course,
                RegSem = request.RegSem,
                SupSem = request.SupSem,
                Regulation = request.Regulation
            };

            var examResult = await _svc.SaveExamsAsync(examRequest);

            // 2️⃣ Save Notification (only if exam saved)
            var notificationRequest = new ExamNotificationSaveRequest
            {
                ENID = request.ENID,
                NNumber = request.NNumber,
                ExamMY = request.ExamMY,
                Course = request.Course,
                Sem = request.Sem,
                NDate = request.NDate,
                ExRegDate = request.ExRegDate,
                ExRegEndDate = request.ExRegEndDate,
                Regulation = request.Regulation
            };

            var notifResult = await _svc.SaveNotificationAsync(notificationRequest);

            // ✅ Both succeeded
            return Ok(new ApiResponse<object> { Success = true, Message = "Exam and Notification saved successfully" });
        }
        catch (Exception ex)
        {
            // Proper error handling
            return StatusCode(500, new ApiResponse<object> { Success = false, Message = $"Error: {ex.Message}" });
        }
    }

    [HttpGet("masterlist")]
    public async Task<IActionResult> GetMasterList([FromQuery] string regulation, [FromQuery] string course)
    {
        var rows = await _svc.LoadExamMasterListAsync(regulation, course);
        return Ok(new ApiResponse<object> { Success = rows != null && rows.Any(), Data = rows });
    }

    [HttpDelete("master")]
    public async Task<IActionResult> DeleteMaster([FromQuery] long aExamId)
    {
        var data = await _svc.DeleteExamMasterAsync(aExamId);
        return Ok(new ApiResponse<object>
        {
            Success = data != null && data.Any(),
            Message = data != null && data.Any() ? "Deleted Successfully" : "Delete Failed",
            Data = data
        });

    }

    [HttpPost("reset")]
    public async Task<IActionResult> ResetRegSup([FromQuery] string examMy, [FromQuery] string course)
    {
        var rows = await _svc.ResetRegsupExamsAsync(examMy, course);
        return Ok(new ApiResponse<object> { Success = rows > 0, Message = rows > 0 ? "Reset done" : "Reset failed" });
    }

    // RegsUp Save endpoint
    [HttpPost("regsup/save")]
    public async Task<IActionResult> RegsUpSave([FromBody] RegsUpSaveRequest request)
    {
        if (request == null)
            return BadRequest(new ApiResponse<object> { Success = false, Message = "Request body is required" });

        if (string.IsNullOrWhiteSpace(request.Regu))
            return BadRequest(new ApiResponse<object> { Success = false, Message = "Regu is required" });

        if (string.IsNullOrWhiteSpace(request.ExamMy))
            return BadRequest(new ApiResponse<object> { Success = false, Message = "ExamMy is required" });

        if (string.IsNullOrWhiteSpace(request.Course))
            return BadRequest(new ApiResponse<object> { Success = false, Message = "Course is required" });

        var rows = await _svc.RegsUp_SaveAsync(request);
        return Ok(new ApiResponse<object>
        {
            Success = rows > 0,
            Message = rows > 0 ? "RegsUp saved successfully" : "Save failed"
        });
    }

    // Get Batch by Course
    [HttpGet("batch")]
    public async Task<IActionResult> GetBatch([FromQuery] int regu, [FromQuery] string course)
    {
        if (regu <= 0)
            return BadRequest(new ApiResponse<object> { Success = false, Message = "Regu must be a positive integer" });

        if (string.IsNullOrWhiteSpace(course))
            return BadRequest(new ApiResponse<object> { Success = false, Message = "Course is required" });

        var data = await _svc.GetBatchByCourseAsync(regu, course);
        return Ok(new ApiResponse<object>
        {
            Success = data != null && data.Any(),
            Message = data != null && data.Any() ? "Batch loaded" : "No batch found",
            Data = data
        });
    }
}
