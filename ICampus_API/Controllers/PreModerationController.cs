using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PreModerationController : BaseApiController
    {
        private readonly IPreModerationService _svc;

        public PreModerationController(IPreModerationService svc)
        {
            _svc = svc;
        }

        // GET api/premoderation/sems?course=BTECH&examMY=NOV2024&regu=R20
        // Page_Load + ddlsem_SelectedIndexChanged — populate ddlsem
        // SQL: SELECT DISTINCT SEM FROM TBL_SH WHERE REGULATION=@Regulation AND EXAMMY=@ExamMY AND COURSE=@Course
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

        // GET api/premoderation/data?course=BTECH&examMY=NOV2024&regu=R20&sem=3
        // btnExport_Click — get all pre-moderation marks data for the semester
        // SP: PROC_MODERATION_REG_SEM_GRP_PAP (@Course, @ExamMY, @Regu, @Sem, @Grp='', @PapCode='')
        // Returns all branches and papers (Grp='' and PapCode='' = no filter)
        [HttpGet("data")]
        public async Task<IActionResult> GetData(
            [FromQuery] string course,
            [FromQuery] string examMY,
            [FromQuery] string regu,
            [FromQuery] string sem)
        {
            if (string.IsNullOrWhiteSpace(sem))
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Please select Semester.",
                    Data    = null
                });

            var data = await _svc.GetDataAsync(course, examMY, regu, sem);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Pre-moderation data loaded." : "No data found.",
                Data    = data
            });
        }
    }
}
