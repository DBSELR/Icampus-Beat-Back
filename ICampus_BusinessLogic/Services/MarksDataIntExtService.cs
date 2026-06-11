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
    public class MarksDataIntExtService : IMarksDataIntExtService
    {
        private readonly IGenericRepository<object> _repo;

        public MarksDataIntExtService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Load Batch/REGU dropdown (ddlbatch — Page_Load)
        /// SQL: SELECT DISTINCT REGU FROM TBL_COURSE ORDER BY REGU
        /// BAL: Bal_Reports_Source → Load_Regu
        /// </summary>
        public async Task<IEnumerable<object>> LoadBatchAsync()
        {
            var sql = "SELECT DISTINCT REGU FROM TBL_COURSE ORDER BY REGU";
            var result = await _repo.QueryFromStoredProcAsync(sql);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Load ExamMY dropdown (cmbExamMY — cascades from Batch)
        /// SQL: SELECT DISTINCT EXAMMY FROM TBL_SH WHERE REGU=@Regu ORDER BY EXAMMY DESC
        /// Pass regu='' to get all exam months regardless of batch
        /// BAL: Bal_Reports_Source → loadExammy
        /// </summary>
        public async Task<IEnumerable<object>> LoadExammyAsync(string regu)
        {
            string sql;
            IEnumerable<object> result;

            if (string.IsNullOrWhiteSpace(regu))
            {
                sql = "SELECT DISTINCT EXAMMY FROM TBL_SH ORDER BY EXAMMY DESC";
                result = await _repo.QueryFromStoredProcAsync(sql);
            }
            else
            {
                sql = "SELECT DISTINCT EXAMMY FROM TBL_SH WHERE REGU=@Regu ORDER BY EXAMMY DESC";
                var p = new SqlParameter("@Regu", SqlDbType.VarChar, 10) { Value = regu };
                result = await _repo.QueryFromStoredProcAsync(sql, (object[])new[] { p });
            }

            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Load Semester dropdown (cmbSemester — cascades from Batch + ExamMY)
        /// SQL: SELECT DISTINCT CAST(SEM AS VARCHAR) SEM FROM TBL_SH
        ///      WHERE REGU=@Regu AND EXAMMY=@ExamMY ORDER BY SEM
        /// BAL: Bal_Reports_Source → cmbSemester_SelectedIndexChanged
        /// </summary>
        public async Task<IEnumerable<object>> LoadSemestersAsync(string regu, string examMY)
        {
            var sql = "SELECT DISTINCT CAST(SEM AS VARCHAR) SEM FROM TBL_SH " +
                      "WHERE REGU=@Regu AND EXAMMY=@ExamMY ORDER BY SEM";

            var parameters = new[]
            {
                new SqlParameter("@Regu",   SqlDbType.VarChar, 10)  { Value = regu   ?? string.Empty },
                new SqlParameter("@ExamMY", SqlDbType.VarChar, 20)  { Value = examMY ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Get marks data for university formats 1–3 and 5 (btnView_Click → loadexcel)
        /// SP: PROC_EXPORT_MARKSDATA (@EXAMMY, @REGULATION, @COURSE, @SEM INT, @REGU)
        /// Confirmed: EXEC PROC_EXPORT_MARKSDATA 'May-2024','R20','B.TECH',8,'20' → rows
        /// </summary>
        public async Task<IEnumerable<object>> GetShDataAsync(string regulation, string course, string regu, string examMY, string sem)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.PROC_EXPORT_MARKSDATA,
                "@EXAMMY", "@REGULATION", "@COURSE", "@SEM", "@REGU");

            var parameters = new[]
            {
                new SqlParameter("@EXAMMY",     SqlDbType.VarChar, 20) { Value = examMY     ?? string.Empty },
                new SqlParameter("@REGULATION", SqlDbType.VarChar, 20) { Value = regulation ?? string.Empty },
                new SqlParameter("@COURSE",     SqlDbType.VarChar, 20) { Value = course     ?? string.Empty },
                new SqlParameter("@SEM",        SqlDbType.Int)          { Value = int.TryParse(sem, out var s1) ? s1 : 0 },
                new SqlParameter("@REGU",       SqlDbType.VarChar, 20) { Value = regu       ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Get result marks data — Format 4: V1, RV, V3 Month &amp; Year Wise (rbtnfinalmarks)
        /// SP: PROC_EXPORT_RES_DATA (@regulation, @course, @exammy, @sem INT)
        /// Confirmed: EXEC PROC_EXPORT_RES_DATA 'R20','B.TECH','May-2024',8 → rows
        /// </summary>
        public async Task<IEnumerable<object>> GetResultDataAsync(string regulation, string course, string examMY, string sem)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.PROC_EXPORT_RES_DATA,
                "@regulation", "@course", "@exammy", "@sem");

            var parameters = new[]
            {
                new SqlParameter("@regulation", SqlDbType.VarChar, 20) { Value = regulation ?? string.Empty },
                new SqlParameter("@course",     SqlDbType.VarChar, 20) { Value = course     ?? string.Empty },
                new SqlParameter("@exammy",     SqlDbType.VarChar, 20) { Value = examMY     ?? string.Empty },
                new SqlParameter("@sem",        SqlDbType.Int)          { Value = int.TryParse(sem, out var s2) ? s2 : 0 }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }
    }
}
