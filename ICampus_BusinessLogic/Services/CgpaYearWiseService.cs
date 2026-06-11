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
    public class CgpaYearWiseService : ICgpaYearWiseService
    {
        private readonly IGenericRepository<object> _repo;

        public CgpaYearWiseService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Load ExamMY dropdown (ddlexammy — Page_Load)
        /// SP: SPM_EXAMS_ExamMY_Load (@Course)
        /// BAL: loadExammy (BAL_CGPA_Yearwise)
        /// </summary>
        public async Task<IEnumerable<object>> LoadExammyAsync(string course, string regulation)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.SPM_EXAMS_ExamMY_Load, "@Regulation", "@Course");
            var parameters = new[]
            {
                new SqlParameter("@Regulation", SqlDbType.VarChar, 20) { Value = regulation ?? string.Empty },
                new SqlParameter("@Course",     SqlDbType.VarChar, 20) { Value = course     ?? string.Empty }
            };
            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Load Batch dropdown (ddlbatch — cascades from ExamMY)
        /// SQL: SELECT DISTINCT REGU FROM TBL_SH WHERE EXAMMY=@ExamMY ORDER BY REGU
        /// BAL: loadbatch (BAL_CGPA_Yearwise)
        /// </summary>
        public async Task<IEnumerable<object>> LoadBatchAsync(string examMY)
        {
            var sql = "SELECT DISTINCT REGU FROM TBL_SH WHERE EXAMMY=@ExamMY ORDER BY REGU";
            var p = new SqlParameter("@ExamMY", SqlDbType.VarChar, 20) { Value = examMY ?? string.Empty };
            var result = await _repo.QueryFromStoredProcAsync(sql, p);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Load Semester dropdown (ddlsemester — cascades from ExamMY + Batch)
        /// SQL: SELECT DISTINCT CAST(SEM AS VARCHAR) SEM FROM TBL_SH
        ///      WHERE EXAMMY=@ExamMY AND REGU=@Batch ORDER BY SEM
        /// BAL: LoadSem (BAL_CGPA_Yearwise)
        /// </summary>
        public async Task<IEnumerable<object>> LoadSemestersAsync(string examMY, string batch)
        {
            var sql = "SELECT DISTINCT CAST(SEM AS VARCHAR) SEM FROM TBL_SH " +
                      "WHERE EXAMMY=@ExamMY AND REGU=@Batch ORDER BY SEM";

            var parameters = new[]
            {
                new SqlParameter("@ExamMY", SqlDbType.VarChar, 20) { Value = examMY ?? string.Empty },
                new SqlParameter("@Batch",  SqlDbType.VarChar, 10) { Value = batch  ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Excel download — CGPA Year Wise (btnDownLoad_Click)
        /// SP: sp_cgpa_excel (@ExamMY, @Batch)
        /// 2 params confirmed: App_Web_m2jhophz.dll "sp_cgpa_excel '','  CGPA_YearWise"
        /// </summary>
        public async Task<IEnumerable<object>> DownloadAsync(
            string regulation, string course, string examMY, string regu, string sem)
        {
            var reguNumeric = regu?.Length > 1 && (regu[0] == 'R' || regu[0] == 'r')
                ? regu.Substring(1) : regu ?? string.Empty;
            var regulationFmt = reguNumeric.Length > 0 ? "R" + reguNumeric : regulation ?? string.Empty;

            int semInt = int.TryParse(sem, out var s) ? s : 0;

            var sql = StoredProcSql.Exec(StoredProcedures.sp_cgpa_excel,
                "@REGULATION", "@COURSE", "@EXAMMY", "@REGU", "@SEM");

            var parameters = new[]
            {
                new SqlParameter("@REGULATION", SqlDbType.VarChar, 20) { Value = regulationFmt },
                new SqlParameter("@COURSE",     SqlDbType.VarChar, 20) { Value = course   ?? string.Empty },
                new SqlParameter("@EXAMMY",     SqlDbType.VarChar, 20) { Value = examMY   ?? string.Empty },
                new SqlParameter("@REGU",       SqlDbType.VarChar, 20) { Value = reguNumeric },
                new SqlParameter("@SEM",        SqlDbType.Int)         { Value = semInt }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }
    }
}
