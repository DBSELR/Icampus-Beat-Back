using ICampus_Api.Controllers;
using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ModerationController : BaseApiController
    {
        private readonly IModerationService _svc;

        public ModerationController(IModerationService svc)
        {
            _svc = svc;
        }

        // GET api/moderation/batch?course=BTECH
        // Load batch/regu dropdown for moderation
        // SP: PROC_RESULT_LOAD_REGU_COURSE_GRP (@Course)
        [HttpGet("batch")]
        public async Task<IActionResult> GetBatch([FromQuery] string course)
        {
            var data = await _svc.LoadBatchAsync(course);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Batch list loaded" : "No batch found",
                Data = data
            });
        }

        // GET api/moderation/branch?course=BTECH&examMy=NOV2023&sem=3
        // Load branch (GRP) dropdown for moderation
        // Raw SQL: select distinct GRP from tbl_sh where Course=@Course and sem=@Sem order by GRP asc
        [HttpGet("branch")]
        public async Task<IActionResult> GetBranch(
            [FromQuery] string course, [FromQuery] string examMy, [FromQuery] string sem)
        {
            var data = await _svc.LoadBranchAsync(course, examMy, sem);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Branch list loaded" : "No branches found",
                Data = data
            });
        }

        // GET api/moderation/papers?course=BTECH&sem=3
        // Load papers dropdown for moderation
        // Raw SQL: SELECT * FROM TBL_PAP WHERE COURSE=@Course AND SEM=@Sem
        [HttpGet("papers")]
        public async Task<IActionResult> GetPapers([FromQuery] string course, [FromQuery] string sem)
        {
            var data = await _svc.LoadPapersAsync(course, sem);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Papers loaded" : "No papers found",
                Data = data
            });
        }

        // GET api/moderation/data?course=BTECH&examMy=NOV2023&regu=20&sem=3&grp=CSE&papCode=CS301
        // Load moderation marks grid
        // SP: PROC_MODERATION_REG_SEM_GRP_PAP (@Course, @ExamMY, @Regu, @Sem, @Grp, @PapCode)
        [HttpGet("data")]
        public async Task<IActionResult> GetModerationData(
            [FromQuery] string course,
            [FromQuery] string examMy,
            [FromQuery] string regu,
            [FromQuery] string sem,
            [FromQuery] string grp,
            [FromQuery] string? papCode = null)
        {
            var data = await _svc.LoadModerationDataAsync(course, examMy, regu, sem, grp, papCode ?? string.Empty);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Moderation data loaded" : "No moderation data found",
                Data = data
            });
        }

        // POST api/moderation/save
        // Save moderation marks
        // SP: PROC_MODERATION_AND_MCNT (@Course, @ExamMY, @Regu, @Sem, @Grp, @PapCode, @ModMarks)
        // Body: { "course": "BTECH", "examMy": "NOV2023", "regu": "20", "sem": "3",
        //         "grp": "CSE", "papCode": "CS301", "modMarks": "5" }
        [HttpPost("save")]
        public async Task<IActionResult> SaveModeration([FromBody] SaveModerationRequest request)
        {
            var result = await _svc.SaveModerationAsync(
                request.Course, request.ExamMy, request.Regu,
                request.Sem, request.Grp, request.PapCode, request.ModMarks);

            return Ok(new ApiResponse<object>
            {
                Success = result >= 0,
                Message = result >= 0 ? "Moderation marks saved" : "Failed to save moderation marks",
                Data = result
            });
        }
    }

    public class SaveModerationRequest
    {
        public string Course   { get; set; }
        public string ExamMy   { get; set; }
        public string Regu     { get; set; }
        public string Sem      { get; set; }
        public string Grp      { get; set; }
        public string PapCode  { get; set; }
        public string ModMarks { get; set; }
    }
}
