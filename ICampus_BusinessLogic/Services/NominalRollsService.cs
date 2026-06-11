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
    public class NominalRollsService : INominalRollsService
    {
        private readonly IGenericRepository<object> _repo;

        public NominalRollsService(IGenericRepository<object> repo)
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

        // Get nominal rolls data (regular or readmit)
        // Stored Procedures: SP_REP_NOMINALROLLS or SP_REP_NOMINALROLLS_Readmit
        // Parameters: @COURSE VARCHAR(30), @EXAMMY VARCHAR(12), @REGULATION VARCHAR(10), 
        //            @SEM VARCHAR(20) = NULL, @EDATE VARCHAR(50) = NULL, @ROOM VARCHAR(50) = NULL
        // Note: SP has conditional logic - if @ROOM IS NULL, it uses hardcoded values (bug in SP)
        //       If @ROOM IS NOT NULL, it uses parameters correctly
        public async Task<IEnumerable<object>> GetNominalRollsDataAsync(NominalRollsRequest request)
        {
            // Determine which stored procedure to use
            var storedProc = request.IsReadmit 
                ? StoredProcedures.SP_REP_NOMINALROLLS_Readmit 
                : StoredProcedures.SP_REP_NOMINALROLLS;

            // Convert date to yyyy-MM-dd format if provided
            string edateFormatted = null;
            if (!string.IsNullOrWhiteSpace(request.Edate))
            {
                if (DateTime.TryParse(request.Edate, out var dateValue))
                {
                    edateFormatted = dateValue.ToString("yyyy-MM-dd");
                }
                else
                {
                    edateFormatted = request.Edate; // Use as-is if not parseable
                }
            }

            // Build parameters - always pass all parameters (use DBNull.Value for optional ones)
            // Parameter names must match SP exactly: @COURSE, @EXAMMY, @REGULATION, @SEM, @EDATE, @ROOM
            var parameters = new[]
            {
                new SqlParameter("@COURSE", SqlDbType.VarChar, 30) { Value = request.Course ?? string.Empty },
                new SqlParameter("@EXAMMY", SqlDbType.VarChar, 12) { Value = request.ExamMY ?? string.Empty },
                new SqlParameter("@REGULATION", SqlDbType.VarChar, 10) { Value = request.Regulation ?? string.Empty },
                new SqlParameter("@SEM", SqlDbType.VarChar, 20) { Value = string.IsNullOrWhiteSpace(request.Sem) ? DBNull.Value : request.Sem },
                new SqlParameter("@EDATE", SqlDbType.VarChar, 50) { Value = string.IsNullOrWhiteSpace(edateFormatted) ? DBNull.Value : edateFormatted },
                new SqlParameter("@ROOM", SqlDbType.VarChar, 50) { Value = string.IsNullOrWhiteSpace(request.Room) ? DBNull.Value : request.Room }
            };

            var paramNames = new[] { "@COURSE", "@EXAMMY", "@REGULATION", "@SEM", "@EDATE", "@ROOM" };

            var sql = StoredProcSql.Exec(storedProc, paramNames);
            var raw = await _repo.QueryFromStoredProcAsync(sql, parameters);
            return raw ?? Enumerable.Empty<object>();
        }
    }
}

