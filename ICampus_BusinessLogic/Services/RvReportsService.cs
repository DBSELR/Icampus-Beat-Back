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
    public class RvReportsService : IRvReportsService
    {
        private readonly IGenericRepository<object> _repo;

        public RvReportsService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Load Semester dropdown — ddlSemester (Page_Load)
        /// SQL: SELECT DISTINCT cast(SEM as varchar(250)) SEM FROM tbl_sh WHERE COURSE=@Course
        /// Note: Course comes from session (master page) — no ExamMY/Regu filter on this page.
        /// Confirmed: DataAccessLayer.dll UTF-16LE US heap offset 159480
        /// </summary>
        public async Task<IEnumerable<object>> LoadSemsAsync(string course)
        {
            var sql = "SELECT DISTINCT cast(SEM as varchar(250)) SEM FROM tbl_sh WHERE COURSE=@Course";

            var parameters = new[]
            {
                new SqlParameter("@Course", SqlDbType.VarChar, 30) { Value = course ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Get RV Reports data (btnExport_Click)
        /// SP selection (RvMarksCheckList.aspx):
        ///   isReadmit=false → PROC_RV_REPDATA  params: @Course, @ExamMY, @Regu, @Sem
        ///   isReadmit=true  → PROC_RV_REPDATA_Readmit  params: + @ReadmitRegu
        /// reportType (radio button selection) → Crystal Report title only; same SP for all 3 types:
        ///   1 = rbtn1 (Check List-I)
        ///   2 = rbtn2 (Check List-II)
        ///   3 = rbtnRSheet (Result Sheet)
        /// Confirmed: DataAccessLayer.dll UTF-16LE US heap byte 159892/159934
        /// </summary>
        public async Task<IEnumerable<object>> GetDataAsync(
            string course, string examMY, string regu, string sem,
            bool isReadmit, string readmitRegu)
        {
            string sql;
            SqlParameter[] parameters;

            if (isReadmit)
            {
                sql = StoredProcSql.Exec(StoredProcedures.PROC_RV_REPDATA_Readmit,
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
                sql = StoredProcSql.Exec(StoredProcedures.PROC_RV_REPDATA,
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
