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
    public class OMRSheetService : IOMRSheetService
    {
        private readonly IGenericRepository<object> _repo;

        public OMRSheetService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        // Get semester list for dropdown
        // Query: SELECT DISTINCT cast( SEM as varchar(250)) SEM,cast(sem as int )sem1 
        //        FROM tbl_sh WHERE COURSE = @Course and ExamMY = @ExamMy ORDER BY sem1
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

        // Get exam date list for dropdown (depends on semester)
        // Query: select distinct convert(nVarchar,EDate,105) edate1,convert(nVarchar,EDate,105) EDATE 
        //        from tbl_sh 
        //        WHERE EDATE is not null AND COURSE = @Course AND sem = @Sem and ExamMY = @ExamMy
        public async Task<IEnumerable<object>> GetExamDatesAsync(string course, string examMY, string sem)
        {
            if (string.IsNullOrWhiteSpace(sem))
                return Enumerable.Empty<object>();

            var sql = "SELECT DISTINCT convert(nVarchar,EDate,105) edate1, convert(nVarchar,EDate,105) EDATE " +
                      "FROM tbl_sh " +
                      "WHERE EDATE IS NOT NULL AND COURSE = @Course AND sem = @Sem AND ExamMY = @ExamMy";

            var ps = new[]
            {
                new SqlParameter("@Course", SqlDbType.VarChar) { Value = course ?? string.Empty },
                new SqlParameter("@Sem", SqlDbType.VarChar) { Value = sem ?? string.Empty },
                new SqlParameter("@ExamMy", SqlDbType.VarChar) { Value = examMY ?? string.Empty }
            };

            var raw = await _repo.QueryFromStoredProcAsync(sql, ps);
            return raw ?? Enumerable.Empty<object>();
        }

        // Get room list for dropdown (depends on exam date)
        // Query: SELECT DISTINCT ROOM FROM TBL_SH 
        //        WHERE ROOM IS NOT NULL 
        //        AND CONVERT(date,EDATE,105) = CONVERT(date,@Edate,105) 
        //        AND COURSE = @Course AND sem = @Sem and ExamMY = @ExamMy
        public async Task<IEnumerable<object>> GetRoomsAsync(string course, string examMY, string sem, string edate)
        {
            if (string.IsNullOrWhiteSpace(sem) || string.IsNullOrWhiteSpace(edate))
                return Enumerable.Empty<object>();

            var sql = "SELECT DISTINCT ROOM FROM TBL_SH " +
                      "WHERE ROOM IS NOT NULL " +
                      "AND CONVERT(date,EDATE,105) = CONVERT(date,@Edate,105) " +
                      "AND COURSE = @Course AND sem = @Sem AND ExamMY = @ExamMy";

            var ps = new[]
            {
                new SqlParameter("@Edate", SqlDbType.VarChar) { Value = edate ?? string.Empty },
                new SqlParameter("@Course", SqlDbType.VarChar) { Value = course ?? string.Empty },
                new SqlParameter("@Sem", SqlDbType.VarChar) { Value = sem ?? string.Empty },
                new SqlParameter("@ExamMy", SqlDbType.VarChar) { Value = examMY ?? string.Empty }
            };

            var raw = await _repo.QueryFromStoredProcAsync(sql, ps);
            return raw ?? Enumerable.Empty<object>();
        }

        // Get OMR sheet data
        // Stored Procedure: SP_REP_OMRSHEET
        // Parameters: @REGULATION VARCHAR(10), @COURSE VARCHAR(30), @EXAMMY VARCHAR(12), @sem INT
        public async Task<IEnumerable<object>> GetOMRSheetDataAsync(OMRSheetRequest request)
        {
            // Sem is required for this stored procedure
            if (string.IsNullOrWhiteSpace(request.Sem))
                throw new ArgumentException("Sem parameter is required for SP_REP_OMRSHEET");

            if (!int.TryParse(request.Sem, out var semInt))
                throw new ArgumentException("Sem parameter must be a valid integer");

            var ps = new[]
            {
                new SqlParameter("@REGULATION", SqlDbType.VarChar, 10) { Value = request.Regulation ?? string.Empty },
                new SqlParameter("@COURSE", SqlDbType.VarChar, 30) { Value = request.Course ?? string.Empty },
                new SqlParameter("@EXAMMY", SqlDbType.VarChar, 12) { Value = request.ExamMY ?? string.Empty },
                new SqlParameter("@sem", SqlDbType.Int) { Value = semInt }
            };

            var sql = StoredProcSql.Exec(StoredProcedures.SP_REP_OMRSHEET, "@REGULATION", "@COURSE", "@EXAMMY", "@sem");
            var raw = await _repo.QueryFromStoredProcAsync(sql, ps);

            // Apply additional filters in memory if provided (sem is already filtered by SP)
            if (raw != null)
            {
                var filtered = raw;

                // Filter by exam date if provided
                if (!string.IsNullOrWhiteSpace(request.Edate))
                {
                    filtered = filtered.Where(item =>
                    {
                        var edateValue = GetPropertyValue(item, "EDate", "Edate", "edate");
                        if (edateValue != null)
                        {
                            var edateStr = edateValue.ToString();
                            // Compare dates (handle different formats)
                            return edateStr.Contains(request.Edate) || request.Edate.Contains(edateStr);
                        }
                        return false;
                    });
                }

                // Filter by room if provided
                if (!string.IsNullOrWhiteSpace(request.Room))
                {
                    filtered = filtered.Where(item =>
                    {
                        var roomValue = GetPropertyValue(item, "ROOM", "Room", "room");
                        return roomValue?.ToString() == request.Room;
                    });
                }

                // Always filter by: NOT ISNULL(BarCode) and ExamMY and REGD='Y'
                //filtered = filtered.Where(item =>
                //{
                //    var barCode = GetPropertyValue(item, "BarCode", "barcode");
                //    var examMy = GetPropertyValue(item, "ExamMY", "EXAMMY", "exammy");
                //    var regd = GetPropertyValue(item, "REGD", "Regd", "regd");

                //    return barCode != null && 
                //           examMy?.ToString() == request.ExamMY && 
                //           regd?.ToString()?.ToUpper() == "Y";
                //});

                return filtered;
            }

            return Enumerable.Empty<object>();
        }

        // Generate OMR numbers (call sp_SH_Omrnumber 8 times)
        // Stored Procedure: sp_SH_Omrnumber
        // Parameters: @StartNumber, @ExamMY, @RangeNumber, @Course, @Regulation
        public async Task<List<int>> GenerateOMRNumbersAsync(string examMY, string course, string regulation)
        {
            var results = new List<int>();

            // Call sp_SH_Omrnumber 8 times (for i = 1 to 8)
            for (int i = 1; i <= 8; i++)
            {
                var startNumber = $"{i}00000";
                var ps = new[]
                {
                    new SqlParameter("@StartNumber", SqlDbType.VarChar) { Value = startNumber },
                    new SqlParameter("@ExamMY", SqlDbType.VarChar) { Value = examMY ?? string.Empty },
                    new SqlParameter("@RangeNumber", SqlDbType.Int) { Value = i },
                    new SqlParameter("@Course", SqlDbType.VarChar) { Value = course ?? string.Empty },
                    new SqlParameter("@Regulation", SqlDbType.VarChar) { Value = regulation ?? string.Empty }
                };

                var sql = StoredProcSql.Exec(StoredProcedures.sp_SH_Omrnumber, "@StartNumber", "@ExamMY", "@RangeNumber", "@Course", "@Regulation");
                var rowsAffected = await _repo.ExecuteStoredProcAsync(sql, ps);
                results.Add(rowsAffected);
            }

            return results;
        }

        // Get OMR data for export
        // Stored Procedure: SP_OMRDATA_EXPORT
        // Parameters: @ExamMY, @Course, @Regulation
        public async Task<IEnumerable<object>> GetOMRDataForExportAsync(string examMY, string course, string regulation)
        {
            var ps = new[]
            {
                new SqlParameter("@ExamMY", SqlDbType.VarChar) { Value = examMY ?? string.Empty },
                new SqlParameter("@Course", SqlDbType.VarChar) { Value = course ?? string.Empty },
                new SqlParameter("@Regulation", SqlDbType.VarChar) { Value = regulation ?? string.Empty }
            };

            var sql = StoredProcSql.Exec(StoredProcedures.SP_OMRDATA_EXPORT, "@ExamMY", "@Course", "@Regulation");
            var raw = await _repo.QueryFromStoredProcAsync(sql, ps);
            return raw ?? Enumerable.Empty<object>();
        }

        // Helper method to get property value from dynamic object
        private object GetPropertyValue(object item, params string[] propertyNames)
        {
            if (item is System.Dynamic.ExpandoObject expando)
            {
                var dict = (IDictionary<string, object>)expando;
                foreach (var propName in propertyNames)
                {
                    if (dict.ContainsKey(propName))
                        return dict[propName];
                }
            }

            // Try reflection for strongly-typed objects
            foreach (var propName in propertyNames)
            {
                var property = item.GetType().GetProperty(propName);
                if (property != null)
                    return property.GetValue(item);
            }

            return null;
        }
    }
}

