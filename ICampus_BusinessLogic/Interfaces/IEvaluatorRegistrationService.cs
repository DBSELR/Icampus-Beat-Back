using System.Collections.Generic;
using System.Threading.Tasks;
using ICampus_Models.Requests;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IEvaluatorRegistrationService
    {
        /// <summary>
        /// Returns evaluator/scrutinizer list grid.
        /// SP: SP_EVALUATOR_LOAD @UserGroup
        /// Returns: EID, USERID, NAME, DESIGNATION, DEPTARTMENT, USERGROUP, MOBILE, EMAIL, COLLEGE, ISACTIVE
        /// </summary>
        Task<IEnumerable<object>> GetEvaluatorListAsync(string userGroup);

        /// <summary>
        /// Returns active evaluators dropdown (for Scripts Assign page usage too).
        /// SP: SP_OE_EVALUATOR_LIST (no params)
        /// Inline SQL context: SELECT USERID, USERID+'_'+NAME as NAME FROM tbl_Eval_Registrations
        /// </summary>
        Task<IEnumerable<object>> GetEvaluatorDropdownAsync();

        /// <summary>
        /// Returns full details of one evaluator for editing.
        /// SP: SP_EVALUATOR_LOAD_USER_DETAILS @UserID
        /// </summary>
        Task<IEnumerable<object>> GetUserDetailsAsync(string userId);

        /// <summary>
        /// Auto-generates the next UserID sequence for a user group.
        /// Inline SQL: select isnull(max(right(UserID,3)),0)+1 as UserID
        ///             from tbl_Eval_Registrations where USERGROUP='...'
        /// </summary>
        Task<IEnumerable<object>> GetNextUserIdAsync(string userGroup);

        /// <summary>
        /// Returns papers already assigned to an evaluator.
        /// Inline SQL: Select PCode, PCode+'_'+PNAME as PNAME
        ///             from tbl_Eval_UserPapers where UserId='...'
        /// </summary>
        Task<IEnumerable<object>> GetUserPapersAsync(string userId);

        /// <summary>
        /// Returns department/branch dropdown for the form.
        /// Inline SQL: select distinct GRP from tbl_sh
        ///             where Regulation='...' and Course='...' and EXAMMY='...' order by GRP
        /// </summary>
        Task<IEnumerable<object>> GetDepartmentsAsync(string regulation, string course, string examMY);

        /// <summary>
        /// Saves evaluator registration and paper assignments.
        /// Flow: SP_EVALUATOR_REGISTRATIONS (profile)
        ///       → Sp_Eval_Save_UserPapers @UserId, @PapCode (loop per paper)
        /// Returns rows affected by SP_EVALUATOR_REGISTRATIONS.
        /// </summary>
        Task<int> SaveEvaluatorAsync(SaveEvaluatorRequest req);
    }
}
