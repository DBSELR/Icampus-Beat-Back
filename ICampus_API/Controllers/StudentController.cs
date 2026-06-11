using ICampus_Api.Requests;
using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ICampus_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentController : BaseApiController
    {
        private readonly IStudentService _studentService;
        private readonly IWebHostEnvironment _env;
        private const string StudentPhotoFolder = "StudentPhoto";
        private const string StudentSignFolder = "StudentSign";

        public StudentController(IStudentService studentService, IWebHostEnvironment env)
        {
            _studentService = studentService;
            _env = env;
        }

         // 1. Register Student
        [HttpPost("register")]
        public async Task<IActionResult> RegisterStudent([FromBody] SaveStudentRequest request)
        {
            var result = await _studentService.SaveStudentAsync(request);
            return Ok(new { Success = result > 0, Message = result > 0 ? "Student saved successfully" : "Failed to save student" });
        }

        // 2. Update Student
        [HttpPut("update")]
        public async Task<IActionResult> UpdateStudent([FromBody] SaveStudentRequest request)
        {
            var result = await _studentService.SaveStudentAsync(request);
            return Ok(new { Success = result > 0, Message = result > 0 ? "Student updated successfully" : "Failed to update student" });
        }

        // 3. Get Student by RegNo
        [HttpGet("{regNo}")]
        public async Task<IActionResult> GetStudentByRegNo(string regNo)
        {
            var student = await _studentService.GetStudentByRegNoAsync(regNo);
            if (student == null)
                return NotFound(new { Success = false, Message = "Student not found" });

            if (student != null)
            {
                student.PhotoUrl = $"{Request.Scheme}://{Request.Host}/{StudentPhotoFolder}/{student.RegNo}.jpg";
                student.SignatureUrl = $"{Request.Scheme}://{Request.Host}/{StudentSignFolder}/{student.RegNo}.jpg";

            }

            return Ok(new { Success = true, Data = student });
        }

        // 4. Get Students by Batch & Branch
        [HttpGet("list")]
        public async Task<IActionResult> GetStudents([FromQuery] string regu, [FromQuery] string course, [FromQuery] string branch)
        {
            var students = await _studentService.GetStudentsAsync(regu, course, branch);
            return Ok(new { Success = true, Data = students });
        }

        // 5. Inactivate Student
        [HttpPost("inactivate")]
        public async Task<IActionResult> InactivateStudent([FromBody] InactivateRequest request)
        {
            var result = await _studentService.InactivateStudentAsync(request);
            return Ok(new { Success = result > 0, Message = result > 0 ? "Student inactivated successfully" : "Failed to inactivate student" });
        }

        // 6. Reactivate Student
        [HttpPost("reactivate")]
        public async Task<IActionResult> ReactivateStudent([FromBody] ReactivateRequest request)
        {
            var result = await _studentService.ReactivateStudentAsync(request);
            return Ok(new { Success = result > 0, Message = result > 0 ? "Student re-activated successfully" : "Failed to re-activate student" });
        }

        // 7. Re-Admission
        [HttpPost("readmit")]
        public async Task<IActionResult> ReAdmission([FromBody] ReadmissionRequest request)
        {
            var result = await _studentService.ReAdmissionAsync(request);
            return Ok(new { Success = result > 0, Message = result > 0 ? "Student re-admitted successfully" : "Failed to re-admit student" });
        }

        // 8. Export Reports
        [HttpGet("export/detained")]
        public async Task<IActionResult> ExportDetainedList()
        {
            var dt = await _studentService.ExportDetainedListAsync();
            return Ok(dt);
        }

        [HttpGet("export/passed")]
        public async Task<IActionResult> ExportPassedList([FromQuery] string regulation, [FromQuery] string course, [FromQuery] string examMy)
        {
            var dt = await _studentService.ExportPassedListAsync(regulation, course, examMy);
            return Ok(dt);
        }

        [HttpGet("export/branchwise")]
        public async Task<IActionResult> ExportBranchWise([FromQuery] string regulation, [FromQuery] string course)
        {
            var dt = await _studentService.ExportBranchWiseAsync(regulation, course);
            return Ok(dt);
        }

        [HttpGet("export/readmission")]
        public async Task<IActionResult> ExportReadmissionList([FromQuery] string regulation, [FromQuery] string course)
        {
            var dt = await _studentService.ExportReadmissionListAsync(regulation, course);
            return Ok(dt);
        }

        [HttpGet("export/lateral")]
        public async Task<IActionResult> ExportLateralStudents()
        {
            var dt = await _studentService.ExportLateralAsync();
            return Ok(dt);
        }

        // 9. Upload Student Photo
        [HttpPost("upload/photo")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadPhoto([FromForm] UploadFileFormRequest request)
        {
            if (request.File == null || request.File.Length == 0)
                return BadRequest("Invalid file.");
            if (string.IsNullOrWhiteSpace(request.RegNo))
                return BadRequest("Registration number is required.");

            try
            {
                // Root path (wwwroot)
                var wwwroot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

                // Folder path for photos
                var photoDir = Path.Combine(wwwroot, StudentPhotoFolder);
                if (!Directory.Exists(photoDir)) Directory.CreateDirectory(photoDir);

                // File name and path
                var photoFileName = $"{request.RegNo}.jpg";
                var photoPath = Path.Combine(photoDir, photoFileName);

                // Save file to wwwroot
                using (var stream = new FileStream(photoPath, FileMode.Create))
                {
                    await request.File.CopyToAsync(stream);
                }

                // Optionally, inform service if needed (e.g., update DB flag)
                // await _studentService.MarkPhotoUploadedAsync(request.RegNo);

                return Ok(new
                {
                    Success = true,
                    Message = "Photo uploaded successfully",
                    PhotoUrl = $"{Request.Scheme}://{Request.Host}/{StudentPhotoFolder}/{photoFileName}"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Error uploading photo", Error = ex.Message });
            }
        }

        // 11. Upload Student Signature
        [HttpPost("upload/signature")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadSignature([FromForm] UploadFileFormRequest request)
        {
            if (request.File == null || request.File.Length == 0)
                return BadRequest("Invalid file.");
            if (string.IsNullOrWhiteSpace(request.RegNo))
                return BadRequest("Registration number is required.");

            try
            {
                // Root path (wwwroot)
                var wwwroot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

                // Folder path for signatures
                var signDir = Path.Combine(wwwroot, StudentSignFolder);
                if (!Directory.Exists(signDir)) Directory.CreateDirectory(signDir);

                // File name and path
                var signFileName = $"{request.RegNo}.jpg";
                var signPath = Path.Combine(signDir, signFileName);

                // Save file
                using (var stream = new FileStream(signPath, FileMode.Create))
                {
                    await request.File.CopyToAsync(stream);
                }

                // Optionally, update DB status
                // await _studentService.MarkSignatureUploadedAsync(request.RegNo);

                return Ok(new
                {
                    Success = true,
                    Message = "Signature uploaded successfully",
                    SignatureUrl = $"{Request.Scheme}://{Request.Host}/{StudentSignFolder}/{signFileName}"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Error uploading signature", Error = ex.Message });
            }
        }
    }
}
