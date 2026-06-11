using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IBackLogsListService
    {
        // ── BackLogsList.aspx ──────────────────────────────────────────────

        /// <summary>Load ExamMY dropdown (SPM_EXAMS_ExamMY_Load)</summary>
        Task<IEnumerable<object>> LoadExamMYAsync(string regulation, string course);

        /// <summary>
        /// Load Batch dropdown — raw SQL on TBL_COURSE
        /// SQL: SELECT DISTINCT REGU,'20'+REGU+'-'+CAST(REGU+MAXSEM/2 AS VARCHAR) BATCH
        ///      FROM TBL_COURSE WHERE COURSE = @Course ORDER BY REGU
        /// </summary>
        Task<IEnumerable<object>> LoadBatchAsync(string course);

        /// <summary>
        /// Get BackLogs list with filters (SP_BACKLOGS_LIST)
        /// params: @Course, @REGU, @ExamMY, @SemFrom, @SemTo, @NoOfBackLogs, @Op
        /// Op values: '=', '&lt;=', '&gt;='
        /// Returns: REGNO, SNAME, SECTION, GRP, SEM, PCODE, PNAME, LAST ATTEMPT
        /// </summary>
        Task<IEnumerable<object>> GetBackLogsListAsync(
            string course, string regu, string examMY,
            string semFrom, string semTo,
            string noOfBackLogs, string op);

        // ── Backlogs_Regno.aspx ────────────────────────────────────────────

        /// <summary>
        /// Get regno-wise backlogs data (PROC_Backlogs_RegNowise)
        /// params: @Course, @ExamMY, @REGU
        /// Returns: REGNO, SNAME, SEM, PCODE, PNAME, and related backlog fields
        /// </summary>
        Task<IEnumerable<object>> GetBackLogsRegnoDataAsync(string course, string examMY, string regu, string regulation);

        /// <summary>
        /// Get backlogs count per student (Sp_BacklogsData)
        /// params: @Regulation, @Course, @Exammy, @Regu
        /// Returns: REGNO, SNAME, and count/summary fields
        /// </summary>
        Task<IEnumerable<object>> GetBackLogsCountAsync(string course, string examMY, string regu, string regulation);
    }
}
