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
    public class ReAdmissionController : BaseApiController
    {
        private readonly IReAdmissionService _svc;

        public ReAdmissionController(IReAdmissionService svc)
        {
            _svc = svc;
        }

        // GET api/readmission/details?regNo=22481A0501
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

        // GET api/readmission/history?regNo=22481A0501
        // Load full subject-wise history grid (all semesters, all papers)
        // SP: SPM_STUDENTHISTORY (@RegNo)
        // Returns: ASHID, PCODE, PNAME (LinkButton → edit), MMARKS, RVMARKS, MRK_FIN,
        //          SMARKS, PMARKS, MARKS, SEM (hidden), GR, Papres (hidden), Exammy
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

        // GET api/readmission/papertypes
        // Load Entry Type dropdown (cmbEntryType) — all distinct PTYPE values from TBL_PAP
        // Raw SQL: SELECT DISTINCT Cast(PTYPE as varchar(50)) PTYPE FROM TBL_PAP (no params)
        [HttpGet("papertypes")]
        public async Task<IActionResult> GetPaperTypes()
        {
            var data = await _svc.LoadPaperTypesAsync();
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Paper types loaded" : "No paper types found",
                Data = data
            });
        }

        // GET api/readmission/marks?ashId=100123
        // Load marks for edit modal (getReAdmissionStudentMarks) — triggered when PNAME clicked
        // SP: SPM_ReAdmissionsStudentMarks (@ASHID)
        // Returns: PCODE, PNAME, TMARKS, MMARKS, RVMARKS, SMARKS, PMARKS, PTYPE,
        //          CR, SGPA_CR, TMAX, TPASS, SMAX, SPASS, PMAX, PPASS, MAXMRK, PASS
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

        // GET api/readmission/paperdetails?pCode=CS301&regulation=R20&sem=3
        // Load paper structure when paper code is typed (getPaperDetails / txtPCode_TextChanged)
        // SP: SPM_PaperDetails_ReAdmission (@Regulation, @PCODE, @SEM)
        // Returns: PNAME, PTYPE, TMAX, TPASS, SMAX, SPASS, PMAX, PPASS, MAXMRK, PASS, CREDITS, SUB_CR
        [HttpGet("paperdetails")]
        public async Task<IActionResult> GetPaperDetails(
            [FromQuery] string pCode,
            [FromQuery] string? regulation = null,
            [FromQuery] string? sem = null)
        {
            var data = await _svc.GetPaperDetailsAsync(pCode, regulation ?? string.Empty, sem ?? string.Empty);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Paper details loaded" : "Paper not found",
                Data = data
            });
        }

        // PUT api/readmission/marks
        // Save/update marks for a readmit TBL_SH record (SetReAdmissionsStudentMarks / btnSave_Click)
        // Raw SQL UPDATE on TBL_SH with elec=CASE WHEN (pcode!=tempcode AND elec IS NULL) THEN 'R'
        // Body: { "ashId": "100123", "pCode": "CS301", "pName": "DATA STRUCTURES",
        //         "regulation": "R20", "sem": "3", "tMarks": "55", "mMarks": "2",
        //         "rvMarks": "0", "sMarks": "25", "pMarks": "0", "entryType": "Theory",
        //         "credits": "3", "sgpaCr": "3", "tMax": "75", "tPass": "26",
        //         "sMax": "25", "sPass": "9", "pMax": "0", "pPass": "0",
        //         "maxMrk": "100", "pass": "35" }
        [HttpPut("marks")]
        public async Task<IActionResult> UpdateMarks([FromBody] UpdateReAdmissionMarks request)
        {
            var result = await _svc.UpdateMarksAsync(request);
            return Ok(new ApiResponse<object>
            {
                Success = result >= 0,
                Message = result >= 0 ? "Marks updated successfully" : "Failed to update marks",
                Data = result
            });
        }

        // DELETE api/readmission/{ashId}
        // Delete a TBL_SH record by ASHID (btnDelete → confirm popup → btnConfirmOk_Click)
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
    }
}
