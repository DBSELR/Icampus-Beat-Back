using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UniversitySubjectDataController : BaseApiController
    {
        private readonly IUniversitySubjectDataService _svc;

        public UniversitySubjectDataController(IUniversitySubjectDataService svc)
        {
            _svc = svc;
        }

        // GET api/universitysubjectdata/batch?course=BTECH&regulation=R20
        // Page_Load → loadCombo → ddlbatch
        // SQL: SELECT DISTINCT EXAMMY,AEXAMID FROM TBL_EXAMS WHERE COURSE=@Course AND REGULATION=@Regulation ORDER BY AEXAMID DESC
        [HttpGet("batch")]
        public async Task<IActionResult> GetBatch([FromQuery] string course, [FromQuery] string regulation)
        {
            if (string.IsNullOrWhiteSpace(course) || string.IsNullOrWhiteSpace(regulation))
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "course and regulation are required.",
                    Data    = null
                });

            var data = await _svc.LoadBatchAsync(course, regulation);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Batch list loaded.",
                Data    = data
            });
        }

        // GET api/universitysubjectdata/sem?course=BTECH&examMY=NOV2024&regulation=R20
        // SubjectData_LoadSemesters → ddlSemester
        // SP: Proc_SubjectData_LoadSem (@Course, @ExamMY, @Regulation)
        [HttpGet("sem")]
        public async Task<IActionResult> GetSem([FromQuery] string course, [FromQuery] string examMY, [FromQuery] string regulation = "")
        {
            if (string.IsNullOrWhiteSpace(course) || string.IsNullOrWhiteSpace(examMY))
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "course and examMY are required.",
                    Data    = null
                });

            var data = await _svc.LoadSemAsync(course, examMY, regulation);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Semester list loaded.",
                Data    = data
            });
        }

        // GET api/universitysubjectdata/subject-list?course=BTECH&regulation=R20&examMY=NOV2024&sem=1
        // btnSubjectList_Click → "Subjects Data"
        // SP: Proc_University_SubjectList (@Course, @Regulation, @ExamMY, @Sem)
        [HttpGet("subject-list")]
        public async Task<IActionResult> GetSubjectList(
            [FromQuery] string course,
            [FromQuery] string regulation,
            [FromQuery] string examMY,
            [FromQuery] string sem)
        {
            if (string.IsNullOrWhiteSpace(course) || string.IsNullOrWhiteSpace(regulation) ||
                string.IsNullOrWhiteSpace(examMY) || string.IsNullOrWhiteSpace(sem))
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "course, regulation, examMY, and sem are required.",
                    Data    = null
                });

            var data = await _svc.GetSubjectListAsync(course, regulation, examMY, sem);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Subject list loaded." : "No subjects found.",
                Data    = data
            });
        }

        // GET api/universitysubjectdata/students-data?course=BTECH&regulation=R20&examMY=NOV2024&sem=1&regSup=REG
        // btnSubjectData_Click → "Students Data"
        // SP: Proc_University_SubjectData (@Course, @Regulation, @ExamMY, @Sem, @RegSup)
        // regSup values: "REG" (Regular) or "SUP" (Supplementary) — from ddlregsup hardcoded options
        // ChkIsrv (hidden) = RV filter, display:none in ASPX — frontend-only, not passed to SP
        [HttpGet("students-data")]
        public async Task<IActionResult> GetStudentsData(
            [FromQuery] string course,
            [FromQuery] string regulation,
            [FromQuery] string examMY,
            [FromQuery] string sem,
            [FromQuery] string regSup = "REG")
        {
            if (string.IsNullOrWhiteSpace(course) || string.IsNullOrWhiteSpace(regulation) ||
                string.IsNullOrWhiteSpace(examMY) || string.IsNullOrWhiteSpace(sem))
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "course, regulation, examMY, and sem are required.",
                    Data    = null
                });

            var data = await _svc.GetStudentsDataAsync(course, regulation, examMY, sem, regSup);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Students data loaded." : "No data found.",
                Data    = data
            });
        }
    }
}
