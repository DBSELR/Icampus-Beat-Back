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
    public class GraceDataService : IGraceDataService
    {
        private readonly IGenericRepository<object> _repo;

        public GraceDataService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Load Batch dropdown (Page_Load)
        /// SQL: SELECT DISTINCT CAST(REGU AS VARCHAR) AS REGU,
        ///      '20'+CAST(REGU AS VARCHAR)+'-'+CAST(REGU+MAXSEM/2 AS VARCHAR) BATCH
        ///      FROM TBL_COURSE WHERE COURSE=@Course ORDER BY REGU
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
        /// Load Semester dropdown — queries tbl_sh (tbl_audit_course is unused/empty)
        /// </summary>
        public async Task<IEnumerable<object>> LoadSemesterAsync(string course, string regu)
        {
            var reguNumeric = regu?.Length > 1 && (regu[0] == 'R' || regu[0] == 'r')
                ? regu.Substring(1) : regu ?? string.Empty;
            var regulation  = reguNumeric.Length > 0 ? "R" + reguNumeric : string.Empty;

            var sql = "SELECT DISTINCT CAST(SEM as varchar(10)) SEM FROM tbl_sh " +
                      "WHERE COURSE=@Course AND REGULATION=@Regulation ORDER BY SEM";

            var parameters = new[]
            {
                new SqlParameter("@Course",     SqlDbType.VarChar, 50) { Value = course     ?? string.Empty },
                new SqlParameter("@Regulation", SqlDbType.VarChar, 20) { Value = regulation }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Get Grace Eligible Data (btnGetData_Click)
        /// SP: proc_Grace_Data(@Course,@Regulation,@Exammy,@Batch,@Sem,@is_LE)
        /// @is_LE: 'Y' = Lateral Entry, 'N' = regular students
        /// </summary>
        public async Task<IEnumerable<object>> GetGraceDataAsync(
            string course, string regu, string exammy, string sem, bool isLE)
        {
            var reguNumeric = regu?.Length > 1 && (regu[0] == 'R' || regu[0] == 'r')
                ? regu.Substring(1) : regu ?? string.Empty;
            var regulation  = reguNumeric.Length > 0 ? "R" + reguNumeric : string.Empty;

            var sql = StoredProcSql.Exec(StoredProcedures.proc_Grace_Data,
                "@Course", "@Regulation", "@Exammy", "@Batch", "@Sem", "@is_LE");

            var parameters = new[]
            {
                new SqlParameter("@Course",     SqlDbType.VarChar, 50) { Value = course     ?? string.Empty },
                new SqlParameter("@Regulation", SqlDbType.VarChar, 25) { Value = regulation },
                new SqlParameter("@Exammy",     SqlDbType.VarChar, 50) { Value = exammy     ?? string.Empty },
                new SqlParameter("@Batch",      SqlDbType.VarChar, 20) { Value = reguNumeric },
                new SqlParameter("@Sem",        SqlDbType.VarChar, 10) { Value = sem        ?? string.Empty },
                new SqlParameter("@is_LE",      SqlDbType.VarChar, 20) { Value = isLE ? "Y" : "N" }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }
    }
}
