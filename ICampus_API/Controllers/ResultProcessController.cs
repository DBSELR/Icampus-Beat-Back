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
    public class ResultProcessController : BaseApiController
    {
        private readonly IResultProcessService _svc;

        public ResultProcessController(IResultProcessService svc)
        {
            _svc = svc;
        }

        // GET api/resultprocess/exammy?course=B.Tech&regulation=R20
        // Load ExamMY dropdown for Result Process page
        // SP: SPM_EXAMS_ExamMY_Load(@Regulation, @COURSE)
        // Confirmed: EXEC SPM_EXAMS_ExamMY_Load 'R20','B.Tech' → 23 rows
        [HttpGet("exammy")]
        public async Task<IActionResult> GetExamMy([FromQuery] string course, [FromQuery] string regulation)
        {
            var data = await _svc.LoadExamMyAsync(course, regulation);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "ExamMY list loaded" : "No ExamMY found",
                Data = data
            });
        }

        // POST api/resultprocess/run
        // Run result process for a student (regno-wise)
        // SP: SP_RESULT_PROCESS_REGNOWISE(@Regulation,@COURSE,@EXAMMY1,@SEM,@GRP,@REGNO,@flag='NORMAL')
        // Body: { "regNo":"20671A0101","examMy":"May-2024","regulation":"R20","course":"B.Tech","sem":"8","grp":"CE" }
        [HttpPost("run")]
        public async Task<IActionResult> RunResultProcess([FromBody] ResultProcessRequest request)
        {
            var result = await _svc.RunResultProcessAsync(
                request.RegNo, request.ExamMy, request.Regulation, request.Course, request.Sem, request.Grp);
            return Ok(new ApiResponse<object>
            {
                Success = result >= 0,
                Message = result >= 0 ? "Result process completed" : "Result process failed",
                Data = result
            });
        }

        // POST api/resultprocess/run-readmit
        // Run result process for a readmit student
        // SP: SP_RESULT_PROCESS_readmit(@Regulation,@COURSE,@EXAMMY1,@SEM,@flag='NORMAL',@GRP,@READMIT_REGULATION)
        // Body: { "regNo":"...","examMy":"...","regulation":"R20","course":"B.Tech","sem":"8","grp":"CE","readmitRegulation":"R19" }
        [HttpPost("run-readmit")]
        public async Task<IActionResult> RunReadmitResultProcess([FromBody] ReadmitResultProcessRequest request)
        {
            var result = await _svc.RunReadmitResultProcessAsync(
                request.RegNo, request.ExamMy, request.Regulation, request.Course,
                request.Sem, request.Grp, request.ReadmitRegulation);

            return Ok(new ApiResponse<object>
            {
                Success = result >= 0,
                Message = result >= 0 ? "Readmit result process completed" : "Readmit result process failed",
                Data = result
            });
        }
    }

    public class ResultProcessRequest
    {
        public string RegNo      { get; set; }
        public string ExamMy     { get; set; }
        public string Regulation { get; set; }
        public string Course     { get; set; }
        public string Sem        { get; set; }
        public string Grp        { get; set; }
    }

    public class ReadmitResultProcessRequest
    {
        public string RegNo              { get; set; }
        public string ExamMy             { get; set; }
        public string Regulation         { get; set; }
        public string Course             { get; set; }
        public string Sem                { get; set; }
        public string Grp                { get; set; }
        public string ReadmitRegulation  { get; set; }
    }
}
