using ICampus_BusinessLogic.Interfaces;
using ICampus_DataAccessLayer.Helpers;
using ICampus_DataAccessLayer.Interfaces;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Services
{
    public class PreModerationService : IPreModerationService
    {
        private readonly IGenericRepository<object> _repo;

        public PreModerationService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Load Semester dropdown (Page_Load / ddlsem_SelectedIndexChanged)
        /// Inline SQL: SELECT DISTINCT SEM FROM TBL_SH
        ///   WHERE REGULATION=@Regulation AND EXAMMY=@ExamMY AND COURSE=@Course ORDER BY SEM
        /// </summary>
        public async Task<IEnumerable<object>> LoadSemsAsync(string course, string examMY, string regulation)
        {
            var sql = "SELECT DISTINCT SEM FROM TBL_SH " +
                      "WHERE REGULATION=@Regulation AND EXAMMY=@ExamMY AND COURSE=@Course " +
                      "ORDER BY SEM";

            var parameters = new[]
            {
                new SqlParameter("@Regulation", SqlDbType.VarChar, 10) { Value = regulation ?? string.Empty },
                new SqlParameter("@ExamMY",     SqlDbType.VarChar, 20) { Value = examMY     ?? string.Empty },
                new SqlParameter("@Course",     SqlDbType.VarChar, 30) { Value = course     ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Get PreModeration data for all branches and papers in a semester (btnExport_Click)
        /// SP: PROC_MODERATION_REG_SEM_GRP_PAP with @Grp='' and @PapCode='' returns all
        /// </summary>
        public async Task<IEnumerable<object>> GetDataAsync(string course, string examMY, string regu, string sem)
        {
            int semInt = int.TryParse(sem, out var s) ? s : 0;

            // Step 1: Run PROC_MODERATION_NEW — calculates moderation marks and populates tbl_P%
            var execSql = StoredProcSql.Exec(StoredProcedures.proc_moderation_new,
                "@EXAMMY", "@COURSE", "@SEMESTER");

            var execParams = new[]
            {
                new SqlParameter("@EXAMMY",   SqlDbType.VarChar, 10) { Value = examMY ?? string.Empty },
                new SqlParameter("@COURSE",   SqlDbType.VarChar, 10) { Value = course ?? string.Empty },
                new SqlParameter("@SEMESTER", SqlDbType.Int)         { Value = semInt }
            };

            await _repo.ExecuteStoredProcAsync(execSql, (object[])execParams);

            // Step 2: SELECT the populated data from tbl_P%
            var selectSql = "SELECT * FROM [tbl_P%] ORDER BY PCODE";
            var result = await _repo.QueryFromStoredProcAsync(selectSql);
            return result ?? Enumerable.Empty<object>();
        }
    }
}
