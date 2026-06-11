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
    public class V3DataService : IV3DataService
    {
        private readonly IGenericRepository<object> _repo;

        public V3DataService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Load Semester dropdown (Page_Load)
        /// Inline SQL on tbl_sh filtered by Course, Regulation, ExamMY
        /// </summary>
        public async Task<IEnumerable<object>> LoadSemestersAsync(
            string course, string regulation, string examMY)
        {
            var sql = "SELECT DISTINCT SEM FROM tbl_sh " +
                      "WHERE COURSE=@Course AND REGULATION=@Regulation AND EXAMMY=@ExamMY " +
                      "ORDER BY SEM";

            var parameters = new[]
            {
                new SqlParameter("@Course",     SqlDbType.VarChar, 30) { Value = course     ?? string.Empty },
                new SqlParameter("@Regulation", SqlDbType.VarChar, 10) { Value = regulation ?? string.Empty },
                new SqlParameter("@ExamMY",     SqlDbType.VarChar, 20) { Value = examMY     ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Get V3 (Second Revaluation / RV2) data (btnView_Click)
        /// Calls PROC_DATAFOR_V3 for regular students or
        ///       PROC_DATAFOR_V3_READMIT for readmit students
        /// </summary>
        public async Task<IEnumerable<object>> GetV3DataAsync(
            string course, string regulation, string examMY,
            string sem, string diffMarks,
            bool isReadmit, string readmitReg)
        {
            string sql;
            SqlParameter[] parameters;

            int semInt      = int.TryParse(sem,       out var s) ? s : 0;
            int diffMarksInt = int.TryParse(diffMarks, out var d) ? d : 0;

            if (!isReadmit)
            {
                // SP: PROC_DATAFOR_V3 @EXAMMY, @COURSE, @SEM int, @REGULATION, @DEFFMRK int
                sql = StoredProcSql.Exec(StoredProcedures.PROC_DATAFOR_V3,
                    "@EXAMMY", "@COURSE", "@SEM", "@REGULATION", "@DEFFMRK");

                parameters = new[]
                {
                    new SqlParameter("@EXAMMY",     SqlDbType.VarChar, 20) { Value = examMY     ?? string.Empty },
                    new SqlParameter("@COURSE",     SqlDbType.VarChar, 20) { Value = course     ?? string.Empty },
                    new SqlParameter("@SEM",        SqlDbType.Int)         { Value = semInt },
                    new SqlParameter("@REGULATION", SqlDbType.VarChar, 20) { Value = regulation ?? string.Empty },
                    new SqlParameter("@DEFFMRK",    SqlDbType.Int)         { Value = diffMarksInt }
                };
            }
            else
            {
                // SP: PROC_DATAFOR_V3_READMIT @EXAMMY, @COURSE, @SEM int, @REGULATION, @DEFFMRK int, @readmit_regulation
                sql = StoredProcSql.Exec(StoredProcedures.PROC_DATAFOR_V3_READMIT,
                    "@EXAMMY", "@COURSE", "@SEM", "@REGULATION", "@DEFFMRK", "@readmit_regulation");

                parameters = new[]
                {
                    new SqlParameter("@EXAMMY",              SqlDbType.VarChar, 20) { Value = examMY     ?? string.Empty },
                    new SqlParameter("@COURSE",              SqlDbType.VarChar, 20) { Value = course     ?? string.Empty },
                    new SqlParameter("@SEM",                 SqlDbType.Int)         { Value = semInt },
                    new SqlParameter("@REGULATION",          SqlDbType.VarChar, 20) { Value = regulation ?? string.Empty },
                    new SqlParameter("@DEFFMRK",             SqlDbType.Int)         { Value = diffMarksInt },
                    new SqlParameter("@readmit_regulation",  SqlDbType.VarChar, 20) { Value = readmitReg ?? string.Empty }
                };
            }

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            if (result == null) return Enumerable.Empty<object>();

            // SP returns SELECT 0 (one row, empty column name) when no RV data exists — treat as empty
            var list = result.ToList();
            if (list.Count == 1)
            {
                var row = list[0] as System.Collections.Generic.IDictionary<string, object>;
                if (row != null && row.Count == 1 && row.ContainsKey(""))
                    return Enumerable.Empty<object>();
            }
            return list;
        }
    }
}
