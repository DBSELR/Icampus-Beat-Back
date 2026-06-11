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
    public class TranscriptService : ITranscriptService
    {
        private readonly IGenericRepository<object> _repo;

        public TranscriptService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Load Semester dropdown — ddlSemester (Page_Load)
        /// SQL: SELECT DISTINCT cast(SEM as varchar(250)) SEM
        ///      FROM tbl_sh WHERE COURSE=@Course AND Exammy=@ExamMY AND Regulation=@Regu ORDER BY SEM
        /// 3 params: @Course, @ExamMY, @Regu
        /// Confirmed: DataAccessLayer.dll UTF-16LE US heap context at offset 0x26ddb
        /// </summary>
        public async Task<IEnumerable<object>> LoadSemsAsync(string course, string examMY, string regu)
        {
            var sql = "SELECT DISTINCT cast(SEM as varchar(250)) SEM FROM tbl_sh" +
                      " WHERE COURSE=@Course AND Exammy=@ExamMY AND Regulation=@Regu ORDER BY SEM";

            var parameters = new[]
            {
                new SqlParameter("@Course",     SqlDbType.VarChar, 30) { Value = course ?? string.Empty },
                new SqlParameter("@ExamMY",     SqlDbType.VarChar, 20) { Value = examMY ?? string.Empty },
                new SqlParameter("@Regu",       SqlDbType.VarChar, 10) { Value = regu   ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Load Branch dropdown — ddlBranch (Page_Load)
        /// SQL: SELECT DISTINCT grp FROM tbl_sh WHERE COURSE=@Course ORDER BY grp
        /// 1 param: @Course
        /// Confirmed: DataAccessLayer.dll UTF-16LE context near Transcript sequence
        /// </summary>
        public async Task<IEnumerable<object>> LoadBranchesAsync(string course)
        {
            var sql = "SELECT DISTINCT grp FROM tbl_sh WHERE COURSE=@Course ORDER BY grp";

            var parameters = new[]
            {
                new SqlParameter("@Course", SqlDbType.VarChar, 30) { Value = course ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Get Transcript / MarksMemo data
        /// isMarksMemo=true  → SP_MRK_MEMO  (8 params, @RV='N', @Date='')
        /// isMarksMemo=false → SP_Transcript_New (6 params)
        /// branch / regNo can be empty to return all branches / all students
        /// Confirmed: DataAccessLayer.dll UTF-16LE offsets 0x23cf0 (SP_MRK_MEMO) and 0x26dd5 (SP_Transcript_New)
        /// </summary>
        public async Task<IEnumerable<object>> GetDataAsync(
            string course, string examMY, string regu, string sem,
            string branch, string regNo, bool isMarksMemo)
        {
            object[] parameters;
            string sql;

            if (isMarksMemo)
            {
                // SP_MRK_MEMO — MarksMemo mode (chkgcard checked, default)
                // Same SP used by DupCertificate page; @RV='N', @Date='' for Transcript page
                sql = StoredProcSql.Exec(StoredProcedures.SP_MRK_MEMO,
                    "@REGULATION", "@EXAMMY", "@Course", "@SEMESTER", "@RV", "@BRANCH", "@REGNO", "@Date");

                parameters = new object[]
                {
                    new SqlParameter("@REGULATION", SqlDbType.VarChar, 10) { Value = regu     ?? string.Empty },
                    new SqlParameter("@EXAMMY",     SqlDbType.VarChar, 20) { Value = examMY   ?? string.Empty },
                    new SqlParameter("@Course",     SqlDbType.VarChar, 30) { Value = course   ?? string.Empty },
                    new SqlParameter("@SEMESTER",   SqlDbType.VarChar, 5)  { Value = sem      ?? string.Empty },
                    new SqlParameter("@RV",         SqlDbType.VarChar, 2)  { Value = "N" },
                    new SqlParameter("@BRANCH",     SqlDbType.VarChar, 20) { Value = branch   ?? string.Empty },
                    new SqlParameter("@REGNO",      SqlDbType.VarChar, 20) { Value = regNo    ?? string.Empty },
                    new SqlParameter("@Date",       SqlDbType.VarChar, 50) { Value = string.Empty }
                };
            }
            else
            {
                // SP_Transcript_New — Transcript mode (chkgcard unchecked)
                sql = StoredProcSql.Exec(StoredProcedures.SP_Transcript_New,
                    "@Course", "@ExamMY", "@Regu", "@Sem", "@Branch", "@RegNo");

                parameters = new object[]
                {
                    new SqlParameter("@Course",  SqlDbType.VarChar, 30) { Value = course  ?? string.Empty },
                    new SqlParameter("@ExamMY",  SqlDbType.VarChar, 20) { Value = examMY  ?? string.Empty },
                    new SqlParameter("@Regu",    SqlDbType.VarChar, 10) { Value = regu    ?? string.Empty },
                    new SqlParameter("@Sem",     SqlDbType.VarChar, 5)  { Value = sem     ?? string.Empty },
                    new SqlParameter("@Branch",  SqlDbType.VarChar, 20) { Value = branch  ?? string.Empty },
                    new SqlParameter("@RegNo",   SqlDbType.VarChar, 20) { Value = regNo   ?? string.Empty }
                };
            }

            var result = await _repo.QueryFromStoredProcAsync(sql, parameters);
            return result ?? Enumerable.Empty<object>();
        }
    }
}
