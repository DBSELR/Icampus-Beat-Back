using ICampus_BusinessLogic.Interfaces;
using ICampus_DataAccessLayer.Helpers;
using ICampus_DataAccessLayer.Interfaces;
using ICampus_Models.Requests;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Services
{
    public class ExamUnRegistrationService : IExamUnRegistrationService
    {
        private readonly IGenericRepository<object> _repo;

        public ExamUnRegistrationService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        // Load student registration data for display
        // Stored Procedure: PROC_GET_DATA
        // Parameters: @Regulation, @Course, @EXAMMY, @REGNO
        public async Task<IEnumerable<object>> LoadDataAsync(string regulation, string course, string examMY, string regno)
        {
            var ps = new[]
            {
                new SqlParameter("@Regulation", SqlDbType.VarChar) { Value = regulation ?? string.Empty },
                new SqlParameter("@Course", SqlDbType.VarChar) { Value = course ?? string.Empty },
                new SqlParameter("@EXAMMY", SqlDbType.VarChar) { Value = examMY ?? string.Empty },
                new SqlParameter("@REGNO", SqlDbType.VarChar) { Value = regno ?? string.Empty }
            };

            var sql = StoredProcSql.Exec(StoredProcedures.PROC_GET_DATA, "@Regulation", "@Course", "@EXAMMY", "@REGNO");
            var raw = await _repo.QueryFromStoredProcAsync(sql, ps);
            return raw ?? Enumerable.Empty<object>();
        }

        // Perform unregistration (update REGD flag to 'N')
        // Stored Procedure: PROC_REGDUPDATE
        // Parameters: @Regulation, @Course, @EXAMMY, @REGNO
        public async Task<int> UnRegisterAsync(UnRegistrationRequest request)
        {
            var ps = new[]
            {
                new SqlParameter("@Regulation", SqlDbType.VarChar) { Value = request.Regulation ?? string.Empty },
                new SqlParameter("@Course", SqlDbType.VarChar) { Value = request.Course ?? string.Empty },
                new SqlParameter("@EXAMMY", SqlDbType.VarChar) { Value = request.ExamMY ?? string.Empty },
                new SqlParameter("@REGNO", SqlDbType.VarChar) { Value = request.Regno ?? string.Empty }
            };

            var sql = StoredProcSql.Exec(StoredProcedures.PROC_REGDUPDATE, "@Regulation", "@Course", "@EXAMMY", "@REGNO");
            return await _repo.ExecuteStoredProcAsync(sql, ps);
        }
    }
}

