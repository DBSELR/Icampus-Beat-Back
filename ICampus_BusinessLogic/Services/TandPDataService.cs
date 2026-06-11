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
    public class TandPDataService : ITandPDataService
    {
        private readonly IGenericRepository<object> _repo;

        public TandPDataService(IGenericRepository<object> repo)
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
        /// Download T&P data (btnDownLoad_Click)
        /// SP: sp_t_and_p_data(@Regulation, @Course, @batch, @EXAMMY)
        /// </summary>
        public async Task<IEnumerable<object>> GetTandPDataAsync(string course, string regu, string exammy)
        {
            var reguNumeric = regu?.Length > 1 && (regu[0] == 'R' || regu[0] == 'r')
                ? regu.Substring(1) : regu ?? string.Empty;
            var regulation  = reguNumeric.Length > 0 ? "R" + reguNumeric : string.Empty;

            var sql = StoredProcSql.Exec(StoredProcedures.sp_t_and_p_data,
                "@Regulation", "@Course", "@batch", "@EXAMMY");

            var parameters = new[]
            {
                new SqlParameter("@Regulation", SqlDbType.VarChar, 20) { Value = regulation },
                new SqlParameter("@Course",     SqlDbType.VarChar, 15) { Value = course      ?? string.Empty },
                new SqlParameter("@batch",      SqlDbType.VarChar, 20) { Value = reguNumeric },
                new SqlParameter("@EXAMMY",     SqlDbType.VarChar, 15) { Value = exammy      ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }
    }
}
