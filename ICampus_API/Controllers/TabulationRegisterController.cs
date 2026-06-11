using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TabulationRegisterController : BaseApiController
    {
        private readonly ITabulationRegisterService _svc;

        public TabulationRegisterController(ITabulationRegisterService svc)
        {
            _svc = svc;
        }

        // GET api/tabulationregister/batches?course=BTECH
        // ddlBatch AutoPostBack — populate batch/regulation dropdown
        // SQL: SELECT DISTINCT CAST(REGU AS VARCHAR) AS REGU,
        //      '20'+CAST(REGU AS VARCHAR)+'-'+CAST(REGU+MAXSEM/2 AS VARCHAR) BATCH
        //      FROM TBL_COURSE WHERE COURSE=@Course ORDER BY REGU
        [HttpGet("batches")]
        public async Task<IActionResult> LoadBatches([FromQuery] string course)
        {
            var data = await _svc.LoadBatchesAsync(course);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Batches loaded." : "No batches found.",
                Data    = data
            });
        }

        // GET api/tabulationregister/branches?course=BTECH&regu=20
        // ddlBranch — loaded on ddlBatch_SelectedIndexChanged
        // SQL: SELECT DISTINCT GRP FROM TBL_COURSE WHERE COURSE=@Course AND REGU=@Regu ORDER BY GRP
        [HttpGet("branches")]
        public async Task<IActionResult> LoadBranches(
            [FromQuery] string course,
            [FromQuery] string regu)
        {
            var data = await _svc.LoadBranchesAsync(course, regu);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Branches loaded." : "No branches found.",
                Data    = data
            });
        }

        // GET api/tabulationregister/examMYs?course=BTECH&regu=20
        // cmbExamMY — exam month/year dropdown
        // SQL: SELECT DISTINCT EXAMMY, AEXAMID FROM TBL_EXAMS WHERE COURSE=@Course
        //      and REGULATION=@Regu ORDER BY AEXAMID DESC
        [HttpGet("examMYs")]
        public async Task<IActionResult> LoadExamMYs(
            [FromQuery] string course,
            [FromQuery] string regu)
        {
            var data = await _svc.LoadExamMYsAsync(course, regu);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "ExamMYs loaded." : "No ExamMYs found.",
                Data    = data
            });
        }

        // GET api/tabulationregister/data?course=BTECH&examMY=NOV2024&regu=20&branch=CSE&regNo=&isReadmit=false&readmitRegu=
        // btnView_Click / btnexport_Click — load tabulation register
        // SP: isReadmit=false → SP_TABULATION_REGISTER (@Course,@ExamMY,@Regu,@Branch,@RegNo)
        //     isReadmit=true  → SP_TABULATION_REGISTER_Readmit + @ReadmitRegu
        // Crystal reports: TabulationRegister_btech.rpt / _mca.rpt / _mba.rpt (frontend selects by Course)
        [HttpGet("data")]
        public async Task<IActionResult> GetData(
            [FromQuery] string course,
            [FromQuery] string examMY,
            [FromQuery] string regu,
            [FromQuery] string branch,
            [FromQuery] string regNo       = "",
            [FromQuery] bool   isReadmit   = false,
            [FromQuery] string readmitRegu = "")
        {
            if (string.IsNullOrWhiteSpace(course) || string.IsNullOrWhiteSpace(examMY) ||
                string.IsNullOrWhiteSpace(regu))
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Course, ExamMY, and Regu are required.",
                    Data    = null
                });

            var data = await _svc.GetDataAsync(course, examMY, regu, branch, regNo, isReadmit, readmitRegu);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Tabulation register data loaded." : "No data found.",
                Data    = data
            });
        }
    }
}
