using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BranchwiseCourseSecPercentController : BaseApiController
    {
        private readonly IBranchwiseCourseSecPercentService _svc;

        public BranchwiseCourseSecPercentController(IBranchwiseCourseSecPercentService svc)
        {
            _svc = svc;
        }

        // GET api/branchwisecoursesecpercent/sems?course=BTECH&examMY=NOV2024
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

        // GET api/branchwisecoursesecpercent/data?course=BTECH&examMY=NOV2024&regu=20&sem=3&isRv=false
        // btnView_Click
        // SP: PROC_CLASSWISE_COUNT (@Course, @ExamMY, @Regu, @Sem, @IsRv)
        // @IsRv: false → 'N' (regular), true → 'Y' (after RV/SM, ChkIsrv checked)
        // Crystal Report: ClassWiseCnt.rpt
        [HttpGet("data")]
        public async Task<IActionResult> GetData(
            [FromQuery] string course,
            [FromQuery] string examMY,
            [FromQuery] string regu,
            [FromQuery] string sem,
            [FromQuery] bool isRv = false)
        {
            if (string.IsNullOrWhiteSpace(course) || string.IsNullOrWhiteSpace(examMY) ||
                string.IsNullOrWhiteSpace(sem))
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "course, examMY, and sem are required.",
                    Data    = null
                });

            var data = await _svc.GetDataAsync(course, examMY, regu, sem, isRv);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Branch-wise course section percent data loaded." : "No data found.",
                Data    = data
            });
        }
    }
}
