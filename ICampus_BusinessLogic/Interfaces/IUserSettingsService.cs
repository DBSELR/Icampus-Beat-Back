using System.Collections.Generic;
using System.Threading.Tasks;
using ICampus_Models.Requests;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IUserSettingsService
    {
        /// <summary>
        /// Returns all available menu/sub-menu forms.
        /// Confirmed: PageMethod getForms() → SPM_FORMS (no params)
        /// </summary>
        Task<IEnumerable<object>> GetFormsAsync();

        /// <summary>
        /// Returns all user groups stored in TBL_USERS_MENU.
        /// Confirmed: PageMethod UserGroups_Settings() → SELECT DISTINCT USERID FROM TBL_USERS_MENU
        /// </summary>
        Task<IEnumerable<object>> GetUserGroupsAsync();

        /// <summary>
        /// Returns forms currently assigned to a user group.
        /// Confirmed: PageMethod checkusergroup(group) → SPM_UserGroup_Menu_Load 'userGroup'
        /// </summary>
        Task<IEnumerable<object>> GetUserGroupMenuAsync(string userGroup);

        /// <summary>
        /// Saves form assignments for a user group (delete-then-insert).
        /// Confirmed flow from UserSettings.aspx:
        ///   1. DeleteUserForms(userGroup) → SPM_USERS_MENU_DELETE
        ///   2. SaveUserForms(userGroup, menuId, subMenuId) → SPM_USERS_MENU_SAVE (loop)
        /// Returns total rows affected by all save calls.
        /// </summary>
        Task<int> SaveUserGroupFormsAsync(SaveUserGroupFormsRequest req);

        /// <summary>
        /// Deletes an entire user group and all its assignments.
        /// Confirmed: PageMethod DeleteUserGroup(group) → PROC_DEL_USERS 'userGroup'
        /// </summary>
        Task<int> DeleteUserGroupAsync(string userGroup);
    }
}
