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
    public class RegnoWiseSgpaCgpaService : IRegnoWiseSgpaCgpaService
    {
        private readonly IGenericRepository<object> _repo;

        public RegnoWiseSgpaCgpaService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Load batch dropdown — ddlBatch (ddlBatch_SelectedIndexChanged cascade)
        /// BAL: SgpaCgpaList_Batch (Bal_Reports_Results)
        /// </summary>
        public async Task<IEnumerable<object>> LoadBatchAsync(string course)
        {
            var sql = @"SELECT DISTINCT REGU,
                '20'+REGU+'-'+CAST(REGU + MAXSEM/2 AS VARCHAR) BATCH
                FROM TBL_COURSE WHERE COURSE = @Course ORDER BY REGU";

            var parameters = new[]
            {
                new SqlParameter("@Course", SqlDbType.VarChar, 20) { Value = course ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Load semester dropdown — ddlSemester
        /// BAL: SgpaCgpaList_Semester (Bal_Reports_Results)
        /// </summary>
        public async Task<IEnumerable<object>> LoadSemestersAsync(string course, string regu)
        {
            var sql = "SELECT DISTINCT CAST(SEM AS VARCHAR) SEM FROM TBL_SH " +
                      "WHERE COURSE=@Course AND REGU=@REGU ORDER BY SEM";

            var parameters = new[]
            {
                new SqlParameter("@Course", SqlDbType.VarChar, 20) { Value = course ?? string.Empty },
                new SqlParameter("@REGU",   SqlDbType.VarChar, 10) { Value = regu   ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// SGPA and CGPA grid — Regular AND Supply exams considered (RegularAndSup_Click)
        /// SP: PROC_GET_CGPA_SGPA_EXCEL (@Regulation, @regu, @course, @exammy, @sem INT)
        /// Confirmed: EXEC PROC_GET_CGPA_SGPA_EXCEL 'R20','20','B.TECH','May-2024',8 → rows
        /// </summary>
        public async Task<IEnumerable<object>> GetListAsync(string regulation, string course, string regu, string exammy, string sem)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.PROC_GET_CGPA_SGPA_EXCEL,
                "@Regulation", "@REGU", "@Course", "@exammy", "@Sem");

            var parameters = new[]
            {
                new SqlParameter("@Regulation", SqlDbType.VarChar, 20) { Value = regulation ?? string.Empty },
                new SqlParameter("@REGU",        SqlDbType.VarChar, 20) { Value = regu        ?? string.Empty },
                new SqlParameter("@Course",      SqlDbType.VarChar, 20) { Value = course      ?? string.Empty },
                new SqlParameter("@exammy",      SqlDbType.VarChar, 20) { Value = exammy      ?? string.Empty },
                new SqlParameter("@Sem",         SqlDbType.Int)          { Value = int.TryParse(sem, out var s1) ? s1 : 0 }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// SGPA and CGPA grid — Regular exams only, Supply NOT considered (RegularOnly_Click)
        /// SP: PROC_SGPA_AVERAGE (@REGULATION, @REGU, @COURSE, @EXAMMY, @SEM INT)
        /// Confirmed: EXEC PROC_SGPA_AVERAGE 'R20','20','B.TECH','May-2024',8 → rows
        /// </summary>
        public async Task<IEnumerable<object>> GetListRegularOnlyAsync(string regulation, string course, string regu, string exammy, string sem)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.PROC_SGPA_AVERAGE,
                "@REGULATION", "@REGU", "@COURSE", "@EXAMMY", "@SEM");

            var parameters = new[]
            {
                new SqlParameter("@REGULATION", SqlDbType.VarChar, 20) { Value = regulation ?? string.Empty },
                new SqlParameter("@REGU",        SqlDbType.VarChar, 20) { Value = regu        ?? string.Empty },
                new SqlParameter("@COURSE",      SqlDbType.VarChar, 20) { Value = course      ?? string.Empty },
                new SqlParameter("@EXAMMY",      SqlDbType.VarChar, 20) { Value = exammy      ?? string.Empty },
                new SqlParameter("@SEM",         SqlDbType.Int)          { Value = int.TryParse(sem, out var s2) ? s2 : 0 }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }
    }
}
