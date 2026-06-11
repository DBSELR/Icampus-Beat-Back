using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CreditSecuredController : BaseApiController
    {
        private readonly ICreditSecuredService _svc;

        public CreditSecuredController(ICreditSecuredService svc)
        {
            _svc = svc;
        }

        // GET api/creditsecured/exammy?course=
        // Load ddlexammy on Page_Load
        // SP: SPM_EXAMS_ExamMY_Load (@Course)
        [HttpGet("exammy")]
        public async Task<IActionResult> LoadExammy([FromQuery] string regulation = "", [FromQuery] string course = "")
        {
            var data = await _svc.LoadExammyAsync(regulation, course);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Exam months loaded" : "No exam months found",
                Data = data
            });
        }

        // GET api/creditsecured/batch?course=
        // Load DDLBatch on Page_Load
        // SQL: SELECT DISTINCT REGU, '20'+REGU+'-'+CAST(REGU+MAXSEM/2 AS VARCHAR) BATCH
        //      FROM TBL_COURSE WHERE COURSE=@Course ORDER BY REGU
        [HttpGet("batch")]
        public async Task<IActionResult> LoadBatch([FromQuery] string course = "")
        {
            var data = await _svc.LoadBatchAsync(course);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Batches loaded" : "No batches found",
                Data = data
            });
        }

        // GET api/creditsecured/branch?course=&regu=
        // Load ddlbranch — cascades from DDLBatch
        // BAL: CreditSecured_Branch
        // SQL: SELECT DISTINCT GRP, GRP+' - '+GSUB BRANCH FROM TBL_COURSE
        //      WHERE COURSE=@Course AND REGU=@Regu ORDER BY GRP
        [HttpGet("branch")]
        public async Task<IActionResult> LoadBranch([FromQuery] string course, [FromQuery] string regu)
        {
            var data = await _svc.LoadBranchAsync(course, regu);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Branches loaded" : "No branches found",
                Data = data
            });
        }

        // GET api/creditsecured/data?regu=&examMY=&branch=&noofcredits=
        // Credits_NextSem.aspx — "Credit Secured" — btnCredit_Click
        // BAL: Bal_Reports_Results → Get_CreditList
        // SP: SP_Credit_Secured (@REGU, @Noofcredits, @Branch, @ExamMY)
        // Validation: regu, examMY, branch must not be empty; noofcredits must be > 0
        [HttpGet("data")]
        public async Task<IActionResult> GetCreditSecured(
            [FromQuery] string regulation = "",
            [FromQuery] string course = "",
            [FromQuery] string regu = "",
            [FromQuery] string examMY = "",
            [FromQuery] string branch = "",
            [FromQuery] int noofcredits = 0)
        {
            if (string.IsNullOrWhiteSpace(regu))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Please Select Batch", Data = null });
            if (string.IsNullOrWhiteSpace(examMY))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Please Select Exammy", Data = null });
            if (string.IsNullOrWhiteSpace(branch))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Please Select Branch", Data = null });
            if (noofcredits <= 0)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Please enter No.OfCredits", Data = null });

            var data = await _svc.GetCreditSecuredAsync(regulation, course, regu, examMY, branch, noofcredits);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Credit secured data loaded" : "No data found",
                Data = data
            });
        }

        // GET api/creditsecured/total-data?regu=&examMY=&branch=&sem=
        // RegnoWiseTotalCredits.aspx — "Total Credit Secured" — btnCredittotal_Click
        // BAL: Bal_Reports_Results → Get_CreditList_total
        // SP: SP_Credit_Secured_Total (@REGU, @Sem, @Branch, @ExamMY)
        // Validation: regu, examMY, sem must not be empty
        [HttpGet("total-data")]
        public async Task<IActionResult> GetCreditSecuredTotal(
            [FromQuery] string regulation = "",
            [FromQuery] string course = "",
            [FromQuery] string regu = "",
            [FromQuery] string examMY = "",
            [FromQuery] string branch = "",
            [FromQuery] string sem = "")
        {
            if (string.IsNullOrWhiteSpace(regu))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Please Select Batch", Data = null });
            if (string.IsNullOrWhiteSpace(examMY))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Please Select Exammy", Data = null });

            var data = await _svc.GetCreditSecuredTotalAsync(regulation, course, regu, examMY, branch, sem);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Total credit secured data loaded" : "No data found",
                Data = data
            });
        }
    }
}
