using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ICampus_Models.Requests; // existing SaveEmployeeRequest
using System;

[ApiController]
[Route("api/[controller]")]
public class EmployeeController : ControllerBase
{
    private readonly IEmployeeService _svc;
    private readonly IWebHostEnvironment _env;
    private const string StaffPhotoFolder = "StaffPhoto";
    private const string StaffSignFolder = "StaffSign";

    public EmployeeController(IEmployeeService svc, IWebHostEnvironment env)
    {
        _svc = svc;
        _env = env;
    }

    // POST api/employee/register  (JSON only) - backward compatible (no files)
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] SaveEmployeeRequest request)
    {
        var result = await _svc.SaveEmployeeAsync(request);
        return Ok(new { Success = result > 0, Message = result > 0 ? "Employee saved successfully" : "Failed to save employee" });
    }

    [HttpPost("register-with-files")]
    [Consumes("multipart/form-data")]
    [DisableRequestSizeLimit]
    public async Task<IActionResult> RegisterWithFiles([FromForm] SaveEmployeeFormModel form)
    {
        // map form -> existing SaveEmployeeRequest used by service
        var saveReq = new SaveEmployeeRequest
        {
            EmployeeId = form.EmployeeId,
            EmployeeName = form.EmployeeName,
            UserName = form.UserName,
            Password = form.Password,
            UserGroup = form.UserGroup,
            Course = form.Course,
            ExamMY = form.ExamMY,
            Gender = form.Gender,
            DOB = form.DOB,
            Category = form.Category,
            Mobile = form.Mobile,
            Department = form.Department,
            Qualification = form.Qualification,
            DOJ = form.DOJ,
            TeachingSubject = form.TeachingSubject,
            Email = form.Email,
            Designation = form.Designation,
            AadharNo = form.AadharNo,
            IsActive = form.IsActive
        };

        var result = await _svc.SaveEmployeeAsync(saveReq);

        if (result <= 0)
            return Ok(new { Success = false, Message = "Failed to save employee" });

        // if saved successfully, persist files (if provided)
        var wwwroot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

        // Ensure directories exist
        var photoDir = Path.Combine(wwwroot, StaffPhotoFolder);
        var signDir = Path.Combine(wwwroot, StaffSignFolder);
        if (!Directory.Exists(photoDir)) Directory.CreateDirectory(photoDir);
        if (!Directory.Exists(signDir)) Directory.CreateDirectory(signDir);

        try
        {
            if (form.Photo != null && form.Photo.Length > 0 && !string.IsNullOrWhiteSpace(saveReq.EmployeeId))
            {
                var photoFileName = $"P{saveReq.EmployeeId}.jpg";
                var photoPath = Path.Combine(photoDir, photoFileName);
                using (var stream = new FileStream(photoPath, FileMode.Create))
                {
                    await form.Photo.CopyToAsync(stream);
                }
            }

            if (form.Signature != null && form.Signature.Length > 0 && !string.IsNullOrWhiteSpace(saveReq.EmployeeId))
            {
                var signFileName = $"S{saveReq.EmployeeId}.jpg";
                var signPath = Path.Combine(signDir, signFileName);
                using (var stream = new FileStream(signPath, FileMode.Create))
                {
                    await form.Signature.CopyToAsync(stream);
                }
            }
        }
        catch (Exception ex)
        {
            // Log exception as needed. Do not fail the entire request if file save fails,
            // but return info about file save failure.
            return Ok(new
            {
                Success = true,
                Message = "Employee saved, but file save failed",
                FileError = ex.Message
            });
        }

        // Build URLs for client (assuming static files are served from wwwroot)
        string baseUrl = $"{Request.Scheme}://{Request.Host}";
        string photoUrl = null;
        string signUrl = null;
        if (!string.IsNullOrWhiteSpace(saveReq.EmployeeId))
        {
            photoUrl = $"{baseUrl}/{StaffPhotoFolder}/P{saveReq.EmployeeId}.jpg";
            signUrl = $"{baseUrl}/{StaffSignFolder}/S{saveReq.EmployeeId}.jpg";
        }

        return Ok(new
        {
            Success = true,
            Message = "Employee saved successfully",
            PhotoUrl = photoUrl,
            SignatureUrl = signUrl
        });
    }


    // GET api/employee/grid
    [HttpGet("grid")]
    public async Task<IActionResult> Grid()
    {
        var data = await _svc.LoadEmployeesAsync();
        return Ok(new { Success = data != null, Data = data });
    }

    // GET api/employee/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        var rows = await _svc.LoadEmployeesAsync(id);
        var item = rows?.FirstOrDefault();
        if (item == null) return Ok(new { Success = false, Message = "Not found" });

        // attach photo/sign URLs (if file exists)
        string baseUrl = $"{Request.Scheme}://{Request.Host}";
        var wwwroot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

        var photoPath = Path.Combine(wwwroot, StaffPhotoFolder, $"P{id}.jpg");
        var signPath = Path.Combine(wwwroot, StaffSignFolder, $"S{id}.jpg");

        if (System.IO.File.Exists(photoPath))
            item.PhotoUrl = $"{baseUrl}/{StaffPhotoFolder}/P{id}.jpg"; // you may need to add PhotoUrl property to DTO
        if (System.IO.File.Exists(signPath))
            item.SignatureUrl = $"{baseUrl}/{StaffSignFolder}/S{id}.jpg"; // add SignatureUrl property to DTO

        return Ok(new { Success = true, Data = item });
    }

    // DELETE api/employee/{empId}?userName=...
    [HttpDelete("{empId}")]
    public async Task<IActionResult> Delete(string empId, [FromQuery] string userName)
    {
        if (string.IsNullOrWhiteSpace(empId))
            return BadRequest(new { Success = false, Message = "Employee id is required" });

        if (string.IsNullOrWhiteSpace(userName))
            return BadRequest(new { Success = false, Message = "User name is required" });

        var result = await _svc.DeleteEmployeeAsync(empId, userName);

        if (result <= 0)
            return NotFound(new { Success = false, Message = "Employee not found or delete failed" });

        // delete files from disk (best effort)
        var wwwroot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var photo = Path.Combine(wwwroot, StaffPhotoFolder, $"P{empId}.jpg");
        var sign = Path.Combine(wwwroot, StaffSignFolder, $"S{empId}.jpg");

        try
        {
            if (System.IO.File.Exists(photo)) System.IO.File.Delete(photo);
            if (System.IO.File.Exists(sign)) System.IO.File.Delete(sign);
        }
        catch
        {
            // optional: log file deletion failures without affecting API response
        }

        return Ok(new { Success = true, Message = "Employee deleted successfully" });
    }

    // GET api/employee/usergroups
    [HttpGet("usergroups")]
    public async Task<IActionResult> UserGroups()
    {
        var data = await _svc.LoadUserGroupsAsync();
        return Ok(new { Success = true, Data = data });
    }

    // GET api/employee/checkuser/{userId}
    [HttpGet("checkuser/{userId}")]
    public async Task<IActionResult> CheckUser(string userId)
    {
        var exists = await _svc.CheckUserIdAsync(userId);
        return Ok(new { Exists = exists });
    }
}
