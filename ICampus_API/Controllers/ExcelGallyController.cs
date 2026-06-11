using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExcelGallyController : BaseApiController
    {
        private readonly IExcelGallyService _svc;

        public ExcelGallyController(IExcelGallyService svc)
        {
            _svc = svc;
        }

        // GET api/excelgally/sems?course=
        // ddlSemester Page_Load population
        // SQL: SELECT DISTINCT CAST(SEM AS VARCHAR(250)) SEM FROM TBL_GPAP
        //      WHERE COURSE = @Course ORDER BY SEM
        [HttpGet("sems")]
        public async Task<IActionResult> LoadSems([FromQuery] string course)
        {
            if (string.IsNullOrWhiteSpace(course))
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Course is required.",
                    Data = null
                });

            var data = await _svc.LoadSemsAsync(course);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Semesters loaded.",
                Data = data
            });
        }

        // GET api/excelgally/branches?course=&sem=
        // ddlBranch population on ddlSemester AutoPostBack
        // SQL: SELECT DISTINCT GRP FROM TBL_GPAP
        //      WHERE COURSE = @Course AND SEM = @Sem ORDER BY GRP
        [HttpGet("branches")]
        public async Task<IActionResult> LoadBranches([FromQuery] string course, [FromQuery] string sem)
        {
            if (string.IsNullOrWhiteSpace(course) || string.IsNullOrWhiteSpace(sem))
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Course and Sem are required.",
                    Data = null
                });

            var data = await _svc.LoadBranchesAsync(course, sem);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Branches loaded.",
                Data = data
            });
        }

        // GET api/excelgally/data?regulation=&course=&examMY=&sem=&branch=&semType=
        // btnExcelExort_Click → get_EXCEL_GALLY
        // SP: PROC_EXCEL_GALLY (@Regulation, @Course, @ExamMy, @Grp, @REGSUP, @Sem)
        [HttpGet("data")]
        public async Task<IActionResult> GetExcelGally(
            [FromQuery] string regulation,
            [FromQuery] string course,
            [FromQuery] string examMY,
            [FromQuery] string sem,
            [FromQuery] string branch,
            [FromQuery] string semType = "Reg")
        {
            if (string.IsNullOrWhiteSpace(course)     ||
                string.IsNullOrWhiteSpace(regulation) ||
                string.IsNullOrWhiteSpace(examMY)     ||
                string.IsNullOrWhiteSpace(sem)        ||
                string.IsNullOrWhiteSpace(branch))
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Regulation, Course, ExamMY, Sem, and Branch are required.",
                    Data = null
                });

            var data = await _svc.GetExcelGallyAsync(regulation, course, examMY, branch, semType, sem);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Data loaded." : "No data found.",
                Data = data
            });
        }

        // GET api/excelgally/backlogs?regulation=&course=&examMY=&sem=&branch=&semType=
        // SP: PROC_GETfiledasubwithmarks — not found in DB, returns empty
        [HttpGet("backlogs")]
        public async Task<IActionResult> GetBacklogsExcel(
            [FromQuery] string regulation,
            [FromQuery] string course,
            [FromQuery] string examMY,
            [FromQuery] string sem,
            [FromQuery] string branch,
            [FromQuery] string semType = "Reg")
        {
            var data = await _svc.GetBacklogsExcelAsync(regulation, course, examMY, branch, semType, sem);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Backlogs loaded." : "No backlogs found.",
                Data = data
            });
        }
    }
}
