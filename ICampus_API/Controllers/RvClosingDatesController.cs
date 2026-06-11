using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RvClosingDatesController : BaseApiController
    {
        private readonly IRvClosingDatesService _svc;

        public RvClosingDatesController(IRvClosingDatesService svc)
        {
            _svc = svc;
        }

        // GET api/rvclosingdates?regulation=R20&course=B.TECH&examMY=May-2024
        // Page_Load equivalent — loads gvRvCloseDate
        // SP: SP_RV_CLOSINGDATES_LIST (@REGULATION, @COURSE, @EXAMMY)
        // Returns: REGULATION, COURSE, EXAMMY, SEM, RV_CLOSEDATE, RV_CDATE_SUP
        [HttpGet]
        public async Task<IActionResult> GetClosingDates(
            [FromQuery] string regulation,
            [FromQuery] string course,
            [FromQuery] string examMY)
        {
            var data = await _svc.GetClosingDatesAsync(regulation, course, examMY);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "RV Closing Dates loaded" : "No records found",
                Data = data
            });
        }

        // PUT api/rvclosingdates
        // btnSave_Click per row — updates RV_CLOSEDATE and RV_CDATE_SUP for one row
        // SP: SP_RV_CLOSINGDATES_Update (6 params)
        // Body: { regulation, course, examMY, sem, rvCloseDate, rvCDateSup }
        // Row key: regulation + course + examMY + sem
        // Date format: yyyy-MM-dd (from ASPX datepicker)
        [HttpPut]
        public async Task<IActionResult> UpdateClosingDate([FromBody] RvClosingDateUpdateRequest request)
        {
            if (request == null)
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Request body is required",
                    Data = null
                });

            var rows = await _svc.UpdateClosingDateAsync(
                request.Regulation,
                request.Course,
                request.ExamMY,
                request.Sem,
                request.RvCloseDate,
                request.RvCDateSup);

            return Ok(new ApiResponse<object>
            {
                Success = rows > 0,
                Message = rows > 0
                    ? "Revaluation date(s) updated successfully"
                    : "Please check revaluation date(s)",
                Data = rows
            });
        }
    }

    /// <summary>
    /// Request body for PUT api/rvclosingdates
    /// Identifies the row to update and provides new date values.
    /// </summary>
    public class RvClosingDateUpdateRequest
    {
        /// <summary>Row key — matches REGULATION column (gvRvCloseDate)</summary>
        public string Regulation { get; set; }

        /// <summary>Row key — matches COURSE column</summary>
        public string Course { get; set; }

        /// <summary>Row key — matches EXAMMY column</summary>
        public string ExamMY { get; set; }

        /// <summary>Row key — matches SEM column (lblSem)</summary>
        public string Sem { get; set; }

        /// <summary>Reg. Rv Closed Dt. — from txtReg_DATE (BOL: RvRegCloseDt), format yyyy-MM-dd</summary>
        public string RvCloseDate { get; set; }

        /// <summary>Suppl. Rv Closed Dt. — from txtSup_DATE (BOL: RvSupCloseDt), format yyyy-MM-dd</summary>
        public string RvCDateSup { get; set; }
    }
}
