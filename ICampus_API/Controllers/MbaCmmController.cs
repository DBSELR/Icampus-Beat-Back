using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MbaCmmController : BaseApiController
    {
        private readonly IMbaCmmService _svc;

        public MbaCmmController(IMbaCmmService svc)
        {
            _svc = svc;
        }

        // GET api/mbacmm/data?course=MBA&examMY=NOV2024&regu=R11
        // Page_Load — loads automatically (no View button in ASPX)
        // SP: SP_CMM (@Course, @ExamMY, @Regu)
        // Returns: MBA Consolidated Marks Memo data
        // Note: MBACMM.aspx has no filter controls — identical pattern to MTECHCMM
        //       BAL: MBACMM → Load_Btech_CMM (shared with BTECHCMM/MTECHCMM)
        //       BusinessAccessLayer.dll ASCII offset 36075
        [HttpGet("data")]
        public async Task<IActionResult> GetData(
            [FromQuery] string course,
            [FromQuery] string examMY,
            [FromQuery] string regu,
            [FromQuery] string grp = "")
        {
            if (string.IsNullOrWhiteSpace(course) || string.IsNullOrWhiteSpace(examMY))
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Course and ExamMY are required.",
                    Data    = null
                });

            var data = await _svc.GetDataAsync(course, examMY, regu, grp ?? string.Empty);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "MBA CMM data loaded." : "No data found.",
                Data    = data
            });
        }
    }
}
