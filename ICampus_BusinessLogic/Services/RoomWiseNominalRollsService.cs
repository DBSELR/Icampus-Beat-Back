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
    public class RoomWiseNominalRollsService : IRoomWiseNominalRollsService
    {
        private readonly IGenericRepository<object> _repo;

        public RoomWiseNominalRollsService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Get list of semesters for dropdown
        /// Query: SELECT DISTINCT cast( SEM as varchar(250)) SEM,cast(sem as int )sem1 
        ///        FROM tbl_sh WHERE COURSE = '{Course}' and ExamMY = '{ExamMy}' ORDER BY sem1
        /// </summary>
        public async Task<IEnumerable<object>> GetSemestersAsync(string course, string examMY)
        {
            var sql = "SELECT DISTINCT cast( SEM as varchar(250)) SEM, cast(sem as int) sem1 " +
                      "FROM tbl_sh WHERE COURSE = @Course AND ExamMY = @ExamMY ORDER BY sem1";

            var parameters = new[]
            {
                new SqlParameter("@Course", SqlDbType.VarChar) { Value = course ?? string.Empty },
                new SqlParameter("@ExamMY", SqlDbType.VarChar) { Value = examMY ?? string.Empty }
            };

            var raw = await _repo.QueryFromStoredProcAsync(sql, parameters);
            return raw ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Get list of exam dates for dropdown (depends on Semester and ExamType)
        /// Stored Procedure: Sp_REP_Nominal_LoadEdate
        /// Parameters: @Course, @Sem, @ExamMy, @Regulation, @ExamType
        /// </summary>
        public async Task<IEnumerable<object>> GetExamDatesAsync(string course, string sem, string examMY, string regulation, string examType)
        {
            // Convert examType from numeric to text if needed
            string examTypeText = ConvertExamTypeToText(examType);

            var parameters = new[]
            {
                new SqlParameter("@Course", SqlDbType.VarChar) { Value = course ?? string.Empty },
                new SqlParameter("@Sem", SqlDbType.VarChar) { Value = sem ?? string.Empty },
                new SqlParameter("@ExamMy", SqlDbType.VarChar) { Value = examMY ?? string.Empty },
                new SqlParameter("@Regulation", SqlDbType.VarChar) { Value = regulation ?? string.Empty },
                new SqlParameter("@ExamType", SqlDbType.VarChar) { Value = examTypeText }
            };

            var sql = StoredProcSql.Exec(StoredProcedures.Sp_REP_Nominal_LoadEdate, "@Course", "@Sem", "@ExamMy", "@Regulation", "@ExamType");
            var raw = await _repo.QueryFromStoredProcAsync(sql, parameters);
            return raw ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Get list of branches for dropdown (depends on Exam Date)
        /// Stored Procedure: Sp_REP_Nominal_LoadBranch
        /// Parameters: @Course, @Sem, @ExamMy, @Regulation, @Edate, @ExamType
        /// </summary>
        public async Task<IEnumerable<object>> GetBranchesAsync(string course, string sem, string examMY, string regulation, string edate, string examType)
        {
            // Convert examType from numeric to text if needed
            string examTypeText = ConvertExamTypeToText(examType);

            // Convert date format to yyyy-MM-dd if needed
            string edateFormatted = FormatDateForSP(edate);

            var parameters = new[]
            {
                new SqlParameter("@Course", SqlDbType.VarChar) { Value = course ?? string.Empty },
                new SqlParameter("@Sem", SqlDbType.VarChar) { Value = sem ?? string.Empty },
                new SqlParameter("@ExamMy", SqlDbType.VarChar) { Value = examMY ?? string.Empty },
                new SqlParameter("@Regulation", SqlDbType.VarChar) { Value = regulation ?? string.Empty },
                new SqlParameter("@Edate", SqlDbType.VarChar) { Value = edateFormatted },
                new SqlParameter("@ExamType", SqlDbType.VarChar) { Value = examTypeText }
            };

            var sql = StoredProcSql.Exec(StoredProcedures.Sp_REP_Nominal_LoadBranch, "@Course", "@Sem", "@ExamMy", "@Regulation", "@Edate", "@ExamType");
            var raw = await _repo.QueryFromStoredProcAsync(sql, parameters);
            return raw ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Get room-wise nominal rolls data
        /// Stored Procedure: Sp_REP_NominalRolls_ROOMWISE
        /// Parameters: @COURSE, @EXAMMY, @REGULATION, @ExamType, @SEM (optional), @EDATE (optional), @GRP (optional)
        /// Note: SP handles empty string for @GRP: if @GRP='' set @GRP=null
        /// Note: Although @SEM and @EDATE are optional in SP definition, they are used in WHERE clauses (SH.SEM = @SEM, SH.EDATE = @EDATE)
        ///       So they should be provided for the query to work correctly. Passing DBNull.Value if not provided.
        /// </summary>
        public async Task<IEnumerable<object>> GetRoomWiseNominalRollsDataAsync(RoomWiseNominalRollsRequest request)
        {
            // Convert examType from numeric to text if needed
            string examTypeText = ConvertExamTypeToText(request.ExamType);

            // Build parameters list - all parameters are passed, using DBNull.Value for optional ones if not provided
            // SP parameter names are uppercase: @COURSE, @EXAMMY, @REGULATION, @ExamType, @SEM, @EDATE, @GRP
            var parameters = new[]
            {
                new SqlParameter("@COURSE", SqlDbType.VarChar, 30) { Value = request.Course ?? string.Empty },
                new SqlParameter("@EXAMMY", SqlDbType.VarChar, 12) { Value = request.ExamMY ?? string.Empty },
                new SqlParameter("@REGULATION", SqlDbType.VarChar, 10) { Value = request.Regulation ?? string.Empty },
                new SqlParameter("@ExamType", SqlDbType.VarChar, 50) { Value = examTypeText },
                new SqlParameter("@SEM", SqlDbType.VarChar, 20) { Value = string.IsNullOrWhiteSpace(request.Sem) ? DBNull.Value : (object)request.Sem },
                new SqlParameter("@EDATE", SqlDbType.VarChar, 50) { Value = string.IsNullOrWhiteSpace(request.Edate) ? DBNull.Value : (object)FormatDateForSP(request.Edate) },
                new SqlParameter("@GRP", SqlDbType.VarChar, 50) { Value = string.IsNullOrWhiteSpace(request.Branch) ? DBNull.Value : (object)request.Branch }
            };

            var sql = StoredProcSql.Exec(StoredProcedures.Sp_REP_NominalRolls_ROOMWISE, "@COURSE", "@EXAMMY", "@REGULATION", "@ExamType", "@SEM", "@EDATE", "@GRP");
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

