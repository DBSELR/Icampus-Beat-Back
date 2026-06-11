using ICampus_BusinessLogic.Interfaces;
using ICampus_DataAccessLayer.Helpers;
using ICampus_DataAccessLayer.Interfaces;
using ICampus_Models.Requests;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Services
{
    public class UserSettingsService : IUserSettingsService
    {
        private readonly IGenericRepository<object> _repo;

        public UserSettingsService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Returns all available menu/sub-menu forms.
        /// SP: SPM_FORMS (no params)
        /// Confirmed: PageMethod getForms() → SPM_FORMS
        /// Returns: SubMenuID, MenuID, Text columns
        /// </summary>
        public async Task<IEnumerable<object>> GetFormsAsync()
        {
            var sql = StoredProcSql.Exec(StoredProcedures.SPM_FORMS);
            var raw = await _repo.QueryFromStoredProcAsync(sql);
            return raw ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Returns all user groups.
        /// Inline SQL: SELECT DISTINCT USERID FROM TBL_USERS_MENU
        /// Confirmed: PageMethod UserGroups_Settings()
        /// Returns: USERID column (used as both value and display)
        /// </summary>
        public async Task<IEnumerable<object>> GetUserGroupsAsync()
        {
            var sql = "SELECT DISTINCT USERID FROM TBL_USERS_MENU";
            var raw = await _repo.QueryFromStoredProcAsync(sql);
            return raw ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Returns forms assigned to a user group.
        /// SP: SPM_UserGroup_Menu_Load @UserGroup
        /// Confirmed: PageMethod checkusergroup(group)
        /// </summary>
        public async Task<IEnumerable<object>> GetUserGroupMenuAsync(string userGroup)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.SPM_UserGroup_Menu_Load, "@UserGroup");
            var parameters = new[]
            {
                new SqlParameter("@UserGroup", SqlDbType.VarChar, 100) { Value = userGroup ?? string.Empty }
            };
            var raw = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return raw ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Saves form assignments for a user group.
        /// Flow confirmed from UserSettings.aspx:
        ///   1. SPM_USERS_MENU_DELETE @UserGroup  — clears all existing assignments
        ///   2. SPM_USERS_MENU_SAVE @UserGroup, @MenuId, @SubMenuId — per form in loop
        /// Returns total rows affected by save calls (delete rows not counted).
        /// </summary>
        public async Task<int> SaveUserGroupFormsAsync(SaveUserGroupFormsRequest req)
        {
            // Step 1: Delete all existing form assignments for this group
            var deleteSql = StoredProcSql.Exec(StoredProcedures.SPM_USERS_MENU_DELETE, "@UserGroup");
            var deleteParams = new[]
            {
                new SqlParameter("@UserGroup", SqlDbType.VarChar, 100) { Value = req.UserGroup ?? string.Empty }
            };
            await _repo.ExecuteStoredProcAsync(deleteSql, (object[])deleteParams);

            // Step 2: Save each selected form (loop, same as original ASPX PageMethod loop)
            int totalSaved = 0;
            if (req.Forms != null)
            {
                foreach (var form in req.Forms)
                {
                    var saveSql = StoredProcSql.Exec(StoredProcedures.SPM_USERS_MENU_SAVE, "@UserGroup", "@MenuId", "@SubMenuId");
                    var saveParams = new[]
                    {
                        new SqlParameter("@UserGroup", SqlDbType.VarChar, 100) { Value = req.UserGroup  ?? string.Empty },
                        new SqlParameter("@MenuId",    SqlDbType.VarChar, 50)  { Value = form.MenuId    ?? string.Empty },
                        new SqlParameter("@SubMenuId", SqlDbType.VarChar, 50)  { Value = form.SubMenuId ?? string.Empty }
                    };
                    totalSaved += await _repo.ExecuteStoredProcAsync(saveSql, (object[])saveParams);
                }
            }

            return totalSaved;
        }

        /// <summary>
        /// Deletes an entire user group and all its assignments.
        /// SP: PROC_DEL_USERS @UserGroup
        /// Confirmed: PageMethod DeleteUserGroup(group)
        /// </summary>
        public async Task<int> DeleteUserGroupAsync(string userGroup)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.PROC_DEL_USERS, "@UserGroup");
            var parameters = new[]
            {
                new SqlParameter("@UserGroup", SqlDbType.VarChar, 100) { Value = userGroup ?? string.Empty }
            };
            return await _repo.ExecuteStoredProcAsync(sql, (object[])parameters);
        }
    }
}
