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
    public class UnblockRegistrationsService : IUnblockRegistrationsService
    {
        private readonly IGenericRepository<object> _repo;

        public UnblockRegistrationsService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        // Load exam month-year list for dropdown
        // Query: SELECT DISTINCT Exammy FROM TBL_REGISTRATIONS_BLOCK
        public async Task<IEnumerable<object>> LoadExamMyAsync()
        {
            var sql = "SELECT DISTINCT Exammy FROM TBL_REGISTRATIONS_BLOCK ORDER BY Exammy DESC";
            var raw = await _repo.QueryFromStoredProcAsync(sql);
            return raw ?? Enumerable.Empty<object>();
        }

        // Load blocked registrations data
        // Stored Procedure: PROC_GET_UNBLOCK
        // Parameters: @Exammy
        public async Task<IEnumerable<object>> LoadBlockedStudentsAsync(string exammy)
        {
            var ps = new[]
            {
                new SqlParameter("@Exammy", SqlDbType.VarChar) { Value = exammy ?? string.Empty }
            };

            var sql = StoredProcSql.Exec(StoredProcedures.PROC_GET_UNBLOCK, "@Exammy");
            var raw = await _repo.QueryFromStoredProcAsync(sql, ps);
            return raw ?? Enumerable.Empty<object>();
        }

        // Save/Perform unblock operation for a single student
        // Stored Procedure: PRO_SAVE_UNBLOCK
        // Parameters: @Exammy, @Regno
        public async Task<int> UnblockStudentAsync(string exammy, string regno)
        {
            var ps = new[]
            {
                new SqlParameter("@Exammy", SqlDbType.VarChar) { Value = exammy ?? string.Empty },
                new SqlParameter("@Regno", SqlDbType.VarChar) { Value = regno ?? string.Empty }
            };

            var sql = StoredProcSql.Exec(StoredProcedures.PRO_SAVE_UNBLOCK, "@Exammy", "@Regno");
            return await _repo.ExecuteStoredProcAsync(sql, ps);
        }

        // Save/Perform unblock operation for multiple students
        // Loops through each regno and calls PRO_SAVE_UNBLOCK
        public async Task<UnblockBatchResult> UnblockMultipleAsync(string exammy, List<string> regnos)
        {
            var result = new UnblockBatchResult
            {
                TotalSelected = regnos?.Count ?? 0
            };

            if (regnos == null || !regnos.Any())
            {
                result.Errors.Add("No registration numbers provided");
                return result;
            }

            foreach (var regno in regnos)
            {
                if (string.IsNullOrWhiteSpace(regno))
                {
                    result.Failed++;
                    result.Errors.Add($"Empty registration number skipped");
                    continue;
                }

                try
                {
                    var rowsAffected = await UnblockStudentAsync(exammy, regno);
                    if (rowsAffected > 0)
                    {
                        result.SuccessfullyUnblocked++;
                    }
                    else
                    {
                        result.Failed++;
                        result.Errors.Add($"Unblock failed for registration number: {regno}");
                    }
                }
                catch (System.Exception ex)
                {
                    result.Failed++;
                    result.Errors.Add($"Error unblocking {regno}: {ex.Message}");
                }
            }

            return result;
        }
    }
}

