using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScIssueController : BaseApiController
    {
        private readonly IScIssueService _svc;

        public ScIssueController(IScIssueService svc)
        {
            _svc = svc;
        }

        // GET api/scissue/student-info?regNo=20B91A0501
        // txtRegNo_TextChanged (AutoPostBack) → loadingSC_Issue
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

        // POST api/scissue/issue
        // btnSCIssue_Click → SC_issue
        // SP: Proc_SC_Issue — issues the Study Certificate and renders Crystal Report
        // Body: { regNo, regulation, section, sName, fName, mName, conduct,
        //         dob, gender, caste, email, mobile, aadhaarNo, religion }
        // conduct: "0"=Good, "1"=Satisfactory
        // Crystal: Studycertificate_JBIET.rpt
        [HttpPost("issue")]
        public async Task<IActionResult> IssueSc([FromBody] ScIssueRequest req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.RegNo))
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "regNo is required.",
                    Data    = null
                });

            var rows = await _svc.IssueScAsync(
                req.RegNo, req.Regulation, req.Section,
                req.SName, req.FName, req.MName,
                req.Conduct, req.Dob, req.Gender, req.Caste,
                req.Email, req.Mobile, req.AadhaarNo, req.Religion);

            return Ok(new ApiResponse<object>
            {
                Success = rows >= 0,
                Message = rows >= 0 ? "Study Certificate issued successfully." : "Failed to issue SC.",
                Data    = new { RowsAffected = rows }
            });
        }
    }

    public class ScIssueRequest
    {
        public string RegNo      { get; set; }
        public string Regulation { get; set; }
        public string Section    { get; set; }
        public string SName      { get; set; }
        public string FName      { get; set; }
        public string MName      { get; set; }
        public string Conduct    { get; set; }
        public string Dob        { get; set; }
        public string Gender     { get; set; }
        public string Caste      { get; set; }
        public string Email      { get; set; }
        public string Mobile     { get; set; }
        public string AadhaarNo  { get; set; }
        public string Religion   { get; set; }
    }
}
