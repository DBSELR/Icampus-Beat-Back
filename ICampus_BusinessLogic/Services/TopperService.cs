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
    public class TopperService : ITopperService
    {
        private readonly IGenericRepository<object> _repo;

        public TopperService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Load Batch dropdown (DDLBatch) — raw SQL on TBL_COURSE
        /// SQL: SELECT DISTINCT REGU,'20'+REGU+'-'+CAST(REGU+MAXSEM/2 AS VARCHAR) BATCH
        ///      FROM TBL_COURSE WHERE COURSE = @Course ORDER BY REGU
        /// params: @Course (varchar)
        /// Confirmed: DataAccessLayer.dll UTF-16LE fragment before [PROC_TOPPERSLIST_NEW]
        ///   BAL method: Load_Batch (DDLBatch_SelectedIndexChanged also triggers LoadSem)
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
        /// Load max semester for selected batch (DDLBatch_SelectedIndexChanged → fills txtSemTo)
        /// SQL: SELECT DISTINCT max(SEM) FROM tbl_sh WHERE REGU = @REGU
        /// params: @REGU (varchar)
        /// Confirmed: DataAccessLayer.dll UTF-16LE fragment "SELECT Distinct max(SEM) FROM tbl_sh WHERE REGU = "
        ///   immediately before [PROC_TOPPERSLIST_NEW] at offset 169649
        ///   BAL method: LoadSem
        /// </summary>
        public async Task<IEnumerable<object>> LoadMaxSemAsync(string regu)
        {
            var sql = "SELECT DISTINCT max(SEM) MaxSEM FROM tbl_sh WHERE REGU = @REGU";

            var parameters = new[]
            {
                new SqlParameter("@REGU", SqlDbType.VarChar, 10) { Value = regu ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Get Toppers List — overall (not semester-wise)
        /// SP: PROC_TOPPERSLIST_NEW
        /// params (all varchar): @Course, @REGU, @Sem, @NoOfToppers, @Branch, @Caste, @Gender, @WithRv
        /// Returns: REGNO, SNAME, COURSE, BRANCH, CASTE, SEM, GENDER,
        ///          TOTAL GRADE POINTS, TOTAL CREDITS, SGPA, CGPA, RANK, exammy, regsup
        /// Confirmed: DataAccessLayer.dll UTF-16LE offset 169649 "[PROC_TOPPERSLIST_NEW]"
        ///   BAL method: Get_Toppers_List (btnToppersList_Click when chksemwise=false)
        ///   @Branch/Caste/Gender = '1' → group/rank within that category; '0' → overall
        ///   @WithRv = '1' → include revaluation marks (chkWithRv checked by default)
        ///   Note: includes Supply students (see "Including Supply" note in ASPX)
        /// </summary>
        public async Task<IEnumerable<object>> GetToppersListAsync(
            string regulation, string course, string regu, string sem,
            string exammy, string rank, string branch, string caste, string gender, string rv)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.PROC_TOPPERSLIST_NEW,
                "@Regulation", "@Course", "@BRANCH", "@CASTE", "@GENDER",
                "@REGU", "@SEM", "@EXAMMY", "@RV", "@RANK");

            var parameters = new[]
            {
                new SqlParameter("@Regulation", SqlDbType.VarChar, 20) { Value = regulation ?? string.Empty },
                new SqlParameter("@Course",     SqlDbType.VarChar, 20) { Value = course     ?? string.Empty },
                new SqlParameter("@BRANCH",     SqlDbType.VarChar, 5)  { Value = branch     ?? "N" },
                new SqlParameter("@CASTE",      SqlDbType.VarChar, 5)  { Value = caste      ?? "N" },
                new SqlParameter("@GENDER",     SqlDbType.VarChar, 5)  { Value = gender     ?? "N" },
                new SqlParameter("@REGU",       SqlDbType.VarChar, 10) { Value = regu       ?? string.Empty },
                new SqlParameter("@SEM",        SqlDbType.VarChar, 5)  { Value = sem        ?? string.Empty },
                new SqlParameter("@EXAMMY",     SqlDbType.VarChar, 20) { Value = exammy     ?? string.Empty },
                new SqlParameter("@RV",         SqlDbType.VarChar, 5)  { Value = rv         ?? "Y" },
                new SqlParameter("@RANK",       SqlDbType.Int)          { Value = int.TryParse(rank, out var r1) ? r1 : 10 }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Get Toppers List — semester-wise
        /// SP: PROC_TOPPERSLIST_SemWise
        /// params (all varchar): @Course, @REGU, @Sem, @NoOfToppers, @Branch, @Caste, @Gender, @WithRv
        /// Returns: same columns as PROC_TOPPERSLIST_NEW, ranked within each semester
        /// Confirmed: DataAccessLayer.dll UTF-16LE offset 169701 "[PROC_TOPPERSLIST_SemWise]"
        ///   BAL method: Get_Toppers_List_SemWise (btnToppersList_Click when chksemwise=true)
        /// </summary>
        public async Task<IEnumerable<object>> GetToppersListSemWiseAsync(
            string regulation, string course, string regu, string sem,
            string exammy, string rank, string branch, string caste, string gender, string rv)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.PROC_TOPPERSLIST_SemWise,
                "@Regulation", "@Course", "@BRANCH", "@CASTE", "@GENDER",
                "@REGU", "@SEM", "@EXAMMY", "@RV", "@RANK");

            var parameters = new[]
            {
                new SqlParameter("@Regulation", SqlDbType.VarChar, 20) { Value = regulation ?? string.Empty },
                new SqlParameter("@Course",     SqlDbType.VarChar, 20) { Value = course     ?? string.Empty },
                new SqlParameter("@BRANCH",     SqlDbType.VarChar, 5)  { Value = branch     ?? "N" },
                new SqlParameter("@CASTE",      SqlDbType.VarChar, 5)  { Value = caste      ?? "N" },
                new SqlParameter("@GENDER",     SqlDbType.VarChar, 5)  { Value = gender     ?? "N" },
                new SqlParameter("@REGU",       SqlDbType.VarChar, 10) { Value = regu       ?? string.Empty },
                new SqlParameter("@SEM",        SqlDbType.VarChar, 5)  { Value = sem        ?? string.Empty },
                new SqlParameter("@EXAMMY",     SqlDbType.VarChar, 20) { Value = exammy     ?? string.Empty },
                new SqlParameter("@RV",         SqlDbType.VarChar, 5)  { Value = rv         ?? "Y" },
                new SqlParameter("@RANK",       SqlDbType.Int)          { Value = int.TryParse(rank, out var r2) ? r2 : 10 }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }
    }
}
