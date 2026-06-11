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
    public class CreditsMismatchService : ICreditsMismatchService
    {
        private readonly IGenericRepository<object> _repo;

        public CreditsMismatchService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        // Get batches for dropdown
        // Query: SELECT DISTINCT REGU,'20' + REGU + '-20' + CAST(CAST(REGU AS INT) + (MAXSEM/2) AS VARCHAR) AS BATCH 
        //        FROM TBL_COURSE WHERE Regulation = @Regulation
        public async Task<IEnumerable<object>> GetBatchesAsync(string regulation)
        {
            var sql = "SELECT DISTINCT REGU, '20' + REGU + '-20' + CAST(CAST(REGU AS INT) + (MAXSEM/2) AS VARCHAR) AS BATCH " +
                      "FROM TBL_COURSE WHERE Regulation = @Regulation";
            var p = new SqlParameter("@Regulation", SqlDbType.VarChar) { Value = regulation ?? string.Empty };
            var raw = await _repo.QueryFromStoredProcAsync(sql, p);
            return raw ?? Enumerable.Empty<object>();
        }

        // Get exam month-years for dropdown
        // Query: SELECT DISTINCT EXAMMY,AEXAMID FROM TBL_EXAMS 
        //        WHERE COURSE = @Course and REGULATION = @Regulation ORDER BY AEXAMID DESC
        public async Task<IEnumerable<object>> GetExamMyAsync(string course, string regulation)
        {
            var sql = "SELECT DISTINCT EXAMMY, AEXAMID FROM TBL_EXAMS " +
                      "WHERE COURSE = @Course AND REGULATION = @Regulation ORDER BY AEXAMID DESC";
            var ps = new[]
            {
                new SqlParameter("@Course", SqlDbType.VarChar) { Value = course ?? string.Empty },
                new SqlParameter("@Regulation", SqlDbType.VarChar) { Value = regulation ?? string.Empty }
            };
            var raw = await _repo.QueryFromStoredProcAsync(sql, ps);
            return raw ?? Enumerable.Empty<object>();
        }

        // Get semesters for dropdown
        // Query: SELECT DISTINCT SEM, cast(sem as int) sem1 
        //        FROM TBL_SH WHERE Regulation = @Regulation AND Exammy = @ExamMy order by sem1
        public async Task<IEnumerable<object>> GetSemestersAsync(string regulation, string examMy)
        {
            var sql = "SELECT DISTINCT SEM, cast(sem as int) sem1 " +
                      "FROM TBL_SH WHERE Regulation = @Regulation AND Exammy = @ExamMy ORDER BY sem1";
            var ps = new[]
            {
                new SqlParameter("@Regulation", SqlDbType.VarChar) { Value = regulation ?? string.Empty },
                new SqlParameter("@ExamMy", SqlDbType.VarChar) { Value = examMy ?? string.Empty }
            };
            var raw = await _repo.QueryFromStoredProcAsync(sql, ps);
            return raw ?? Enumerable.Empty<object>();
        }

        // Get credits mismatch data - MAIN EXPORT METHOD
        // Stored Procedure: SPM_CREDITSMISMATCH
        // Parameters: @Regulation, @ExamMy, @Batch, @Course, @Sem
        public async Task<IEnumerable<object>> GetMismatchCreditsAsync(string regulation, string examMy, string batch, string course, string sem)
        {
            var ps = new[]
            {
                new SqlParameter("@Regulation", SqlDbType.VarChar) { Value = regulation ?? string.Empty },
                new SqlParameter("@ExamMy", SqlDbType.VarChar) { Value = examMy ?? string.Empty },
                new SqlParameter("@Batch", SqlDbType.VarChar) { Value = batch ?? string.Empty },
                new SqlParameter("@Course", SqlDbType.VarChar) { Value = course ?? string.Empty },
                new SqlParameter("@Sem", SqlDbType.VarChar) { Value = sem ?? string.Empty }
            };

            var sql = StoredProcSql.Exec(StoredProcedures.SPM_CREDITSMISMATCH, "@Regulation", "@ExamMy", "@Batch", "@Course", "@Sem");
            var raw = await _repo.QueryFromStoredProcAsync(sql, ps);
            return raw ?? Enumerable.Empty<object>();
        }
    }
}

