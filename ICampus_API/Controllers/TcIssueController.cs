using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TcIssueController : BaseApiController
    {
        private readonly ITcIssueService _svc;

        public TcIssueController(ITcIssueService svc)
        {
            _svc = svc;
        }

        // GET api/tcissue/sample-regnos
        // Diagnostic: returns TOP 10 REGNOs from TBL_STDDATA for testing
        [HttpGet("sample-regnos")]
        public async Task<IActionResult> GetSampleRegnos()
        {
            var data = await _svc.GetSampleRegnosAsync();
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Sample REGNOs found." : "No students in TBL_STDDATA.",
                Data    = data
            });
        }

        // GET api/tcissue/student-info?regNo=20B91A0501
        // txtRegNo_TextChanged (AutoPostBack) → loadingTC_Issue
        // SQL: SELECT SNAME, COURSE, GRP, PHOTO FROM TBL_STDDATA WHERE REGNO=@RegNo
        [HttpGet("student-info")]
        public async Task<IActionResult> GetStudentInfo([FromQuery] string regNo)
        {
            if (string.IsNullOrWhiteSpace(regNo))
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "regNo is required.",
                    Data    = null
                });

            var data = await _svc.GetStudentInfoAsync(regNo);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Student info loaded." : "Student not found.",
                Data    = data
            });
        }

        // POST api/tcissue/issue
        // ddlTCissue_Click → TC_issue (normal) / TC_issue_Noresult (chknoresult=true)
        // SP: Proc_TC_Issue — issues the Transfer Certificate and renders Crystal Report
        // Body: { regNo, regulation, section, sName, fName, mName, dob, gender, caste,
        //         email, mobile, aadhaarNo, mole1, mole2, religion, dateofAdmitted,
        //         scholar, courseComplete, higherEdu, dateofLeave }
        // scholar: "0"=YES, "1"=NO | courseComplete: "0"=COMPLETED, "1"=NOT | higherEdu: "0"=YES, "1"=NO
        // Crystal: Transfercertificate_JBIET.rpt
        [HttpPost("issue")]
        public async Task<IActionResult> IssueTc([FromBody] TcIssueRequest req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.RegNo))
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "regNo is required.",
                    Data    = null
                });

            var rows = await _svc.IssueTcAsync(
                req.RegNo, req.Regulation, req.Section,
                req.SName, req.FName, req.MName,
                req.Dob, req.Gender, req.Caste,
                req.Email, req.Mobile, req.AadhaarNo,
                req.Mole1, req.Mole2, req.Religion,
                req.DateofAdmitted, req.Scholar,
                req.CourseComplete, req.HigherEdu, req.DateofLeave);

            return Ok(new ApiResponse<object>
            {
                Success = rows >= 0,
                Message = rows >= 0 ? "Transfer Certificate issued successfully." : "Failed to issue TC.",
                Data    = new { RowsAffected = rows }
            });
        }
    }

    public class TcIssueRequest
    {
        public string RegNo          { get; set; }
        public string Regulation     { get; set; }
        public string Section        { get; set; }
        public string SName          { get; set; }
        public string FName          { get; set; }
        public string MName          { get; set; }
        public string Dob            { get; set; }
        public string Gender         { get; set; }
        public string Caste          { get; set; }
        public string Email          { get; set; }
        public string Mobile         { get; set; }
        public string AadhaarNo      { get; set; }
        public string Mole1          { get; set; }
        public string Mole2          { get; set; }
        public string Religion       { get; set; }
        public string DateofAdmitted { get; set; }
        public string Scholar        { get; set; }
        public string CourseComplete { get; set; }
        public string HigherEdu      { get; set; }
        public string DateofLeave    { get; set; }
    }
}
