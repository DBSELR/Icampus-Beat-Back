using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RvMarksCheckListController : BaseApiController
    {
        private readonly IRvMarksCheckListService _svc;

        public RvMarksCheckListController(IRvMarksCheckListService svc)
        {
            _svc = svc;
        }

        // GET api/rvmarkschecklist/sems?course=BTECH
        // Page_Load + ddlSemester_SelectedIndexChanged
        // SQL: SELECT DISTINCT cast(SEM as varchar(250)) SEM FROM tbl_sh WHERE COURSE=@Course
        // Note: No EXAMMY/Regu filter — matches Page_Load behaviour in RvMarksCheckList.aspx
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

        // GET api/rvmarkschecklist/data?course=BTECH&examMY=NOV2024&regu=20&sem=3&isReadmit=false&readmitRegu=&reportType=1
        // btnExport_Click — export RV Marks Check List or RV Result Sheet (Crystal Report)
        // SP: isReadmit=true  → PROC_RV_REPDATA_Readmit (@Course,@ExamMY,@Regu,@Sem,@ReadmitRegu)
        //                else → PROC_RV_REPDATA (@Course,@ExamMY,@Regu,@Sem)
        // reportType is frontend-only (same SP regardless):
        //   1 = Check List-I  → Crystal Report: RVMarksChecklist.rpt  (title: "RESULT CHECK LIST - I")
        //   2 = Check List-II → Crystal Report: RVMarksChecklist.rpt  (title: "RESULT CHECK LIST - II")
        //   3 = Result Sheet  → Crystal Report: RVResultSheet.rpt     (title: "RV RESULT SHEET")
        //   Readmit variants: "READMIT RESULT CHECK LIST - I/II", "READMIT RV RESULT SHEET"
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
                    Message = "course, examMY, and sem are required.",
                    Data    = null
                });

            var data = await _svc.GetDataAsync(course, examMY, regu, sem, isReadmit, readmitRegu);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "RV marks data loaded." : "No data found.",
                Data    = data
            });
        }
    }
}
