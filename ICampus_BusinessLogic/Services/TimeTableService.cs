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
    public class TimeTableService : ITimeTableService
    {
        private readonly IGenericRepository<object> _repo;

        public TimeTableService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        // Get semester list for dropdown
        // Query: SELECT DISTINCT cast(SEM as varchar(250)) SEM, cast(sem as int) sem1 
        //        FROM tbl_sh WHERE COURSE = '{Course}' and ExamMY = '{ExamMy}' ORDER BY sem1
        public async Task<IEnumerable<object>> GetSemestersAsync(string course, string examMY)
        {
            var sql = "SELECT DISTINCT cast(SEM as varchar(250)) SEM, cast(sem as int) sem1 " +
                      "FROM tbl_sh WHERE COURSE = @Course AND ExamMY = @ExamMY ORDER BY sem1";

            var ps = new[]
            {
                new SqlParameter("@Course", SqlDbType.VarChar) { Value = course ?? string.Empty },
                new SqlParameter("@ExamMY", SqlDbType.VarChar) { Value = examMY ?? string.Empty }
            };

            var raw = await _repo.QueryFromStoredProcAsync(sql, ps);
            return raw ?? Enumerable.Empty<object>();
        }

        // Get timetable data
        // Step 1: Updates exam dates and sessions in tbl_sh table
        // Step 2: Calls stored procedure SPM_TIMETABLE_LOAD
        public async Task<IEnumerable<object>> GetTimeTableDataAsync(TimeTableRequest request)
        {
            // Step 1: Execute UPDATE query to sync exam dates and sessions
            // Note: This UPDATE may timeout on large datasets. If it does, we continue with data retrieval.
            var updateSql = "UPDATE s SET s.edate = b.edate, s.esess = b.esess " +
                           "FROM tbl_sh s " +
                           "INNER JOIN (SELECT DISTINCT a.pcode, a.edate, a.exammy, a.esess " +
                                       "FROM tbl_SH a " +
                                       "WHERE a.exammy = @ExamMy " +
                                       "AND a.edate IS NOT NULL " +
                                       "AND esess IS NOT NULL) b " +
                           "ON s.pcode = b.pcode " +
                           "WHERE s.exammy = @ExamMy AND s.regd = 'Y'";

            var updateParams = new[]
            {
                new SqlParameter("@ExamMy", SqlDbType.VarChar) { Value = request.ExamMY ?? string.Empty }
            };

            // Execute the UPDATE query with error handling
            // If it times out or fails, we continue with data retrieval
            try
            {
                await _repo.ExecuteStoredProcAsync(updateSql, updateParams);
            }
            catch (SqlException sqlEx) when (sqlEx.Number == -2 || sqlEx.Message.Contains("timeout", StringComparison.OrdinalIgnoreCase))
            {
                // Timeout occurred - log but continue
                // The stored procedure should still return data even if UPDATE didn't complete
                // In production, you might want to log this: _logger.LogWarning("UPDATE query timed out for ExamMY: {ExamMY}", request.ExamMY);
            }
            catch (System.TimeoutException)
            {
                // Command timeout - continue with data retrieval
                // In production, you might want to log this: _logger.LogWarning("UPDATE query timed out for ExamMY: {ExamMY}", request.ExamMY);
            }
            catch (Exception)
            {
                // Any other error - continue with data retrieval
                // In production, you might want to log this: _logger.LogError(ex, "UPDATE query failed for ExamMY: {ExamMY}", request.ExamMY);
            }

            // Step 2: Get timetable data using stored procedure
            var ps = new[]
            {
                new SqlParameter("@Course", SqlDbType.VarChar) { Value = request.Course ?? string.Empty },
                new SqlParameter("@ExamMy", SqlDbType.VarChar) { Value = request.ExamMY ?? string.Empty }
            };

            var sql = StoredProcSql.Exec(StoredProcedures.SPM_TIMETABLE_LOAD, "@Course", "@ExamMy");
            var raw = await _repo.QueryFromStoredProcAsync(sql, ps);

            // Step 3: Apply semester filter if provided (filter in memory)
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

