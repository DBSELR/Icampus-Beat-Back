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
    public class ScriptsAssignService : IScriptsAssignService
    {
        private readonly IGenericRepository<object> _repo;

        public ScriptsAssignService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Count of all unassigned scripts in Tbl_DV_Marks.
        /// Shown as "N Scripts Pending" in lblNoofScripts on page load.
        /// Inline SQL confirmed from DataAccessLayer.dll:
        ///   'select COUNT(*) as PendingScripts from Tbl_DV_Marks where EvaluatorId is null'
        /// </summary>
        public async Task<IEnumerable<object>> GetPendingScriptCountAsync()
        {
            var sql = "SELECT COUNT(*) AS PendingScripts FROM Tbl_DV_Marks WHERE EvaluatorId IS NULL";
            var raw = await _repo.QueryFromStoredProcAsync(sql);
            return raw ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Papers/subjects assigned to a specific evaluator.
        /// Inline SQL confirmed from DataAccessLayer.dll:
        ///   "Select PCode,PCode+'_'+PNAME as PNAME from tbl_Eval_UserPapers where UserId='...' order by PName"
        /// Populates ddlSubject dropdown when an evaluator is selected.
        /// </summary>
        public async Task<IEnumerable<object>> GetEvaluatorSubjectsAsync(string userId)
        {
            var sql = "SELECT PCode, PCode+'_'+PNAME AS PNAME, Regulation FROM tbl_Eval_UserPapers WHERE UserId = @UserId ORDER BY PName";
            var parameters = new[]
            {
                new SqlParameter("@UserId", SqlDbType.VarChar, 100) { Value = userId ?? string.Empty }
            };
            var raw = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return raw ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Semesters for evaluator + paper combination.
        /// SP: Sp_Eval_Script_Assign_Load_Sem @EvaluatorId, @PapCode
        /// Confirmed from DataAccessLayer.dll string analysis.
        /// </summary>
        public async Task<IEnumerable<object>> GetSemestersAsync(string regulation, string papCode)
        {
            // SP: Sp_Eval_Script_Assign_Load_Sem @Regulation, @PCode
            var sql = StoredProcSql.ExecNamed(StoredProcedures.Sp_Eval_Script_Assign_Load_Sem, "@Regulation", "@PCode");
            var parameters = new[]
            {
                new SqlParameter("@Regulation", SqlDbType.VarChar, 100) { Value = regulation ?? string.Empty },
                new SqlParameter("@PCode",      SqlDbType.VarChar, 100) { Value = papCode    ?? string.Empty }
            };
            var raw = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return raw ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Bundle numbers for a paper + semester.
        /// SP: SP_Eval_Get_BundleNo @PapCode, @Sem
        /// Populates lstBundleNo. Source: Tbl_DV_Marks.
        /// Confirmed from DataAccessLayer.dll string analysis.
        /// </summary>
        public async Task<IEnumerable<object>> GetBundleNumbersAsync(string papCode, string sem)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.SP_Eval_Get_BundleNo, "@PapCode", "@Sem");
            var parameters = new[]
            {
                new SqlParameter("@PapCode", SqlDbType.VarChar, 100) { Value = papCode ?? string.Empty },
                new SqlParameter("@Sem",     SqlDbType.VarChar, 50)  { Value = sem     ?? string.Empty }
            };
            var raw = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return raw ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Unassigned scripts (answer booklets) within a bundle.
        /// SP: SP_Eval_Get_Scripts @PapCode, @Sem, @BundleNo
        /// Returns only scripts where EvaluatorId IS NULL.
        /// Populates lstScripts when a bundle is clicked.
        /// </summary>
        public async Task<IEnumerable<object>> GetScriptsAsync(string papCode, string sem, string bundleNo)
        {
            // SP: SP_Eval_Get_Scripts @ControlBundleNo, @PCode  (no @Sem param)
            var sql = StoredProcSql.Exec(StoredProcedures.SP_Eval_Get_Scripts, "@ControlBundleNo", "@PCode");
            var parameters = new[]
            {
                new SqlParameter("@ControlBundleNo", SqlDbType.VarChar, 100) { Value = bundleNo ?? string.Empty },
                new SqlParameter("@PCode",           SqlDbType.VarChar, 100) { Value = papCode  ?? string.Empty }
            };
            var raw = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return raw ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Assigns scripts to an evaluator and records QP/Key file paths.
        /// SP: SP_EVAL_SAVE_SCRIPTS @EvaluatorId, @PapCode, @Sem, @EvalDate,
        ///                          @BundleNo, @ScriptIds, @QpPath, @KeyPath
        ///
        /// ScriptIds and BundleNos are joined as comma-separated strings (same as old ASPX loop).
        /// Success: "Scripts assign to Evaluator successfully.."
        /// Confirmed from App_Web_xplim0cm.dll + DataAccessLayer.dll.
        /// </summary>
        public async Task<int> SaveScriptsAssignAsync(SaveScriptsAssignRequest req)
        {
            // SP: SP_EVAL_SAVE_SCRIPTS @ScriptNo, @EvaluatorId, @EvaluationDate
            // WHERE Barcode=@ScriptNo — matches one row at a time, so loop per script
            int total = 0;
            if (req.ScriptIds == null || req.ScriptIds.Count == 0) return total;

            foreach (var scriptId in req.ScriptIds)
            {
                var barcode = (scriptId ?? string.Empty).Trim();
                if (string.IsNullOrEmpty(barcode)) continue;

                var sql = StoredProcSql.Exec(
                    StoredProcedures.SP_EVAL_SAVE_SCRIPTS,
                    "@ScriptNo", "@EvaluatorId", "@EvaluationDate");

                var parameters = new[]
                {
                    new SqlParameter("@ScriptNo",       SqlDbType.VarChar, 100) { Value = barcode                       },
                    new SqlParameter("@EvaluatorId",    SqlDbType.VarChar, 100) { Value = req.EvaluatorId ?? string.Empty },
                    new SqlParameter("@EvaluationDate", SqlDbType.VarChar, 50)  { Value = req.EvalDate    ?? string.Empty }
                };

                total += await _repo.ExecuteStoredProcAsync(sql, (object[])parameters);
            }

            return total;
        }
    }
}
