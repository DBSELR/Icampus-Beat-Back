using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BranchWiseSectionPercentController : BaseApiController
    {
        private readonly IBranchWiseSectionPercentService _svc;

        public BranchWiseSectionPercentController(IBranchWiseSectionPercentService svc)
        {
            _svc = svc;
        }

        // GET api/branchwisesectionpercent/sems?course=BTECH&examMY=NOV2024
        // ddlSemester — Page_Load
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

        // GET api/branchwisesectionpercent/data?course=BTECH&examMY=NOV2024&regu=20&sem=3
        // btnView_Click
        // SP: sp_PAP_SEC_PERCENT (@Course, @ExamMY, @Regu, @Sem)
        // No RV mode — no ChkIsrv on this page
        [HttpGet("data")]
        public async Task<IActionResult> GetData(
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

            var data = await _svc.GetDataAsync(course, examMY, regu, sem);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Branch-wise section percent data loaded." : "No data found.",
                Data    = data
            });
        }
    }
}
