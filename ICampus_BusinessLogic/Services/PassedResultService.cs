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
    public class PassedResultService : IPassedResultService
    {
        private readonly IGenericRepository<object> _repo;

        public PassedResultService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Load Semester dropdown (Page_Load)
        /// Inline SQL: SELECT DISTINCT CAST(SEM AS VARCHAR(250)) SEM
        ///   FROM tbl_sh WHERE COURSE=@Course AND Exammy=@ExamMY AND Regulation=@Regu ORDER BY SEM
        /// Confirmed: DataAccessLayer.dll UTF-16LE US heap offset 158863
        /// </summary>
        public async Task<IEnumerable<object>> LoadSemsAsync(string course, string examMY, string regu)
        {
            var sql = "SELECT DISTINCT CAST(SEM AS VARCHAR(250)) SEM " +
                      "FROM tbl_sh WHERE COURSE=@Course AND Exammy=@ExamMY AND Regulation=@Regu ORDER BY SEM";

            var parameters = new[]
            {
                new SqlParameter("@Course",     SqlDbType.VarChar, 30) { Value = course ?? string.Empty },
                new SqlParameter("@ExamMY",     SqlDbType.VarChar, 20) { Value = examMY ?? string.Empty },
                new SqlParameter("@Regu",       SqlDbType.VarChar, 10) { Value = regu   ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Get Passed Result list (ddlsem_SelectedIndexChanged / btnDownLoad_Click)
        /// SP: SP_PASSEDLIST_NEW (@regulation, @EXAMMY, @COURSE) — 3 params, no @Sem
        /// SP is process-only: populates TBL_PASSED_LIST (permanent table).
        /// Then SELECT from TBL_PASSED_LIST filtered by SEM.
        /// Confirmed: DataAccessLayer.dll UTF-16LE US heap offset 158822
        /// BAL: PassedResult (DataAccessLayer.dll ASCII offset 115765)
        /// </summary>
        public async Task<IEnumerable<object>> GetDataAsync(
            string course, string examMY, string regu, string sem)
        {
            // SP_PASSEDLIST_NEW: @regulation, @EXAMMY, @COURSE (no @Sem — populates all sems).
            // SP calls GET_CGPA (heavy TVF). Skip SP if TBL_PASSED_LIST already has data
            // for this exam+course — acts as a cache, making repeat calls instant.
            //
            // Two-step approach to avoid SQL Server compile-time "Invalid object name" when
            // TBL_PASSED_LIST doesn't exist yet: references to it inside sp_executesql strings
            // are not resolved at compile time of the outer batch.
            var spParams = new[]
            {
                new SqlParameter("@regulation", SqlDbType.VarChar, 20) { Value = regu   ?? string.Empty },
                new SqlParameter("@EXAMMY",     SqlDbType.VarChar, 25) { Value = examMY ?? string.Empty },
                new SqlParameter("@COURSE",     SqlDbType.VarChar, 20) { Value = course ?? string.Empty }
            };

            // Step 1: conditionally run the SP.
            // TBL_PASSED_LIST reference inside N'...' string → no compile-time resolution issue.
            var condSql =
                "DECLARE @hasData BIT = 0; " +
                "IF OBJECT_ID('TBL_PASSED_LIST') IS NOT NULL " +
                "BEGIN " +
                "    EXEC sp_executesql " +
                "        N'SELECT @h = CASE WHEN EXISTS(SELECT 1 FROM TBL_PASSED_LIST WHERE EXAMMY=@E AND COURSE=@C) THEN 1 ELSE 0 END', " +
                "        N'@h BIT OUTPUT, @E VARCHAR(25), @C VARCHAR(20)', " +
                "        @hasData OUTPUT, @EXAMMY, @COURSE; " +
                "END; " +
                "IF @hasData = 0 " +
                    StoredProcSql.Exec(StoredProcedures.SP_PASSEDLIST_NEW,
                        "@regulation", "@EXAMMY", "@COURSE") + ";";

            await _repo.ExecuteStoredProcAsync(condSql, (object[])spParams);

            // Step 2: SELECT from TBL_PASSED_LIST (guaranteed to exist now).
            var semFilter = string.IsNullOrWhiteSpace(sem) ? "" : " WHERE SEM=@Sem";
            var selectSql = "SELECT * FROM TBL_PASSED_LIST" + semFilter + " ORDER BY SEM, BRANCH, REGNO";

            var selectParams = new System.Collections.Generic.List<SqlParameter>();
            if (!string.IsNullOrWhiteSpace(sem))
                selectParams.Add(new SqlParameter("@Sem", SqlDbType.VarChar, 5) { Value = sem });

            var result = await _repo.QueryFromStoredProcAsync(selectSql, selectParams.Cast<object>().ToArray());
            return result ?? Enumerable.Empty<object>();
        }
    }
}
