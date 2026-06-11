using ICampus_Api.Controllers;
using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using ICampus_Models.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HallTicketController : BaseApiController
    {
        private readonly IHallTicketService _svc;
        public HallTicketController(IHallTicketService svc) => _svc = svc;

        // GET api/halltickets/batches?course=B.Tech&regulation=R18
        [HttpGet("batches")]
        public async Task<IActionResult> GetBatches([FromQuery] string course, [FromQuery] string regulation)
        {
            if (string.IsNullOrWhiteSpace(course))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Course parameter is required" });

            if (string.IsNullOrWhiteSpace(regulation))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Regulation parameter is required" });

            var data = await _svc.GetBatchesAsync(course, regulation);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Batches loaded" : "No batches found",
                Data = data
            });
        }

        // GET api/halltickets/branches?course=B.Tech&regulation=R18&batch=20
        [HttpGet("branches")]
        public async Task<IActionResult> GetBranches([FromQuery] string course, [FromQuery] string regulation, [FromQuery] string batch)
        {
            if (string.IsNullOrWhiteSpace(course))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Course parameter is required" });

            if (string.IsNullOrWhiteSpace(regulation))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Regulation parameter is required" });

            if (string.IsNullOrWhiteSpace(batch))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Batch parameter is required" });

            var data = await _svc.GetBranchesAsync(course, regulation, batch);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Branches loaded" : "No branches found",
                Data = data
            });
        }

        // GET api/halltickets/semesters?course=B.Tech&regulation=R18&examMY=MAY-2024
        [HttpGet("semesters")]
        public async Task<IActionResult> GetSemesters([FromQuery] string course, [FromQuery] string regulation, [FromQuery] string examMY)
        {
            if (string.IsNullOrWhiteSpace(course))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Course parameter is required" });

            if (string.IsNullOrWhiteSpace(regulation))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Regulation parameter is required" });

            if (string.IsNullOrWhiteSpace(examMY))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "ExamMY parameter is required" });

            var data = await _svc.GetSemestersAsync(course, regulation, examMY);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Semesters loaded" : "No semesters found",
                Data = data
            });
        }

        // POST api/halltickets/prepare
        [HttpPost("prepare")]
        public async Task<IActionResult> PrepareHallTickets([FromBody] HallTicketRequest request)
        {
            if (request == null)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Request body is required" });

            if (string.IsNullOrWhiteSpace(request.ExamMY))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "ExamMY is required" });

            if (string.IsNullOrWhiteSpace(request.Course))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Course is required" });

            if (string.IsNullOrWhiteSpace(request.Regulation))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Regulation is required" });

            // Build selection formula from filters if not provided
            // Note: The formula should match SQL WHERE clause syntax as used in the stored procedure
            // Format matches ASPX: " s.REGU = {batch} and s.grp = '{branch}' and s.sem = {sem} "
            if (string.IsNullOrWhiteSpace(request.SelectionFormula))
            {
                var formula = "";
                if (!string.IsNullOrWhiteSpace(request.Batch))
                    formula = $" s.REGU = {request.Batch}";
                else
                    formula = " s.REGU = s.regu";

                if (!string.IsNullOrWhiteSpace(request.Branch))
                    formula += $" and s.grp = '{request.Branch}'";
                else
                    formula += " and s.grp = s.grp";

                if (!string.IsNullOrWhiteSpace(request.Sem))
                    formula += $" and s.sem = {request.Sem}";
                else
                    formula += " and s.sem = s.sem";

                request.SelectionFormula = formula;
            }

            var (rowsAffected, recordsCount) = await _svc.PrepareHallTicketsAsync(request.ExamMY, request.Course, request.Regulation, request.SelectionFormula);
            return Ok(new ApiResponse<object>
            {
                Success = true, // SP executed successfully (even if rowsAffected is 0)
                Message = recordsCount > 0 
                    ? $"Hall tickets prepared successfully. {recordsCount} record(s) found in tbl_hallticket." 
                    : $"Stored procedure executed successfully, but no records found in tbl_hallticket. This may indicate:\n" +
                      $"1. No matching data in TBL_SH or TBL_PAP tables\n" +
                      $"2. Selection formula conditions not matching any records\n" +
                      $"3. Data filtered out by SP conditions (REGD='Y', PTYPE LIKE '%T%', etc.)\n" +
                      $"Please verify the data exists in source tables.",
                Data = new 
                { 
                    RowsAffected = rowsAffected,
                    RecordsCount = recordsCount,
                    SelectionFormula = request.SelectionFormula
                }
            });
        }

        // GET api/halltickets/data?examMY=MAY-2024&course=B.Tech&regulation=R18&batch=20&branch=CS&sem=1&regno=18671A05C0
        // 
        // REQUIRED parameters (must always be provided to identify exam context):
        // - examMY: Exam Month-Year (e.g., "MAY-2024")
        // - course: Course code (e.g., "B.Tech")
        // - regulation: Regulation (e.g., "R18")
        //
        // OPTIONAL filters (can be used in any combination):
        // - batch: Filter by batch/REGU (e.g., "20")
        // - sem: Filter by semester (e.g., "1")
        // - branch: Filter by branch/GRP (e.g., "CS")
        // - regno: Filter by registration number (e.g., "18671A05C0")
        //
        // Examples:
        // - Batch only: ?examMY=MAY-2024&course=B.Tech&regulation=R18&batch=20
        // - Batch + Sem: ?examMY=MAY-2024&course=B.Tech&regulation=R18&batch=20&sem=1
        // - Batch + Sem + Branch: ?examMY=MAY-2024&course=B.Tech&regulation=R18&batch=20&sem=1&branch=CS
        // - Sem only: ?examMY=MAY-2024&course=B.Tech&regulation=R18&sem=1
        // - Branch only: ?examMY=MAY-2024&course=B.Tech&regulation=R18&branch=CS
        // - Regno only: ?examMY=MAY-2024&course=B.Tech&regulation=R18&regno=18671A05C0
        // - All filters: ?examMY=MAY-2024&course=B.Tech&regulation=R18&batch=20&sem=1&branch=CS&regno=18671A05C0
        // - No filters (all records): ?examMY=MAY-2024&course=B.Tech&regulation=R18
        //
        // Note: Data must be prepared first using POST /api/halltickets/prepare endpoint
        [HttpGet("data")]
        public async Task<IActionResult> GetHallTicketData([FromQuery] HallTicketRequest request)
        {
            if (request == null)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Request parameters are required" });

            // Required parameters: ExamMY, Course, Regulation (needed to identify exam context)
            if (string.IsNullOrWhiteSpace(request.ExamMY))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "ExamMY parameter is required" });

            if (string.IsNullOrWhiteSpace(request.Course))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Course parameter is required" });

            if (string.IsNullOrWhiteSpace(request.Regulation))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Regulation parameter is required" });

            // Optional filters (batch, sem, branch, regno) are not validated here
            // They can be used in any combination or omitted entirely

            var data = await _svc.GetHallTicketDataAsync(request);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Hall ticket data loaded" : "No hall ticket data found. Please ensure data is prepared first using /prepare endpoint.",
                Data = data
            });
        }

        // GET api/halltickets/checkdata?examMY=Feb-2022&course=B.Tech&regulation=R20&batch=21&branch=2021-2025&sem=3
        //[HttpGet("checkdata")]
        //public async Task<IActionResult> CheckSourceData([FromQuery] HallTicketRequest request)
        //{
        //    if (request == null)
        //        return BadRequest(new ApiResponse<object> { Success = false, Message = "Request parameters are required" });

        //    if (string.IsNullOrWhiteSpace(request.ExamMY))
        //        return BadRequest(new ApiResponse<object> { Success = false, Message = "ExamMY parameter is required" });

        //    if (string.IsNullOrWhiteSpace(request.Course))
        //        return BadRequest(new ApiResponse<object> { Success = false, Message = "Course parameter is required" });

        //    if (string.IsNullOrWhiteSpace(request.Regulation))
        //        return BadRequest(new ApiResponse<object> { Success = false, Message = "Regulation parameter is required" });

        //    // Build selection formula from filters if not provided
        //    if (string.IsNullOrWhiteSpace(request.SelectionFormula))
        //    {
        //        var formula = "";
        //        if (!string.IsNullOrWhiteSpace(request.Batch))
        //            formula = $" s.REGU = {request.Batch}";
        //        else
        //            formula = " s.REGU = s.regu";

        //        if (!string.IsNullOrWhiteSpace(request.Branch))
        //            formula += $" and s.grp = '{request.Branch}'";
        //        else
        //            formula += " and s.grp = s.grp";

        //        if (!string.IsNullOrWhiteSpace(request.Sem))
        //            formula += $" and s.sem = {request.Sem}";
        //        else
        //            formula += " and s.sem = s.sem";

        //        request.SelectionFormula = formula;
        //    }

        //    var diagnostic = await _svc.CheckSourceDataAsync(request.ExamMY, request.Course, request.Regulation, request.SelectionFormula);
        //    return Ok(new ApiResponse<object>
        //    {
        //        Success = true,
        //        Message = "Source data diagnostic completed",
        //        Data = diagnostic
        //    });
        //}
    }
}

