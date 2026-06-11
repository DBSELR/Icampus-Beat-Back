using ICampus_BusinessLogic.Interfaces;
using ICampus_DataAccessLayer.Helpers;
using ICampus_DataAccessLayer.Interfaces;
using ICampus_Models.Requests;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Services
{
    public class RoomAbstractService : IRoomAbstractService
    {
        private readonly IGenericRepository<object> _repo;

        public RoomAbstractService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Get list of semesters for dropdown
        /// Query: select distinct sem from tbl_sh where course='{Course}' order by sem
        /// </summary>
        public async Task<IEnumerable<object>> GetSemestersAsync(string course)
        {
            var sql = "SELECT DISTINCT sem FROM tbl_sh WHERE course = @Course ORDER BY sem";
            var parameter = new SqlParameter("@Course", SqlDbType.VarChar) { Value = course ?? string.Empty };
            var raw = await _repo.QueryFromStoredProcAsync(sql, parameter);
            return raw ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Get list of sessions for dropdown
        /// Stored Procedure: Spr_Load_Session
        /// Parameters: @Course, @Sem, @ExamType
        /// </summary>
        public async Task<IEnumerable<object>> GetSessionsAsync(string course, string sem, string examType)
        {
            // Convert examType from numeric to text if needed
            string examTypeText = ConvertExamTypeToText(examType);

            var parameters = new[]
            {
                new SqlParameter("@Course", SqlDbType.VarChar) { Value = course ?? string.Empty },
                new SqlParameter("@Sem", SqlDbType.VarChar) { Value = sem ?? string.Empty },
                new SqlParameter("@ExamType", SqlDbType.VarChar) { Value = examTypeText }
            };
            var sql = StoredProcSql.Exec(StoredProcedures.Spr_Load_Session, "@Course", "@Sem", "@ExamType");
            var raw = await _repo.QueryFromStoredProcAsync(sql, parameters);
            return raw ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Get list of exam dates for dropdown
        /// Stored Procedure: Proc_Load_Edate
        /// Parameters: @Course, @Sem, @Esess, @Exammy, @ExamType
        /// </summary>
        public async Task<IEnumerable<object>> GetExamDatesAsync(string course, string sem, string session, string examMY, string examType)
        {
            // Convert examType from numeric to text if needed
            string examTypeText = ConvertExamTypeToText(examType);

            var parameters = new[]
            {
                new SqlParameter("@Course", SqlDbType.VarChar, 50) { Value = course ?? string.Empty },
                new SqlParameter("@Sem", SqlDbType.VarChar, 10) { Value = sem ?? string.Empty },
                new SqlParameter("@Esess", SqlDbType.VarChar, 10) { Value = session ?? string.Empty },
                new SqlParameter("@Exammy", SqlDbType.VarChar, 50) { Value = examMY ?? string.Empty },
                new SqlParameter("@ExamType", SqlDbType.VarChar, 50) { Value = examTypeText }
            };
            var sql = StoredProcSql.Exec(StoredProcedures.Proc_Load_Edate, "@Course", "@Sem", "@Esess", "@Exammy", "@ExamType");
            var raw = await _repo.QueryFromStoredProcAsync(sql, parameters);
            return raw ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Get room abstract data for report
        /// Stored Procedures: SPR_LOAD_EXAMDATES (Regular) or SPR_LOAD_EXAMDATES_Supple (Supply)
        /// Parameters: @Course, @ExamMY, @Sem, @Session, @EDate, @ExamType
        /// Note: Session is passed as integer (not quoted)
        /// If Regsup == 2 (Sup): Uses SPR_LOAD_EXAMDATES_Supple
        /// Otherwise (Regsup == 0 or 1): Uses SPR_LOAD_EXAMDATES
        /// </summary>
        public async Task<IEnumerable<object>> GetRoomAbstractDataAsync(RoomAbstractRequest request)
        {
            // Convert date format to yyyy-MM-dd if needed
            string edateFormatted = FormatDateForSP(request.EDate);

            // Convert examType from numeric to text if needed
            string examTypeText = ConvertExamTypeToText(request.ExamType);

            // Determine which stored procedure to use based on Regsup
            // If Regsup == 2 (Sup): Use SPR_LOAD_EXAMDATES_Supple
            // Otherwise (Regsup == 0 or 1): Use SPR_LOAD_EXAMDATES
            StoredProcedures storedProc = request.Regsup == 2 
                ? StoredProcedures.SPR_LOAD_EXAMDATES_Supple 
                : StoredProcedures.SPR_LOAD_EXAMDATES;

            var parameters = new[]
            {
                new SqlParameter("@Course", SqlDbType.VarChar) { Value = request.Course ?? string.Empty },
                new SqlParameter("@ExamMY", SqlDbType.VarChar) { Value = request.ExamMY ?? string.Empty },
                new SqlParameter("@Sem", SqlDbType.VarChar) { Value = request.Sem ?? string.Empty },
                new SqlParameter("@Session", SqlDbType.Int) { Value = request.Session },
                new SqlParameter("@EDate", SqlDbType.VarChar) { Value = edateFormatted },
                new SqlParameter("@ExamType", SqlDbType.VarChar) { Value = examTypeText }
            };

            var sql = StoredProcSql.Exec(storedProc, "@Course", "@ExamMY", "@Sem", "@Session", "@EDate", "@ExamType");
            var raw = await _repo.QueryFromStoredProcAsync(sql, parameters);
            return raw ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Format date for stored procedure (convert to yyyy-MM-dd format)
        /// Accepts: dd-MM-yyyy, yyyy-MM-dd, or other formats
        /// Returns: yyyy-MM-dd format string (as expected by SP)
        /// </summary>
        private string FormatDateForSP(string dateStr)
        {
            if (string.IsNullOrWhiteSpace(dateStr))
                return string.Empty;

            // Try parsing the date
            if (DateTime.TryParse(dateStr, out var dateValue))
            {
                // Return in yyyy-MM-dd format (as expected by SP)
                return dateValue.ToString("yyyy-MM-dd");
            }

            // If parsing fails, return as-is (might already be in correct format)
            return dateStr;
        }

        /// <summary>
        /// Convert exam type from numeric to text format
        /// Input: "1" or "External" -> Output: "External"
        /// Input: "2" or "MID-I" -> Output: "MID-I"
        /// Input: "3" or "MID-II" -> Output: "MID-II"
        /// </summary>
        private string ConvertExamTypeToText(string examType)
        {
            if (string.IsNullOrWhiteSpace(examType))
                return string.Empty;

            // If already in text format, return as-is
            if (examType.Equals("External", StringComparison.OrdinalIgnoreCase) ||
                examType.Equals("MID-I", StringComparison.OrdinalIgnoreCase) ||
                examType.Equals("MID-II", StringComparison.OrdinalIgnoreCase))
            {
                return examType;
            }

            // Convert numeric to text
            switch (examType)
            {
                case "1":
                    return "External";
                case "2":
                    return "MID-I";
                case "3":
                    return "MID-II";
                default:
                    return examType; // Return as-is if not recognized
            }
        }
    }
}

