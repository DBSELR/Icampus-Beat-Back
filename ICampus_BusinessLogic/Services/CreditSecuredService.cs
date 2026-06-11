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
    public class CreditSecuredService : ICreditSecuredService
    {
        private readonly IGenericRepository<object> _repo;

        public CreditSecuredService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Load ExamMY dropdown (ddlexammy — Page_Load)
        /// SP: SPM_EXAMS_ExamMY_Load (@Course)
        /// BAL: BAL_iCampusMaster_Admin → Get_Exams_ExamMY
        /// </summary>
        public async Task<IEnumerable<object>> LoadExammyAsync(string regulation, string course)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.SPM_EXAMS_ExamMY_Load, "@regulation", "@COURSE");
            var parameters = new[]
            {
                new SqlParameter("@regulation", SqlDbType.VarChar, 10) { Value = regulation ?? string.Empty },
                new SqlParameter("@COURSE",     SqlDbType.VarChar, 20) { Value = course     ?? string.Empty }
            };
            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Load Batch dropdown (DDLBatch — Page_Load)
        /// SQL: SELECT DISTINCT CAST(REGU AS VARCHAR) AS REGU,
        ///      '20'+CAST(REGU AS VARCHAR)+'-'+CAST(REGU+MAXSEM/2 AS VARCHAR) BATCH
        ///      FROM TBL_COURSE WHERE COURSE=@Course ORDER BY REGU
        /// BAL: BAL_iCampusMaster_Admin → Get_Batch
        /// </summary>
        public async Task<IEnumerable<object>> LoadBatchAsync(string course)
        {
            var sql = "SELECT DISTINCT CAST(REGU AS VARCHAR) AS REGU, " +
                      "'20'+CAST(REGU AS VARCHAR)+'-'+CAST(REGU + MAXSEM/2 AS VARCHAR) BATCH " +
                      "FROM TBL_COURSE WHERE COURSE=@Course ORDER BY REGU";

            var p = new SqlParameter("@Course", SqlDbType.VarChar, 20) { Value = course ?? string.Empty };
            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])new[] { p });
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Load Branch dropdown (ddlbranch — cascades from DDLBatch)
        /// SQL: SELECT DISTINCT GRP, GRP+' - '+GSUB BRANCH FROM TBL_COURSE
        ///      WHERE COURSE=@Course AND REGU=@Regu ORDER BY GRP
        /// BAL: Bal_Reports_Results → CreditSecured_Branch
        /// </summary>
        public async Task<IEnumerable<object>> LoadBranchAsync(string course, string regu)
        {
            var sql = "SELECT DISTINCT GRP, GRP+' - '+GSUB BRANCH FROM TBL_COURSE " +
                      "WHERE COURSE=@Course AND REGU=@Regu ORDER BY GRP";

            var parameters = new[]
            {
                new SqlParameter("@Course", SqlDbType.VarChar, 20) { Value = course ?? string.Empty },
                new SqlParameter("@Regu",   SqlDbType.VarChar, 10) { Value = regu   ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Get Credit Secured data — Credits_NextSem.aspx (btnCredit_Click)
        /// SP: SP_Credit_Secured (@REGU, @Noofcredits, @Branch, @ExamMY)
        /// BAL: Bal_Reports_Results → Get_CreditList
        /// Confirmed: DataAccessLayer.dll UTF-16LE "SP_Credit_Secured " + "',  " separator pattern (4 params)
        /// </summary>
        public async Task<IEnumerable<object>> GetCreditSecuredAsync(string regulation, string course, string regu, string examMY, string branch, int noofcredits)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.SP_Credit_Secured,
                "@Regulation", "@Course", "@Branch", "@Regu", "@Exammy", "@noofcr");

            var parameters = new[]
            {
                new SqlParameter("@Regulation", SqlDbType.VarChar, 20) { Value = regulation ?? string.Empty },
                new SqlParameter("@Course",     SqlDbType.VarChar, 10) { Value = course     ?? string.Empty },
                new SqlParameter("@Branch",     SqlDbType.VarChar, 25) { Value = branch     ?? string.Empty },
                new SqlParameter("@Regu",       SqlDbType.VarChar, 5)  { Value = regu       ?? string.Empty },
                new SqlParameter("@Exammy",     SqlDbType.VarChar, 10) { Value = examMY     ?? string.Empty },
                new SqlParameter("@noofcr",     SqlDbType.Int)          { Value = noofcredits }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Get Total Credit Secured data — RegnoWiseTotalCredits.aspx (btnCredittotal_Click)
        /// SP: SP_Credit_Secured_Total (@REGU, @Sem, @Branch, @ExamMY)
        /// BAL: Bal_Reports_Results → Get_CreditList_total
        /// </summary>
        public async Task<IEnumerable<object>> GetCreditSecuredTotalAsync(string regulation, string course, string regu, string examMY, string branch, string sem)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.SP_Credit_Secured_Total,
                "@Regulation", "@Course", "@Branch", "@Regu", "@Exammy", "@Sem");

            var parameters = new[]
            {
                new SqlParameter("@Regulation", SqlDbType.VarChar, 20) { Value = regulation ?? string.Empty },
                new SqlParameter("@Course",     SqlDbType.VarChar, 10) { Value = course     ?? string.Empty },
                new SqlParameter("@Branch",     SqlDbType.VarChar, 25) { Value = branch     ?? string.Empty },
                new SqlParameter("@Regu",       SqlDbType.VarChar, 5)  { Value = regu       ?? string.Empty },
                new SqlParameter("@Exammy",     SqlDbType.VarChar, 10) { Value = examMY     ?? string.Empty },
                new SqlParameter("@Sem",        SqlDbType.Int)          { Value = int.TryParse(sem, out var s) ? s : 0 }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }
    }
}
