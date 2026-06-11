using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubjectwiseFailedListController : BaseApiController
    {
        private readonly ISubjectwiseFailedListService _svc;

        public SubjectwiseFailedListController(ISubjectwiseFailedListService svc)
        {
            _svc = svc;
        }

        // GET api/subjectwisefailedlist/batch?course=CE
        // Page_Load → loads ddlBatch (REGU + BATCH display text)
        // Raw SQL on TBL_COURSE: REGU and '20'+REGU+'-'+CAST(REGU+MAXSEM/2 AS VARCHAR) BATCH
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

        // GET api/subjectwisefailedlist/sems?course=CE&regu=19
        // ddlBatch_SelectedIndexChanged → loads ddlSemester
        // Raw SQL on TBL_SH: DISTINCT SEM WHERE COURSE=@Course AND REGU=@REGU
        [HttpGet("sems")]
        public async Task<IActionResult> GetSemesters([FromQuery] string course, [FromQuery] string regu)
        {
            var data = await _svc.LoadSemestersAsync(course, regu);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Semesters loaded" : "No semesters found",
                Data = data
            });
        }

        // GET api/subjectwisefailedlist/branches?course=CE&regu=19&sem=3
        // ddlSemester_SelectedIndexChanged → loads ddlBranch
        // Raw SQL on TBL_SH: DISTINCT GRP WHERE COURSE=@Course AND REGU=@REGU AND SEM=@Sem
        [HttpGet("branches")]
        public async Task<IActionResult> GetBranches(
            [FromQuery] string course,
            [FromQuery] string regu,
            [FromQuery] string sem)
        {
            var data = await _svc.LoadBranchesAsync(course, regu, sem);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Branches loaded" : "No branches found",
                Data = data
            });
        }

        // GET api/subjectwisefailedlist/subjects?course=CE&regu=19&sem=3&branch=CSE
        // ddlBranch_SelectedIndexChanged → loads ddlPcode (subjects)
        // Confirmed SQL: select distinct pno,pcode,pcode+'-'+pname pname from tbl_sh
        //                where Course='' and Regu='' and grp='' and sem='' order by pno
        [HttpGet("subjects")]
        public async Task<IActionResult> GetSubjects(
            [FromQuery] string course,
            [FromQuery] string regu,
            [FromQuery] string sem,
            [FromQuery] string branch)
        {
            var data = await _svc.LoadSubjectsAsync(course, regu, sem, branch);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Subjects loaded" : "No subjects found",
                Data = data
            });
        }

        // GET api/subjectwisefailedlist/list?regulation=R20&course=B.TECH&examMY=May-2024&sem=5
        // btnView_Click → Load_Grid_SubwiseFailedList → SP_SubJ_FAILEDLIST_NEW
        // SP params: @regulation, @course, @EXAMMY, @sem (sem='' → all sems)
        // Returns: COURSE, GRP, REGU, REGNO, PCODE, PNAME, SEM, EXAMMY, RV, Regulation
        [HttpGet("list")]
        public async Task<IActionResult> GetFailedList(
            [FromQuery] string regulation,
            [FromQuery] string course,
            [FromQuery] string examMY,
            [FromQuery] string sem = "")
        {
            var data = await _svc.GetFailedListAsync(regulation, course, examMY, sem);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Subjectwise failed list loaded" : "No data found",
                Data = data
            });
        }
    }
}
