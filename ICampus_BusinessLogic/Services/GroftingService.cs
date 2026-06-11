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
    public class GroftingService : IGroftingService
    {
        private readonly IGenericRepository<object> _repo;

        public GroftingService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Load ExamMY dropdown for Grofting page
        /// SP: SPM_EXAMS_ExamMY_Load(@Regulation, @Course)
        /// Confirmed: EXEC SPM_EXAMS_ExamMY_Load 'R20','B.Tech' → 23 rows
        /// Note: @Regulation must NOT be empty
        /// </summary>
        public async Task<IEnumerable<object>> LoadExamMyAsync(string course, string regulation)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.SPM_EXAMS_ExamMY_Load,
                "@Regulation", "@Course");

            var parameters = new[]
            {
                new SqlParameter("@Regulation", SqlDbType.VarChar, 20) { Value = regulation ?? string.Empty },
                new SqlParameter("@Course",     SqlDbType.VarChar, 30) { Value = course     ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Load Semesters dropdown for Grofting page
        /// SP: SP_SEMS_RP(@REGULATION, @COURSE, @EXAMMY)
        /// Confirmed: EXEC SP_SEMS_RP 'R20','B.Tech','May-2024' → 4 rows (3,5,7,8)
        /// </summary>
        public async Task<IEnumerable<object>> LoadSemsAsync(string course, string examMy, string regulation)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.SP_SEMS_RP,
                "@REGULATION", "@COURSE", "@EXAMMY");

            var parameters = new[]
            {
                new SqlParameter("@REGULATION", SqlDbType.VarChar, 10) { Value = regulation ?? string.Empty },
                new SqlParameter("@COURSE",     SqlDbType.VarChar, 30) { Value = course     ?? string.Empty },
                new SqlParameter("@EXAMMY",     SqlDbType.VarChar, 20) { Value = examMy     ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Run Grofting/Grafting process
        /// SP: SP_GRACING_GROFTING(@EXAMMY, @COURSE, @GRACEMARKS INT=1, @GRAFTINGMARKS INT=6)
        /// Confirmed: EXEC SP_GRACING_GROFTING 'May-2024','B.Tech' → runs successfully
        /// Note: SP processes ALL REG failed students for given course+exammy (no semester/pcode filter)
        /// After running this, Result Process must be run separately
        /// </summary>
        public async Task<int> RunGroftingAsync(string course, string examMy)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.SP_GRACING_GROFTING,
                "@EXAMMY", "@COURSE");

            var parameters = new[]
            {
                new SqlParameter("@EXAMMY",  SqlDbType.VarChar, 20) { Value = examMy ?? string.Empty },
                new SqlParameter("@COURSE",  SqlDbType.VarChar, 20) { Value = course ?? string.Empty }
            };

            return await _repo.ExecuteStoredProcAsync(sql, parameters);
        }
    }
}
