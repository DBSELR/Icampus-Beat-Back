using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AwardDegreeController : BaseApiController
    {
        private readonly IAwardDegreeService _svc;

        public AwardDegreeController(IAwardDegreeService svc)
        {
            _svc = svc;
        }

        // GET api/awarddegree/batches?course=BTECH
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

        // GET api/awarddegree/exammys?course=BTECH&regu=20
        // cmbExamMY — ddlbatch_SelectedIndexChanged (AutoPostBack)
        // SQL: SELECT DISTINCT EXAMMY, AEXAMID FROM TBL_EXAMS
        //      WHERE COURSE=@Course AND REGULATION=@Regu ORDER BY AEXAMID DESC
        [HttpGet("exammys")]
        public async Task<IActionResult> LoadExamMYs(
            [FromQuery] string course,
            [FromQuery] string regu)
        {
            var data = await _svc.LoadExamMYsAsync(course, regu);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "ExamMYs loaded." : "No ExamMYs found.",
                Data    = data
            });
        }

        // GET api/awarddegree/data?regu=20&examMY=NOV2024
        // Crystal Report (cmbExamMY_SelectedIndexChanged auto-loads) + Excel (btnDownLoad_Click)
        // SP: PROC_AWARD_DEGREE (@Regu, @ExamMY)
        // Crystal Report: AwardDegreeList.rpt
        // Excel filename: AwardDegreeList_[regu].xlsx
        [HttpGet("data")]
        public async Task<IActionResult> GetData(
            [FromQuery] string regu,
            [FromQuery] string examMY,
            [FromQuery] string course = "")
        {
            if (string.IsNullOrWhiteSpace(regu) || string.IsNullOrWhiteSpace(examMY))
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "regu and examMY are required.",
                    Data    = null
                });

            var data = await _svc.GetDataAsync(regu, examMY, course);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Award degree list loaded." : "No data found.",
                Data    = data
            });
        }
    }
}
