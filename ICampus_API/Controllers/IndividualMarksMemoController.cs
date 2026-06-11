using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IndividualMarksMemoController : BaseApiController
    {
        private readonly IIndividualMarksMemoService _svc;

        public IndividualMarksMemoController(IIndividualMarksMemoService svc)
        {
            _svc = svc;
        }

        // GET api/individualmarksmemo/sems?course=BTECH&examMY=NOV2024&regu=20
        // Page_Load — populate ddlSemester
        // SQL: SELECT DISTINCT CAST(SEM AS VARCHAR(250)) SEM FROM tbl_sh
        //      WHERE COURSE=@Course AND Exammy=@ExamMY AND Regulation=@Regu ORDER BY SEM
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

        // GET api/individualmarksmemo/branches?course=BTECH
        // Page_Load — populate ddlBranch
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

        // GET api/individualmarksmemo/valid-regnos?course=B.TECH&examMY=May-2024&regulation=R20&semester=8&branch=CE
        // Diagnostic: returns up to 10 REGNOs from tbl_sh matching the given filters.
        // Use this to find a valid regNo to pass to the /data endpoint.
        [HttpGet("valid-regnos")]
        public async Task<IActionResult> GetValidRegnos(
            [FromQuery] string course,
            [FromQuery] string examMY,
            [FromQuery] string regulation,
            [FromQuery] string semester = "",
            [FromQuery] string branch   = "")
        {
            if (string.IsNullOrWhiteSpace(course) || string.IsNullOrWhiteSpace(examMY) || string.IsNullOrWhiteSpace(regulation))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "course, examMY, and regulation are required.", Data = null });

            var data = await _svc.GetValidRegnosAsync(course, examMY, regulation, semester, branch);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any()
                    ? "Sample REGNOs found. Use one in the /data endpoint."
                    : $"No REGNOs found for course={course} examMY={examMY} regulation={regulation} semester={semester} branch={branch}.",
                Data = data
            });
        }

        // GET api/individualmarksmemo/student-info?regNo=20B91A0501
        // txtRegNo onBlur — auto-fill student details (name, course, branch, photo)
        // SQL: SELECT SNAME, COURSE, GRP, PHOTO FROM TBL_STDDATA WHERE REGNO=@RegNo
        [HttpGet("student-info")]
        public async Task<IActionResult> GetStudentInfo([FromQuery] string regNo)
        {
            if (string.IsNullOrWhiteSpace(regNo))
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "regNo is required.",
                    Data    = null
                });

            var data = await _svc.GetStudentInfoAsync(regNo);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Student info loaded." : "Student not found.",
                Data    = data
            });
        }

        // GET api/individualmarksmemo/data?regulation=R20&examMY=NOV2024&course=BTECH&semester=3&branch=CSE&regNo=20B91A0501&reportType=1
        // btnview_Click / btnDownLoad_Click — load MarksMemo or GradeCard for a student
        // SP: SP_MRK_MEMO_REGNO (@REGULATION, @EXAMMY, @Course, @SEMESTER, @RV='N', @BRANCH, @REGNO)
        // reportType (frontend-only, same SP regardless):
        //   1 = MarksMemo  → Crystal: MarksMemo_Regno.rpt    (chkgcard=true)
        //   2 = GradeCard  → Crystal: GradeCard_btech_Regno.rpt / GradeCard_Regno.rpt (chkgcard=false)
        [HttpGet("data")]
        public async Task<IActionResult> GetData(
            [FromQuery] string regulation,
            [FromQuery] string examMY,
            [FromQuery] string course,
            [FromQuery] string semester,
            [FromQuery] string branch,
            [FromQuery] string regNo       = "",
            [FromQuery] int    reportType  = 1)
        {
            if (string.IsNullOrWhiteSpace(course) || string.IsNullOrWhiteSpace(examMY))
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "course and examMY are required.",
                    Data    = null
                });

            var data = await _svc.GetDataAsync(regulation, examMY, course, semester, branch, regNo);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Marks memo data loaded." : "No data found.",
                Data    = data
            });
        }
    }
}
