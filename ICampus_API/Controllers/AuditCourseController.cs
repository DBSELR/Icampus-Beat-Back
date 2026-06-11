using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuditCourseController : BaseApiController
    {
        private readonly IAuditCourseService _svc;

        public AuditCourseController(IAuditCourseService svc)
        {
            _svc = svc;
        }

        // GET api/auditcourse/batch?course=BTECH
        // Page_Load → load_audit_regu → ddlbatch
        // SQL: SELECT DISTINCT REGU FROM tbl_audit_course WHERE COURSE=@Course
        [HttpGet("batch")]
        public async Task<IActionResult> GetBatch([FromQuery] string course)
        {
            if (string.IsNullOrWhiteSpace(course))
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "course is required.",
                    Data    = null
                });

            var data = await _svc.LoadBatchAsync(course);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Batch/Regulation list loaded.",
                Data    = data
            });
        }

        // GET api/auditcourse/sem?course=BTECH&regu=R20
        // AuditCourse_data_LoadSemesters → ddlSemester
        // SP: proc_load_audit_sem (@Course, @Regu)
        [HttpGet("sem")]
        public async Task<IActionResult> GetSem([FromQuery] string course, [FromQuery] string regu)
        {
            if (string.IsNullOrWhiteSpace(course) || string.IsNullOrWhiteSpace(regu))
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "course and regu are required.",
                    Data    = null
                });

            var data = await _svc.LoadSemAsync(course, regu);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Semester list loaded.",
                Data    = data
            });
        }

        // GET api/auditcourse/academic-year?course=BTECH
        // Page_Load → Load_AcademicYear → ddlyear (labeled "Academic_year")
        // SQL: SELECT DISTINCT Academic_year FROM tbl_audit_course WHERE COURSE=@Course ORDER BY Academic_year
        [HttpGet("academic-year")]
        public async Task<IActionResult> GetAcademicYear([FromQuery] string course)
        {
            if (string.IsNullOrWhiteSpace(course))
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "course is required.",
                    Data    = null
                });

            var data = await _svc.LoadAcademicYearAsync(course);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Academic year list loaded.",
                Data    = data
            });
        }

        // GET api/auditcourse/data?course=BTECH&regu=R20&sem=1&academicYear=2023-24
        // btnview_Click / btnDownLoad_Click → Audit_CourseData → PROC_AuditCourse_Data
        // SP: PROC_AuditCourse_Data (@Course, @Regu, @Sem, @Academic_year)
        // chkIsPrint checkbox = frontend-only print/export option (does not change SP)
        // Crystal: CrystalReportViewer2
        [HttpGet("data")]
        public async Task<IActionResult> GetData(
            [FromQuery] string course,
            [FromQuery] string regu,
            [FromQuery] string sem,
            [FromQuery] string academicYear)
        {
            if (string.IsNullOrWhiteSpace(course) || string.IsNullOrWhiteSpace(regu) ||
                string.IsNullOrWhiteSpace(sem) || string.IsNullOrWhiteSpace(academicYear))
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "course, regu, sem, and academicYear are required.",
                    Data    = null
                });

            var data = await _svc.GetDataAsync(course, regu, sem, academicYear);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Audit course data loaded." : "No data found.",
                Data    = data
            });
        }
    }
}
