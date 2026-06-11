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
    public class PendingListService : IPendingListService
    {
        private readonly IGenericRepository<object> _repo;

        public PendingListService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Get Internal Pending List
        /// SP: SPS_Get_InternalPendingList
        /// params: @EXAMMY, @COURSE, @Regulation (all varchar)
        /// Confirmed from App_Web_m2jhophz.dll: RBInternalPendingList radio button
        /// </summary>
        public async Task<IEnumerable<object>> GetInternalPendingListAsync(string examMy, string course, string regulation)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.SPS_Get_InternalPendingList,
                "@EXAMMY", "@COURSE", "@Regulation");

            var parameters = new[]
            {
                new SqlParameter("@EXAMMY",     SqlDbType.VarChar, 15) { Value = examMy     ?? string.Empty },
                new SqlParameter("@COURSE",     SqlDbType.VarChar, 30) { Value = course     ?? string.Empty },
                new SqlParameter("@Regulation", SqlDbType.VarChar, 10) { Value = regulation ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Get Practical Pending List
        /// SP: SPS_Get_PracticalPendingList
        /// params: @EXAMMY, @COURSE, @Regulation (all varchar)
        /// Confirmed from App_Web_m2jhophz.dll: RBPracticalPending radio button
        /// </summary>
        public async Task<IEnumerable<object>> GetPracticalPendingListAsync(string examMy, string course, string regulation)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.SPS_Get_PracticalPendingList,
                "@EXAMMY", "@COURSE", "@Regulation");

            var parameters = new[]
            {
                new SqlParameter("@EXAMMY",     SqlDbType.VarChar, 15) { Value = examMy     ?? string.Empty },
                new SqlParameter("@COURSE",     SqlDbType.VarChar, 30) { Value = course     ?? string.Empty },
                new SqlParameter("@Regulation", SqlDbType.VarChar, 10) { Value = regulation ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Get Theory Pending List
        /// SP: SPS_Get_TheoryPendingList
        /// params: @EXAMMY, @COURSE, @Regulation (all varchar)
        /// Confirmed from App_Web_m2jhophz.dll: RBTheoryPending radio button
        /// </summary>
        public async Task<IEnumerable<object>> GetTheoryPendingListAsync(string examMy, string course, string regulation)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.SPS_Get_TheoryPendingList,
                "@EXAMMY", "@COURSE", "@Regulation");

            var parameters = new[]
            {
                new SqlParameter("@EXAMMY",     SqlDbType.VarChar, 15) { Value = examMy     ?? string.Empty },
                new SqlParameter("@COURSE",     SqlDbType.VarChar, 30) { Value = course     ?? string.Empty },
                new SqlParameter("@Regulation", SqlDbType.VarChar, 10) { Value = regulation ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Get RV (Revaluation) Pending List
        /// SP: SPS_Get_RVPendingList
        /// params: @EXAMMY, @COURSE, @Regulation (all varchar)
        /// Confirmed from App_Web_m2jhophz.dll: RBRVPending radio button
        /// </summary>
        public async Task<IEnumerable<object>> GetRVPendingListAsync(string examMy, string course, string regulation)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.SPS_Get_RVPendingList,
                "@EXAMMY", "@COURSE", "@Regulation");

            var parameters = new[]
            {
                new SqlParameter("@EXAMMY",     SqlDbType.VarChar, 15) { Value = examMy     ?? string.Empty },
                new SqlParameter("@COURSE",     SqlDbType.VarChar, 30) { Value = course     ?? string.Empty },
                new SqlParameter("@Regulation", SqlDbType.VarChar, 10) { Value = regulation ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }
    }
}
