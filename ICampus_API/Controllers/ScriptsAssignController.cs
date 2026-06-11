using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using ICampus_Models.Requests;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_Api.Controllers
{
    /// <summary>
    /// Evaluation — Scripts Assign (Evaluation/Scripts_Assign.aspx)
    ///
    /// Page flow:
    ///   Page_Load:
    ///     1. GET /api/scriptsassign/pending-count
    ///        Inline SQL — shows "N Scripts Pending" badge (Tbl_DV_Marks WHERE EvaluatorId IS NULL)
    ///     Note: Evaluator dropdown reuses GET /api/evaluatorregistration/dropdown
    ///           (SP_OE_EVALUATOR_LIST)
    ///
    ///   Evaluator selected (ddlEvaluator):
    ///     2. GET /api/scriptsassign/subjects?userId=EVL001
    ///        Inline SQL on tbl_Eval_UserPapers — loads subject dropdown
    ///
    ///   Subject selected (ddlSubject):
    ///     3. GET /api/scriptsassign/semesters?evaluatorId=EVL001&amp;papCode=CS101
    ///        Sp_Eval_Script_Assign_Load_Sem — loads semester dropdown
    ///
    ///   Semester selected (ddlsem):
    ///     4. GET /api/scriptsassign/bundles?papCode=CS101&amp;sem=1
    ///        SP_Eval_Get_BundleNo — loads bundle number listbox
    ///
    ///   Bundle selected (lstBundleNo):
    ///     5. GET /api/scriptsassign/scripts?papCode=CS101&amp;sem=1&amp;bundleNo=B001
    ///        SP_Eval_Get_Scripts — loads scripts listbox (unassigned only)
    ///
    ///   Save button (btnSave_Click) — multipart/form-data:
    ///     6. POST /api/scriptsassign/save
    ///        Saves QP + Key files to server, then SP_EVAL_SAVE_SCRIPTS
    ///
    /// All SPs confirmed from App_Web_xplim0cm.dll ASPX source + DataAccessLayer.dll analysis.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ScriptsAssignController : BaseApiController
    {
        private readonly IScriptsAssignService _svc;
        private readonly IWebHostEnvironment   _env;

        public ScriptsAssignController(IScriptsAssignService svc, IWebHostEnvironment env)
        {
            _svc = svc;
            _env = env;
        }

        /// <summary>
        /// Count of scripts not yet assigned to any evaluator.
        /// Inline SQL: SELECT COUNT(*) AS PendingScripts FROM Tbl_DV_Marks WHERE EvaluatorId IS NULL
        ///
        /// GET /api/scriptsassign/pending-count
        /// </summary>
        [HttpGet("pending-count")]
        public async Task<IActionResult> GetPendingCount()
        {
            var data = await _svc.GetPendingScriptCountAsync();
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Pending script count loaded",
                Data    = data
            });
        }

        /// <summary>
        /// Papers/subjects assigned to an evaluator (for ddlSubject).
        /// Inline SQL on tbl_Eval_UserPapers.
        ///
        /// GET /api/scriptsassign/subjects?userId=EVL001
        /// </summary>
        [HttpGet("subjects")]
        public async Task<IActionResult> GetSubjects([FromQuery] string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "UserId is required" });

            var data = await _svc.GetEvaluatorSubjectsAsync(userId);
            var list = data?.ToList();

            return Ok(new ApiResponse<object>
            {
                Success = list != null && list.Count > 0,
                Message = list != null && list.Count > 0 ? "Subjects loaded" : "No subjects found for this evaluator",
                Data    = list
            });
        }

        /// <summary>
        /// Semester dropdown for evaluator + paper combination.
        /// SP: Sp_Eval_Script_Assign_Load_Sem @EvaluatorId, @PapCode
        ///
        /// GET /api/scriptsassign/semesters?evaluatorId=EVL001&amp;papCode=CS101
        /// </summary>
        [HttpGet("semesters")]
        public async Task<IActionResult> GetSemesters(
            [FromQuery] string regulation,
            [FromQuery] string papCode)
        {
            if (string.IsNullOrWhiteSpace(regulation) || string.IsNullOrWhiteSpace(papCode))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Regulation and PapCode are required" });

            var data = await _svc.GetSemestersAsync(regulation, papCode);
            var list = data?.ToList();

            return Ok(new ApiResponse<object>
            {
                Success = list != null && list.Count > 0,
                Message = list != null && list.Count > 0 ? "Semesters loaded" : "No semesters found",
                Data    = list
            });
        }

        /// <summary>
        /// Bundle numbers for a paper + semester.
        /// SP: SP_Eval_Get_BundleNo @PapCode, @Sem
        ///
        /// GET /api/scriptsassign/bundles?papCode=CS101&amp;sem=1
        /// </summary>
        [HttpGet("bundles")]
        public async Task<IActionResult> GetBundleNumbers(
            [FromQuery] string papCode,
            [FromQuery] string sem)
        {
            if (string.IsNullOrWhiteSpace(papCode) || string.IsNullOrWhiteSpace(sem))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "PapCode and Sem are required" });

            var data = await _svc.GetBundleNumbersAsync(papCode, sem);
            var list = data?.ToList();

            return Ok(new ApiResponse<object>
            {
                Success = list != null && list.Count > 0,
                Message = list != null && list.Count > 0 ? "Bundle numbers loaded" : "No bundles found",
                Data    = list
            });
        }

        /// <summary>
        /// Unassigned scripts (answer booklet IDs) in a bundle.
        /// SP: SP_Eval_Get_Scripts @PapCode, @Sem, @BundleNo
        ///
        /// GET /api/scriptsassign/scripts?papCode=CS101&amp;sem=1&amp;bundleNo=B001
        /// </summary>
        [HttpGet("scripts")]
        public async Task<IActionResult> GetScripts(
            [FromQuery] string papCode,
            [FromQuery] string sem,
            [FromQuery] string bundleNo)
        {
            if (string.IsNullOrWhiteSpace(papCode) || string.IsNullOrWhiteSpace(sem) || string.IsNullOrWhiteSpace(bundleNo))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "PapCode, Sem and BundleNo are required" });

            var data = await _svc.GetScriptsAsync(papCode, sem, bundleNo);
            var list = data?.ToList();

            return Ok(new ApiResponse<object>
            {
                Success = list != null && list.Count > 0,
                Message = list != null && list.Count > 0 ? "Scripts loaded" : "No unassigned scripts found in this bundle",
                Data    = list
            });
        }

        /// <summary>
        /// Assign scripts to evaluator + upload QP and Key PDF files.
        /// Accepts multipart/form-data.
        ///
        /// POST /api/scriptsassign/save
        /// Form fields: evaluatorId, papCode, sem, evalDate,
        ///              bundleNos (comma-separated), scriptIds (comma-separated),
        ///              qpFile (IFormFile), keyFile (IFormFile)
        /// </summary>
        [HttpPost("save")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> SaveScriptsAssign(
            [FromForm] string evaluatorId,
            [FromForm] string papCode,
            [FromForm] string sem,
            [FromForm] string evalDate,
            [FromForm] string bundleNos,
            [FromForm] string scriptIds,
            IFormFile qpFile,
            IFormFile keyFile)
        {
            if (string.IsNullOrWhiteSpace(evaluatorId) || string.IsNullOrWhiteSpace(papCode) ||
                string.IsNullOrWhiteSpace(sem))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Select all fields..!" });

            // Save QP file
            string qpPath = string.Empty;
            if (qpFile != null && qpFile.Length > 0)
            {
                var qpFolder = Path.Combine(_env.WebRootPath, "Evaluation", "QuestionPaper");
                Directory.CreateDirectory(qpFolder);
                var qpFileName = $"{papCode}_{sem}_{DateTime.Now:ddMMyyyyHHmmss}{Path.GetExtension(qpFile.FileName)}";
                var qpFullPath = Path.Combine(qpFolder, qpFileName);
                using var qpStream = new FileStream(qpFullPath, FileMode.Create);
                await qpFile.CopyToAsync(qpStream);
                qpPath = Path.Combine("Evaluation", "QuestionPaper", qpFileName);
            }

            // Save Key file
            string keyPath = string.Empty;
            if (keyFile != null && keyFile.Length > 0)
            {
                var keyFolder = Path.Combine(_env.WebRootPath, "Evaluation", "Key");
                Directory.CreateDirectory(keyFolder);
                var keyFileName = $"{papCode}_{sem}_{DateTime.Now:ddMMyyyyHHmmss}{Path.GetExtension(keyFile.FileName)}";
                var keyFullPath = Path.Combine(keyFolder, keyFileName);
                using var keyStream = new FileStream(keyFullPath, FileMode.Create);
                await keyFile.CopyToAsync(keyStream);
                keyPath = Path.Combine("Evaluation", "Key", keyFileName);
            }

            var req = new SaveScriptsAssignRequest
            {
                EvaluatorId = evaluatorId,
                PapCode     = papCode,
                Sem         = sem,
                EvalDate    = evalDate,
                BundleNos   = bundleNos?.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList(),
                ScriptIds   = scriptIds?.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).Where(s => s.Length > 0).ToList(),
                QpPath      = qpPath,
                KeyPath     = keyPath
            };

            var rows = await _svc.SaveScriptsAssignAsync(req);

            return Ok(new ApiResponse<object>
            {
                Success = rows > 0,
                Message = rows > 0
                    ? "Scripts assign to Evaluator successfully.."
                    : "Data not saved ..!",
                Data = new
                {
                    RowsAffected = rows,
                    QpPath       = qpPath,
                    KeyPath      = keyPath
                }
            });
        }
    }
}
