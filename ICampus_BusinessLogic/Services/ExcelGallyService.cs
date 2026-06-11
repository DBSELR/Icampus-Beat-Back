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
    public class ExcelGallyService : IExcelGallyService
    {
        private readonly IGenericRepository<object> _repo;

        public ExcelGallyService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Load Semester dropdown (Page_Load)
        /// SQL: SELECT DISTINCT CAST(SEM AS VARCHAR(250)) SEM FROM TBL_GPAP
        ///      WHERE COURSE = @Course ORDER BY SEM
        /// Confirmed: DataAccessLayer.dll UTF-16LE "TBL_GPAP WHERE   COURSE = '"
        /// BAL: BAL_CGPA_Yearwise → Load_Sems_Data
        /// </summary>
        public async Task<IEnumerable<object>> LoadSemsAsync(string course)
        {
            var sql = "SELECT DISTINCT CAST(SEM AS VARCHAR(250)) SEM FROM TBL_GPAP " +
                      "WHERE COURSE = @Course ORDER BY SEM";
            var p = new SqlParameter("@Course", SqlDbType.VarChar, 20) { Value = course ?? string.Empty };
            var result = await _repo.QueryFromStoredProcAsync(sql, p);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Load Branch dropdown (ddlSemester AutoPostBack → Page_Load cascade)
        /// SQL: SELECT DISTINCT GRP FROM TBL_GPAP
        ///      WHERE COURSE = @Course AND SEM = @Sem ORDER BY GRP
        /// </summary>
        public async Task<IEnumerable<object>> LoadBranchesAsync(string course, string sem)
        {
            var sql = "SELECT DISTINCT GRP FROM TBL_GPAP " +
                      "WHERE COURSE = @Course AND SEM = @Sem ORDER BY GRP";
            var parameters = new[]
            {
                new SqlParameter("@Course", SqlDbType.VarChar, 20) { Value = course ?? string.Empty },
                new SqlParameter("@Sem",    SqlDbType.VarChar, 5)  { Value = sem    ?? string.Empty }
            };
            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Export Result Sheet data (btnExcelExort_Click)
        /// SP: PROC_EXCEL_GALLY (@Regulation, @Course, @ExamMy, @Grp, @REGSUP, @Sem)
        /// Confirmed: EXEC PROC_EXCEL_GALLY 'R20','B.TECH','May-2024','CE','Reg','8' → 79 rows
        /// </summary>
        public async Task<IEnumerable<object>> GetExcelGallyAsync(
            string regulation, string course, string examMY, string branch, string regsup, string sem)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.PROC_EXCEL_GALLY,
                "@Regulation", "@Course", "@ExamMy", "@Grp", "@REGSUP", "@Sem");

            var parameters = new[]
            {
                new SqlParameter("@Regulation", SqlDbType.VarChar, 10) { Value = regulation ?? string.Empty },
                new SqlParameter("@Course",     SqlDbType.VarChar, 10) { Value = course     ?? string.Empty },
                new SqlParameter("@ExamMy",     SqlDbType.VarChar, 10) { Value = examMY     ?? string.Empty },
                new SqlParameter("@Grp",        SqlDbType.VarChar, 100){ Value = branch     ?? string.Empty },
                new SqlParameter("@REGSUP",     SqlDbType.VarChar, 10) { Value = regsup     ?? "Reg" },
                new SqlParameter("@Sem",        SqlDbType.VarChar, 10) { Value = sem        ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Export Backlogs (failed subjects with marks)
        /// SP: SP_SubJ_FAILEDLIST_NEW (@regulation, @course, @EXAMMY, @sem)
        /// Returns: COURSE, GRP, REGU, REGNO, PCODE, PNAME, SEM, ORD, EXAMMY, RV, Regulation
        /// Confirmed: EXEC SP_SubJ_FAILEDLIST_NEW 'R20','B.TECH','May-2024','8' → 90 rows
        /// </summary>
        public async Task<IEnumerable<object>> GetBacklogsExcelAsync(
            string regulation, string course, string examMY, string branch, string regsup, string sem)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.SP_SubJ_FAILEDLIST_NEW,
                "@regulation", "@course", "@EXAMMY", "@sem");

            var parameters = new[]
            {
                new SqlParameter("@regulation", SqlDbType.VarChar, 20) { Value = regulation ?? string.Empty },
                new SqlParameter("@course",     SqlDbType.VarChar, 20) { Value = course     ?? string.Empty },
                new SqlParameter("@EXAMMY",     SqlDbType.VarChar, 25) { Value = examMY     ?? string.Empty },
                new SqlParameter("@sem",        SqlDbType.VarChar, 20) { Value = sem        ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }
    }
}
