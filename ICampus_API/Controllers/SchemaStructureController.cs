using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using ICampus_Models.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_Api.Controllers
{
    /// <summary>
    /// Evaluation — Schema Structure (Evaluation/Schema_Structure.aspx)
    ///
    /// Page flow:
    ///   Page_Load:
    ///     1. GET /api/schemastructure/schemas?course=&amp;regulation=&amp;sem=
    ///        Inline SQL on TBL_EVAL_SCHEMAMASTER — populates Schema Name dropdown
    ///
    ///   Schema Name selected (or on "New"):
    ///     2. GET /api/schemastructure/check?schemaName=
    ///        Sp_Eval_Check_Schema — verify name is unique before save
    ///     3. GET /api/schemastructure/load-edit?schemaName=
    ///        SP_EVAL_LOAD_STRUCTURE_Edit — load existing data into form
    ///
    ///   View mode (read-only grid):
    ///     4. GET /api/schemastructure/load?schemaName=
    ///        SP_EVAL_LOAD_STRUCTURE — display-only load
    ///
    ///   Save button:
    ///     5. POST /api/schemastructure/save
    ///        SP_EVAL_SCHEMAMASTER_SAVE (header) → SP_EVAL_SCHEMASTRUCTURE_SAVE (loop per question)
    ///
    ///   Delete button:
    ///     6. DELETE /api/schemastructure?schemaName=
    ///        Sp_Eval_Delete_Pap_Data — removes schema and all question rows
    ///
    /// All SPs confirmed from DataAccessLayer.dll UTF-16LE string analysis.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SchemaStructureController : BaseApiController
    {
        private readonly ISchemaStructureService _svc;

        public SchemaStructureController(ISchemaStructureService svc) => _svc = svc;

        /// <summary>
        /// Get schema names for a given course/regulation/sem (populates Schema dropdown).
        /// Inline SQL on TBL_EVAL_SCHEMAMASTER.
        ///
        /// GET /api/schemastructure/schemas?course=B.TECH&amp;regulation=R20&amp;sem=1
        /// </summary>
        [HttpGet("schemas")]
        public async Task<IActionResult> GetSchemaList(
            [FromQuery] string course,
            [FromQuery] string regulation,
            [FromQuery] string sem = "")
        {
            var data = await _svc.GetSchemaListAsync(course, regulation, sem);
            var list = data?.ToList();

            return Ok(new ApiResponse<object>
            {
                Success = list != null && list.Count > 0,
                Message = list != null && list.Count > 0 ? "Schemas loaded" : "No schemas found",
                Data    = list
            });
        }

        /// <summary>
        /// Check if a schema name already exists (before saving a new one).
        /// SP: Sp_Eval_Check_Schema @SchemaName
        /// Returns count — 0 = available, >0 = already exists.
        ///
        /// GET /api/schemastructure/check?schemaName=Schema1
        /// </summary>
        [HttpGet("check")]
        public async Task<IActionResult> CheckSchema([FromQuery] string schemaName)
        {
            if (string.IsNullOrWhiteSpace(schemaName))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "SchemaName is required" });

            var data = await _svc.CheckSchemaAsync(schemaName);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Check complete",
                Data    = data
            });
        }

        /// <summary>
        /// Load schema master + question rows for edit form.
        /// SP: SP_EVAL_LOAD_STRUCTURE_Edit @SchemaName
        ///
        /// GET /api/schemastructure/load-edit?schemaName=Schema1
        /// </summary>
        [HttpGet("load-edit")]
        public async Task<IActionResult> LoadStructureForEdit([FromQuery] string schemaName)
        {
            if (string.IsNullOrWhiteSpace(schemaName))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "SchemaName is required" });

            var data = await _svc.LoadStructureForEditAsync(schemaName);
            var list = data?.ToList();

            return Ok(new ApiResponse<object>
            {
                Success = list != null && list.Count > 0,
                Message = list != null && list.Count > 0 ? "Schema loaded for edit" : "Schema not found",
                Data    = list
            });
        }

        /// <summary>
        /// Load schema structure for display/view mode.
        /// SP: SP_EVAL_LOAD_STRUCTURE @SchemaName
        ///
        /// GET /api/schemastructure/load?schemaName=Schema1
        /// </summary>
        [HttpGet("load")]
        public async Task<IActionResult> LoadStructure([FromQuery] string schemaName)
        {
            if (string.IsNullOrWhiteSpace(schemaName))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "SchemaName is required" });

            var data = await _svc.LoadStructureAsync(schemaName);
            var list = data?.ToList();

            return Ok(new ApiResponse<object>
            {
                Success = list != null && list.Count > 0,
                Message = list != null && list.Count > 0 ? "Schema structure loaded" : "No structure found",
                Data    = list
            });
        }

        /// <summary>
        /// Save schema master + question rows.
        /// Flow: SP_EVAL_SCHEMAMASTER_SAVE (header) → SP_EVAL_SCHEMASTRUCTURE_SAVE (loop per question)
        ///
        /// POST /api/schemastructure/save
        /// Body: { "schemaName":"Schema1", "course":"B.TECH", "regulation":"R20", "sem":"1",
        ///         "maxMarks":"100", "maxNoofQuestions":"10", "maxSections":"2",
        ///         "questions":[{"qno":"1","maxMrk":"10","qStatus":"C","maxNoofQuestions":"5","maxSections":"2"}] }
        /// </summary>
        [HttpPost("save")]
        public async Task<IActionResult> SaveSchemaStructure([FromBody] SaveSchemaStructureRequest req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.SchemaName))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "SchemaName is required" });

            var saved = await _svc.SaveSchemaStructureAsync(req);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = saved > 0
                    ? $"Schema '{req.SchemaName}' saved with {saved} question row(s)"
                    : $"Schema master '{req.SchemaName}' saved (no question rows)",
                Data    = new { SavedQuestions = saved }
            });
        }

        /// <summary>
        /// Delete a schema and all its question rows.
        /// SP: Sp_Eval_Delete_Pap_Data @SchemaName
        ///
        /// DELETE /api/schemastructure?schemaName=Schema1
        /// </summary>
        [HttpDelete]
        public async Task<IActionResult> DeleteSchema([FromQuery] string schemaName)
        {
            if (string.IsNullOrWhiteSpace(schemaName))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "SchemaName is required" });

            var rows = await _svc.DeleteSchemaAsync(schemaName);

            return Ok(new ApiResponse<object>
            {
                Success = rows > 0,
                Message = rows > 0
                    ? $"Schema '{schemaName}' deleted"
                    : $"No rows affected for schema '{schemaName}'"
            });
        }
    }
}
