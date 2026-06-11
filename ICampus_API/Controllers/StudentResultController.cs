using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentResultController : BaseApiController
    {
        private readonly IStudentResultService _svc;

        public StudentResultController(IStudentResultService svc)
        {
            _svc = svc;
        }

        // GET api/studentresult/details?regNo=22A91A0501
        // Load student header info (txtCourse, txtStudentName, txtGRP)
        // SP: SPM_STUDENT_DETAILS @RegNo
        [HttpGet("details")]
        public async Task<IActionResult> GetStudentDetails([FromQuery] string regNo)
        {
            var data = await _svc.LoadStudentDetailsAsync(regNo);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Student details loaded" : "Student not found",
                Data = data
            });
        }

        // GET api/studentresult/sgpacgpa?regNo=22A91A0501
        // Load SGPA/CGPA grid (gvSGPA_CGPA)
        // SP: PROC_SGPA_AVERAGE @RegNo
        // Returns: SEM, SGPA, CGPA, TCR, SCR
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

        // GET api/studentresult/passed?regNo=20671A0457&exammy=May-2024&con=true
        // Passed papers list (RBPassedList radio)
        // SP: SPM_REQ_DATA_FOR_PASSEDPAPLIST @REGNO, @EXAMMY, @CON
        // con='true' = papers up to exammy; 'false' = all semesters
        [HttpGet("passed")]
        public async Task<IActionResult> GetPassedPapers(
            [FromQuery] string regNo,
            [FromQuery] string exammy,
            [FromQuery] string con = "true")
        {
            var data = await _svc.GetPassedPapersAsync(regNo, exammy, con);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Passed papers loaded" : "No passed papers found",
                Data = data
            });
        }

        // GET api/studentresult/failed?regNo=20671A0457&exammy=May-2024&con=true
        // Failed papers list (RBFailedList radio)
        // SP: SPM_REQ_DATA_FOR_FAILEDPAPLIST @REGNO, @EXAMMY, @CON
        // con='true' = papers up to exammy; 'false' = all semesters
        [HttpGet("failed")]
        public async Task<IActionResult> GetFailedPapers(
            [FromQuery] string regNo,
            [FromQuery] string exammy,
            [FromQuery] string con = "true")
        {
            var data = await _svc.GetFailedPapersAsync(regNo, exammy, con);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Failed papers loaded" : "No failed papers found",
                Data = data
            });
        }

        // GET api/studentresult/all?regNo=22A91A0501&examMY=
        // All papers (Rbtn_allFP radio)
        // Raw SQL on TBL_MRKMEMO
        // examMY: non-empty → adds AND EXAMMY=@ExamMY filter (CBCurrentMonthYear checked)
        //         empty/omitted → no ExamMY filter
        [HttpGet("all")]
        public async Task<IActionResult> GetAllPapers(
            [FromQuery] string regNo,
            [FromQuery] string examMY = "")
        {
            var data = await _svc.GetAllPapersAsync(regNo, examMY);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "All papers loaded" : "No papers found",
                Data = data
            });
        }
    }
}
