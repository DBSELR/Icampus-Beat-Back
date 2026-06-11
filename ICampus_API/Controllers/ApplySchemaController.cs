using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using ICampus_Models.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_Api.Controllers
{
    /// <summary>
    /// Evaluation — Apply Schema (Evaluation/Apply_Schema.aspx)
    ///
    /// Page flow:
    ///   Page_Load (cascade dropdowns):
    ///     1. GET /api/applyschema/semesters?course=&amp;regulation=&amp;examMY=
    ///        Sp_Eval_Load_Sem — populate Semester dropdown
    ///
    ///   Semester selected:
    ///     2. GET /api/applyschema/papers?course=&amp;regulation=&amp;sem=&amp;examMY=
    ///        Sp_Eval_Load_Papers — show papers grid (pcode, Pname, SStatus)
    ///        SStatus shows if schema already applied to that paper
    ///
    ///   View already-assigned papers for a schema:
    ///     3. GET /api/applyschema/assigned?schemaName=&amp;course=&amp;regulation=&amp;sem=
    ///        Sp_Eval_Get_PapData — list of papers linked to this schema
    ///
    ///   Apply button (one call per selected paper):
    ///     4. POST /api/applyschema/save
    ///        Sp_Eval_Save_UserPapers — assign schema to selected papers
    ///        Warning if already applied: "Schema Already apply to subjects..."
    ///
    ///   Delete button:
    ///     5. DELETE /api/applyschema?schemaName=&amp;papCode=
    ///        Sp_Eval_Delete_Pap_Data — remove one paper from schema
    ///
    /// All SPs confirmed from App_Web_xplim0cm.dll + DataAccessLayer.dll UTF-16LE analysis.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ApplySchemaController : BaseApiController
    {
        private readonly IApplySchemaService _svc;

        public ApplySchemaController(IApplySchemaService svc) => _svc = svc;

        /// <summary>
        /// Get semester list for the apply-schema form.
        /// SP: Sp_Eval_Load_Sem @Course, @Regulation, @ExamMY
        ///
        /// GET /api/applyschema/semesters?course=B.TECH&amp;regulation=R20&amp;examMY=NOV2024
        /// </summary>
        [HttpGet("semesters")]
        public async Task<IActionResult> GetSemesters(
            [FromQuery] string course,
            [FromQuery] string regulation,
            [FromQuery] string examMY)
        {
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
        /// Get papers list with schema-applied status (SStatus).
        /// SP: Sp_Eval_Load_Papers @Course, @Regulation, @Sem, @ExamMY
        ///
        /// GET /api/applyschema/papers?course=B.TECH&amp;regulation=R20&amp;sem=1&amp;examMY=NOV2024
        /// </summary>
        [HttpGet("papers")]
        public async Task<IActionResult> GetPapers(
            [FromQuery] string course,
            [FromQuery] string regulation,
            [FromQuery] string sem,
            [FromQuery] string examMY)
        {
            var data = await _svc.GetPapersAsync(course, regulation, sem, examMY);
            var list = data?.ToList();

            return Ok(new ApiResponse<object>
            {
                Success = list != null && list.Count > 0,
                Message = list != null && list.Count > 0 ? "Papers loaded" : "Papers are not found",
                Data    = list
            });
        }

        /// <summary>
        /// Get papers already assigned to a schema.
        /// SP: Sp_Eval_Get_PapData @SchemaName, @Course, @Regulation, @Sem
        ///
        /// GET /api/applyschema/assigned?schemaName=Schema1&amp;course=B.TECH&amp;regulation=R20&amp;sem=1
        /// </summary>
        [HttpGet("assigned")]
        public async Task<IActionResult> GetAssignedPapers(
            [FromQuery] string schemaName,
            [FromQuery] string course,
            [FromQuery] string regulation,
            [FromQuery] string sem)
        {
            if (string.IsNullOrWhiteSpace(schemaName))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "SchemaName is required" });

            var data = await _svc.GetAssignedPapersAsync(schemaName, course, regulation, sem);
            var list = data?.ToList();

            return Ok(new ApiResponse<object>
            {
                Success = list != null && list.Count > 0,
                Message = list != null && list.Count > 0 ? "Assigned papers loaded" : "No papers assigned to this schema",
                Data    = list
            });
        }

        /// <summary>
        /// Apply a schema to selected papers (loop per paper internally).
        /// SP: Sp_Eval_Save_UserPapers @SchemaName, @PapCode, @Course, @Regulation, @Sem, @ExamMY
        ///
        /// POST /api/applyschema/save
        /// Body: { "schemaName":"Schema1", "course":"B.TECH", "regulation":"R20",
        ///         "sem":"1", "examMY":"NOV2024", "papCodes":["CS101","CS102"] }
        /// </summary>
        [HttpPost("save")]
        public async Task<IActionResult> SaveApplySchema([FromBody] SaveApplySchemaRequest req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.SchemaName))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "SchemaName is required" });

            if (req.PapCodes == null || req.PapCodes.Count == 0)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Select all fields..!" });

            var saved = await _svc.SaveApplySchemaAsync(req);

            return Ok(new ApiResponse<object>
            {
                Success = saved > 0,
                Message = saved > 0
                    ? $"Inserted Data Successfully.. ({saved} paper(s) applied)"
                    : "Data not saved ..!",
                Data    = new { SavedCount = saved }
            });
        }

        /// <summary>
        /// Remove one paper assignment from tbl_Eval_UserPapers.
        /// SP: Sp_Eval_Remove_UserPaper @Regulation, @Course, @ExamMY, @TempCode, @SEM
        /// tempCode is extracted from composite papCode (parts[1] after splitting on '@').
        ///
        /// DELETE /api/applyschema?regulation=R20&amp;course=B.TECH&amp;examMY=May-2024&amp;papCode=J211D@J211D@PNAME@3&amp;sem=3
        /// </summary>
        [HttpDelete]
        public async Task<IActionResult> DeleteAppliedPaper(
            [FromQuery] string regulation,
            [FromQuery] string course,
            [FromQuery] string examMY,
            [FromQuery] string papCode,
            [FromQuery] string sem)
        {
            if (string.IsNullOrWhiteSpace(papCode))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Select Paper before delete..!" });

            // Extract TempCode from composite "PCode@TempCode@PName@Sem"
            var parts    = (papCode ?? string.Empty).Split('@');
            var tempCode = parts.Length > 1 ? parts[1] : parts[0];

            var rows = await _svc.DeleteAppliedPaperAsync(regulation, course, examMY, tempCode, sem);

            return Ok(new ApiResponse<object>
            {
                Success = rows > 0,
                Message = rows > 0
                    ? "Pap Deleted successfully"
                    : $"No rows affected for paper '{tempCode}'"
            });
        }
    }
}
