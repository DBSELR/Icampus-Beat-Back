using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResultSheetController : BaseApiController
    {
        private readonly IResultSheetService _svc;

        public ResultSheetController(IResultSheetService svc)
        {
            _svc = svc;
        }

        // GET api/resultsheet/sems?course=BTECH&examMY=NOV2024&regu=R20
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

        // GET api/resultsheet/data?course=BTECH&examMY=NOV2024&regu=R20&sem=1&isReadmit=false&readmitRegu=
        // btnview_Click — load V1 (Marks) result sheet
        // SP: isReadmit=true → SP_REP_MRK_CHKLIST_Readmit (@Course,@ExamMY,@Regu,@Sem,@ReadmitRegu)
        //                else → SP_REP_MRK_CHKLIST (@Course,@ExamMY,@Regu,@Sem)
        // Note: chkRv is HIDDEN in ResultSheet.aspx — RV disabled for this page.
        //       Crystal report: ResTR.rpt
        [HttpGet("data")]
        public async Task<IActionResult> GetData(
            [FromQuery] string course,
            [FromQuery] string examMY,
            [FromQuery] string regu,
            [FromQuery] string sem,
            [FromQuery] bool   isReadmit   = false,
            [FromQuery] string readmitRegu = "")
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
                Message = data != null && data.Any() ? "Result sheet data loaded." : "No data found.",
                Data    = data
            });
        }
    }
}
