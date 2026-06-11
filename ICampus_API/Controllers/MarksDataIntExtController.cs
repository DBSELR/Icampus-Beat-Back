using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MarksDataIntExtController : BaseApiController
    {
        private readonly IMarksDataIntExtService _svc;

        public MarksDataIntExtController(IMarksDataIntExtService svc)
        {
            _svc = svc;
        }

        // GET api/marksdataintext/batch
        // Load ddlbatch on Page_Load
        // SQL: SELECT DISTINCT REGU FROM TBL_COURSE ORDER BY REGU
        [HttpGet("batch")]
        public async Task<IActionResult> LoadBatch()
        {
            var data = await _svc.LoadBatchAsync();
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Batches loaded" : "No batches found",
                Data = data
            });
        }

        // GET api/marksdataintext/exammy?regu=
        // Load cmbExamMY — cascades from ddlbatch
        // SQL: SELECT DISTINCT EXAMMY FROM TBL_SH WHERE REGU=@Regu ORDER BY EXAMMY DESC
        // Pass regu='' for all exam months (Select All case)
        [HttpGet("exammy")]
        public async Task<IActionResult> LoadExammy([FromQuery] string regu = "")
        {
            var data = await _svc.LoadExammyAsync(regu);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Exam months loaded" : "No exam months found",
                Data = data
            });
        }

        // GET api/marksdataintext/sems?regu=&examMY=
        // Load cmbSemester — cascades from ddlbatch + cmbExamMY
        // SQL: SELECT DISTINCT CAST(SEM AS VARCHAR) SEM FROM TBL_SH WHERE REGU=@Regu AND EXAMMY=@ExamMY ORDER BY SEM
        [HttpGet("sems")]
        public async Task<IActionResult> LoadSemesters([FromQuery] string regu, [FromQuery] string examMY)
        {
            var data = await _svc.LoadSemestersAsync(regu, examMY);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Semesters loaded" : "No semesters found",
                Data = data
            });
        }

        // GET api/marksdataintext/sh-data?regulation=&course=&regu=&examMY=&sem=
        // Formats 1–3 and 5 — SP: PROC_EXPORT_MARKSDATA (@EXAMMY, @REGULATION, @COURSE, @SEM INT, @REGU)
        // Confirmed: EXEC PROC_EXPORT_MARKSDATA 'May-2024','R20','B.TECH',8,'20' → rows
        [HttpGet("sh-data")]
        public async Task<IActionResult> GetShData(
            [FromQuery] string regulation,
            [FromQuery] string course,
            [FromQuery] string regu,
            [FromQuery] string examMY,
            [FromQuery] string sem = "")
        {
            if (string.IsNullOrWhiteSpace(regulation))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Regulation missing", Data = null });
            if (string.IsNullOrWhiteSpace(course))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Course missing", Data = null });
            if (string.IsNullOrWhiteSpace(regu))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Select Batch", Data = null });
            if (string.IsNullOrWhiteSpace(examMY))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Select ExamMY", Data = null });

            var data = await _svc.GetShDataAsync(regulation, course, regu, examMY, sem);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Marks data loaded" : "No data found",
                Data = data
            });
        }

        // GET api/marksdataintext/result-data?regulation=&course=&examMY=&sem=
        // Format 4 — SP: PROC_EXPORT_RES_DATA (@regulation, @course, @exammy, @sem INT)
        // Confirmed: EXEC PROC_EXPORT_RES_DATA 'R20','B.TECH','May-2024',8 → rows
        [HttpGet("result-data")]
        public async Task<IActionResult> GetResultData(
            [FromQuery] string regulation,
            [FromQuery] string course,
            [FromQuery] string examMY,
            [FromQuery] string sem = "")
        {
            if (string.IsNullOrWhiteSpace(regulation))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Regulation missing", Data = null });
            if (string.IsNullOrWhiteSpace(course))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Course missing", Data = null });
            if (string.IsNullOrWhiteSpace(examMY))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Select ExamMY", Data = null });

            var data = await _svc.GetResultDataAsync(regulation, course, examMY, sem);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Result marks data loaded" : "No data found",
                Data = data
            });
        }
    }
}
