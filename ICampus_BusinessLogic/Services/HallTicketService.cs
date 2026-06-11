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
    public class HallTicketService : IHallTicketService
    {
        private readonly IGenericRepository<object> _repo;

        public HallTicketService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        // Get batch list for dropdown
        // Query: SELECT DISTINCT REGU,'20' + REGU + '-20' + CAST(CAST(REGU AS INT) + (MAXSEM/2) AS VARCHAR) AS BATCH 
        //        FROM TBL_COURSE WHERE COURSE = @Course and Regulation = @Regulation
        public async Task<IEnumerable<object>> GetBatchesAsync(string course, string regulation)
        {
            var sql = "SELECT DISTINCT REGU, '20' + REGU + '-20' + CAST(CAST(REGU AS INT) + (MAXSEM/2) AS VARCHAR) AS BATCH " +
                      "FROM TBL_COURSE WHERE COURSE = @Course AND Regulation = @Regulation";

            var ps = new[]
            {
                new SqlParameter("@Course", SqlDbType.VarChar) { Value = course ?? string.Empty },
                new SqlParameter("@Regulation", SqlDbType.VarChar) { Value = regulation ?? string.Empty }
            };

            var raw = await _repo.QueryFromStoredProcAsync(sql, ps);
            return raw ?? Enumerable.Empty<object>();
        }

        // Get branch list for dropdown (depends on batch)
        // Query: SELECT DISTINCT GRP FROM TBL_COURSE 
        //        WHERE Regulation = @Regulation and COURSE = @Course AND REGU = @Batch
        public async Task<IEnumerable<object>> GetBranchesAsync(string course, string regulation, string batch)
        {
            if (string.IsNullOrWhiteSpace(batch))
                return Enumerable.Empty<object>();

            var sql = "SELECT DISTINCT GRP FROM TBL_COURSE " +
                      "WHERE Regulation = @Regulation AND COURSE = @Course AND REGU = @Batch";

            var ps = new[]
            {
                new SqlParameter("@Regulation", SqlDbType.VarChar) { Value = regulation ?? string.Empty },
                new SqlParameter("@Course", SqlDbType.VarChar) { Value = course ?? string.Empty },
                new SqlParameter("@Batch", SqlDbType.VarChar) { Value = batch ?? string.Empty }
            };

            var raw = await _repo.QueryFromStoredProcAsync(sql, ps);
            return raw ?? Enumerable.Empty<object>();
        }

        // Get semester list for dropdown
        // Query: SELECT DISTINCT SEM, cast(sem as int )sem1 
        //        FROM TBL_SH 
        //        WHERE Regulation = @Regulation and COURSE = @Course AND EXAMMY = @ExamMy 
        //        order by sem1
        public async Task<IEnumerable<object>> GetSemestersAsync(string course, string regulation, string examMY)
        {
            var sql = "SELECT DISTINCT SEM, cast(sem as int) sem1 " +
                      "FROM TBL_SH " +
                      "WHERE Regulation = @Regulation AND COURSE = @Course AND EXAMMY = @ExamMy " +
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

        // Prepare hall ticket data (call SPM_HT_LBRCE)
        // Stored Procedure: SPM_HT_LBRCE
        // Parameters: @ExamMY, @Course, @regulation, @CON (selection formula), @REGNO (optional), @isOverWrite (optional)
        // Returns: Tuple with (rowsAffected from ExecuteNonQuery, actual count of records in tbl_hallticket)
        public async Task<(int RowsAffected, int RecordsCount)> PrepareHallTicketsAsync(string examMY, string course, string regulation, string selectionFormula)
        {
            // Default condition if not provided: ' S.REGU = S.REGU AND S.SEM = S.SEM AND S.GRP = S.GRP '
            var condition = string.IsNullOrWhiteSpace(selectionFormula) 
                ? " S.REGU = S.REGU AND S.SEM = S.SEM AND S.GRP = S.GRP " 
                : selectionFormula;

            var ps = new[]
            {
                new SqlParameter("@ExamMY", SqlDbType.VarChar, 20) { Value = examMY ?? string.Empty },
                new SqlParameter("@Course", SqlDbType.VarChar, 30) { Value = course ?? string.Empty },
                new SqlParameter("@regulation", SqlDbType.VarChar, 30) { Value = regulation ?? string.Empty },
                new SqlParameter("@CON", SqlDbType.VarChar, 200) { Value = condition },
                new SqlParameter("@REGNO", SqlDbType.VarChar, 15) { Value = DBNull.Value }, // Optional, defaults to NULL
                new SqlParameter("@isOverWrite", SqlDbType.Char, 1) { Value = "N" } // Optional, defaults to 'N'
            };

            var sql = StoredProcSql.Exec(StoredProcedures.SPM_HT_LBRCE, "@ExamMY", "@Course", "@regulation", "@CON", "@REGNO", "@isOverWrite");
            var rowsAffected = await _repo.ExecuteStoredProcAsync(sql, ps);

            // Check actual count in tbl_Hallticket after execution
            // Using exact column names: ExamMy, COURSE, Regulation
            var countSql = "SELECT COUNT(*) AS RecordCount FROM tbl_Hallticket WHERE ExamMy = @ExamMY AND COURSE = @Course AND Regulation = @Regulation";
            var countParams = new[]
            {
                new SqlParameter("@ExamMY", SqlDbType.VarChar) { Value = examMY ?? string.Empty },
                new SqlParameter("@Course", SqlDbType.VarChar) { Value = course ?? string.Empty },
                new SqlParameter("@Regulation", SqlDbType.VarChar) { Value = regulation ?? string.Empty }
            };
            
            var countResult = await _repo.QueryFromStoredProcAsync(countSql, countParams);
            var recordsCount = 0;
            
            if (countResult != null && countResult.Any())
            {
                var firstRow = countResult.First();
                if (firstRow is System.Dynamic.ExpandoObject expando)
                {
                    var dict = (IDictionary<string, object>)expando;
                    // Try common column names for COUNT(*)
                    if (dict.ContainsKey("RecordCount"))
                        int.TryParse(dict["RecordCount"]?.ToString(), out recordsCount);
                    else if (dict.ContainsKey("Count"))
                        int.TryParse(dict["Count"]?.ToString(), out recordsCount);
                    else if (dict.Values.Any())
                    {
                        var countValue = dict.Values.First();
                        int.TryParse(countValue?.ToString(), out recordsCount);
                    }
                }
                else
                {
                    // Try to get the count value using reflection
                    var countProperty = firstRow.GetType().GetProperty("RecordCount") ?? 
                                       firstRow.GetType().GetProperty("Count") ??
                                       firstRow.GetType().GetProperties().FirstOrDefault();
                    if (countProperty != null)
                    {
                        var countValue = countProperty.GetValue(firstRow);
                        int.TryParse(countValue?.ToString(), out recordsCount);
                    }
                }
            }

            return (rowsAffected, recordsCount);
        }

        // Get hall ticket data from tbl_Hallticket table
        // This queries existing data - does NOT call PrepareHallTicketsAsync
        // Supports flexible filtering: batch only, batch+sem, batch+sem+branch, etc.
        // PrepareHallTicketsAsync should be called separately via /prepare endpoint when needed
        // Column names match actual table structure: REGU, SEM, RegNo, ExamMy, COURSE, Regulation, GRP
        public async Task<IEnumerable<object>> GetHallTicketDataAsync(HallTicketRequest request)
        {
            // Query the tbl_Hallticket table (must be populated first via /prepare endpoint)
            // Supports all filter combinations:
            // - Batch only
            // - Batch + Sem
            // - Batch + Sem + Branch
            // - Batch + Branch
            // - Sem only
            // - Branch only
            // - Regno only
            // - Any combination of filters
            var sql = "SELECT * FROM tbl_Hallticket WHERE 1=1";
            var parameters = new List<SqlParameter>();

            // Required filters (always applied)
            // Using exact column names: ExamMy (not ExamMY)
            if (!string.IsNullOrWhiteSpace(request.ExamMY))
            {
                sql += " AND ExamMy = @ExamMY";
                parameters.Add(new SqlParameter("@ExamMY", SqlDbType.VarChar) { Value = request.ExamMY });
            }

            // Using exact column names: COURSE (uppercase)
            if (!string.IsNullOrWhiteSpace(request.Course))
            {
                sql += " AND COURSE = @Course";
                parameters.Add(new SqlParameter("@Course", SqlDbType.VarChar) { Value = request.Course });
            }

            // Using exact column names: Regulation (mixed case)
            if (!string.IsNullOrWhiteSpace(request.Regulation))
            {
                sql += " AND Regulation = @Regulation";
                parameters.Add(new SqlParameter("@Regulation", SqlDbType.VarChar) { Value = request.Regulation });
            }

            // Optional filters (applied only if provided)
            // Using exact column names: REGU (uppercase)
            if (!string.IsNullOrWhiteSpace(request.Batch))
            {
                sql += " AND REGU = @Batch";
                parameters.Add(new SqlParameter("@Batch", SqlDbType.VarChar) { Value = request.Batch });
            }

            // Using exact column names: GRP (uppercase)
            if (!string.IsNullOrWhiteSpace(request.Branch))
            {
                sql += " AND GRP = @Branch";
                parameters.Add(new SqlParameter("@Branch", SqlDbType.VarChar) { Value = request.Branch });
            }

            // Using exact column names: SEM (uppercase)
            if (!string.IsNullOrWhiteSpace(request.Sem))
            {
                sql += " AND SEM = @Sem";
                parameters.Add(new SqlParameter("@Sem", SqlDbType.VarChar) { Value = request.Sem });
            }

            // Using exact column names: RegNo (mixed case)
            if (!string.IsNullOrWhiteSpace(request.Regno))
            {
                sql += " AND RegNo = @Regno";
                parameters.Add(new SqlParameter("@Regno", SqlDbType.VarChar) { Value = request.Regno });
            }

            var raw = await _repo.QueryFromStoredProcAsync(sql, parameters.ToArray());
            return raw ?? Enumerable.Empty<object>();
        }

        // Build selection formula from request filters (matching ASPX logic)
        // Note: The formula is used in SQL WHERE clause, so it should be SQL condition syntax
        // Format matches ASPX: " s.REGU = {batch} and s.grp = '{branch}' and s.sem = {sem} "
        private string BuildSelectionFormula(HallTicketRequest request)
        {
            var formula = "";

            // Batch filter (matching ASPX format: " s.REGU = {batch} " or " s.REGU = s.regu ")
            if (!string.IsNullOrWhiteSpace(request.Batch))
                formula = $" s.REGU = {request.Batch}";
            else
                formula = " s.REGU = s.regu";

            // Branch filter (matching ASPX format: " and s.grp = '{branch}' " or " and s.grp = s.grp ")
            if (!string.IsNullOrWhiteSpace(request.Branch))
                formula += $" and s.grp = '{request.Branch}'";
            else
                formula += " and s.grp = s.grp";

            // Semester filter (matching ASPX format: " and s.sem = {sem} " or " and s.sem = s.sem ")
            if (!string.IsNullOrWhiteSpace(request.Sem))
                formula += $" and s.sem = {request.Sem}";
            else
                formula += " and s.sem = s.sem";

            return formula;
        }

        // Diagnostic: Check if source data exists for the given criteria
        // This helps identify why no records are being saved
        public async Task<object> CheckSourceDataAsync(string examMY, string course, string regulation, string selectionFormula)
        {
            var condition = string.IsNullOrWhiteSpace(selectionFormula) 
                ? " S.REGU = S.REGU AND S.SEM = S.SEM AND S.GRP = S.GRP " 
                : selectionFormula;

            // Check TBL_SH records matching basic criteria
            var shCountSql = $"SELECT COUNT(*) AS SHCount FROM TBL_SH s " +
                            $"WHERE S.REGD = 'Y' AND S.EXAMMY = @ExamMY AND S.COURSE = @Course AND S.Regulation = @Regulation " +
                            $"AND ({condition})";
            
            var shParams = new[]
            {
                new SqlParameter("@ExamMY", SqlDbType.VarChar) { Value = examMY ?? string.Empty },
                new SqlParameter("@Course", SqlDbType.VarChar) { Value = course ?? string.Empty },
                new SqlParameter("@Regulation", SqlDbType.VarChar) { Value = regulation ?? string.Empty }
            };

            var shResult = await _repo.QueryFromStoredProcAsync(shCountSql, shParams);
            var shCount = GetCountFromResult(shResult);

            // Check TBL_SH records that also have matching TBL_PAP records with PTYPE LIKE '%T%'
            var joinCountSql = $"SELECT COUNT(*) AS JoinCount FROM TBL_SH s " +
                              $"INNER JOIN TBL_PAP P ON P.REGU = S.REGU AND S.SEM = P.SEM AND S.TEMPCODE = P.TEMPCODE " +
                              $"WHERE S.REGD = 'Y' AND S.EXAMMY = @ExamMY AND S.COURSE = @Course AND S.Regulation = @Regulation " +
                              $"AND P.PTYPE LIKE '%T%' AND ({condition})";

            var joinResult = await _repo.QueryFromStoredProcAsync(joinCountSql, shParams);
            var joinCount = GetCountFromResult(joinResult);

            return new
            {
                TBL_SH_Count = shCount,
                TBL_SH_With_TBL_PAP_Count = joinCount,
                SelectionFormula = condition,
                Message = shCount == 0 
                    ? "No records found in TBL_SH matching the criteria. Please verify ExamMY, Course, and Regulation values."
                    : joinCount == 0
                        ? "Records found in TBL_SH, but no matching records in TBL_PAP with PTYPE LIKE '%T%'. Please verify TBL_PAP data."
                        : "Source data exists. The stored procedure should populate tbl_hallticket."
            };
        }

        private int GetCountFromResult(IEnumerable<object> result)
        {
            if (result == null || !result.Any())
                return 0;

            var firstRow = result.First();
            if (firstRow is System.Dynamic.ExpandoObject expando)
            {
                var dict = (IDictionary<string, object>)expando;
                foreach (var key in new[] { "SHCount", "JoinCount", "RecordCount", "Count" })
                {
                    if (dict.ContainsKey(key))
                    {
                        if (int.TryParse(dict[key]?.ToString(), out var count))
                            return count;
                    }
                }
                // If no named key found, try first value
                if (dict.Values.Any())
                {
                    if (int.TryParse(dict.Values.First()?.ToString(), out var count))
                        return count;
                }
            }
            else
            {
                // Try reflection
                var properties = firstRow.GetType().GetProperties();
                foreach (var prop in properties)
                {
                    var value = prop.GetValue(firstRow);
                    if (int.TryParse(value?.ToString(), out var count))
                        return count;
                }
            }

            return 0;
        }
    }
}

