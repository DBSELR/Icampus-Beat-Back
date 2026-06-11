using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegnoWiseSgpaCgpaController : BaseApiController
    {
        private readonly IRegnoWiseSgpaCgpaService _svc;

        public RegnoWiseSgpaCgpaController(IRegnoWiseSgpaCgpaService svc)
        {
            _svc = svc;
        }

        // GET api/regnowisesgpacgpa/batch?course=
        // ddlBatch population (Page_Load / ddlBatch_SelectedIndexChanged)
        // BAL: SgpaCgpaList_Batch (Bal_Reports_Results)
        [HttpGet("batch")]
        public async Task<IActionResult> LoadBatch([FromQuery] string course)
        {
            var data = await _svc.LoadBatchAsync(course);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Batches loaded" : "No batches found",
                Data = data
            });
        }

        // GET api/regnowisesgpacgpa/sems?course=&regu=
        // ddlSemester population (ddlBatch_SelectedIndexChanged)
        // BAL: SgpaCgpaList_Semester (Bal_Reports_Results)
        [HttpGet("sems")]
        public async Task<IActionResult> LoadSemesters([FromQuery] string course, [FromQuery] string regu)
        {
            var data = await _svc.LoadSemestersAsync(course, regu);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Semesters loaded" : "No semesters found",
                Data = data
            });
        }

        // GET api/regnowisesgpacgpa/list?regulation=&course=&regu=&exammy=&sem=
        // RegularAndSup_Click — Regular AND Supply exams considered
        // SP: PROC_GET_CGPA_SGPA_EXCEL (@Regulation, @regu, @course, @exammy, @sem INT)
        [HttpGet("list")]
        public async Task<IActionResult> GetList(
            [FromQuery] string regulation,
            [FromQuery] string course,
            [FromQuery] string regu,
            [FromQuery] string exammy,
            [FromQuery] string sem)
        {
            var data = await _svc.GetListAsync(regulation, course, regu, exammy, sem);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "SGPA and CGPA data loaded" : "No records found",
                Data = data
            });
        }

        // GET api/regnowisesgpacgpa/list-regular?regulation=&course=&regu=&exammy=&sem=
        // RegularOnly_Click — Regular exams only (Supply NOT considered)
        // SP: PROC_SGPA_AVERAGE (@REGULATION, @REGU, @COURSE, @EXAMMY, @SEM INT)
        [HttpGet("list-regular")]
        public async Task<IActionResult> GetListRegularOnly(
            [FromQuery] string regulation,
            [FromQuery] string course,
            [FromQuery] string regu,
            [FromQuery] string exammy,
            [FromQuery] string sem)
        {
            var data = await _svc.GetListRegularOnlyAsync(regulation, course, regu, exammy, sem);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "SGPA and CGPA data (regular only) loaded" : "No records found",
                Data = data
            });
        }
    }
}
