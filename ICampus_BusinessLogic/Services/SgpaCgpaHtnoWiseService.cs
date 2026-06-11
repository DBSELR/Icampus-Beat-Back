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
    public class SgpaCgpaHtnoWiseService : ISgpaCgpaHtnoWiseService
    {
        private readonly IGenericRepository<object> _repo;

        public SgpaCgpaHtnoWiseService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Load Batch (Regulation) dropdown — ddlBatch AutoPostBack
        /// SQL: SELECT DISTINCT CAST(REGU AS VARCHAR) AS REGU,
        ///      '20'+CAST(REGU AS VARCHAR)+'-'+CAST(REGU+MAXSEM/2 AS VARCHAR) BATCH
        ///      FROM TBL_COURSE WHERE COURSE=@Course ORDER BY REGU
        /// Confirmed: DataAccessLayer.dll UTF-16LE US heap (same pattern as Tabulation Register)
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
        /// Load Branch dropdown — ddlBranch (loaded on ddlBatch_SelectedIndexChanged)
        /// SQL: SELECT DISTINCT GRP FROM tbl_SH WHERE COURSE=@Course Order by GRP
        /// Note: uses tbl_SH (not TBL_COURSE) — branch list from actual marks records.
        /// Confirmed: DataAccessLayer.dll UTF-16LE US heap offset 160214
        /// </summary>
        public async Task<IEnumerable<object>> LoadBranchesAsync(string course)
        {
            var sql = "SELECT DISTINCT GRP FROM tbl_SH WHERE COURSE=@Course Order by GRP";

            var parameters = new[]
            {
                new SqlParameter("@Course", SqlDbType.VarChar, 30) { Value = course ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Load ExamMY dropdown — cmbExamMY
        /// SQL: SELECT DISTINCT EXAMMY, AEXAMID FROM TBL_EXAMS
        ///      WHERE COURSE=@Course and REGULATION=@Regu ORDER BY AEXAMID DESC
        /// Confirmed: DataAccessLayer.dll UTF-16LE US heap offset 158384
        /// </summary>
        public async Task<IEnumerable<object>> LoadExamMYsAsync(string course, string regu)
        {
            var sql = "SELECT DISTINCT EXAMMY, AEXAMID FROM TBL_EXAMS" +
                      " WHERE COURSE=@Course AND REGULATION='R'+@Regu ORDER BY AEXAMID DESC";

            var parameters = new[]
            {
                new SqlParameter("@Course", SqlDbType.VarChar, 30) { Value = course ?? string.Empty },
                new SqlParameter("@Regu",   SqlDbType.VarChar, 10) { Value = regu   ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Get SGPA &amp; CGPA H.T.No. wise data (btnView_Click / btnDownLoad_Click)
        /// SP: PROC_SGPA_Report
        ///   params (6): @REGULATION varchar(20), @Course VARCHAR(20), @BRANCH VARCHAR(20),
        ///               @REGU VARCHAR(20), @EXAMMY VARCHAR(20), @RV VARCHAR(20)
        ///   @REGULATION = 'R' + regu (e.g. 'R20'), @RV = 'Y' (withRv) or 'N' (without)
        /// Crystal Report: SGPA.rpt
        ///   Subtitle: "(With Revaluation)" when withRv=true, "(Without Revaluation)" when false
        /// Confirmed: PROC_SGPA_Report returns 959 rows for B.TECH/CSE/R20/May-2024
        /// </summary>
        public async Task<IEnumerable<object>> GetDataAsync(
            string course, string examMY, string regu, string branch, bool withRv)
        {
            var regulation = "R" + (regu ?? string.Empty);
            var rv = withRv ? "Y" : "N";

            var sql = StoredProcSql.Exec(StoredProcedures.PROC_SGPA_Report,
                "@Regulation", "@Course", "@Branch", "@Regu", "@ExamMY", "@RV");

            var parameters = new[]
            {
                new SqlParameter("@Regulation", SqlDbType.VarChar, 20) { Value = regulation },
                new SqlParameter("@Course",     SqlDbType.VarChar, 20) { Value = course ?? string.Empty },
                new SqlParameter("@Branch",     SqlDbType.VarChar, 20) { Value = branch ?? string.Empty },
                new SqlParameter("@Regu",       SqlDbType.VarChar, 20) { Value = regu   ?? string.Empty },
                new SqlParameter("@ExamMY",     SqlDbType.VarChar, 20) { Value = examMY ?? string.Empty },
                new SqlParameter("@RV",         SqlDbType.VarChar, 20) { Value = rv }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }
    }
}
