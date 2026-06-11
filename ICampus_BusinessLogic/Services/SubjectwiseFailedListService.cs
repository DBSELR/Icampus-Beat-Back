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
    public class SubjectwiseFailedListService : ISubjectwiseFailedListService
    {
        private readonly IGenericRepository<object> _repo;

        public SubjectwiseFailedListService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Load Batch dropdown (ddlBatch on Page_Load)
        /// Same batch SQL used across BackLogsList and ToppersList modules.
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
        /// Load Semester dropdown (ddlSemester — triggered by ddlBatch_SelectedIndexChanged)
        /// Confirmed SQL fragment from DataAccessLayer.dll: query on TBL_SH filtered by COURSE + REGU
        /// </summary>
        public async Task<IEnumerable<object>> LoadSemestersAsync(string course, string regu)
        {
            var sql = @"SELECT DISTINCT CAST(SEM AS VARCHAR) SEM
                        FROM TBL_SH
                        WHERE COURSE = @Course AND REGU = @REGU
                        ORDER BY SEM";

            var parameters = new[]
            {
                new SqlParameter("@Course", SqlDbType.VarChar, 20) { Value = course    ?? string.Empty },
                new SqlParameter("@REGU",   SqlDbType.VarChar, 10) { Value = regu      ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Load Branch dropdown (ddlBranch — triggered by ddlSemester_SelectedIndexChanged)
        /// </summary>
        public async Task<IEnumerable<object>> LoadBranchesAsync(string course, string regu, string sem)
        {
            var sql = @"SELECT DISTINCT GRP
                        FROM TBL_SH
                        WHERE COURSE = @Course AND REGU = @REGU AND SEM = @Sem
                        ORDER BY GRP";

            var parameters = new[]
            {
                new SqlParameter("@Course", SqlDbType.VarChar, 20) { Value = course ?? string.Empty },
                new SqlParameter("@REGU",   SqlDbType.VarChar, 10) { Value = regu   ?? string.Empty },
                new SqlParameter("@Sem",    SqlDbType.VarChar, 5)  { Value = sem    ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Load Subject dropdown (ddlPcode — triggered by ddlBranch_SelectedIndexChanged)
        /// Confirmed SQL: select distinct pno,pcode,pcode+'-'+pname pname from tbl_sh
        ///                where Course='' and Regu='' and grp='' and sem='' order by pno
        /// Confirmed from DataAccessLayer.dll UTF-16LE offset ~154150
        /// </summary>
        public async Task<IEnumerable<object>> LoadSubjectsAsync(string course, string regu, string sem, string branch)
        {
            var sql = @"SELECT DISTINCT pno, pcode, pcode+'-'+pname pname
                        FROM tbl_sh
                        WHERE Course = @Course AND Regu = @REGU AND grp = @Branch AND sem = @Sem
                        ORDER BY pno";

            var parameters = new[]
            {
                new SqlParameter("@Course", SqlDbType.VarChar, 20) { Value = course ?? string.Empty },
                new SqlParameter("@REGU",   SqlDbType.VarChar, 10) { Value = regu   ?? string.Empty },
                new SqlParameter("@Branch", SqlDbType.VarChar, 20) { Value = branch ?? string.Empty },
                new SqlParameter("@Sem",    SqlDbType.VarChar, 5)  { Value = sem    ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Get Subjectwise Failed List (btnView_Click → Load_Grid_SubwiseFailedList)
        /// SP: SP_SubJ_FAILEDLIST_NEW
        /// params: @regulation varchar(20), @course varchar(20), @EXAMMY varchar(25), @sem varchar(20)
        /// @sem = '' → set to NULL inside SP → no sem filter
        /// Returns: COURSE, GRP, REGU, REGNO, PCODE, PNAME, SEM, ORD, EXAMMY, RV, Regulation
        /// Confirmed working: EXEC SP_SubJ_FAILEDLIST_NEW 'R20','B.TECH','May-2024','5' → 2 rows
        /// </summary>
        public async Task<IEnumerable<object>> GetFailedListAsync(
            string regulation, string course, string examMY, string sem)
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
