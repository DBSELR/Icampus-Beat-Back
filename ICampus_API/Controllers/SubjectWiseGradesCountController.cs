using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubjectWiseGradesCountController : BaseApiController
    {
        private readonly ISubjectWiseGradesCountService _svc;

        public SubjectWiseGradesCountController(ISubjectWiseGradesCountService svc)
        {
            _svc = svc;
        }

        // GET api/subjectwisegradescount/batches?course=BTECH
        // ddlbatch — Page_Load
        // SQL: SELECT DISTINCT CAST(REGU AS VARCHAR) AS REGU,
        //      '20'+CAST(REGU AS VARCHAR)+'-'+CAST(REGU+MAXSEM/2 AS VARCHAR) BATCH
        //      FROM TBL_COURSE WHERE COURSE=@Course ORDER BY REGU
        [HttpGet("batches")]
        public async Task<IActionResult> LoadBatches([FromQuery] string course)
        {
            var data = await _svc.LoadBatchesAsync(course);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Batches loaded." : "No batches found.",
                Data    = data
            });
        }

        // GET api/subjectwisegradescount/sems?course=BTECH&examMY=NOV2024
        // ddlSemester — Page_Load (AutoPostBack)
        // ddlSemester_SelectedIndexChanged auto-reloads Crystal Report
        // SQL: SELECT DISTINCT cast(SEM as varchar(250)) SEM FROM tbl_sh
        //      WHERE COURSE=@Course AND EXAMMY=@ExamMY ORDER BY SEM
        [HttpGet("sems")]
        public async Task<IActionResult> LoadSems(
            [FromQuery] string course,
            [FromQuery] string examMY)
        {
            var data = await _svc.LoadSemsAsync(course, examMY);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Semesters loaded." : "No semesters found.",
                Data    = data
            });
        }

        // GET api/subjectwisegradescount/data?course=BTECH&examMY=NOV2024&regu=20&sem=3
        // btnview_Click → PROC_GRADE_CNT
        // Crystal Report: ResGradesecwise.rpt
        // Title: "RESULTS GRADE ANALYSIS" or "READMIT RESULTS GRADE ANALYSIS"
        // ChkIsreadmitresult is frontend-only — pass isReadmit flag as metadata only
        // Same SP regardless of isReadmit value
        [HttpGet("data")]
        public async Task<IActionResult> GetData(
            [FromQuery] string course,
            [FromQuery] string examMY,
            [FromQuery] string regu,
            [FromQuery] string sem,
            [FromQuery] bool isReadmit = false)
        {
            if (string.IsNullOrWhiteSpace(course) || string.IsNullOrWhiteSpace(examMY) ||
                string.IsNullOrWhiteSpace(sem))
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "course, examMY, and sem are required.",
                    Data    = null
                });

            var data = await _svc.GetViewDataAsync(course, examMY, regu, sem);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Grade analysis data loaded." : "No data found.",
                Data    = data
            });
        }

        // GET api/subjectwisegradescount/excel?course=BTECH&examMY=NOV2024&regu=20&sem=3
        // btnexcel_Click (PostBackTrigger) → PROC_GRADE_CNT_EXCEL
        // Confirmed: App_Web_gp3pforx.dll UTF-16LE offset 191010
        [HttpGet("excel")]
        public async Task<IActionResult> GetExcel(
            [FromQuery] string course,
            [FromQuery] string examMY,
            [FromQuery] string regu,
            [FromQuery] string sem)
        {
            if (string.IsNullOrWhiteSpace(course) || string.IsNullOrWhiteSpace(examMY) ||
                string.IsNullOrWhiteSpace(sem))
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "course, examMY, and sem are required.",
                    Data    = null
                });

            var data = await _svc.GetExcelDataAsync(course, examMY, regu, sem);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Grade analysis Excel data loaded." : "No data found.",
                Data    = data
            });
        }
    }
}
