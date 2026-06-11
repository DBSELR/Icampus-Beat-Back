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
    public class ResultCheckListService : IResultCheckListService
    {
        private readonly IGenericRepository<object> _repo;

        public ResultCheckListService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Load Semester dropdown (Page_Load)
        /// SQL: SELECT DISTINCT SEM FROM TBL_SH
        ///      WHERE REGULATION=@Regu AND EXAMMY=@ExamMY AND COURSE=@Course ORDER BY SEM
        /// Confirmed: App_Web_oxqewfcs.dll UTF-16LE offset 43598
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
        /// Get Result Check List data (btnView_Click / btnreadmitok_Click)
        /// SP selection (ResultCheckList.aspx):
        ///   isReadmit=false → SP_REP_MRK_CHKLIST  params: @Course, @ExamMY, @Regu, @Sem
        ///   isReadmit=true  → SP_REP_MRK_CHKLIST_Readmit  params: @Course, @ExamMY, @Regu, @Sem, @ReadmitRegu
        /// Note: checkListType (1=Check List-I, 2=Check List-II) controls Crystal Report title only;
        ///       same SP is used for both — frontend responsibility.
        /// Confirmed: App_Web_oxqewfcs.dll UTF-16LE offset 44424
        ///   Crystal reports: CheckList.rpt (type I), CheckList_r17.rpt / CheckList_r14.rpt (alt regs)
        ///   Readmit titles: 'READMIT RESULT CHECK LIST - I', 'READMIT RESULT CHECK LIST - II'
        ///   Regular titles: 'RESULT CHECK LIST - I', 'RESULT CHECK LIST - II'
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
