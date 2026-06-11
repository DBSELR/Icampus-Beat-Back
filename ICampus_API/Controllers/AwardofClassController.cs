using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AwardofClassController : BaseApiController
    {
        private readonly IAwardofClassService _svc;

        public AwardofClassController(IAwardofClassService svc)
        {
            _svc = svc;
        }

        // GET api/awardofclass/batches?course=BTECH
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

        // GET api/awardofclass/sems?course=BTECH&examMY=NOV2024
        // ddlSemester — loaded on cmbExamMY_SelectedIndexChanged (AutoPostBack)
        // SQL: SELECT DISTINCT cast(SEM as varchar(250)) SEM FROM tbl_sh
        //      WHERE COURSE=@Course AND EXAMMY=@ExamMY
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

        // GET api/awardofclass/data?course=BTECH&examMY=NOV2024&regu=20&sem=3
        // btnView_Click — load Crystal Report (AwardofClassBranchwise.rpt)
        // SP: PROC_GRADE_CNT (@Course, @ExamMY, @Regu, @Sem)
        // ChkIsrv HIDDEN — no RV mode on this page
        // Title/Subtitle: "RESULTS GRADE ANALYSIS"
        [HttpGet("data")]
        public async Task<IActionResult> GetViewData(
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
                    Message = "Course, ExamMY, and Sem are required.",
                    Data    = null
                });

            var data = await _svc.GetViewDataAsync(course, examMY, regu, sem);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Award of class data loaded." : "No data found.",
                Data    = data
            });
        }

        // GET api/awardofclass/excel?course=BTECH&examMY=NOV2024&regu=20&sem=3
        // btnDownLoad_Click — Excel export
        // SP: PROC_GRADE_CNT_EXCEL (@Course, @ExamMY, @Regu, @Sem)
        // Excel filename: AwardofClassBranchwise.xlsx
        [HttpGet("excel")]
        public async Task<IActionResult> GetExcelData(
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
                    Message = "Course, ExamMY, and Sem are required.",
                    Data    = null
                });

            var data = await _svc.GetExcelDataAsync(course, examMY, regu, sem);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Award of class Excel data loaded." : "No data found.",
                Data    = data
            });
        }
    }
}
