using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.DTOs;
using ICampus_Models.Requests;
using ICampus_Models.Common;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace ICampus_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeeController : BaseApiController
    {
        private readonly IFeeService _svc;
        public FeeController(IFeeService svc) => _svc = svc;

        [HttpGet("structures")]
        public async Task<IActionResult> GetStructures([FromQuery] string course, [FromQuery] string examMy, [FromQuery] string regulation)
        {
            var data = await _svc.LoadFeeStructureGridAsync(course, examMy, regulation);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Fee structures loaded" : "No fee structures found",
                Data = data
            });
        }

        [HttpGet("structures/filter")]
        public async Task<IActionResult> GetStructuresFilter([FromQuery] string course, [FromQuery] string examMy, [FromQuery] string regulation, [FromQuery] string regu, [FromQuery] string grp)
        {
            var data = await _svc.LoadFeeStructureFilterAsync(course, examMy, regulation, regu, grp);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Filtered fee structures loaded" : "No fee structures found for filter",
                Data = data
            });
        }

        [HttpGet("dropdowns")]
        public async Task<IActionResult> GetDropdowns([FromQuery] string course, [FromQuery] string examMy, [FromQuery] string regulation, [FromQuery] string type)
        {
            // type = SEMS | GRP | FINE | RSUPSEMS
            var data = await _svc.LoadBatchBranchFineAsync(course, examMy, regulation, type);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Dropdowns loaded" : "No dropdown data found",
                Data = data
            });
        }

        [HttpPost("save/regular")]
        public async Task<IActionResult> SaveRegular([FromBody] FeeSaveRegularRequest request)
        {
            // Optionally: request.UserId = UserId ?? request.UserId; // if FeeSaveRegularRequest has UserId
            var rows = await _svc.SaveFeeRegularAsync(request);
            return Ok(new ApiResponse<object>
            {
                Success = rows > 0,
                Message = rows > 0 ? "Regular fee saved successfully" : "Save failed"
            });
        }

        [HttpGet("supply/grid")]
        public async Task<IActionResult> GetSupplyGrid([FromQuery] string course, [FromQuery] string examMy, [FromQuery] string regulation, [FromQuery] string type)
        {
            var data = await _svc.LoadSupplyGridAsync(course, examMy, regulation, type);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Supply grid loaded" : "No supply data found",
                Data = data
            });
        }

        [HttpPost("supply/save")]
        public async Task<IActionResult> SaveSupply([FromBody] SupplyFeeSaveRequest request)
        {
            var res = await _svc.SaveSupplyFeeAsync(request);
            return Ok(new ApiResponse<object>
            {
                Success = res > 0,
                Message = res > 0 ? "Supply fee saved successfully" : "Save failed"
            });
        }

        [HttpGet("fine/list")]
        public async Task<IActionResult> GetFineList([FromQuery] string course, [FromQuery] string examMy, [FromQuery] string regulation)
        {
            var data = await _svc.LoadFineListAsync(course, examMy, regulation);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Fine list loaded" : "No fine records found",
                Data = data
            });
        }

        [HttpGet("fine/check")]
        public async Task<IActionResult> CheckFine([FromQuery] string course, [FromQuery] string examMy, [FromQuery] int sem, [FromQuery] decimal fineAmt, [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate, [FromQuery] int? fid, [FromQuery] string regulation)
        {
            var req = new FineSaveRequest
            {
                Course = course,
                ExamMy = examMy,
                Sem = sem,
                FineAmt = fineAmt,
                FromDate = fromDate,
                ToDate = toDate,
                Fid = fid,
                Regulation = regulation
            };

            var data = await _svc.CheckFineAsync(req);
            // consider a positive duplicate check when data not null
            var hasConflict = data != null;
            return Ok(new ApiResponse<object>
            {
                Success = hasConflict,
                Message = !hasConflict ? "Fine check OK" : "Overlapping fine(s) found",
                Data = data
            });
        }

        [HttpPost("fine/save")]
        public async Task<IActionResult> SaveFine([FromBody] FineSaveRequest request)
        {
            var res = await _svc.SaveFineAsync(request);
            return Ok(new ApiResponse<object>
            {
                Success = res > 0,
                Message = res > 0 ? "Fine saved successfully" : "Save failed"
            });
        }

        [HttpPost("fine/delete")]
        public async Task<IActionResult> DeleteFine([FromBody] FineSaveRequest request)
        {
            // ensure request carries the Fid and Regulation
            request.SType = "DEL_FEE";
            var res = await _svc.DeleteFineAsync(request);
            return Ok(new ApiResponse<object>
            {
                Success = res > 0,
                Message = res > 0 ? "Fine deleted successfully" : "Delete failed"
            });
        }

        // Condination endpoints
        //[HttpGet("condination/sems")]
        //public async Task<IActionResult> GetCondinationSems([FromQuery] string regulation, [FromQuery] string examMy, [FromQuery] string course)
        //{
        //    var data = await _svc.LoadCondinationSemsAsync(regulation, examMy, course);
        //    return Ok(new ApiResponse<object>
        //    {
        //        Success = data != null && data.Any(),
        //        Message = data != null && data.Any() ? "Condination sems loaded" : "No condination sems found",
        //        Data = data
        //    });
        //}

        //[HttpGet("condination/dates")]
        //public async Task<IActionResult> GetCondinationDates([FromQuery] string regulation, [FromQuery] string examMy, [FromQuery] string course)
        //{
        //    var data = await _svc.LoadCondinationDatesAsync(regulation, examMy, course);
        //    return Ok(new ApiResponse<object>
        //    {
        //        Success = data != null && data.Any(),
        //        Message = data != null && data.Any() ? "Condination dates loaded" : "No condination dates found",
        //        Data = data
        //    });
        //}

        //[HttpPost("condination/save")]
        //public async Task<IActionResult> SaveCondination([FromBody] CondinationDateSaveRequest request)
        //{
        //    var res = await _svc.SaveCondinationDatesAsync(request);
        //    return Ok(new ApiResponse<object>
        //    {
        //        Success = res > 0,
        //        Message = res > 0 ? "Condination dates saved successfully" : "Save failed"
        //    });
        //}

        //[HttpDelete("condination/{fid}")]
        //public async Task<IActionResult> DeleteCondination(int fid)
        //{
        //    var res = await _svc.DeleteCondinationDateAsync(fid);
        //    return Ok(new ApiResponse<object>
        //    {
        //        Success = res > 0,
        //        Message = res > 0 ? "Condination date deleted" : "Delete failed"
        //    });
        //}
    }
}
