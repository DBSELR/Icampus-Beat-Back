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
    /// Subject Wise Present List — Post Exam-Reports module (Studentwise_presentlist.aspx)
    ///
    /// Mirrors the reference project's Studentwise_presentlist.aspx flow:
    ///   1. Page_Load    → GET /semesters — populates ddlSemester (uses ExamMY+Course+Regulation from session)
    ///   2. Select Sem + Select Regsup (REG/Sup — static dropdown, no cascade)
    ///   3. Click Export → GET /report   — calls SP_Pcode_Presentlist → returns Excel data
    ///
    /// SP params confirmed from DLL IL (DataAccessLayer.dll DAL_StudentHistory::Pcode_PresentList,
    ///   US heap US[0xc0c1], 11-element String.Concat at FileOff=0x12c04):
    ///   Course(1,varchar), Regulation(2,varchar), ExamMY(3,varchar), Sem(4,varchar), Regsup(5,varchar)
    ///
    /// Regsup values: "REG" = Regular students, "Sup" = Supplementary students
    /// Original Excel output: worksheet "REGNO_VS_OMR", file "{Course}_{Regulation}_{ExamMY}.xlsx"
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class StudentWisePresentListController : BaseApiController
    {
        private readonly IStudentWisePresentListService _svc;

        public StudentWisePresentListController(IStudentWisePresentListService svc) => _svc = svc;

        /// <summary>
        /// Load semester list (populated on Page_Load)
        /// Inline SQL on TBL_SH: SELECT DISTINCT SH.SEM WHERE SH.EXAMMY=@ExamMY AND SH.course=@Course AND SH.Regulation=@Regulation
        /// All 3 params required (all come from session in original: ExamMY, Course, Regulation)
        ///
        /// GET /api/studentwisepresentlist/semesters?course=B.TECH&amp;regulation=R19&amp;examMY=Nov-2024
        /// </summary>
        [HttpGet("semesters")]
        public async Task<IActionResult> GetSemesters(
            [FromQuery] string course,
            [FromQuery] string regulation,
            [FromQuery] string examMY)
        {
            if (string.IsNullOrWhiteSpace(course))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Course is required" });
            if (string.IsNullOrWhiteSpace(regulation))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Regulation is required" });
            if (string.IsNullOrWhiteSpace(examMY))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "ExamMY is required" });

            var data = await _svc.GetSemestersAsync(course, regulation, examMY);
            var list = data?.ToList();

            return Ok(new ApiResponse<object>
            {
                Success = list != null && list.Count > 0,
                Message = list != null && list.Count > 0 ? "Semesters loaded" : "No semesters found",
                Data    = list
            });
        }

        /// <summary>
        /// Generate Subject Wise Present List report data
        /// Calls SP_Pcode_Presentlist — 5 params in order: Course, Regulation, ExamMY, Sem, Regsup
        /// All parameters are varchar (all quoted in DLL IL string concat)
        /// Both Sem and Regsup are required (original validates SelectedIndex != 0 for both)
        ///
        /// GET /api/studentwisepresentlist/report?course=B.TECH&amp;regulation=R19&amp;examMY=Nov-2024&amp;sem=3&amp;regsup=REG
        /// GET /api/studentwisepresentlist/report?course=B.TECH&amp;regulation=R19&amp;examMY=Nov-2024&amp;sem=5&amp;regsup=Sup
        /// </summary>
        [HttpGet("report")]
        public async Task<IActionResult> GetReport(
            [FromQuery] string course,
            [FromQuery] string regulation,
            [FromQuery] string examMY,
            [FromQuery] string sem,
            [FromQuery] string regsup)
        {
            if (string.IsNullOrWhiteSpace(course))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Course is required" });
            if (string.IsNullOrWhiteSpace(regulation))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Regulation is required" });
            if (string.IsNullOrWhiteSpace(examMY))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "ExamMY is required" });
            if (string.IsNullOrWhiteSpace(sem))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Sem is required" });
            if (string.IsNullOrWhiteSpace(regsup))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Regsup is required (REG or Sup)" });

            var request = new StudentWisePresentListRequest
            {
                Course     = course,
                Regulation = regulation,
                ExamMY     = examMY,
                Sem        = sem,
                Regsup     = regsup
            };

            var data = await _svc.GetReportDataAsync(request);
            var list = data?.ToList();

            return Ok(new ApiResponse<object>
            {
                Success = list != null && list.Count > 0,
                Message = list != null && list.Count > 0 ? "Present list loaded" : "No data found",
                Data    = list
            });
        }
    }
}
