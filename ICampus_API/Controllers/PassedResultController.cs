using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PassedResultController : BaseApiController
    {
        private readonly IPassedResultService _svc;

        public PassedResultController(IPassedResultService svc)
        {
            _svc = svc;
        }

        // GET api/passedresult/sems?course=BTECH&examMY=NOV2024&regu=R20
        // Page_Load — populate ddlsem
        // Inline SQL: SELECT DISTINCT CAST(SEM AS VARCHAR(250)) SEM
        //   FROM tbl_sh WHERE COURSE=@Course AND Exammy=@ExamMY AND Regulation=@Regu ORDER BY SEM
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

        // GET api/passedresult/data?course=BTECH&examMY=NOV2024&regu=R20&sem=3
        // ddlsem_SelectedIndexChanged (auto-load on sem select) / btnDownLoad_Click
        // SP: SP_PASSEDLIST_NEW (@Course, @ExamMY, @Regu, @Sem)
        // Note: no View button — data loads automatically when semester is selected
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
                Message = data != null && data.Any() ? "Passed result data loaded." : "No data found.",
                Data    = data
            });
        }
    }
}
