using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RvReportsController : BaseApiController
    {
        private readonly IRvReportsService _svc;

        public RvReportsController(IRvReportsService svc)
        {
            _svc = svc;
        }

        // GET api/rvreports/sems?course=BTECH
        // Page_Load — populate ddlSemester
        // SQL: SELECT DISTINCT cast(SEM as varchar(250)) SEM FROM tbl_sh WHERE COURSE=@Course
        // Note: Course-only filter (ExamMY/Regu come from session in original ASPX)
        [HttpGet("sems")]
        public async Task<IActionResult> LoadSems([FromQuery] string course)
        {
            var data = await _svc.LoadSemsAsync(course);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Semesters loaded." : "No semesters found.",
                Data    = data
            });
        }

        // GET api/rvreports/data?course=BTECH&examMY=NOV2024&regu=R20&sem=1&isReadmit=false&readmitRegu=&reportType=1
        // btnExport_Click — export RV report
        // SP: isReadmit=false → PROC_RV_REPDATA (@Course,@ExamMY,@Regu,@Sem)
        //     isReadmit=true  → PROC_RV_REPDATA_Readmit + @ReadmitRegu
        // reportType: 1=rbtn1 (Check List-I), 2=rbtn2 (Check List-II), 3=rbtnRSheet (Result Sheet)
        //   → same SP for all 3 types; frontend uses reportType to select Crystal Report title/template
        [HttpGet("data")]
        public async Task<IActionResult> GetData(
            [FromQuery] string course,
            [FromQuery] string examMY,
            [FromQuery] string regu,
            [FromQuery] string sem,
            [FromQuery] bool   isReadmit   = false,
            [FromQuery] string readmitRegu = "",
            [FromQuery] int    reportType  = 1)
        {
            if (string.IsNullOrWhiteSpace(course) || string.IsNullOrWhiteSpace(examMY) ||
                string.IsNullOrWhiteSpace(sem))
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Course, ExamMY, and Sem are required.",
                    Data    = null
                });

            var data = await _svc.GetDataAsync(course, examMY, regu, sem, isReadmit, readmitRegu);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "RV report data loaded." : "No data found.",
                Data    = data
            });
        }
    }
}
