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
    /// Settings / User Form(s) — UserSettings.aspx
    ///
    /// Mirrors the reference project's UserSettings.aspx PageMethod flow:
    ///   Page_Load:
    ///     1. getForms()           → GET /api/usersettings/forms
    ///        SPM_FORMS (no params) — populates all available forms list
    ///     2. UserGroups_Settings() → GET /api/usersettings/groups
    ///        SELECT DISTINCT USERID FROM TBL_USERS_MENU — populates group dropdown
    ///
    ///   Group selected (checkusergroup):
    ///     3. GET /api/usersettings/group-menu?userGroup=xxx
    ///        SPM_UserGroup_Menu_Load — shows forms currently assigned to group
    ///
    ///   Save button (DeleteUserForms + SaveUserForms loop):
    ///     4. POST /api/usersettings/save
    ///        Runs SPM_USERS_MENU_DELETE then SPM_USERS_MENU_SAVE per form
    ///
    ///   Delete Group button (DeleteUserGroup):
    ///     5. DELETE /api/usersettings/group?userGroup=xxx
    ///        PROC_DEL_USERS — removes group and all its form assignments
    ///
    /// All SPs confirmed from DataAccessLayer.dll UTF-16LE string analysis.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UserSettingsController : BaseApiController
    {
        private readonly IUserSettingsService _svc;

        public UserSettingsController(IUserSettingsService svc) => _svc = svc;

        /// <summary>
        /// Get all available menu/sub-menu forms.
        /// SP: SPM_FORMS (no params)
        /// Confirmed: PageMethod getForms()
        ///
        /// GET /api/usersettings/forms
        /// </summary>
        [HttpGet("forms")]
        public async Task<IActionResult> GetForms()
        {
            var data = await _svc.GetFormsAsync();
            var list = data?.ToList();

            return Ok(new ApiResponse<object>
            {
                Success = list != null && list.Count > 0,
                Message = list != null && list.Count > 0 ? "Forms loaded" : "No forms found",
                Data    = list
            });
        }

        /// <summary>
        /// Get all user groups stored in TBL_USERS_MENU.
        /// Inline SQL: SELECT DISTINCT USERID FROM TBL_USERS_MENU
        /// Confirmed: PageMethod UserGroups_Settings()
        ///
        /// GET /api/usersettings/groups
        /// </summary>
        [HttpGet("groups")]
        public async Task<IActionResult> GetUserGroups()
        {
            var data = await _svc.GetUserGroupsAsync();
            var list = data?.ToList();

            return Ok(new ApiResponse<object>
            {
                Success = list != null && list.Count > 0,
                Message = list != null && list.Count > 0 ? "User groups loaded" : "No user groups found",
                Data    = list
            });
        }

        /// <summary>
        /// Get forms currently assigned to a user group.
        /// SP: SPM_UserGroup_Menu_Load 'userGroup'
        /// Confirmed: PageMethod checkusergroup(group)
        ///
        /// GET /api/usersettings/group-menu?userGroup=ADMIN
        /// </summary>
        [HttpGet("group-menu")]
        public async Task<IActionResult> GetUserGroupMenu([FromQuery] string userGroup)
        {
            if (string.IsNullOrWhiteSpace(userGroup))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "UserGroup is required" });

            var data = await _svc.GetUserGroupMenuAsync(userGroup);
            var list = data?.ToList();

            return Ok(new ApiResponse<object>
            {
                Success = list != null && list.Count > 0,
                Message = list != null && list.Count > 0 ? "Group menu loaded" : "No forms assigned to this group",
                Data    = list
            });
        }

        /// <summary>
        /// Save form assignments for a user group (delete-then-insert).
        /// Flow: SPM_USERS_MENU_DELETE → SPM_USERS_MENU_SAVE (per form in loop)
        /// Confirmed: PageMethod DeleteUserForms(group) + SaveUserForms(group,cid,subid)
        ///
        /// POST /api/usersettings/save
        /// Body: { "userGroup": "ADMIN", "forms": [{ "menuId": "1", "subMenuId": "101" }] }
        /// </summary>
        [HttpPost("save")]
        public async Task<IActionResult> SaveUserGroupForms([FromBody] SaveUserGroupFormsRequest req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.UserGroup))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "UserGroup is required" });

            var saved = await _svc.SaveUserGroupFormsAsync(req);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = saved > 0
                    ? $"Saved {saved} form(s) for group '{req.UserGroup}'"
                    : $"All existing forms cleared for group '{req.UserGroup}' (no new forms to save)",
                Data    = new { SavedCount = saved }
            });
        }

        /// <summary>
        /// Delete an entire user group and all its form assignments.
        /// SP: PROC_DEL_USERS 'userGroup'
        /// Confirmed: PageMethod DeleteUserGroup(group)
        ///
        /// DELETE /api/usersettings/group?userGroup=ADMIN
        /// </summary>
        [HttpDelete("group")]
        public async Task<IActionResult> DeleteUserGroup([FromQuery] string userGroup)
        {
            if (string.IsNullOrWhiteSpace(userGroup))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "UserGroup is required" });

            var rows = await _svc.DeleteUserGroupAsync(userGroup);

            return Ok(new ApiResponse<object>
            {
                Success = rows > 0,
                Message = rows > 0
                    ? $"User group '{userGroup}' deleted"
                    : $"No rows affected for group '{userGroup}'"
            });
        }
    }
}
