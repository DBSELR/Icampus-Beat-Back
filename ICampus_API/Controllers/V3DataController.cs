using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class V3DataController : BaseApiController
    {
        private readonly IV3DataService _svc;

        public V3DataController(IV3DataService svc)
        {
            _svc = svc;
        }

        // GET api/v3data/sems?course=&regu=&exammy=
        // Page_Load — populate ddlSem
        [HttpGet("sems")]
        public async Task<IActionResult> LoadSemesters(
            [FromQuery] string course,
            [FromQuery] string regu,
            [FromQuery] string exammy)
        {
            var data = await _svc.LoadSemestersAsync(course, regu, exammy);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Semesters loaded." : "No semesters found.",
                Data    = data
            });
        }

        // GET api/v3data/data?course=&regu=&exammy=&sem=&diffmarks=&isreadmit=false&readmitreg=
        // btnView_Click + btnDownLoad_Click
        // isreadmit=true → calls PROC_DATAFOR_V3_READMIT with readmitreg
        // isreadmit=false (default) → calls PROC_DATAFOR_V3
        [HttpGet("data")]
        public async Task<IActionResult> GetV3Data(
            [FromQuery] string course,
            [FromQuery] string regu,
            [FromQuery] string exammy,
            [FromQuery] string sem,
            [FromQuery] string diffmarks   = "0",
            [FromQuery] bool   isreadmit   = false,
            [FromQuery] string readmitreg  = "")
        {
            if (string.IsNullOrWhiteSpace(course))
                return BadRequest(new ApiResponse<object>
                {
                    Success = false, Message = "Course is required.", Data = null
                });

            if (string.IsNullOrWhiteSpace(sem))
                return BadRequest(new ApiResponse<object>
                {
                    Success = false, Message = "Please select Semester.", Data = null
                });

            var data = await _svc.GetV3DataAsync(course, regu, exammy, sem, diffmarks, isreadmit, readmitreg);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "V3 data loaded." : "No V3 data found.",
                Data    = data
            });
        }
    }
}
