using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Common;
using ICampus_Models.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_Api.Controllers
{
    /// <summary>
    /// Evaluation — Evaluator Registration (Evaluation/Evaluator_Registration.aspx)
    ///
    /// Page flow (confirmed from ASPX source):
    ///   Page_Load:
    ///     1. GET /api/evaluatorregistration/list?userGroup=Evaluator
    ///        SP_EVALUATOR_LOAD — loads gvUsers grid
    ///
    ///   UserType changed (ddlUserType_SelectedIndexChanged):
    ///     2. GET /api/evaluatorregistration/next-userid?userGroup=Evaluator
    ///        Inline SQL — auto-generates next UserID sequence
    ///
    ///   Row clicked (lblNAME_Click):
    ///     3. GET /api/evaluatorregistration/user-details?userId=xxx
    ///        SP_EVALUATOR_LOAD_USER_DETAILS — fills form fields for edit
    ///     4. GET /api/evaluatorregistration/user-papers?userId=xxx
    ///        Inline SQL on tbl_Eval_UserPapers — pre-selects papers in lstPap
    ///
    ///   Department/Sem changed (ddlDepartment_SelectedIndexChanged):
    ///     5. GET /api/evaluatorregistration/departments?regulation=&amp;course=&amp;examMY=
    ///        Inline SQL on tbl_sh — loads department dropdown
    ///
    ///   Save button (btnSaveCourse_Click):
    ///     6. POST /api/evaluatorregistration/save
    ///        SP_EVALUATOR_REGISTRATIONS + Sp_Eval_Save_UserPapers loop
    ///
    ///   Evaluator dropdown (for Scripts Assign and other pages):
    ///     7. GET /api/evaluatorregistration/dropdown
    ///        SP_OE_EVALUATOR_LIST — active evaluators only
    ///
    /// Confirmed from App_Web_xplim0cm.dll ASPX source + DataAccessLayer.dll analysis.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class EvaluatorRegistrationController : BaseApiController
    {
        private readonly IEvaluatorRegistrationService _svc;

        public EvaluatorRegistrationController(IEvaluatorRegistrationService svc) => _svc = svc;

        /// <summary>
        /// Load evaluator/scrutinizer grid.
        /// SP: SP_EVALUATOR_LOAD @UserGroup
        /// Returns: EID, USERID, NAME, DESIGNATION, DEPTARTMENT, USERGROUP, MOBILE, EMAIL, COLLEGE, ISACTIVE
        ///
        /// GET /api/evaluatorregistration/list?userGroup=Evaluator
        /// GET /api/evaluatorregistration/list?userGroup=Scrutinizer
        /// </summary>
        [HttpGet("list")]
        public async Task<IActionResult> GetEvaluatorList([FromQuery] string userGroup)
        {
            if (string.IsNullOrWhiteSpace(userGroup))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "UserGroup is required (Evaluator or Scrutinizer)" });

            var data = await _svc.GetEvaluatorListAsync(userGroup);
            var list = data?.ToList();

            return Ok(new ApiResponse<object>
            {
                Success = list != null && list.Count > 0,
                Message = list != null && list.Count > 0 ? $"{userGroup} list loaded" : $"No {userGroup.ToLower()}s found",
                Data    = list
            });
        }

        /// <summary>
        /// Active evaluators dropdown (used in Scripts Assign too).
        /// SP: SP_OE_EVALUATOR_LIST (no params)
        ///
        /// GET /api/evaluatorregistration/dropdown
        /// </summary>
        [HttpGet("dropdown")]
        public async Task<IActionResult> GetEvaluatorDropdown()
        {
            var data = await _svc.GetEvaluatorDropdownAsync();
            var list = data?.ToList();

            return Ok(new ApiResponse<object>
            {
                Success = list != null && list.Count > 0,
                Message = list != null && list.Count > 0 ? "Evaluator list loaded" : "No active evaluators found",
                Data    = list
            });
        }

        /// <summary>
        /// Load all fields for one evaluator (for edit form).
        /// SP: SP_EVALUATOR_LOAD_USER_DETAILS @UserID
        ///
        /// GET /api/evaluatorregistration/user-details?userId=EVL001
        /// </summary>
        [HttpGet("user-details")]
        public async Task<IActionResult> GetUserDetails([FromQuery] string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "UserID is required" });

            var data = await _svc.GetUserDetailsAsync(userId);
            var list = data?.ToList();

            return Ok(new ApiResponse<object>
            {
                Success = list != null && list.Count > 0,
                Message = list != null && list.Count > 0 ? "User details loaded" : "User not found",
                Data    = list
            });
        }

        /// <summary>
        /// Auto-generate next UserID sequence for a group.
        /// Inline SQL on tbl_Eval_Registrations.
        ///
        /// GET /api/evaluatorregistration/next-userid?userGroup=Evaluator
        /// </summary>
        [HttpGet("next-userid")]
        public async Task<IActionResult> GetNextUserId([FromQuery] string userGroup)
        {
            if (string.IsNullOrWhiteSpace(userGroup))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "UserGroup is required" });

            var data = await _svc.GetNextUserIdAsync(userGroup);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Next UserID generated",
                Data    = data
            });
        }

        /// <summary>
        /// Load papers already assigned to an evaluator (for lstPap pre-selection).
        /// Inline SQL on tbl_Eval_UserPapers.
        ///
        /// GET /api/evaluatorregistration/user-papers?userId=EVL001
        /// </summary>
        [HttpGet("user-papers")]
        public async Task<IActionResult> GetUserPapers([FromQuery] string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "UserID is required" });

            var data = await _svc.GetUserPapersAsync(userId);
            var list = data?.ToList();

            return Ok(new ApiResponse<object>
            {
                Success = list != null && list.Count > 0,
                Message = list != null && list.Count > 0 ? "User papers loaded" : "No papers assigned",
                Data    = list
            });
        }

        /// <summary>
        /// Load department dropdown (ddlDepartment).
        /// Inline SQL: SELECT DISTINCT GRP FROM tbl_sh WHERE Regulation=... AND Course=... AND EXAMMY=...
        ///
        /// GET /api/evaluatorregistration/departments?regulation=R20&amp;course=B.TECH&amp;examMY=NOV2024
        /// </summary>
        [HttpGet("departments")]
        public async Task<IActionResult> GetDepartments(
            [FromQuery] string regulation,
            [FromQuery] string course,
            [FromQuery] string examMY)
        {
            var data = await _svc.GetDepartmentsAsync(regulation, course, examMY);
            var list = data?.ToList();

            return Ok(new ApiResponse<object>
            {
                Success = list != null && list.Count > 0,
                Message = list != null && list.Count > 0 ? "Departments loaded" : "No departments found",
                Data    = list
            });
        }

        /// <summary>
        /// Save evaluator/scrutinizer registration + paper assignments.
        /// Flow: SP_EVALUATOR_REGISTRATIONS → Sp_Eval_Save_UserPapers (loop per paper)
        ///
        /// POST /api/evaluatorregistration/save
        /// Body: { "userID":"EVL001", "name":"John Doe", "designation":"Professor",
        ///         "department":"CSE", "userGroup":"Evaluator", "mobile":"9876543210",
        ///         "email":"john@college.edu", "college":"ABC College",
        ///         "isActive":"1", "sem":"1",
        ///         "papCodes":["CS101","CS102"] }
        /// </summary>
        [HttpPost("save")]
        public async Task<IActionResult> SaveEvaluator([FromBody] SaveEvaluatorRequest req)
        {
            if (req == null)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Please Fill All Fields..!" });

            if (string.IsNullOrWhiteSpace(req.UserID) ||
                string.IsNullOrWhiteSpace(req.Name)   ||
                string.IsNullOrWhiteSpace(req.UserGroup))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Please Fill All Fields..!" });

            var rows = await _svc.SaveEvaluatorAsync(req);

            return Ok(new ApiResponse<object>
            {
                Success = rows > 0,
                Message = rows > 0 ? "Details Saved Successfully" : "Details not Saved",
                Data    = new { RowsAffected = rows, PapersAssigned = req.PapCodes?.Count ?? 0 }
            });
        }
    }
}
