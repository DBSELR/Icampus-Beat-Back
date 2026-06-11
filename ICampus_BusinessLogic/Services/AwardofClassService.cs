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
    public class AwardofClassService : IAwardofClassService
    {
        private readonly IGenericRepository<object> _repo;

        public AwardofClassService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Load Batch (Regulation) dropdown — ddlbatch (Page_Load)
        /// SQL: SELECT DISTINCT CAST(REGU AS VARCHAR) AS REGU,
        ///      '20'+CAST(REGU AS VARCHAR)+'-'+CAST(REGU+MAXSEM/2 AS VARCHAR) BATCH
        ///      FROM TBL_COURSE WHERE COURSE=@Course ORDER BY REGU
        /// Confirmed: DataAccessLayer.dll UTF-16LE US heap (same pattern as BTECHCMM, Tabulation Register)
        /// </summary>
        public async Task<IEnumerable<object>> LoadBatchesAsync(string course)
        {
            var sql = "SELECT DISTINCT CAST(REGU AS VARCHAR) AS REGU," +
                      "'20'+CAST(REGU AS VARCHAR)+'-'+CAST(REGU+MAXSEM/2 AS VARCHAR) BATCH" +
                      " FROM TBL_COURSE WHERE COURSE=@Course ORDER BY REGU";

            var parameters = new[]
            {
                new SqlParameter("@Course", SqlDbType.VarChar, 30) { Value = course ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Load Semester dropdown — ddlSemester (cmbExamMY_SelectedIndexChanged)
        /// SQL: SELECT DISTINCT cast(SEM as varchar(250)) SEM FROM tbl_sh
        ///      WHERE COURSE=@Course AND EXAMMY=@ExamMY
        /// Triggered by: ExamMY dropdown AutoPostBack
        /// Confirmed: DataAccessLayer.dll UTF-16LE US heap (same pattern at offset 138600)
        /// </summary>
        public async Task<IEnumerable<object>> LoadSemsAsync(string course, string examMY)
        {
            var sql = "SELECT DISTINCT cast(SEM as varchar(250)) SEM FROM tbl_sh" +
                      " WHERE COURSE=@Course AND EXAMMY=@ExamMY";

            var parameters = new[]
            {
                new SqlParameter("@Course", SqlDbType.VarChar, 30) { Value = course ?? string.Empty },
                new SqlParameter("@ExamMY", SqlDbType.VarChar, 20) { Value = examMY ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Get Award of Class Branchwise Crystal Report data (btnView_Click)
        /// SP: PROC_GRADE_CNT  params: @Course, @ExamMY, @Regu, @Sem
        /// Crystal Report: AwardofClassBranchwise.rpt
        /// Subtitle: "RESULTS GRADE ANALYSIS"
        /// ChkIsrv is HIDDEN (display:none) — RV mode disabled on this page.
        /// BAL method: AwardofClassCnt (DataAccessLayer.dll ASCII offset 115859)
        /// Confirmed: DataAccessLayer.dll UTF-16LE US heap offset 162245
        /// </summary>
        public async Task<IEnumerable<object>> GetViewDataAsync(
            string course, string examMY, string regu, string sem)
        {
            // SP param order: @REGULATION, @EXAMMY, @COURSE, @SEM INT
            var regulation = "R" + (regu ?? string.Empty);

            var sql = StoredProcSql.Exec(StoredProcedures.PROC_GRADE_CNT,
                "@Regulation", "@ExamMY", "@Course", "@Sem");

            var parameters = new[]
            {
                new SqlParameter("@Regulation", SqlDbType.VarChar, 20) { Value = regulation },
                new SqlParameter("@ExamMY",     SqlDbType.VarChar, 20) { Value = examMY ?? string.Empty },
                new SqlParameter("@Course",     SqlDbType.VarChar, 20) { Value = course ?? string.Empty },
                new SqlParameter("@Sem",        SqlDbType.Int)         { Value = int.TryParse(sem, out var s1) ? s1 : (object)DBNull.Value }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Get Award of Class Branchwise Excel export data (btnDownLoad_Click)
        /// SP: PROC_GRADE_CNT_EXCEL  params: @Course, @ExamMY, @Regu, @Sem
        /// Excel filename: AwardofClassBranchwise.xlsx
        /// Confirmed: App_Web_gp3pforx.dll UTF-16LE offset 191010
        ///   "RESULTS GRADE ANALYSIS" context + attachment;filename=AwardofClassBranchwise.xlsx
        /// </summary>
        public async Task<IEnumerable<object>> GetExcelDataAsync(
            string course, string examMY, string regu, string sem)
        {
            // SP param order: @REGULATION, @EXAMMY, @COURSE, @SEM INT
            var regulation = "R" + (regu ?? string.Empty);

            var sql = StoredProcSql.Exec(StoredProcedures.PROC_GRADE_CNT_EXCEL,
                "@Regulation", "@ExamMY", "@Course", "@Sem");

            var parameters = new[]
            {
                new SqlParameter("@Regulation", SqlDbType.VarChar, 20) { Value = regulation },
                new SqlParameter("@ExamMY",     SqlDbType.VarChar, 20) { Value = examMY ?? string.Empty },
                new SqlParameter("@Course",     SqlDbType.VarChar, 20) { Value = course ?? string.Empty },
                new SqlParameter("@Sem",        SqlDbType.Int)         { Value = int.TryParse(sem, out var s2) ? s2 : (object)DBNull.Value }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }
    }
}
