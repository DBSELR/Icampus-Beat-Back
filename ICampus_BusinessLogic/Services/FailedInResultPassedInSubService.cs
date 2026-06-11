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
    public class FailedInResultPassedInSubService : IFailedInResultPassedInSubService
    {
        private readonly IGenericRepository<object> _repo;

        public FailedInResultPassedInSubService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Get students who failed in semester result but passed in individual subjects (Page_Load)
        /// SP: SP_SubJ_FAILEDLIST_NEW (@regulation, @course, @EXAMMY, @sem)
        /// @sem = '' → no sem filter (all semesters)
        /// Crystal Report: FailedResult.rpt ("Passed_List Subjectwise")
        /// </summary>
        public async Task<IEnumerable<object>> GetDataAsync(string regulation, string course, string examMY)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.SP_SubJ_FAILEDLIST_NEW,
                "@regulation", "@course", "@EXAMMY", "@sem");

            var parameters = new[]
            {
                new SqlParameter("@regulation", SqlDbType.VarChar, 20) { Value = regulation ?? string.Empty },
                new SqlParameter("@course",     SqlDbType.VarChar, 30) { Value = course     ?? string.Empty },
                new SqlParameter("@EXAMMY",     SqlDbType.VarChar, 25) { Value = examMY     ?? string.Empty },
                new SqlParameter("@sem",        SqlDbType.VarChar, 20) { Value = string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }
    }
}
