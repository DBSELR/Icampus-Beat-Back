using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.DTOs;
using ICampus_Models.Requests;
using ICampus_Models.Common; // <-- for ApiResponse<T>
using Microsoft.AspNetCore.Mvc;

namespace ICampus_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CgpController : BaseApiController
    {
        private readonly ICgpService _svc;
        public CgpController(ICgpService svc) => _svc = svc;

        // GET /api/cgp/grid?type=...&string=...&regulation=...
        [HttpGet("grid")]
        public async Task<IActionResult> GetGrid([FromQuery] string type, [FromQuery] string @string, [FromQuery] string regulation)
        {
            var result = await _svc.LoadGridAsync(type, @string, regulation);

            if (result == null || !result.Any())
            {
                return Ok(new ApiResponse<object>
                {
                    Success = false,
                    Message = "No data found"
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Data fetched successfully",
                Data = result
            });
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string regu, [FromQuery] string? course, [FromQuery] string? grp)
        {
            var result = await _svc.SearchCgAsync(regu, course, grp);

            if (result == null || !result.Any())
                return Ok(new ApiResponse<object> { Success = false, Message = "No records found" });

            return Ok(new ApiResponse<object> { Success = true, Message = "Records fetched successfully", Data = result });
        }

        //// Autocomplete endpoints
        //[HttpGet("search/regu")]
        //public async Task<IActionResult> SearchRegu([FromQuery] string prefix)
        //{
        //    var result = await _svc.SearchReguAsync(prefix);
        //    return Ok(new ApiResponse<object>
        //    {
        //        Success = true,
        //        Message = "Regulations fetched successfully",
        //        Data = result
        //    });
        //}

        //[HttpGet("search/batch")]
        //public async Task<IActionResult> SearchBatch([FromQuery] string prefix)
        //{
        //    var result = await _svc.SearchBatchAsync(prefix);
        //    return Ok(new ApiResponse<object>
        //    {
        //        Success = true,
        //        Message = "Batches fetched successfully",
        //        Data = result
        //    });
        //}

        //[HttpGet("search/course")]
        //public async Task<IActionResult> SearchCourse([FromQuery] string prefix)
        //{
        //    var result = await _svc.SearchCourseAsync(prefix);
        //    return Ok(new ApiResponse<object>
        //    {
        //        Success = true,
        //        Message = "Courses fetched successfully",
        //        Data = result
        //    });
        //}

        //[HttpGet("search/coursename")]
        //public async Task<IActionResult> SearchCourseName([FromQuery] string prefix)
        //{
        //    var result = await _svc.SearchCourseNameAsync(prefix);
        //    return Ok(new ApiResponse<object>
        //    {
        //        Success = true,
        //        Message = "Course names fetched successfully",
        //        Data = result
        //    });
        //}

        //[HttpGet("search/branch")]
        //public async Task<IActionResult> SearchGrp([FromQuery] string prefix)
        //{
        //    var result = await _svc.SearchGrpAsync(prefix);
        //    return Ok(new ApiResponse<object>
        //    {
        //        Success = true,
        //        Message = "Branches fetched successfully",
        //        Data = result
        //    });
        //}

        //[HttpGet("search/branchname")]
        //public async Task<IActionResult> SearchGrpName([FromQuery] string prefix)
        //{
        //    var result = await _svc.SearchGrpNameAsync(prefix);
        //    return Ok(new ApiResponse<object>
        //    {
        //        Success = true,
        //        Message = "Branch names fetched successfully",
        //        Data = result
        //    });
        //}

        // POST save
        [HttpPost("save")]
        public async Task<IActionResult> Save([FromBody] SaveCourseRequest request)
        {
            var affected = await _svc.SaveCourseAsync(request);

            return Ok(new ApiResponse<object>
            {
                Success = affected > 0,
                Message = affected > 0 ? "Course saved successfully" : "No course was saved",
                Data = new { affected }
            });
        }

        // POST delete
        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody] DeleteCourseRequest request)
        {
            var affected = await _svc.DeleteCourseAsync(request);

            return Ok(new ApiResponse<object>
            {
                Success = affected > 0,
                Message = affected > 0 ? "Course deleted successfully" : "No course was deleted",
                Data = new { affected }
            });
        }

        // POST copy
        [HttpPost("copy")]
        public async Task<IActionResult> Copy([FromBody] CopyGroupRequest request)
        {
            var affected = await _svc.CopyGroupAsync(request);

            return Ok(new ApiResponse<object>
            {
                Success = affected > 0,
                Message = affected > 0 ? "Group copied successfully" : "No group was copied",
                Data = new { affected }
            });
        }

        // POST check-and-copy (returns { requiresConfirmation: bool, copied: bool })
        [HttpPost("check-and-copy")]
        public async Task<IActionResult> CheckAndCopy([FromBody] CopyGroupRequest request)
        {
            var result = await _svc.CheckAndCopyAsync(request);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Check and copy executed successfully",
                Data = result
            });
        }
    }
}
