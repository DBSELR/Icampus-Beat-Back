using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using ICampus_Models.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_Api.Controllers
{
    /// <summary>
    /// Evaluation — Schema Marks Entry (Evaluation/Schema_MarksEntry.aspx)
    ///
    /// Page flow:
    ///   OMR Number entered → btnDisplay_Click:
    ///     1. GET /api/schemamarksentry/structure?schemaName=SCHEMA1
    ///        [SP_EVAL_LOAD_STRUCTURE_MarksEntry] — loads schema question grid
    ///
    ///     2. GET /api/schemamarksentry/qp-questions?schemaName=SCHEMA1
    ///        Inline SQL — loads QP question rules for JS best-of calculations
    ///
    ///     3. GET /api/schemamarksentry/optional-questions?course=&amp;regulation=&amp;schemaName=
    ///        Inline SQL — loads optional questions for best-of rules
    ///
    ///   Load existing marks (if previously saved):
    ///     4. GET /api/schemamarksentry/secured-marks?schemaName=&amp;papCode=&amp;omrNo=
    ///        Sp_Eval_Get_SecuredMarks — loads student's saved marks
    ///
    ///   Save button:
    ///     5. POST /api/schemamarksentry/save
    ///        Sp_Eval_Insert_Student_SecuredMarks per question
    ///
    ///   Finalize:
    ///     6. POST /api/schemamarksentry/finalize
    ///        UPDATE FStatus='Y' in tbl_Eval_Student_Secured_Marks
    ///
    /// All SPs confirmed from App_Web_xplim0cm.dll + DataAccessLayer.dll analysis.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SchemaMarksEntryController : BaseApiController
    {
        private readonly ISchemaMarksEntryService _svc;

        public SchemaMarksEntryController(ISchemaMarksEntryService svc)
        {
            _svc = svc;
        }

        /// <summary>
        /// Load schema structure for marks entry.
        /// SP: [SP_EVAL_LOAD_STRUCTURE_MarksEntry] @SchemaName
        ///
        /// GET /api/schemamarksentry/structure?schemaName=SCHEMA1
        /// </summary>
        [HttpGet("structure")]
        public async Task<IActionResult> LoadStructure([FromQuery] string schemaName)
        {
            if (string.IsNullOrWhiteSpace(schemaName))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "SchemaName is required" });

            var data = await _svc.LoadStructureForMarksEntryAsync(schemaName);
            var list = data?.ToList();

            return Ok(new ApiResponse<object>
            {
                Success = list != null && list.Count > 0,
                Message = list != null && list.Count > 0 ? "Structure loaded for marks entry" : "No structure found",
                Data    = list
            });
        }

        /// <summary>
        /// Get QP questions text (for JS best-of / OR group calculations).
        /// Inline SQL: SELECT QUES FROM TBL_EVAL_QPSTRUCTURE WHERE SCHEMANAME=@SchemaName
        ///
        /// GET /api/schemamarksentry/qp-questions?schemaName=SCHEMA1
        /// </summary>
        [HttpGet("qp-questions")]
        public async Task<IActionResult> GetQPQuestions([FromQuery] string schemaName)
        {
            if (string.IsNullOrWhiteSpace(schemaName))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "SchemaName is required" });

            var data = await _svc.GetQPQuestionsAsync(schemaName);
            var list = data?.ToList();

            return Ok(new ApiResponse<object>
            {
                Success = list != null && list.Count > 0,
                Message = list != null && list.Count > 0 ? "QP questions loaded" : "No QP questions found",
                Data    = list
            });
        }

        /// <summary>
        /// Get optional questions for best-of rules.
        /// Inline SQL on TBL_EVAL_SCHEMASTRUCTURE WHERE QSTATUS='O'
        ///
        /// GET /api/schemamarksentry/optional-questions?course=B.TECH&amp;regulation=R20&amp;schemaName=SCHEMA1
        /// </summary>
        [HttpGet("optional-questions")]
        public async Task<IActionResult> GetOptionalQuestions(
            [FromQuery] string course,
            [FromQuery] string regulation,
            [FromQuery] string schemaName)
        {
            if (string.IsNullOrWhiteSpace(schemaName))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "SchemaName is required" });

            var data = await _svc.GetOptionalQuestionsAsync(course, regulation, schemaName);
            var list = data?.ToList();

            return Ok(new ApiResponse<object>
            {
                Success = list != null && list.Count > 0,
                Message = list != null && list.Count > 0 ? "Optional questions loaded" : "No optional questions",
                Data    = list
            });
        }

        /// <summary>
        /// Get student's previously saved secured marks.
        /// SP: Sp_Eval_Get_SecuredMarks @SchemaName, @PapCode, @OmrNo
        ///
        /// GET /api/schemamarksentry/secured-marks?schemaName=SCHEMA1&amp;papCode=CS101&amp;omrNo=12345
        /// </summary>
        [HttpGet("secured-marks")]
        public async Task<IActionResult> GetSecuredMarks(
            [FromQuery] string schemaName,
            [FromQuery] string papCode,
            [FromQuery] string omrNo)
        {
            if (string.IsNullOrWhiteSpace(schemaName) || string.IsNullOrWhiteSpace(papCode) || string.IsNullOrWhiteSpace(omrNo))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "SchemaName, PapCode and OmrNo are required" });

            var data = await _svc.GetSecuredMarksAsync(schemaName, papCode, omrNo);
            var list = data?.ToList();

            return Ok(new ApiResponse<object>
            {
                Success = list != null && list.Count > 0,
                Message = list != null && list.Count > 0 ? "Secured marks loaded" : "No secured marks found",
                Data    = list
            });
        }

        /// <summary>
        /// Save student's secured marks per question.
        /// SP: Sp_Eval_Insert_Student_SecuredMarks (per question in loop)
        ///
        /// POST /api/schemamarksentry/save
        /// </summary>
        [HttpPost("save")]
        public async Task<IActionResult> SaveMarks([FromBody] SaveSchemaMarksRequest req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.SchemaName) ||
                string.IsNullOrWhiteSpace(req.PapCode) || string.IsNullOrWhiteSpace(req.OmrNo))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "SchemaName, PapCode and OmrNo are required"
                });
            }

            if (req.Marks == null || req.Marks.Count == 0)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Marks data is required"
                });
            }

            var rows = await _svc.SaveSecuredMarksAsync(req);
            return Ok(new ApiResponse<object>
            {
                Success = rows > 0,
                Message = rows > 0 ? "Marks saved successfully" : "Marks not saved"
            });
        }

        /// <summary>
        /// Finalize marks — set FStatus='Y'.
        /// UPDATE tbl_Eval_Student_Secured_Marks SET FStatus='Y' WHERE SchemaName=@SchemaName AND PapCode=@PapCode
        ///
        /// POST /api/schemamarksentry/finalize
        /// </summary>
        [HttpPost("finalize")]
        public async Task<IActionResult> FinalizeMarks([FromBody] FinalizeMarksRequest req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.SchemaName) || string.IsNullOrWhiteSpace(req.PapCode))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "SchemaName and PapCode are required"
                });
            }

            var rows = await _svc.FinalizeMarksAsync(req);
            return Ok(new ApiResponse<object>
            {
                Success = rows > 0,
                Message = rows > 0 ? "Marks finalized successfully" : "No marks to finalize"
            });
        }
    }
}
