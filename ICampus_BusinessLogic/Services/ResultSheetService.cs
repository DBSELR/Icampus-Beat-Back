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
    public class ResultSheetService : IResultSheetService
    {
        private readonly IGenericRepository<object> _repo;

        public ResultSheetService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Load Semester dropdown (Page_Load)
        /// SQL: SELECT DISTINCT SEM FROM TBL_SH
        ///      WHERE REGULATION=@Regu AND EXAMMY=@ExamMY AND COURSE=@Course ORDER BY SEM
        /// Confirmed: App_Web_gp3pforx.dll UTF-16LE offset 205422 (same SQL as ResultCheckList)
        /// </summary>
        public async Task<IEnumerable<object>> LoadSemsAsync(string course, string examMY, string regu)
        {
            var sql = "SELECT DISTINCT SEM FROM TBL_SH " +
                      "WHERE REGULATION=@Regu AND EXAMMY=@ExamMY AND COURSE=@Course ORDER BY SEM";

            var parameters = new[]
            {
                new SqlParameter("@Course", SqlDbType.VarChar, 30) { Value = course ?? string.Empty },
                new SqlParameter("@ExamMY", SqlDbType.VarChar, 20) { Value = examMY ?? string.Empty },
                new SqlParameter("@Regu",   SqlDbType.VarChar, 10) { Value = regu   ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Get Result Sheet (V1 Marks) data (btnview_Click / btnreadmitok_Click)
        /// SP selection (ResultSheet.aspx):
        ///   isReadmit=false → SP_REP_MRK_CHKLIST  params: @Course, @ExamMY, @Regu, @Sem
        ///   isReadmit=true  → SP_REP_MRK_CHKLIST_Readmit  params: @Course, @ExamMY, @Regu, @Sem, @ReadmitRegu
        /// Note: chkRv is HIDDEN in the ASPX (display:none) — RV mode disabled in this page.
        ///       Same SPs as ResultCheckList; Crystal Report differs (ResTR.rpt vs CheckList.rpt).
        /// Confirmed: App_Web_gp3pforx.dll UTF-16LE offset 205422
        ///   'READMIT RESULTS SHEET' / 'RESULTS SHEET' Crystal Report page titles
        ///   Crystal report: ResTR.rpt
        /// </summary>
        public async Task<IEnumerable<object>> GetDataAsync(
            string course, string examMY, string regu, string sem,
            bool isReadmit, string readmitRegu)
        {
            string sql;
            SqlParameter[] parameters;

            if (isReadmit)
            {
                sql = StoredProcSql.Exec(StoredProcedures.SP_REP_MRK_CHKLIST_Readmit,
                    "@Course", "@ExamMY", "@Regu", "@Sem", "@ReadmitRegu");
                parameters = new[]
                {
                    new SqlParameter("@Course",     SqlDbType.VarChar, 30) { Value = course      ?? string.Empty },
                    new SqlParameter("@ExamMY",     SqlDbType.VarChar, 20) { Value = examMY      ?? string.Empty },
                    new SqlParameter("@Regu",       SqlDbType.VarChar, 10) { Value = regu        ?? string.Empty },
                    new SqlParameter("@Sem",        SqlDbType.VarChar, 5)  { Value = sem         ?? string.Empty },
                    new SqlParameter("@ReadmitRegu",SqlDbType.VarChar, 10) { Value = readmitRegu ?? string.Empty }
                };
            }
            else
            {
                sql = StoredProcSql.Exec(StoredProcedures.SP_REP_MRK_CHKLIST,
                    "@Course", "@ExamMY", "@Regu", "@Sem");
                parameters = new[]
                {
                    new SqlParameter("@Course", SqlDbType.VarChar, 30) { Value = course ?? string.Empty },
                    new SqlParameter("@ExamMY", SqlDbType.VarChar, 20) { Value = examMY ?? string.Empty },
                    new SqlParameter("@Regu",   SqlDbType.VarChar, 10) { Value = regu   ?? string.Empty },
                    new SqlParameter("@Sem",    SqlDbType.VarChar, 5)  { Value = sem    ?? string.Empty }
                };
            }

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }
    }
}
