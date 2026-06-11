using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Linq;
using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Requests;
using ICampus_Models.Common;
using System.Collections.Generic;

namespace ICampus_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImportDataController : BaseApiController
    {
        private readonly IImportService _svc;
        public ImportDataController(IImportService svc) => _svc = svc;

        // GET /api/importdata/courses?regulation=R20
        [HttpGet("courses")]
        public async Task<IActionResult> GetCourses([FromQuery] string regulation)
        {
            var data = await _svc.LoadCourseListAsync(regulation);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Courses loaded" : "No courses found",
                Data = data
            });
        }

        // GET /api/importdata/regu?course=B.TECH&regulation=R20&examMy=Apr-2025
        [HttpGet("regu")]
        public async Task<IActionResult> GetRegu([FromQuery] string course, [FromQuery] string regulation, [FromQuery] string examMy)
        {
            var data = await _svc.LoadReguAsync(course, regulation, examMy);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Batches loaded" : "No batches found",
                Data = data
            });
        }

        // POST /api/importdata/upload  (multipart/form-data)
        // Form fields: file (IFormFile), hasHeader (optional)
        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] FileUploadRequest req)
        {
            if (req?.File == null) return BadRequest(new ApiResponse<object> { Success = false, Message = "File is required" });

            var sheets = await _svc.GetExcelSheetsAsync(req.File);
            return Ok(new ApiResponse<IEnumerable<string>>
            {
                Success = sheets != null && sheets.Any(),
                Message = sheets != null && sheets.Any() ? "Sheets read" : "No sheets found",
                Data = sheets
            });
        }

        // POST /api/importdata/import/student  (multipart/form-data)
        [HttpPost("import/student")]
        public async Task<IActionResult> ImportStudent([FromForm] ImportStudentRequest req)
        {
            if (req?.File == null) return BadRequest(new ApiResponse<object> { Success = false, Message = "File is required" });

            var rows = await _svc.ImportStudentDataAsync(req);
            return Ok(new ApiResponse<object>
            {
                Success = rows > 0,
                Message = rows > 0 ? "Student Data imported successfully" : "Student Data importing failed"
            });
        }

        // POST /api/importdata/import/elective
        // JSON body with JsonRows array as described in models
        [HttpPost("import/elective")]
        public async Task<IActionResult> ImportElective([FromBody] ImportTvTableRequest req)
        {
            var rows = await _svc.ImportElectiveDataAsync(req);
            return Ok(new ApiResponse<object> { Success = rows > 0, Message = rows > 0 ? "Elective Data imported successfully" : "Elective Data import failed" });
        }

        // POST /api/importdata/import/examsessiondates
        [HttpPost("import/examsessiondates")]
        public async Task<IActionResult> ImportExamSessionDates([FromBody] ImportTvTableRequest req)
        {
            var rows = await _svc.ImportExamSessionDatesAsync(req);
            return Ok(new ApiResponse<object> { Success = rows > 0, Message = rows > 0 ? "Exam Session Dates imported successfully" : "Exam Session Dates import failed" });
        }

        // POST /api/importdata/import/condonation
        [HttpPost("import/condonation")]
        public async Task<IActionResult> ImportCondonation([FromBody] ImportTvTableRequest req)
        {
            var rows = await _svc.ImportCondonationFeeAsync(req);
            return Ok(new ApiResponse<object> { Success = rows > 0, Message = rows > 0 ? "Condonation Fee imported successfully" : "Condonation import failed" });
        }

        // POST /api/importdata/import/registration-block
        [HttpPost("import/registration-block")]
        public async Task<IActionResult> ImportRegistrationBlock([FromBody] ImportTvTableRequest req)
        {
            var rows = await _svc.ImportRegistrationBlockAsync(req);
            return Ok(new ApiResponse<object> { Success = rows > 0, Message = rows > 0 ? "Registration block imported" : "Import failed" });
        }

        // POST /api/importdata/import/halltickets-block
        [HttpPost("import/halltickets-block")]
        public async Task<IActionResult> ImportHallticketsBlock([FromBody] ImportTvTableRequest req)
        {
            var rows = await _svc.ImportHallticketsBlockAsync(req);
            return Ok(new ApiResponse<object> { Success = rows > 0, Message = rows > 0 ? "Halltickets block imported" : "Import failed" });
        }

        // POST /api/importdata/import/result-block
        [HttpPost("import/result-block")]
        public async Task<IActionResult> ImportResultBlock([FromBody] ImportTvTableRequest req)
        {
            var rows = await _svc.ImportResultBlockAsync(req);
            return Ok(new ApiResponse<object> { Success = rows > 0, Message = rows > 0 ? "Result block imported" : "Import failed" });
        }

        // POST /api/importdata/import/internal-marks
        [HttpPost("import/internal-marks")]
        public async Task<IActionResult> ImportInternalMarks([FromBody] ImportTvTableRequest req)
        {
            var rows = await _svc.ImportInternalMarksAsync(req);
            return Ok(new ApiResponse<object> { Success = rows > 0, Message = rows > 0 ? "Internal marks imported" : "Import failed" });
        }

        // POST /api/importdata/import/auditcourse
        [HttpPost("import/auditcourse")]
        public async Task<IActionResult> ImportAuditCourse([FromBody] ImportTvTableRequest req)
        {
            var rows = await _svc.ImportAuditCourseDataAsync(req);
            return Ok(new ApiResponse<object> { Success = rows > 0, Message = rows > 0 ? "Audit course imported" : "Import failed" });
        }

        // POST /api/importdata/import/rcrv
        [HttpPost("import/rcrv")]
        public async Task<IActionResult> ImportRCRV([FromBody] ImportTvTableRequest req)
        {
            var rows = await _svc.ImportRCRVMarksAsync(req);
            return Ok(new ApiResponse<object> { Success = rows > 0, Message = rows > 0 ? "RCRV marks imported" : "Import failed" });
        }

        // GET /api/importdata/excelformat/columns?tableName=TBL_STDDATA
        [HttpGet("excelformat/columns")]
        public async Task<IActionResult> GetExcelFormatColumns([FromQuery] string tableName, [FromQuery] string userId)
        {
            var colData = await _svc.LoadExcelFormatExtraColumnsAsync(tableName, userId);
            return Ok(new ApiResponse<object> { Success = colData != null, Message = colData != null ? "Format loaded" : "Not found", Data = colData });
        }
    }
}
