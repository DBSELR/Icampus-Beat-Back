using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResultCheckListController : BaseApiController
    {
        private readonly IResultCheckListService _svc;

        public ResultCheckListController(IResultCheckListService svc)
        {
            _svc = svc;
        }

        // GET api/resultchecklist/sems?course=BTECH&examMY=NOV2024&regu=R20
        // Page_Load — populate ddlSem
        // SQL: SELECT DISTINCT SEM FROM TBL_SH WHERE REGULATION=@Regu AND EXAMMY=@ExamMY AND COURSE=@Course ORDER BY SEM
        [HttpGet("sems")]
        public async Task<IActionResult> LoadSems(
            [FromQuery] string course,
            [FromQuery] string examMY,
            [FromQuery] string regu)
        {
            var data = await _svc.LoadSemsAsync(course, examMY, regu);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Semesters loaded." : "No semesters found.",
                Data    = data
            });
        }

        // GET api/resultchecklist/data?course=BTECH&examMY=NOV2024&regu=R20&sem=1&isReadmit=false&readmitRegu=&checkListType=1
        // btnView_Click — load result check list
        // SP: isReadmit=true → SP_REP_MRK_CHKLIST_Readmit (@Course,@ExamMY,@Regu,@Sem,@ReadmitRegu)
        //                else → SP_REP_MRK_CHKLIST (@Course,@ExamMY,@Regu,@Sem)
        // checkListType: 1 = "RESULT CHECK LIST - I" / "READMIT RESULT CHECK LIST - I"
        //                2 = "RESULT CHECK LIST - II" / "READMIT RESULT CHECK LIST - II"
        //   → Controls Crystal Report page title on frontend only; same SP for both types.
        [HttpGet("data")]
        public async Task<IActionResult> GetData(
            [FromQuery] string course,
            [FromQuery] string examMY,
            [FromQuery] string regu,
            [FromQuery] string sem,
            [FromQuery] bool   isReadmit    = false,
            [FromQuery] string readmitRegu  = "",
            [FromQuery] int    checkListType = 1)
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
                Message = data != null && data.Any() ? "Result check list loaded." : "No data found.",
                Data    = data
            });
        }
    }
}
