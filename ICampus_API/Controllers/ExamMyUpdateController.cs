using ICampus_Api.Controllers;
using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using ICampus_Models.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_Api.Controllers
{
    /// <summary>
    /// Update Exam Month and Year — Post-Exams module
    ///
    /// Mirrors the reference project's ExamMyUpdate.aspx flow:
    ///   1. GET  /load?regulation=&course=&batch=&branch=&sem= → load student grid (sp_loadstdexammy_update)
    ///   2. POST /update                                        → bulk update ACT_EXAMMY (sp_Exammy_update)
    ///   3. DELETE /delete/{ashid}                             → delete a student record (PROC_DEL_ASHID)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ExamMyUpdateController : BaseApiController
    {
        private readonly IExamMyUpdateService _svc;

        public ExamMyUpdateController(IExamMyUpdateService svc) => _svc = svc;

        /// <summary>
        /// Load student grid filtered by Regulation, Course, Batch, Branch and Semester
        /// Corresponds to ddlBatch / ddlBranch / ddlsem SelectedIndexChanged events
        /// Calls sp_loadstdexammy_update (@Regulation, @Course, @Batch, @Branch, @Sem)
        ///
        /// Returns: ASHID, REGULATION, REGU, COURSE, GRP, SEM, STREAM, REGNO, EXAMMY, ACT_EXAMMY
        ///
        /// GET /api/exammyupdate/load?regulation=R18&course=B.Tech&batch=2020&branch=CSE&sem=3
        /// </summary>
        [HttpGet("load")]
        public async Task<IActionResult> Load(
            [FromQuery] string regulation,
            [FromQuery] string course,
            [FromQuery] string batch,
            [FromQuery] string branch,
            [FromQuery] string sem)
        {
            if (string.IsNullOrWhiteSpace(regulation))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Regulation is required" });
            if (string.IsNullOrWhiteSpace(course))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Course is required" });
            if (string.IsNullOrWhiteSpace(batch))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Batch is required" });
            if (string.IsNullOrWhiteSpace(branch))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Branch is required" });
            if (string.IsNullOrWhiteSpace(sem))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Sem is required" });

            var data = await _svc.LoadAsync(new ExamMyLoadRequest
            {
                Regulation = regulation,
                Course     = course,
                Batch      = batch,
                Branch     = branch,
                Sem        = sem
            });

            var list = data?.ToList();
            return Ok(new ApiResponse<object>
            {
                Success = list != null && list.Any(),
                Message = list != null && list.Any() ? "Data loaded" : "No records found",
                Data    = list
            });
        }

        /// <summary>
        /// Bulk update ACT_EXAMMY for all edited grid rows
        /// Corresponds to btnUpdateExammy "UPDATE EXAMMY" click
        /// Calls sp_Exammy_update once per row — 8 params: ACT_EXAMMY, Regulation, Batch, Course, Branch, Sem, REGNO, EXAMMY
        ///
        /// POST /api/exammyupdate/update
        /// Body:
        /// {
        ///   "Rows": [
        ///     {
        ///       "ACT_EXAMMY": "NOV2024",
        ///       "Regulation": "R18", "Batch": "2020", "Course": "B.Tech", "Branch": "CSE",
        ///       "Sem": "3", "REGNO": "20B91A0501", "EXAMMY": "NOV2023"
        ///     }
        ///   ]
        /// }
        /// </summary>
        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] ExamMyUpdateRequest request)
        {
            if (request?.Rows == null || request.Rows.Count == 0)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "At least one row is required" });

            var total = await _svc.UpdateExamMyAsync(request);
            return Ok(new ApiResponse<object>
            {
                Success = total >= 0,
                Message = $"ExamMY updated for {total} record(s)",
                Data    = new { RowsAffected = total }
            });
        }

        /// <summary>
        /// Delete a single student record by ASHID
        /// Corresponds to Delete ImageButton per grid row (GrdOmrNum_RowCommand CommandName="Delete")
        /// Calls PROC_DEL_ASHID (@ASHID)
        ///
        /// DELETE /api/exammyupdate/delete/123456
        /// </summary>
        [HttpDelete("delete/{ashid}")]
        public async Task<IActionResult> Delete(long ashid)
        {
            if (ashid <= 0)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Valid ASHID is required" });

            var result = await _svc.DeleteAsync(ashid);
            return Ok(new ApiResponse<object>
            {
                Success = result > 0,
                Message = result > 0 ? "Record deleted" : "Record not found or already deleted",
                Data    = new { RowsAffected = result }
            });
        }
    }
}
