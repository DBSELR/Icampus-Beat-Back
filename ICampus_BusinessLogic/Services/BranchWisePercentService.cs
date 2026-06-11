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
    public class BranchWisePercentService : IBranchWisePercentService
    {
        private readonly IGenericRepository<object> _repo;

        public BranchWisePercentService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Load Semester dropdown (Page_Load)
        /// Inline SQL: SELECT DISTINCT cast(SEM as varchar(250)) SEM
        ///   FROM tbl_sh WHERE COURSE=@Course AND EXAMMY=@ExamMY ORDER BY SEM
        /// Confirmed: DataAccessLayer.dll UTF-16LE US heap offset 159480
        /// </summary>
        public async Task<IEnumerable<object>> LoadSemsAsync(string course, string examMY)
        {
            var sql = "SELECT DISTINCT cast(SEM as varchar(250)) SEM " +
                      "FROM tbl_sh WHERE COURSE=@Course AND EXAMMY=@ExamMY ORDER BY SEM";

            var parameters = new[]
            {
                new SqlParameter("@Course", SqlDbType.VarChar, 30) { Value = course ?? string.Empty },
                new SqlParameter("@ExamMY", SqlDbType.VarChar, 20) { Value = examMY ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Get Branch-wise Pass Percentage data (btnView_Click / btnDownLoad_Click)
        /// SP: sp_COURSE_STAT (@REGULATION, @EXAMMY, @COURSE, @SECWISE='N', @RV, @REGSUP, @Sem)
        /// SP starts with DROP TABLE [COU_STAT_USERID1] without IF EXISTS — pre-create with schema.
        /// SP returns SELECT * FROM COU_STAT — use execute then SELECT pattern for reliability.
        /// BAL: BRANCHWISE_PERCENT_Chart
        /// </summary>
        public async Task<IEnumerable<object>> GetDataAsync(
            string course, string examMY, string regu, string sem, bool isRv, bool isSup)
        {
            var regulation = regu ?? string.Empty;
            var rvFlag  = isRv  ? "Y" : "N";
            var regsup  = isSup ? "SUP" : "REG";

            // sp_COURSE_STAT starts with DROP TABLE [COU_STAT_USERID1] without IF EXISTS.
            // Combine pre-create + SP EXEC into one batch so there is only one connection
            // operation, avoiding EF Core connection state issues between sequential calls.
            var batchSql =
                "IF OBJECT_ID('[COU_STAT_USERID1]') IS NOT NULL DROP TABLE [COU_STAT_USERID1]; " +
                "CREATE TABLE [COU_STAT_USERID1] (" +
                "REGNO varchar(30), SEC varchar(20), COURSE varchar(20), EXAMMY varchar(20), " +
                "SEM varchar(10), GRP varchar(20), REGD varchar(5), CODE varchar(20), " +
                "TMARKS varchar(50), REGSUP varchar(20), RESULT varchar(10), REGULATION varchar(20), pmarks varchar(50)); " +
                StoredProcSql.Exec(StoredProcedures.sp_COURSE_STAT,
                    "@REGULATION", "@EXAMMY", "@COURSE", "@SECWISE", "@RV", "@REGSUP", "@Sem");

            var execParams = new[]
            {
                new SqlParameter("@REGULATION", SqlDbType.VarChar, 10) { Value = regulation },
                new SqlParameter("@EXAMMY",     SqlDbType.VarChar, 10) { Value = examMY ?? string.Empty },
                new SqlParameter("@COURSE",     SqlDbType.VarChar, 20) { Value = course ?? string.Empty },
                new SqlParameter("@SECWISE",    SqlDbType.Char,    1)  { Value = "N" },
                new SqlParameter("@RV",         SqlDbType.VarChar, 5)  { Value = rvFlag },
                new SqlParameter("@REGSUP",     SqlDbType.VarChar, 10) { Value = regsup },
                new SqlParameter("@Sem",        SqlDbType.VarChar, 10) { Value = sem ?? string.Empty }
            };

            // QueryFromStoredProcAsync reads the first result set (the SP's final SELECT * FROM COU_STAT)
            IEnumerable<object> spResult = null;
            try { spResult = await _repo.QueryFromStoredProcAsync(batchSql, (object[])execParams); }
            catch { /* fallback handles it */ }

            // SP returns COU_STAT columns (GRP, REGD etc.) — return directly
            if (spResult != null && spResult.Any())
            {
                var first = spResult.First() as System.Collections.Generic.IDictionary<string, object>;
                if (first != null && (first.ContainsKey("GRP") || first.ContainsKey("REGD")))
                    return spResult;
            }

            // SP validation truncated COU_STAT — SELECT from it
            var result = await _repo.QueryFromStoredProcAsync(
                "SELECT *, CASE WHEN REGD = 0 THEN 0 ELSE ROUND(PASSED * 100.0 / CAST(REGD AS FLOAT), 2) END AS PER FROM COU_STAT ORDER BY SEM, GRP");
            if (result != null && result.Any())
                return result;

            // Fallback: aggregate from COU_STAT_USERID1 if COU_STAT was truncated
            var fallbackSql = @"
IF OBJECT_ID('[COU_STAT_USERID1]') IS NOT NULL
SELECT
    SEM, SEC, COURSE, GRP, EXAMMY, REGSUP,
    COUNT(DISTINCT REGNO) AS REGD,
    COUNT(DISTINCT CASE WHEN CODE IS NULL OR CODE = '' THEN REGNO END) AS APPEARED,
    COUNT(DISTINCT CASE WHEN RESULT = 'P' THEN REGNO END) AS PASSED,
    COUNT(DISTINCT REGNO)
        - COUNT(DISTINCT CASE WHEN RESULT = 'P' THEN REGNO END) AS FAILED,
    CASE WHEN COUNT(DISTINCT CASE WHEN CODE IS NULL OR CODE = '' THEN REGNO END) = 0 THEN 0
         ELSE ROUND(COUNT(DISTINCT CASE WHEN RESULT = 'P' THEN REGNO END) * 100.0
              / COUNT(DISTINCT CASE WHEN CODE IS NULL OR CODE = '' THEN REGNO END), 2)
    END AS PER,
    NULL AS REGULATION
FROM [COU_STAT_USERID1]
GROUP BY SEM, SEC, COURSE, GRP, EXAMMY, REGSUP
ORDER BY SEM, GRP";

            var fallback = await _repo.QueryFromStoredProcAsync(fallbackSql);
            return fallback ?? Enumerable.Empty<object>();
        }
    }
}
