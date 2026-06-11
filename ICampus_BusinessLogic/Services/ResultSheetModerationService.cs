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
    public class ResultSheetModerationService : IResultSheetModerationService
    {
        private readonly IGenericRepository<object> _repo;

        public ResultSheetModerationService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Load Semester dropdown — ddlSem (Page_Load)
        /// SQL: SELECT DISTINCT SEM FROM TBL_SH
        ///      WHERE REGULATION=@Regu AND EXAMMY=@ExamMY AND COURSE=@Course ORDER BY SEM
        /// 3 params: @Regu, @ExamMY, @Course
        /// Confirmed: App_Web_gp3pforx.dll UTF-16LE offset 0x2e10b
        ///   "SELECT DISTINCT SEM FROM TBL_SH WHERE REGULATION = '@Regu' AND EXAMMY = '@ExamMY' AND COURSE = '@Course' order by sem"
        /// </summary>
        public async Task<IEnumerable<object>> LoadSemsAsync(string course, string examMY, string regu)
        {
            var sql = "SELECT DISTINCT SEM FROM TBL_SH" +
                      " WHERE REGULATION=@Regu AND EXAMMY=@ExamMY AND COURSE=@Course ORDER BY SEM";

            var parameters = new[]
            {
                new SqlParameter("@Regu",   SqlDbType.VarChar, 10) { Value = regu   ?? string.Empty },
                new SqlParameter("@ExamMY", SqlDbType.VarChar, 20) { Value = examMY ?? string.Empty },
                new SqlParameter("@Course", SqlDbType.VarChar, 30) { Value = course ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Get Result Sheet - Subject Moderation data (btnview_Click)
        /// isReadmit=false → SP_REP_MRK_CHKLIST_SMFLAG (4 params)
        /// isReadmit=true  → SP_REP_MRK_CHKLIST_Readmit_SMFLAG (5 params, adds @ReadmitRegu)
        /// Crystal Report: ResTR_SMFLAG.rpt
        /// chkRv (Revaluation) hidden on page — not a SP parameter
        /// Confirmed: App_Web_gp3pforx.dll UTF-16LE offset 0x2e10b
        /// </summary>
        public async Task<IEnumerable<object>> GetDataAsync(
            string course, string examMY, string regu, string sem,
            bool isReadmit, string readmitRegu)
        {
            object[] parameters;
            string sql;

            if (isReadmit)
            {
                sql = StoredProcSql.Exec(StoredProcedures.SP_REP_MRK_CHKLIST_Readmit_SMFLAG,
                    "@Course", "@ExamMY", "@Regu", "@Sem", "@ReadmitRegu");

                parameters = new object[]
                {
                    new SqlParameter("@Course",      SqlDbType.VarChar, 30) { Value = course      ?? string.Empty },
                    new SqlParameter("@ExamMY",      SqlDbType.VarChar, 20) { Value = examMY      ?? string.Empty },
                    new SqlParameter("@Regu",        SqlDbType.VarChar, 10) { Value = regu        ?? string.Empty },
                    new SqlParameter("@Sem",         SqlDbType.VarChar, 5)  { Value = sem         ?? string.Empty },
                    new SqlParameter("@ReadmitRegu", SqlDbType.VarChar, 10) { Value = readmitRegu ?? string.Empty }
                };
            }
            else
            {
                sql = StoredProcSql.Exec(StoredProcedures.SP_REP_MRK_CHKLIST_SMFLAG,
                    "@Course", "@ExamMY", "@Regu", "@Sem");

                parameters = new object[]
                {
                    new SqlParameter("@Course", SqlDbType.VarChar, 30) { Value = course ?? string.Empty },
                    new SqlParameter("@ExamMY", SqlDbType.VarChar, 20) { Value = examMY ?? string.Empty },
                    new SqlParameter("@Regu",   SqlDbType.VarChar, 10) { Value = regu   ?? string.Empty },
                    new SqlParameter("@Sem",    SqlDbType.VarChar, 5)  { Value = sem    ?? string.Empty }
                };
            }

            var result = await _repo.QueryFromStoredProcAsync(sql, parameters);
            return result ?? Enumerable.Empty<object>();
        }
    }
}
