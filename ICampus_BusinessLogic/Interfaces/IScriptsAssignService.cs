using System.Collections.Generic;
using System.Threading.Tasks;
using ICampus_Models.Requests;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IScriptsAssignService
    {
        /// <summary>
        /// Returns count of scripts not yet assigned to any evaluator.
        /// Inline SQL: SELECT COUNT(*) AS PendingScripts FROM Tbl_DV_Marks WHERE EvaluatorId IS NULL
        /// Shown as lblNoofScripts on page load.
        /// </summary>
        Task<IEnumerable<object>> GetPendingScriptCountAsync();

        /// <summary>
        /// Returns papers/subjects assigned to a specific evaluator.
        /// Inline SQL: Select PCode, PCode+'_'+PNAME AS PNAME FROM tbl_Eval_UserPapers
        ///             WHERE UserId='...' ORDER BY PName
        /// Populates ddlSubject after evaluator is selected.
        /// </summary>
        Task<IEnumerable<object>> GetEvaluatorSubjectsAsync(string userId);

        /// <summary>
        /// Returns semester list for an evaluator and paper combination.
        /// SP: Sp_Eval_Script_Assign_Load_Sem @EvaluatorId, @PapCode
        /// Populates ddlsem after subject is selected.
        /// </summary>
        Task<IEnumerable<object>> GetSemestersAsync(string regulation, string papCode);

        /// <summary>
        /// Returns bundle numbers available for a paper and semester.
        /// SP: SP_Eval_Get_BundleNo @PapCode, @Sem
        /// Populates lstBundleNo after semester is selected.
        /// Source: Tbl_DV_Marks
        /// </summary>
        Task<IEnumerable<object>> GetBundleNumbersAsync(string papCode, string sem);

        /// <summary>
        /// Returns unassigned scripts (answer booklet IDs) inside a bundle.
        /// SP: SP_Eval_Get_Scripts @PapCode, @Sem, @BundleNo
        /// Populates lstScripts when a bundle is selected.
        /// Only returns scripts where EvaluatorId IS NULL.
        /// </summary>
        Task<IEnumerable<object>> GetScriptsAsync(string papCode, string sem, string bundleNo);

        /// <summary>
        /// Assigns selected scripts to an evaluator and saves QP/Key file paths.
        /// SP: SP_EVAL_SAVE_SCRIPTS @EvaluatorId, @PapCode, @Sem, @EvalDate,
        ///                          @BundleNo, @ScriptIds, @QpPath, @KeyPath
        /// Returns rows affected.
        /// </summary>
        Task<int> SaveScriptsAssignAsync(SaveScriptsAssignRequest req);
    }
}
