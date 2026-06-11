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
    public class ResultGradeSheetService : IResultGradeSheetService
    {
        private readonly IGenericRepository<object> _repo;

        public ResultGradeSheetService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Load Semester dropdown (Page_Load)
        /// Inline SQL: SELECT DISTINCT cast(SEM as varchar(250)) SEM
        ///   FROM tbl_sh WHERE COURSE=@Course AND EXAMMY=@ExamMY ORDER BY SEM
        /// </summary>
        public async Task<IEnumerable<object>> LoadSemsAsync(string course, string examMY)
        {
            var sql = "SELECT DISTINCT cast(SEM as varchar(250)) SEM " +
                      "FROM tbl_sh WHERE COURSE=@Course AND EXAMMY=@ExamMY ORDER BY SEM";

            var parameters = new[]
            {
                new SqlParameter("@Course", SqlDbType.VarChar, 30) { Value = course ?? string.Empty },
                new SqlParameter("@ExamMY", SqlDbType.VarChar, 20) { Value = examMY ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Get Grade Sheet data (btnView_Click / btnreadmitok_Click)
        /// SP selected by isRv + isReadmit flags:
        ///   false/false → SP_REP_GRADE_CHKLIST (@Course, @ExamMY, @Regu, @Sem)
        ///   true/false  → SP_REP_GRADE_CHKLIST_RV (@Course, @ExamMY, @Regu, @Sem)
        ///   false/true  → SP_REP_GRADE_CHKLIST_Readmit (@Course, @ExamMY, @Regu, @Sem, @ReadmitRegu)
        ///   true/true   → SP_REP_GRADE_CHKLIST_RV_Readmit (@Course, @ExamMY, @Regu, @Sem, @ReadmitRegu)
        /// Confirmed: App_Web_oxqewfcs.dll UTF-16LE US heap offsets 48901/48841/49063/48993
        /// </summary>
        public async Task<IEnumerable<object>> GetDataAsync(
            string course, string examMY, string regu, string sem,
            bool isRv, bool isReadmit, string readmitRegu)
        {
            StoredProcedures spName;
            if (!isRv && !isReadmit) spName = StoredProcedures.SP_REP_GRADE_CHKLIST;
            else if (isRv && !isReadmit) spName = StoredProcedures.SP_REP_GRADE_CHKLIST_RV;
            else if (!isRv && isReadmit) spName = StoredProcedures.SP_REP_GRADE_CHKLIST_Readmit;
            else spName = StoredProcedures.SP_REP_GRADE_CHKLIST_RV_Readmit;

            string sql;
            SqlParameter[] parameters;

            if (isReadmit)
            {
                // SP param order: @COURSE, @EXAMMY, @REGULATION, @readmit_regulation, @SEM
                sql = StoredProcSql.Exec(spName, "@Course", "@ExamMY", "@Regu", "@ReadmitRegu", "@Sem");
                parameters = new[]
                {
                    new SqlParameter("@Course",     SqlDbType.VarChar, 30) { Value = course     ?? string.Empty },
                    new SqlParameter("@ExamMY",     SqlDbType.VarChar, 20) { Value = examMY     ?? string.Empty },
                    new SqlParameter("@Regu",       SqlDbType.VarChar, 10) { Value = regu       ?? string.Empty },
                    new SqlParameter("@ReadmitRegu",SqlDbType.VarChar, 10) { Value = readmitRegu ?? string.Empty },
                    new SqlParameter("@Sem",        SqlDbType.VarChar, 5)  { Value = sem        ?? string.Empty }
                };
            }
            else
            {
                sql = StoredProcSql.Exec(spName, "@Course", "@ExamMY", "@Regu", "@Sem");
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
