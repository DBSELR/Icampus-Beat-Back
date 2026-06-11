using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BranchWisePercentController : BaseApiController
    {
        private readonly IBranchWisePercentService _svc;

        public BranchWisePercentController(IBranchWisePercentService svc)
        {
            _svc = svc;
        }

        // GET api/branchwisepercent/sems?course=BTECH&examMY=NOV2024
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

        // GET api/branchwisepercent/data?course=BTECH&examMY=NOV2024&regu=R20&sem=3&isRv=false
        // btnView_Click / btnDownLoad_Click
        // SP: sp_COURSE_STAT (@Course, @ExamMY, @Regu, @Sem, @IsRv)
        // isRv=false (default) → @IsRv='N' — Before RV/SM/GR (ChkIsrv unchecked)
        // isRv=true            → @IsRv='Y' — After RV/SM/GR (ChkIsrv checked)
        // Note: for Supply exam (Chkregsup), pass the supply examMY in the examMY param
        [HttpGet("data")]
        public async Task<IActionResult> GetData(
            [FromQuery] string course,
            [FromQuery] string examMY,
            [FromQuery] string regu,
            [FromQuery] string sem,
            [FromQuery] bool   isRv  = false,
            [FromQuery] bool   isSup = false)
        {
            if (string.IsNullOrWhiteSpace(sem))
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Please select Semester.",
                    Data    = null
                });

            var data = await _svc.GetDataAsync(course, examMY, regu, sem, isRv, isSup);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Branch-wise percentage data loaded." : "No data found.",
                Data    = data
            });
        }
    }
}
