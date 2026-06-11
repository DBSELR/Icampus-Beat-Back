using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TopperController : BaseApiController
    {
        private readonly ITopperService _svc;

        public TopperController(ITopperService svc)
        {
            _svc = svc;
        }

        // GET api/topper/batch?course=CE
        // Load Batch dropdown (DDLBatch) on page load
        // Raw SQL on TBL_COURSE — returns REGU + BATCH display text
        [HttpGet("batch")]
        public async Task<IActionResult> GetBatch([FromQuery] string course)
        {
            var data = await _svc.LoadBatchAsync(course);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Batch list loaded" : "No batches found",
                Data = data
            });
        }

        // GET api/topper/maxsem?regu=R20
        // Load max semester for selected batch (DDLBatch_SelectedIndexChanged)
        // Auto-fills txtSemTo with the maximum available semester
        // Raw SQL: SELECT DISTINCT max(SEM) FROM tbl_sh WHERE REGU = @REGU
        [HttpGet("maxsem")]
        public async Task<IActionResult> GetMaxSem([FromQuery] string regu)
        {
            var data = await _svc.LoadMaxSemAsync(regu);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Max semester loaded" : "No semester data found",
                Data = data
            });
        }

        // GET api/topper/list?regulation=R20&course=B.TECH&regu=20&sem=8&exammy=May-2024&rank=10&branch=N&caste=N&gender=N&rv=Y
        // Get Toppers List — overall (btnToppersList_Click when chksemwise = false)
        // SP: PROC_TOPPERSLIST_NEW (@Regulation,@Course,@BRANCH,@CASTE,@GENDER,@REGU,@SEM,@EXAMMY,@RV,@RANK INT)
        // Returns: REGNO, SNAME, COURSE, BRANCH, CASTE, SEM, GENDER,
        //          TOTAL GRADE POINTS, TOTAL CREDITS, SGPA, CGPA, RANK, exammy, regsup
        [HttpGet("list")]
        public async Task<IActionResult> GetToppersList(
            [FromQuery] string regulation,
            [FromQuery] string course,
            [FromQuery] string regu,
            [FromQuery] string sem,
            [FromQuery] string exammy,
            [FromQuery] string rank,
            [FromQuery] string branch = "N",
            [FromQuery] string caste = "N",
            [FromQuery] string gender = "N",
            [FromQuery] string rv = "Y")
        {
            var data = await _svc.GetToppersListAsync(
                regulation, course, regu, sem, exammy, rank, branch, caste, gender, rv);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Toppers list loaded" : "No data found",
                Data = data
            });
        }

        // GET api/topper/semwise?regulation=R20&course=B.TECH&regu=20&sem=8&exammy=May-2024&rank=10&branch=N&caste=N&gender=N&rv=Y
        // Get Toppers List — semester-wise (btnToppersList_Click when chksemwise = true)
        // SP: PROC_TOPPERSLIST_SemWise (same params as PROC_TOPPERSLIST_NEW)
        [HttpGet("semwise")]
        public async Task<IActionResult> GetToppersListSemWise(
            [FromQuery] string regulation,
            [FromQuery] string course,
            [FromQuery] string regu,
            [FromQuery] string sem,
            [FromQuery] string exammy,
            [FromQuery] string rank,
            [FromQuery] string branch = "N",
            [FromQuery] string caste = "N",
            [FromQuery] string gender = "N",
            [FromQuery] string rv = "Y")
        {
            var data = await _svc.GetToppersListSemWiseAsync(
                regulation, course, regu, sem, exammy, rank, branch, caste, gender, rv);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Toppers list (sem-wise) loaded" : "No data found",
                Data = data
            });
        }
    }
}
