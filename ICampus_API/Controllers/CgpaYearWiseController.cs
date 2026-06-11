using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CgpaYearWiseController : BaseApiController
    {
        private readonly ICgpaYearWiseService _svc;

        public CgpaYearWiseController(ICgpaYearWiseService svc)
        {
            _svc = svc;
        }

        // GET api/cgpayearwise/exammy?course=
        // ddlexammy population (Page_Load)
        // SP: SPM_EXAMS_ExamMY_Load — pass course='' for all exam months
        [HttpGet("exammy")]
        public async Task<IActionResult> LoadExammy(
            [FromQuery] string course = "",
            [FromQuery] string regulation = "")
        {
            var data = await _svc.LoadExammyAsync(course, regulation);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Exam months loaded" : "No exam months found",
                Data = data
            });
        }

        // GET api/cgpayearwise/batch?examMY=
        // ddlbatch population (cascades from ddlexammy)
        // SQL: SELECT DISTINCT REGU FROM TBL_SH WHERE EXAMMY=@ExamMY
        [HttpGet("batch")]
        public async Task<IActionResult> LoadBatch([FromQuery] string examMY)
        {
            var data = await _svc.LoadBatchAsync(examMY);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Batches loaded" : "No batches found",
                Data = data
            });
        }

        // GET api/cgpayearwise/sems?examMY=&batch=
        // ddlsemester population (cascades from ddlexammy + ddlbatch)
        // SQL: SELECT DISTINCT CAST(SEM AS VARCHAR) SEM FROM TBL_SH WHERE EXAMMY=@ExamMY AND REGU=@Batch
        [HttpGet("sems")]
        public async Task<IActionResult> LoadSemesters([FromQuery] string examMY, [FromQuery] string batch)
        {
            var data = await _svc.LoadSemestersAsync(examMY, batch);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Semesters loaded" : "No semesters found",
                Data = data
            });
        }

        // GET api/cgpayearwise/download?regulation=&course=&examMY=&regu=&sem=
        // btnDownLoad_Click — returns CGPA Year Wise data (Excel source)
        // SP: sp_cgpa_excel (@REGULATION, @COURSE, @EXAMMY, @REGU, @SEM)
        // Validation: ExamMY must not be empty
        [HttpGet("download")]
        public async Task<IActionResult> Download(
            [FromQuery] string regulation = "",
            [FromQuery] string course     = "",
            [FromQuery] string examMY     = "",
            [FromQuery] string regu       = "",
            [FromQuery] string sem        = "")
        {
            if (string.IsNullOrWhiteSpace(examMY))
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Select ExamMY",
                    Data = null
                });

            var data = await _svc.DownloadAsync(regulation, course, examMY, regu, sem);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "CGPA Year Wise data loaded" : "No data found",
                Data = data
            });
        }
    }
}
