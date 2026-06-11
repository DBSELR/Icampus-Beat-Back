using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UniversityDataController : BaseApiController
    {
        private readonly IUniversityDataService _svc;

        public UniversityDataController(IUniversityDataService svc)
        {
            _svc = svc;
        }

        // ── Shared Dropdowns ─────────────────────────────────────────────────────

        // GET api/universitydata/batch?course=
        // Batch dropdown (shared by SubjectData, CDData, PC Format pages)
        [HttpGet("batch")]
        public async Task<IActionResult> LoadBatch([FromQuery] string course = "")
        {
            var data = await _svc.LoadBatchAsync(course);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Batches loaded." : "No batches found.",
                Data = data
            });
        }

        // GET api/universitydata/branch?course=&regu=
        // Branch cascade for University_PC_Formate.aspx (ddlBatch AutoPostBack)
        [HttpGet("branch")]
        public async Task<IActionResult> LoadBranch([FromQuery] string course, [FromQuery] string regu)
        {
            var data = await _svc.LoadBranchAsync(course, regu);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Branches loaded." : "No branches found.",
                Data = data
            });
        }

        // GET api/universitydata/sem?course=&exammy=
        // Semester dropdown (UniversityData, Formate_Data, SubjectData, CDData pages)
        [HttpGet("sem")]
        public async Task<IActionResult> LoadSemester([FromQuery] string course, [FromQuery] string exammy)
        {
            var data = await _svc.LoadSemesterAsync(course, exammy);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Semesters loaded." : "No semesters found.",
                Data = data
            });
        }

        // ── UniversityData.aspx ───────────────────────────────────────────────────

        // GET api/universitydata/result-format?course=&regu=&sem=&exammy=
        // btnview — "Result Format" → Get_Univeristy_Formate (Bal_Reports_Results)
        [HttpGet("result-format")]
        public async Task<IActionResult> GetResultFormat(
            [FromQuery] string course,
            [FromQuery] string regu,
            [FromQuery] string sem,
            [FromQuery] string exammy)
        {
            if (string.IsNullOrWhiteSpace(course) || string.IsNullOrWhiteSpace(regu) ||
                string.IsNullOrWhiteSpace(sem)    || string.IsNullOrWhiteSpace(exammy))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "course, regu, sem and exammy are required.", Data = null });

            var data = await _svc.GetResultFormatAsync(course, regu, sem, exammy);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Result format data loaded." : "No data found.",
                Data = data
            });
        }

        // GET api/universitydata/registered-format?course=&regu=&sem=&exammy=
        // btnView2 — "Registred Format" → Get_Univeristy_Formate_R18
        [HttpGet("registered-format")]
        public async Task<IActionResult> GetRegisteredFormat(
            [FromQuery] string course,
            [FromQuery] string regu,
            [FromQuery] string sem,
            [FromQuery] string exammy)
        {
            if (string.IsNullOrWhiteSpace(course) || string.IsNullOrWhiteSpace(regu) ||
                string.IsNullOrWhiteSpace(sem)    || string.IsNullOrWhiteSpace(exammy))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "course, regu, sem and exammy are required.", Data = null });

            var data = await _svc.GetRegisteredFormatAsync(course, regu, sem, exammy);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Registered format data loaded." : "No data found.",
                Data = data
            });
        }

        // GET api/universitydata/registered-format2?course=&regu=&sem=&exammy=
        // btnview3 — "Registred Format2" → UniversityData_format2
        [HttpGet("registered-format2")]
        public async Task<IActionResult> GetRegisteredFormat2(
            [FromQuery] string course,
            [FromQuery] string regu,
            [FromQuery] string sem,
            [FromQuery] string exammy)
        {
            if (string.IsNullOrWhiteSpace(course) || string.IsNullOrWhiteSpace(regu) ||
                string.IsNullOrWhiteSpace(sem)    || string.IsNullOrWhiteSpace(exammy))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "course, regu, sem and exammy are required.", Data = null });

            var data = await _svc.GetRegisteredFormat2Async(course, regu, sem, exammy);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Registered format2 data loaded." : "No data found.",
                Data = data
            });
        }

        // ── University_Formate_Data.aspx ──────────────────────────────────────────

        // GET api/universitydata/format-data?course=&regu=&sem=&regsup=&exammy=
        // btnDownLoad → Univeristy_Formate_Data (BAL, App_Web_m2jhophz)
        // regsup: "REG" | "SUP" | "" (SELECT = all)
        [HttpGet("format-data")]
        public async Task<IActionResult> GetFormateData(
            [FromQuery] string course,
            [FromQuery] string regu,
            [FromQuery] string sem,
            [FromQuery] string regsup  = "",
            [FromQuery] string exammy  = "")
        {
            if (string.IsNullOrWhiteSpace(course) || string.IsNullOrWhiteSpace(regu) ||
                string.IsNullOrWhiteSpace(sem))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "course, regu and sem are required.", Data = null });

            var data = await _svc.GetFormateDataAsync(course, regu, sem, regsup, exammy);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Format data loaded." : "No data found.",
                Data = data
            });
        }

        // ── University_SubjectData.aspx ───────────────────────────────────────────

        // GET api/universitydata/subject-list?course=&regu=&sem=&regsup=&exammy=&isrv=false
        // btnSubjectList → "Subjects Data"
        [HttpGet("subject-list")]
        public async Task<IActionResult> GetSubjectList(
            [FromQuery] string course,
            [FromQuery] string regu,
            [FromQuery] string sem,
            [FromQuery] string regsup   = "",
            [FromQuery] string exammy   = "",
            [FromQuery] bool   isrv     = false)
        {
            if (string.IsNullOrWhiteSpace(course) || string.IsNullOrWhiteSpace(regu) ||
                string.IsNullOrWhiteSpace(sem))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "course, regu and sem are required.", Data = null });

            var data = await _svc.GetSubjectListAsync(course, regu, sem, regsup, exammy, isrv);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Subject list loaded." : "No data found.",
                Data = data
            });
        }

        // GET api/universitydata/subject-data?course=&regu=&sem=&regsup=&exammy=&isrv=false
        // btnSubjectData → "Students Data"
        [HttpGet("subject-data")]
        public async Task<IActionResult> GetSubjectData(
            [FromQuery] string course,
            [FromQuery] string regu,
            [FromQuery] string sem,
            [FromQuery] string regsup   = "",
            [FromQuery] string exammy   = "",
            [FromQuery] bool   isrv     = false)
        {
            if (string.IsNullOrWhiteSpace(course) || string.IsNullOrWhiteSpace(regu) ||
                string.IsNullOrWhiteSpace(sem))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "course, regu and sem are required.", Data = null });

            var data = await _svc.GetSubjectDataAsync(course, regu, sem, regsup, exammy, isrv);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "Subject data loaded." : "No data found.",
                Data = data
            });
        }

        // ── University_CD_Data.aspx ───────────────────────────────────────────────

        // GET api/universitydata/cd-subject-list?course=&regu=&sem=&regsup=&exammy=&isrv=false
        // btnSubjectList → "Subjects Data" (CD variant)
        [HttpGet("cd-subject-list")]
        public async Task<IActionResult> GetCdSubjectList(
            [FromQuery] string course,
            [FromQuery] string regu,
            [FromQuery] string sem,
            [FromQuery] string regsup   = "",
            [FromQuery] string exammy   = "",
            [FromQuery] bool   isrv     = false)
        {
            if (string.IsNullOrWhiteSpace(course) || string.IsNullOrWhiteSpace(regu) ||
                string.IsNullOrWhiteSpace(sem))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "course, regu and sem are required.", Data = null });

            var data = await _svc.GetCdSubjectListAsync(course, regu, sem, regsup, exammy, isrv);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "CD subject list loaded." : "No data found.",
                Data = data
            });
        }

        // GET api/universitydata/cd-student-data?course=&regu=&sem=&regsup=&exammy=&isrv=false
        // btnStudentData → "Students Data" (CD variant)
        [HttpGet("cd-student-data")]
        public async Task<IActionResult> GetCdStudentData(
            [FromQuery] string course,
            [FromQuery] string regu,
            [FromQuery] string sem,
            [FromQuery] string regsup   = "",
            [FromQuery] string exammy   = "",
            [FromQuery] bool   isrv     = false)
        {
            if (string.IsNullOrWhiteSpace(course) || string.IsNullOrWhiteSpace(regu) ||
                string.IsNullOrWhiteSpace(sem))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "course, regu and sem are required.", Data = null });

            var data = await _svc.GetCdStudentDataAsync(course, regu, sem, regsup, exammy, isrv);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "CD student data loaded." : "No data found.",
                Data = data
            });
        }

        // ── University_PC_Formate.aspx ────────────────────────────────────────────

        // GET api/universitydata/pc-format?course=&regu=&exammy=&sem=&type=
        // btnDownLoad → PROC_AWARD_DEGREE_UNIVERSITY(@regulation,@course,@exammy,@regu,@sem,@type)
        // type values: CourseComplete | All | Regnowise | JNTUK CE
        [HttpGet("pc-format")]
        public async Task<IActionResult> GetPcFormat(
            [FromQuery] string course,
            [FromQuery] string regu,
            [FromQuery] string exammy = "",
            [FromQuery] string sem    = "",
            [FromQuery] string type   = "CourseComplete")
        {
            if (string.IsNullOrWhiteSpace(course) || string.IsNullOrWhiteSpace(regu))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "course and regu are required.", Data = null });

            var data = await _svc.GetPcFormatAsync(course, regu, exammy, sem, type);
            return Ok(new ApiResponse<object>
            {
                Success = data != null && data.Any(),
                Message = data != null && data.Any() ? "PC format data loaded." : "No data found.",
                Data = data
            });
        }
    }
}
