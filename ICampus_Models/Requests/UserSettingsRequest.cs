using System.Collections.Generic;

namespace ICampus_Models.Requests
{
    /// <summary>
    /// Request body for saving user group form assignments.
    /// Called from UserSettings.aspx PageMethods:
    ///   1. DeleteUserForms(userGroup)         → SPM_USERS_MENU_DELETE
    ///   2. SaveUserForms(userGroup,cid,subid) → SPM_USERS_MENU_SAVE (one per form in loop)
    /// </summary>
    public class SaveUserGroupFormsRequest
    {
        public string UserGroup { get; set; }
        public List<UserFormEntry> Forms { get; set; } = new List<UserFormEntry>();
    }

    public class UserFormEntry
    {
        public string MenuId { get; set; }
        public string SubMenuId { get; set; }
    }
}
