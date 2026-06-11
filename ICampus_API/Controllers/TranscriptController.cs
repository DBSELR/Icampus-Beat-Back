using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TranscriptController : BaseApiController
    {
        private readonly ITranscriptService _svc;

        public TranscriptController(ITranscriptService svc)
        {
            _svc = svc;
        }

        // GET api/transcript/sems?course=BTECH&examMY=NOV2024&regu=20
        // ddlSemester — Page_Load
        // SQL: SELECT DISTINCT cast(SEM as varchar(250)) SEM
        //      FROM tbl_sh WHERE COURSE=@Course AND Exammy=@ExamMY AND Regulation=@Regu ORDER BY SEM
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

        // GET api/transcript/branches?course=BTECH
        // ddlBranch — Page_Load
        // SQL: SELECT DISTINCT grp FROM tbl_sh WHERE COURSE=@Course ORDER BY grp
        [HttpGet("branches")]
        public async Task<IActionResult> LoadBranches([FromQuery] string course)
        {
            var data = await _svc.LoadBranchesAsync(course);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Branches loaded." : "No branches found.",
                Data    = data
            });
        }

        // GET api/transcript/data?course=BTECH&examMY=NOV2024&regu=20&sem=3&branch=CSE&regNo=20B01A0501&isMarksMemo=true
        // btnview_Click (View) and btnDownLoad_Click (Download, PostBackTrigger)
        // isMarksMemo=true  (chkgcard checked, default) → SP_MRK_MEMO
        //   Crystal Reports: Transcript_R14_MBAMCA.rpt / Transcript.rpt etc (frontend selects by course)
        // isMarksMemo=false (chkgcard unchecked) → SP_Transcript_New
        //   Crystal Reports: Transcript_Gr_btech.rpt / Transcript_Gr.rpt etc
        // branch: '' = all branches, 'CSE' = filter to branch
        // regNo:  '' = all students, '20B01A0501' = single student
        [HttpGet("data")]
        public async Task<IActionResult> GetData(
            [FromQuery] string course,
            [FromQuery] string examMY,
            [FromQuery] string regu,
            [FromQuery] string sem,
            [FromQuery] string branch = "",
            [FromQuery] string regNo = "",
            [FromQuery] bool isMarksMemo = true)
        {
            if (string.IsNullOrWhiteSpace(course) || string.IsNullOrWhiteSpace(examMY) ||
                string.IsNullOrWhiteSpace(regu) || string.IsNullOrWhiteSpace(sem))
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "course, examMY, regu, and sem are required.",
                    Data    = null
                });

            var data = await _svc.GetDataAsync(course, examMY, regu, sem, branch, regNo, isMarksMemo);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Transcript data loaded." : "No data found.",
                Data    = data
            });
        }
    }
}
