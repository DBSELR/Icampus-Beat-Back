using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GraceDataController : BaseApiController
    {
        private readonly IGraceDataService _svc;

        public GraceDataController(IGraceDataService svc)
        {
            _svc = svc;
        }

        // GET api/gracedata/batch?course=
        // Page_Load — populate ddlbatch
        [HttpGet("batch")]
        public async Task<IActionResult> LoadBatch([FromQuery] string course = "")
        {
            var data = await _svc.LoadBatchAsync(course);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Batches loaded." : "No batches found.",
                Data = data
            });
        }

        // GET api/gracedata/sem?course=&regu=
        // Page_Load — populate ddlSemester (proc_load_audit_sem)
        [HttpGet("sem")]
        public async Task<IActionResult> LoadSemester(
            [FromQuery] string course, [FromQuery] string regu)
        {
            var data = await _svc.LoadSemesterAsync(course, regu);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Semesters loaded." : "No semesters found.",
                Data = data
            });
        }

        // GET api/gracedata/data?course=&regu=&exammy=&sem=&isle=false
        // btnGetData_Click — proc_Grace_Data(@Course,@Regulation,@Exammy,@Batch,@Sem,@is_LE)
        // isle: true = Lateral Entry ('Y'), false = regular ('N')
        [HttpGet("data")]
        public async Task<IActionResult> GetGraceData(
            [FromQuery] string course,
            [FromQuery] string regu,
            [FromQuery] string exammy = "",
            [FromQuery] string sem    = "",
            [FromQuery] bool   isle   = false)
        {
            if (string.IsNullOrWhiteSpace(course))
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Course is required.",
                    Data = null
                });

            if (string.IsNullOrWhiteSpace(regu))
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Please select Batch.",
                    Data = null
                });

            var data = await _svc.GetGraceDataAsync(course, regu, exammy, sem, isle);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Grace eligible data loaded." : "No grace eligible data found.",
                Data = data
            });
        }
    }
}
