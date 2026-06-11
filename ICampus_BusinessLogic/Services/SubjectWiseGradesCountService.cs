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
    public class SubjectWiseGradesCountService : ISubjectWiseGradesCountService
    {
        private readonly IGenericRepository<object> _repo;

        public SubjectWiseGradesCountService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Load Batch dropdown — ddlbatch (Page_Load)
        /// Same pattern as AwardofClassService.LoadBatchesAsync
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
        /// Load Semester dropdown — ddlSemester (Page_Load, AutoPostBack)
        /// ddlSemester_SelectedIndexChanged triggers auto-reload of Crystal Report
        /// SQL: SELECT DISTINCT cast(SEM as varchar(250)) SEM FROM tbl_sh
        ///      WHERE COURSE=@Course AND EXAMMY=@ExamMY ORDER BY SEM
        /// Same pattern as AwardofClassService.LoadSemsAsync
        /// </summary>
        public async Task<IEnumerable<object>> LoadSemsAsync(string course, string examMY)
        {
            var sql = "SELECT DISTINCT cast(SEM as varchar(250)) SEM FROM tbl_sh" +
                      " WHERE COURSE=@Course AND EXAMMY=@ExamMY ORDER BY SEM";

            var parameters = new[]
            {
                new SqlParameter("@Course", SqlDbType.VarChar, 30) { Value = course ?? string.Empty },
                new SqlParameter("@ExamMY", SqlDbType.VarChar, 20) { Value = examMY ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Get Results Grade Analysis Crystal Report data (btnview_Click)
        /// SP: PROC_GRADE_CNT  params: @Course, @ExamMY, @Regu, @Sem
        /// Crystal Report: ResGradesecwise.rpt
        /// Note: ChkIsreadmitresult only changes the Crystal Report title — same SP called
        /// Confirmed: DataAccessLayer.dll UTF-16LE US heap offset 162245
        /// </summary>
        public async Task<IEnumerable<object>> GetViewDataAsync(
            string course, string examMY, string regu, string sem)
        {
            // SP param order: @REGULATION varchar, @EXAMMY, @COURSE, @SEM INT
            var regulation = "R" + (regu ?? string.Empty);

            var sql = StoredProcSql.Exec(StoredProcedures.PROC_GRADE_CNT,
                "@Regulation", "@ExamMY", "@Course", "@Sem");

            var parameters = new[]
            {
                new SqlParameter("@Regulation", SqlDbType.VarChar, 20) { Value = regulation },
                new SqlParameter("@ExamMY",     SqlDbType.VarChar, 20) { Value = examMY ?? string.Empty },
                new SqlParameter("@Course",     SqlDbType.VarChar, 30) { Value = course  ?? string.Empty },
                new SqlParameter("@Sem",        SqlDbType.Int)         { Value = int.TryParse(sem, out var s1) ? s1 : (object)DBNull.Value }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Get Results Grade Analysis Excel export data (btnexcel_Click, PostBackTrigger)
        /// SP: PROC_GRADE_CNT_EXCEL  params: @Course, @ExamMY, @Regu, @Sem
        /// Confirmed: App_Web_gp3pforx.dll UTF-16LE offset 191010
        /// </summary>
        public async Task<IEnumerable<object>> GetExcelDataAsync(
            string course, string examMY, string regu, string sem)
        {
            // SP param order: @REGULATION varchar, @EXAMMY, @COURSE, @SEM INT
            var regulation = "R" + (regu ?? string.Empty);

            var sql = StoredProcSql.Exec(StoredProcedures.PROC_GRADE_CNT_EXCEL,
                "@Regulation", "@ExamMY", "@Course", "@Sem");

            var parameters = new[]
            {
                new SqlParameter("@Regulation", SqlDbType.VarChar, 20) { Value = regulation },
                new SqlParameter("@ExamMY",     SqlDbType.VarChar, 20) { Value = examMY ?? string.Empty },
                new SqlParameter("@Course",     SqlDbType.VarChar, 30) { Value = course  ?? string.Empty },
                new SqlParameter("@Sem",        SqlDbType.Int)         { Value = int.TryParse(sem, out var s2) ? s2 : (object)DBNull.Value }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }
    }
}
