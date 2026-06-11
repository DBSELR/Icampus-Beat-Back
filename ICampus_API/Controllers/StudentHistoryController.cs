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
    public class StudentHistoryController : BaseApiController
    {
        private readonly IStudentHistoryService _svc;

        public StudentHistoryController(IStudentHistoryService svc)
        {
            _svc = svc;
        }

        // GET api/studenthistory/details?regNo=22481A0501
        // Load student personal info (Name, Programme, Branch)
        // SP: SPM_STUDENT_DETAILS (@RegNo)
        [HttpGet("details")]
        public async Task<IActionResult> GetDetails([FromQuery] string regNo)
        {
            var data = await _svc.LoadStudentDetailsAsync(regNo);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Student details loaded" : "Student not found",
                Data = data
            });
        }

        // GET api/studenthistory/history?regNo=22481A0501
        // Load full subject-wise history grid (all semesters, all papers)
        // SP: SPM_STUDENTHISTORY (@RegNo)
        // Returns: ASHID, REGNO, PCODE, PNAME, CR, SMARKS(CIE), MRK_FIN(SEE), MARKS(Total),
        //          SEM, GR, GRPTS, EXAMMY
        [HttpGet("history")]
        public async Task<IActionResult> GetHistory([FromQuery] string regNo)
        {
            var data = await _svc.LoadHistoryAsync(regNo);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "History loaded" : "No history found",
                Data = data
            });
        }

        // GET api/studenthistory/sgpacgpa?regNo=22481A0501
        // Load SGPA/CGPA per-semester grid
        // SP: PROC_SGPA_AVERAGE (@RegNo)
        // Returns: SEM, SGPA, CGPA, TCR, SecuredCR, BackLogs
        [HttpGet("sgpacgpa")]
        public async Task<IActionResult> GetSgpaCgpa([FromQuery] string regNo)
        {
            var data = await _svc.LoadSgpaCgpaAsync(regNo);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "SGPA/CGPA loaded" : "No SGPA/CGPA data found",
                Data = data
            });
        }

        // GET api/studenthistory/marks?ashId=12345
        // Load a single row from TBL_SH for edit modal (by ASHID)
        // Raw SQL: SELECT PCODE, PNAME, TMARKS, MMARKS, RVMARKS, V3, MRK_FIN, SMARKS, PMARKS
        //            FROM TBL_SH WHERE ASHID = @ASHID
        [HttpGet("marks")]
        public async Task<IActionResult> GetMarksByAshId([FromQuery] string ashId)
        {
            var data = await _svc.GetMarksByAshIdAsync(ashId);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Marks loaded" : "Record not found",
                Data = data
            });
        }

        // PUT api/studenthistory/marks
        // Update marks for a TBL_SH record (SetStudentMarks / modal Save)
        // Raw SQL UPDATE on TBL_SH WHERE ASHID = @ASHID
        // Body: { "ashId": "12345", "pName": "...", "sMarks": "25", "tMarks": "60",
        //         "mMarks": "0", "rvMarks": "0", "v3": "0", "pMarks": "0" }
        [HttpPut("marks")]
        public async Task<IActionResult> UpdateMarks([FromBody] UpdateMarksRequest request)
        {
            var result = await _svc.UpdateMarksAsync(
                request.AshId, request.PName, request.SMarks, request.TMarks,
                request.MMarks, request.RVMarks, request.V3, request.PMarks);

            return Ok(new ApiResponse<object>
            {
                Success = result >= 0,
                Message = result >= 0 ? "Marks updated successfully" : "Failed to update marks",
                Data = result
            });
        }

        // DELETE api/studenthistory/{ashId}
        // Delete a TBL_SH record by ASHID
        // SP: PROC_DEL_ASHID (@ASHID)
        [HttpDelete("{ashId}")]
        public async Task<IActionResult> Delete(string ashId)
        {
            var result = await _svc.DeleteByAshIdAsync(ashId);
            return Ok(new ApiResponse<object>
            {
                Success = result >= 0,
                Message = result >= 0 ? "Record deleted successfully" : "Failed to delete record",
                Data = result
            });
        }

        // GET api/studenthistory/maxexammy?regNo=22481A0501
        // Get the latest ExamMY for a student (used to trigger result process after marks edit)
        // SP: SPM_Student_MaxExamMY (@RegNo)
        [HttpGet("maxexammy")]
        public async Task<IActionResult> GetMaxExamMy([FromQuery] string regNo)
        {
            var result = await _svc.GetMaxExamMyAsync(regNo);
            return Ok(new ApiResponse<object>
            {
                Success = !string.IsNullOrEmpty(result),
                Message = !string.IsNullOrEmpty(result) ? "Max ExamMY loaded" : "No ExamMY found",
                Data = result
            });
        }
    }

    public class UpdateMarksRequest
    {
        public string AshId   { get; set; }
        public string PName   { get; set; }
        public string SMarks  { get; set; }
        public string TMarks  { get; set; }
        public string MMarks  { get; set; }
        public string RVMarks { get; set; }
        public string V3      { get; set; }
        public string PMarks  { get; set; }
    }
}
