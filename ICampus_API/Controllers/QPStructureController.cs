using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using ICampus_Models.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_Api.Controllers
{
    /// <summary>
    /// Evaluation — QP Structure (Evaluation/QP_Structure.aspx)
    ///
    /// Page flow:
    ///   Page_Load:
    ///     Schema dropdown reuses GET /api/schemastructure/schemas
    ///     (inline SQL on TBL_EVAL_SCHEMAMASTER — populates ddlSavedSchema)
    ///
    ///   ddlSavedSchema_SelectedIndexChanged:
    ///     1. GET /api/qpstructure/structure?schemaName=SCHEMA1
    ///        Sp_Eval_Get_Qp_Structure — loads lbQNO1 (compulsory) + lbQNO2 (optional)
    ///
    ///   Existing QP questions (txtQuestions pre-populate):
    ///     2. GET /api/qpstructure/questions?schemaName=SCHEMA1
    ///        Inline SQL: SELECT QUES FROM TBL_EVAL_QPSTRUCTURE
    ///
    ///   btnSave_Click:
    ///     3. POST /api/qpstructure/save
    ///        SP_EVAL_QPSTRUCTURE_SAVE — saves selected question structure
    ///
    /// All SPs confirmed from App_Web_xplim0cm.dll + DataAccessLayer.dll analysis.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class QPStructureController : BaseApiController
    {
        private readonly IQPStructureService _svc;

        public QPStructureController(IQPStructureService svc)
        {
            _svc = svc;
        }

        /// <summary>
        /// Load QP structure (compulsory + optional questions) for a schema.
        /// SP: Sp_Eval_Get_Qp_Structure @SchemaName
        ///
        /// GET /api/qpstructure/structure?schemaName=SCHEMA1
        /// </summary>
        [HttpGet("structure")]
        public async Task<IActionResult> GetStructure([FromQuery] string schemaName)
        {
            if (string.IsNullOrWhiteSpace(schemaName))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "SchemaName is required" });

            var data = await _svc.GetQPStructureAsync(schemaName);
            var list = data?.ToList();

            return Ok(new ApiResponse<object>
            {
                Success = list != null && list.Count > 0,
                Message = list != null && list.Count > 0 ? "QP structure loaded" : "No structure found",
                Data    = list
            });
        }

        /// <summary>
        /// Get saved QP questions text from TBL_EVAL_QPSTRUCTURE.
        /// Inline SQL: SELECT QUES FROM TBL_EVAL_QPSTRUCTURE WHERE SCHEMANAME=@SchemaName
        ///
        /// GET /api/qpstructure/questions?schemaName=SCHEMA1
        /// </summary>
        [HttpGet("questions")]
        public async Task<IActionResult> GetSavedQuestions([FromQuery] string schemaName)
        {
            if (string.IsNullOrWhiteSpace(schemaName))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "SchemaName is required" });

            var data = await _svc.GetSavedQuestionsAsync(schemaName);
            var list = data?.ToList();

            return Ok(new ApiResponse<object>
            {
                Success = list != null && list.Count > 0,
                Message = list != null && list.Count > 0 ? "QP questions loaded" : "No saved questions",
                Data    = list
            });
        }

        /// <summary>
        /// Save QP structure (question selections).
        /// SP: SP_EVAL_QPSTRUCTURE_SAVE
        ///
        /// POST /api/qpstructure/save
        /// </summary>
        [HttpPost("save")]
        public async Task<IActionResult> Save([FromBody] SaveQPStructureRequest req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.SchemaName))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "SchemaName is required" });

            var rows = await _svc.SaveQPStructureAsync(req);
            return Ok(new ApiResponse<object>
            {
                Success = rows > 0,
                Message = rows > 0 ? "QP structure saved successfully" : "Data not saved"
            });
        }
    }
}
