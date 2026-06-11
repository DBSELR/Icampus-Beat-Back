using ICampus_BusinessLogic.Interfaces;
using ICampus_DataAccessLayer.Helpers;
using ICampus_DataAccessLayer.Interfaces;
using ICampus_Models.Requests;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace ICampus_BusinessLogic.Services
{
    public class QPStatementService : IQPStatementService
    {
        private readonly IGenericRepository<object> _repo;

        public QPStatementService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        // Get semester list for dropdown
        // Query: SELECT DISTINCT SEM, cast(sem as int )sem1 
        //        FROM TBL_SH 
        //        WHERE Regulation = @Regulation and COURSE = @Course AND Exammy = @ExamMy 
        //        order by sem1
        public async Task<IEnumerable<object>> GetSemestersAsync(string course, string regulation, string examMY)
        {
            var sql = "SELECT DISTINCT SEM, cast(sem as int) sem1 " +
                      "FROM TBL_SH " +
                      "WHERE Regulation = @Regulation AND COURSE = @Course AND Exammy = @ExamMy " +
                      "ORDER BY sem1";

            var ps = new[]
            {
                new SqlParameter("@Regulation", SqlDbType.VarChar) { Value = regulation ?? string.Empty },
                new SqlParameter("@Course", SqlDbType.VarChar) { Value = course ?? string.Empty },
                new SqlParameter("@ExamMy", SqlDbType.VarChar) { Value = examMY ?? string.Empty }
            };

            var raw = await _repo.QueryFromStoredProcAsync(sql, ps);
            return raw ?? Enumerable.Empty<object>();
        }

        // Get question paper statement data
        // Stored Procedure: SPM_QPSTATEMENT
        // Parameters: @Course, @ExamMy
        public async Task<IEnumerable<object>> GetQPStatementDataAsync(QPStatementRequest request)
        {
            // Call stored procedure to get data
            var ps = new[]
            {
                new SqlParameter("@Course", SqlDbType.VarChar) { Value = request.Course ?? string.Empty },
                new SqlParameter("@ExamMy", SqlDbType.VarChar) { Value = request.ExamMY ?? string.Empty }
            };

            var sql = StoredProcSql.Exec(StoredProcedures.SPM_QPSTATEMENT, "@Course", "@ExamMy");
            var raw = await _repo.QueryFromStoredProcAsync(sql, ps);

            // Apply semester filter if provided (filter in memory)
            if (!string.IsNullOrWhiteSpace(request.Sem) && raw != null)
            {
                // Filter by semester if provided
                // Note: This assumes the stored procedure returns data with a "Sem" or "sem" field
                // Adjust the property name based on actual stored procedure output
                var filtered = raw.Where(item =>
                {
                    if (item is System.Dynamic.ExpandoObject expando)
                    {
                        var dict = (IDictionary<string, object>)expando;
                        if (dict.ContainsKey("Sem"))
                            return dict["Sem"]?.ToString() == request.Sem;
                        if (dict.ContainsKey("sem"))
                            return dict["sem"]?.ToString() == request.Sem;
                    }
                    // Try reflection for strongly-typed objects
                    var semProperty = item.GetType().GetProperty("Sem") ?? item.GetType().GetProperty("sem");
                    if (semProperty != null)
                    {
                        var semValue = semProperty.GetValue(item)?.ToString();
                        return semValue == request.Sem;
                    }
                    return true; // If we can't find the property, include the item
                });
                return filtered;
            }

            return raw ?? Enumerable.Empty<object>();
        }
    }
}

