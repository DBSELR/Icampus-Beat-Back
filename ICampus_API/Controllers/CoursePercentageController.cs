using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CoursePercentageController : BaseApiController
    {
        private readonly ICoursePercentageService _svc;

        public CoursePercentageController(ICoursePercentageService svc)
        {
            _svc = svc;
        }

        // GET api/coursepercentage/sems?course=BTECH&examMY=NOV2024
        // Page_Load — populate ddlSemester
        // Inline SQL: SELECT DISTINCT cast(SEM as varchar(250)) SEM
        //   FROM tbl_sh WHERE COURSE=@Course AND EXAMMY=@ExamMY ORDER BY SEM
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

        // GET api/coursepercentage/data?course=BTECH&examMY=NOV2024&regu=R20&sem=3&isSup=false
        // btnView_Click / btnDownLoad_Click
        // isSup=false (default) → sp_PAP_PERCENT       (regular or RV mode — same SP)
        // isSup=true            → SP_Pap_Percent_Sup   (Chkregsup checked)
        // Returns: PAP_STAT data — paper-wise pass percentage for the semester
        [HttpGet("data")]
        public async Task<IActionResult> GetData(
            [FromQuery] string course,
            [FromQuery] string examMY,
            [FromQuery] string regu,
            [FromQuery] string sem,
            [FromQuery] bool   isSup = false,
            [FromQuery] bool   isRv  = false)
        {
            if (string.IsNullOrWhiteSpace(sem))
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Please select Semester.",
                    Data    = null
                });

            var data = await _svc.GetDataAsync(course, examMY, regu, sem, isSup, isRv);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Course percentage data loaded." : "No data found.",
                Data    = data
            });
        }
    }
}
