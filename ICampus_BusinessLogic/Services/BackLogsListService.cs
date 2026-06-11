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
    public class BackLogsListService : IBackLogsListService
    {
        private readonly IGenericRepository<object> _repo;

        public BackLogsListService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        // ── BackLogsList.aspx ──────────────────────────────────────────────

        /// <summary>
        /// Load ExamMY dropdown
        /// SP: SPM_EXAMS_ExamMY_Load
        /// params: @Regulation (varchar), @Course (varchar)
        /// Confirmed: already used by DropdownService, CancelReceiptListService, etc.
        ///   BAL method: PageLoad → ddlexammy.DataSource = DAL.Get_ExamMY(Regulation, Course)
        /// </summary>
        public async Task<IEnumerable<object>> LoadExamMYAsync(string regulation, string course)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.SPM_EXAMS_ExamMY_Load,
                "@Regulation", "@Course");

            var parameters = new[]
            {
                new SqlParameter("@Regulation", SqlDbType.VarChar, 10) { Value = regulation ?? string.Empty },
                new SqlParameter("@Course",     SqlDbType.VarChar, 20) { Value = course     ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Load Batch dropdown — raw SQL on TBL_COURSE
        /// SQL: SELECT DISTINCT REGU,'20'+REGU+'-'+CAST(REGU+MAXSEM/2 AS VARCHAR) BATCH
        ///      FROM TBL_COURSE WHERE COURSE = @Course ORDER BY REGU
        /// params: @Course (varchar)
        /// Confirmed: DataAccessLayer.dll UTF-16LE fragment before SP_BACKLOGS_LIST offset 119682
        ///   DDLBatch AutoPostBack → DDLBatch_SelectedIndexChanged (reloads SemFrom/SemTo hints)
        /// </summary>
        public async Task<IEnumerable<object>> LoadBatchAsync(string course)
        {
            var sql = @"SELECT DISTINCT REGU,
                '20'+REGU+'-'+CAST(REGU + MAXSEM/2 AS VARCHAR) BATCH
                FROM TBL_COURSE WHERE COURSE = @Course ORDER BY REGU";

            var parameters = new[]
            {
                new SqlParameter("@Course", SqlDbType.VarChar, 20) { Value = course ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Get BackLogs list with filters
        /// SP: SP_BACKLOGS_LIST
        /// params (all varchar): @Course, @REGU, @ExamMY, @SemFrom, @SemTo, @NoOfBackLogs, @Op
        /// Returns: REGNO, SNAME, SECTION, GRP, SEM, PCODE, PNAME, LAST ATTEMPT
        /// Confirmed: DataAccessLayer.dll UTF-16LE offset 119682
        ///   BAL method: BackLogsList (btnBackLogsList_Click)
        ///   SQL fragments seen in DAL:
        ///     SEM BETWEEN @SemFrom AND @SemTo
        ///     CONVERT(DATE,'01-'+EXAMMY,105) <= CONVERT(DATE,'01-'+@ExamMY,105)
        ///     REGU = @REGU
        ///     no-of-backlogs column @Op @NoOfBackLogs
        ///   Operator (@Op): '=' | '<=' | '>=' — cycled by btnEquation_Click
        ///   CBCurrentMonthYear checkbox: when checked,
        ///     CONVERT(DATE,'01-'+EXAMMY,105) = CONVERT(DATE,'01-'+@ExamMY,105)
        ///     (exact month match instead of <=)
        /// </summary>
        public async Task<IEnumerable<object>> GetBackLogsListAsync(
            string course, string regu, string examMY,
            string semFrom, string semTo,
            string noOfBackLogs, string op)
        {
            // SP_BACKLOGS_LIST uses dynamic SQL: expects @CON (WHERE clause) and @FC (HAVING COUNT condition)
            var safeCourse  = (course  ?? "").Replace("'", "''");
            var safeRegu    = (regu    ?? "").Replace("'", "''");
            var safeExamMY  = (examMY  ?? "").Replace("'", "''");
            var safeSemFrom = int.TryParse(semFrom, out var sf) ? sf.ToString() : "1";
            var safeSemTo   = int.TryParse(semTo,   out var st) ? st.ToString() : "8";
            var safeCount   = int.TryParse(noOfBackLogs, out var nb) ? nb.ToString() : "0";
            var safeOp      = new[] { "=", ">=", "<=" }.Contains(op) ? op : "=";

            var con = $"WHERE UPPER(COURSE) = UPPER('{safeCourse}') " +
                      $"AND REGU = '{safeRegu}' " +
                      $"AND CONVERT(DATE,'01-'+EXAMMY,105) <= CONVERT(DATE,'01-{safeExamMY}',105) " +
                      $"AND SEM BETWEEN {safeSemFrom} AND {safeSemTo}";

            var fc = $"{safeOp} {safeCount}";

            var sql = StoredProcSql.Exec(StoredProcedures.SP_BACKLOGS_LIST, "@CON", "@FC");

            var parameters = new[]
            {
                new SqlParameter("@CON", SqlDbType.VarChar, 1000) { Value = con },
                new SqlParameter("@FC",  SqlDbType.VarChar, 50)   { Value = fc  }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        // ── Backlogs_Regno.aspx ────────────────────────────────────────────

        /// <summary>
        /// Get regno-wise backlogs data
        /// SP: PROC_Backlogs_RegNowise
        /// params (all varchar): @Course, @ExamMY, @REGU
        /// Returns: REGNO, SNAME, SEM, PCODE, PNAME, and related backlog fields
        /// Confirmed: DataAccessLayer.dll UTF-16LE offset 120392 "[PROC_Backlogs_RegNowise]"
        ///   BAL method: Backlogs_Regno (btnBacklosData_Click)
        ///   "Up To ExamMY" → EXAMMY <= @ExamMY filter
        /// </summary>
        public async Task<IEnumerable<object>> GetBackLogsRegnoDataAsync(
            string course, string examMY, string regu, string regulation)
        {
            // SP: PROC_Backlogs_RegNowise(@regulation, @course, @exammy, @regu, @regno=null)
            var sql = StoredProcSql.Exec(StoredProcedures.PROC_Backlogs_RegNowise,
                "@regulation", "@course", "@exammy", "@regu");

            var parameters = new[]
            {
                new SqlParameter("@regulation", SqlDbType.VarChar, 20) { Value = regulation ?? string.Empty },
                new SqlParameter("@course",     SqlDbType.VarChar, 20) { Value = course     ?? string.Empty },
                new SqlParameter("@exammy",     SqlDbType.VarChar, 20) { Value = examMY     ?? string.Empty },
                new SqlParameter("@regu",       SqlDbType.VarChar, 20) { Value = regu       ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Get backlogs count per student
        /// SP: Sp_BacklogsData
        /// params (all varchar): @Course, @ExamMY, @REGU
        /// Returns: REGNO, SNAME, and count/summary of backlog papers
        /// Confirmed: DataAccessLayer.dll UTF-16LE offset 120446 "[Sp_BacklogsData]"
        ///   BAL method: Backlogs_Count (btnBackLogsCount_Click)
        /// </summary>
        public async Task<IEnumerable<object>> GetBackLogsCountAsync(
            string course, string examMY, string regu, string regulation)
        {
            // SP: Sp_BacklogsData(@Regulation, @Course, @Exammy, @Regu)
            var sql = StoredProcSql.Exec(StoredProcedures.Sp_BacklogsData,
                "@Regulation", "@Course", "@Exammy", "@Regu");

            var parameters = new[]
            {
                new SqlParameter("@Regulation", SqlDbType.VarChar, 250) { Value = regulation ?? string.Empty },
                new SqlParameter("@Course",     SqlDbType.VarChar, 50)  { Value = course     ?? string.Empty },
                new SqlParameter("@Exammy",     SqlDbType.VarChar, 100) { Value = examMY     ?? string.Empty },
                new SqlParameter("@Regu",       SqlDbType.VarChar, 50)  { Value = regu       ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }
    }
}
