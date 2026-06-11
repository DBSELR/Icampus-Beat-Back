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
    public class ModerationService : IModerationService
    {
        private readonly IGenericRepository<object> _repo;

        public ModerationService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Load batch/regu dropdown for moderation
        /// SP: PROC_RESULT_LOAD_REGU_COURSE_GRP
        /// params: @TYPE='REGU', @COURSE, @GRP='', @EXAMMY=''
        /// Confirmed from: DataAccessLayer.dll UTF-16LE "PROC_RESULT_LOAD_REGU_COURSE_GRP '"
        ///   StudentResult.aspx control: ddlModBatch
        /// </summary>
        public async Task<IEnumerable<object>> LoadBatchAsync(string course)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.PROC_RESULT_LOAD_REGU_COURSE_GRP,
                "@TYPE", "@COURSE", "@GRP", "@EXAMMY");

            var parameters = new[]
            {
                new SqlParameter("@TYPE",   SqlDbType.VarChar, 50) { Value = "REGU" },
                new SqlParameter("@COURSE", SqlDbType.VarChar, 20) { Value = course ?? string.Empty },
                new SqlParameter("@GRP",    SqlDbType.VarChar, 20) { Value = string.Empty },
                new SqlParameter("@EXAMMY", SqlDbType.VarChar, 20) { Value = string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Load branch (GRP) dropdown for moderation
        /// Raw SQL from TBL_SH: select distinct GRP from tbl_sh where Course=@Course and sem=@Sem order by GRP asc
        /// Confirmed from: DataAccessLayer.dll UTF-16LE fragment
        ///   " select distinct GRP from tbl_sh where Course ='" + "' and sem ='" + "' order by GRP asc"
        ///   StudentResult.aspx control: ddlModBranch
        /// </summary>
        public async Task<IEnumerable<object>> LoadBranchAsync(string course, string examMy, string sem)
        {
            var sql = "select distinct GRP from tbl_sh where UPPER(Course) = UPPER(@Course) and sem = @Sem order by GRP asc";

            var parameters = new[]
            {
                new SqlParameter("@Course", SqlDbType.VarChar, 30) { Value = course ?? string.Empty },
                new SqlParameter("@Sem",    SqlDbType.VarChar, 5)  { Value = sem    ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Load paper list for moderation dropdown
        /// TBL_PAP.PCODE is NULL — use TBL_SH to get distinct PCODE + PNAME for the dropdown
        /// StudentResult.aspx control: ddlModPapName
        /// </summary>
        public async Task<IEnumerable<object>> LoadPapersAsync(string course, string sem)
        {
            var sql = "SELECT DISTINCT S.PCODE, S.PNAME FROM TBL_SH S " +
                      "INNER JOIN TBL_PAP P ON S.TEMPCODE = P.TEMPCODE AND S.SEM = P.SEM AND UPPER(S.COURSE) = UPPER(P.COURSE) " +
                      "WHERE UPPER(S.COURSE) = UPPER(@Course) AND S.SEM = @Sem AND P.PTYPE = 'TI' " +
                      "ORDER BY S.PCODE";

            var parameters = new[]
            {
                new SqlParameter("@Course", SqlDbType.VarChar, 30) { Value = course ?? string.Empty },
                new SqlParameter("@Sem",    SqlDbType.VarChar, 5)  { Value = sem    ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Load moderation marks grid
        /// Inline SQL: SELECT from TBL_SH JOIN TBL_PAP
        /// params: @Course, @ExamMY, @Regu, @Sem, @Grp, @PapCode
        /// Note: PROC_MODERATION_REG_SEM_GRP_PAP has 8 params with @SELCTION_TYPE flag —
        ///       used for dropdown loading only, not for the marks grid.
        /// </summary>
        public async Task<IEnumerable<object>> LoadModerationDataAsync(
            string course, string examMy, string regu, string sem, string grp, string papCode)
        {
            var sql = "SELECT S.REGNO, S.PCODE, S.PNAME, P.TMAX, P.TPASS, S.TMARKS, S.MODMARKS " +
                      "FROM TBL_SH S " +
                      "INNER JOIN TBL_PAP P ON S.TEMPCODE = P.TEMPCODE AND S.SEM = P.SEM AND UPPER(S.COURSE) = UPPER(P.COURSE) " +
                      "WHERE UPPER(S.COURSE) = UPPER(@Course) AND S.EXAMMY = @ExamMY AND S.REGU = @Regu " +
                      "AND S.SEM = @Sem AND S.GRP = @Grp AND S.PCODE = @PapCode " +
                      "AND P.PTYPE = 'TI' AND S.REGD = 'Y' " +
                      "ORDER BY S.REGNO";

            var parameters = new[]
            {
                new SqlParameter("@Course",  SqlDbType.VarChar, 30) { Value = course  ?? string.Empty },
                new SqlParameter("@ExamMY",  SqlDbType.VarChar, 20) { Value = examMy  ?? string.Empty },
                new SqlParameter("@Regu",    SqlDbType.VarChar, 10) { Value = regu    ?? string.Empty },
                new SqlParameter("@Sem",     SqlDbType.VarChar, 5)  { Value = sem     ?? string.Empty },
                new SqlParameter("@Grp",     SqlDbType.VarChar, 30) { Value = grp     ?? string.Empty },
                new SqlParameter("@PapCode", SqlDbType.VarChar, 20) { Value = papCode ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Save moderation marks
        /// SP: PROC_MODERATION_AND_MCNT
        /// params: @COURSE, @EXAMMY, @REGSUP, @REGU(CHAR5), @SEM(TINYINT), @BRANCH, @PCODE, @MOD_MARKS(TINYINT), @PROCTRYPE
        /// @REGSUP = "Reg" (regular students), @PROCTRYPE = "UPDATE"
        /// </summary>
        public async Task<int> SaveModerationAsync(
            string course, string examMy, string regu, string sem, string grp, string papCode, string modMarks)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.PROC_MODERATION_AND_MCNT,
                "@COURSE", "@EXAMMY", "@REGSUP", "@REGU", "@SEM", "@BRANCH", "@PCODE", "@MOD_MARKS", "@PROCTRYPE");

            var parameters = new[]
            {
                new SqlParameter("@COURSE",    SqlDbType.VarChar, 20) { Value = course    ?? string.Empty },
                new SqlParameter("@EXAMMY",    SqlDbType.VarChar, 20) { Value = examMy    ?? string.Empty },
                new SqlParameter("@REGSUP",    SqlDbType.VarChar, 20) { Value = "Reg" },
                new SqlParameter("@REGU",      SqlDbType.Char,    5)  { Value = regu      ?? string.Empty },
                new SqlParameter("@SEM",       SqlDbType.TinyInt)     { Value = byte.TryParse(sem, out var s) ? s : (object)DBNull.Value },
                new SqlParameter("@BRANCH",    SqlDbType.VarChar, 20) { Value = grp       ?? string.Empty },
                new SqlParameter("@PCODE",     SqlDbType.VarChar, 20) { Value = papCode   ?? string.Empty },
                new SqlParameter("@MOD_MARKS", SqlDbType.TinyInt)     { Value = byte.TryParse(modMarks, out var m) ? m : (object)DBNull.Value },
                new SqlParameter("@PROCTRYPE", SqlDbType.VarChar, 20) { Value = "UPDATE" }
            };

            return await _repo.ExecuteStoredProcAsync(sql, parameters);
        }
    }
}
