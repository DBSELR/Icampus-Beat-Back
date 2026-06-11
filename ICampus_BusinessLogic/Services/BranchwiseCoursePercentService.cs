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
    public class BranchwiseCoursePercentService : IBranchwiseCoursePercentService
    {
        private readonly IGenericRepository<object> _repo;

        public BranchwiseCoursePercentService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Load Semester dropdown (Page_Load / ddlSemester_SelectedIndexChanged)
        /// Inline SQL: SELECT DISTINCT cast(SEM as varchar(250)) SEM
        ///   FROM tbl_sh WHERE COURSE=@Course AND EXAMMY=@ExamMY ORDER BY SEM
        /// BAL: Branch_Wise_Course_Percentage_LoadSemesters
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
        /// Get Branch-wise Course Percentage data (btnView_Click / btnDownLoad_Click)
        /// isSup=false → sp_PAP_PERCENT with STATUS='BS' (branch/GRP grouping)
        /// isSup=true  → SP_Pap_Percent_Sup (Supply)
        /// Both SPs populate PAP_STAT — SELECT from PAP_STAT, fallback from temp table.
        /// BAL: BranchwiseCoursePercent_and_Chart / BranchwiseCoursePercent_and_Chart_Supply
        /// </summary>
        public async Task<IEnumerable<object>> GetDataAsync(
            string course, string examMY, string regu, string sem, bool isSup, bool isRv)
        {
            var reguNumeric = regu?.Length > 1 && (regu[0] == 'R' || regu[0] == 'r')
                ? regu.Substring(1) : regu ?? string.Empty;
            var regulation = reguNumeric.Length > 0 ? "R" + reguNumeric : regu ?? string.Empty;
            var rvFlag = isRv ? "Y" : "N";

            if (!isSup)
            {
                // sp_PAP_PERCENT: @regulation, @EXAMMY, @COURSE, @UserID, @RV, @STATUS, @Sem, @regsup
                // STATUS='BS' groups by GRP (branch-wise) instead of 'OVERALL'
                var execSql = StoredProcSql.Exec(StoredProcedures.sp_PAP_PERCENT,
                    "@regulation", "@EXAMMY", "@COURSE", "@UserID", "@RV", "@STATUS", "@Sem", "@regsup");

                var execParams = new[]
                {
                    new SqlParameter("@regulation", SqlDbType.VarChar, 10)  { Value = regulation },
                    new SqlParameter("@EXAMMY",     SqlDbType.VarChar, 10)  { Value = examMY    ?? string.Empty },
                    new SqlParameter("@COURSE",     SqlDbType.VarChar, 15)  { Value = course    ?? string.Empty },
                    new SqlParameter("@UserID",     SqlDbType.VarChar, 100) { Value = "DBS" },
                    new SqlParameter("@RV",         SqlDbType.VarChar, 20)  { Value = rvFlag },
                    new SqlParameter("@STATUS",     SqlDbType.VarChar, 20)  { Value = "BS" },
                    new SqlParameter("@Sem",        SqlDbType.VarChar, 3)   { Value = sem       ?? string.Empty },
                    new SqlParameter("@regsup",     SqlDbType.VarChar, 10)  { Value = System.DBNull.Value }
                };

                try { await _repo.ExecuteStoredProcAsync(execSql, (object[])execParams); }
                catch { /* validation failure truncates PAP_STAT — fallback handles it */ }
            }
            else
            {
                // SP_Pap_Percent_Sup: pre-create with correct schema so SP's DROP TABLE works
                // and SELECT INTO column validation passes at compile time
                await _repo.ExecuteStoredProcAsync(
                    "IF OBJECT_ID('[PAP_PERCENT_TEMP_Mani]') IS NOT NULL DROP TABLE [PAP_PERCENT_TEMP_Mani]; " +
                    "CREATE TABLE [PAP_PERCENT_TEMP_Mani] (" +
                    "REGULATION varchar(20), REGU varchar(20), REGNO varchar(30), GRP varchar(20), " +
                    "PNO int, PNAME varchar(150), SEM int, PCODE varchar(30), PTYPE varchar(20), " +
                    "EXAMMY varchar(20), COURSE varchar(20), SEC varchar(20), REGSUP varchar(20), " +
                    "CODE varchar(20), V1RES varchar(10), PAPRES varchar(10), tempcode varchar(30), PMARKS varchar(50))");

                int semInt = int.TryParse(sem, out var s) ? s : 0;

                var execSql = StoredProcSql.Exec(StoredProcedures.SP_Pap_Percent_Sup,
                    "@Course", "@Regulation", "@Exammy", "@Sem", "@RV");

                var execParams = new[]
                {
                    new SqlParameter("@Course",     SqlDbType.VarChar, 15) { Value = course     ?? string.Empty },
                    new SqlParameter("@Regulation", SqlDbType.VarChar, 15) { Value = regulation },
                    new SqlParameter("@Exammy",     SqlDbType.VarChar, 10) { Value = examMY     ?? string.Empty },
                    new SqlParameter("@Sem",        SqlDbType.Int)         { Value = semInt },
                    new SqlParameter("@RV",         SqlDbType.VarChar, 20) { Value = rvFlag }
                };

                try { await _repo.ExecuteStoredProcAsync(execSql, (object[])execParams); }
                catch { /* fallback handles it */ }
            }

            // Both SPs populate PAP_STAT — SELECT from it
            var result = await _repo.QueryFromStoredProcAsync("SELECT * FROM PAP_STAT ORDER BY ORD, PCODE");
            if (result != null && result.Any())
                return result;

            // Fallback: SP validation may have truncated PAP_STAT.
            // Query temp table directly with branch-wise (GRP) grouping.
            var rvCol = isRv ? "PAPRES" : "V1RES";
            var tempTable = isSup ? "[PAP_PERCENT_TEMP_Mani]" : "[PAP_PERCENT_TEMP_DBS]";
            var fallbackSql = $@"
IF OBJECT_ID('{tempTable}') IS NOT NULL
SELECT
    REGULATION, SEM, PCODE, PTYPE, EXAMMY, COURSE, PNAME, REGSUP, TEMPCODE, GRP,
    COUNT(*) AS REGD,
    SUM(CASE WHEN CODE IS NULL OR CODE = '' OR CODE = 'MP' THEN 1 ELSE 0 END) AS APPEARED,
    SUM(CASE WHEN {rvCol} = 'P' AND (CODE IS NULL OR CODE = '' OR CODE = 'MP') THEN 1 ELSE 0 END) AS PASSED,
    SUM(CASE WHEN CODE IS NULL OR CODE = '' OR CODE = 'MP' THEN 1 ELSE 0 END)
        - SUM(CASE WHEN {rvCol} = 'P' AND (CODE IS NULL OR CODE = '' OR CODE = 'MP') THEN 1 ELSE 0 END) AS FAILED,
    CASE WHEN SUM(CASE WHEN CODE IS NULL OR CODE = '' OR CODE = 'MP' THEN 1 ELSE 0 END) = 0 THEN 0
         ELSE ROUND(SUM(CASE WHEN {rvCol} = 'P' AND (CODE IS NULL OR CODE = '' OR CODE = 'MP') THEN 1 ELSE 0 END) * 100.0
              / SUM(CASE WHEN CODE IS NULL OR CODE = '' OR CODE = 'MP' THEN 1 ELSE 0 END), 2)
    END AS PER,
    NULL AS ORD
FROM {tempTable}
GROUP BY SEM, PCODE, PTYPE, EXAMMY, COURSE, PNAME, REGSUP, REGULATION, TEMPCODE, GRP
ORDER BY GRP, PCODE";

            var fallback = await _repo.QueryFromStoredProcAsync(fallbackSql);
            return fallback ?? Enumerable.Empty<object>();
        }
    }
}
