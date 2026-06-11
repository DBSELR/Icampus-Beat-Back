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
    public class BranchWiseSectionPercentService : IBranchWiseSectionPercentService
    {
        private readonly IGenericRepository<object> _repo;

        public BranchWiseSectionPercentService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Load Semester dropdown — ddlSemester (Page_Load)
        /// SQL: SELECT DISTINCT cast(SEM as varchar(250)) SEM
        ///      FROM tbl_sh WHERE COURSE=@Course AND EXAMMY=@ExamMY ORDER BY SEM
        /// No AutoPostBack on ddlSemester (no OnSelectedIndexChanged)
        /// Confirmed: same pattern as all other Sem-load SQLs (DataAccessLayer.dll UTF-16LE US heap)
        /// </summary>
        public async Task<IEnumerable<object>> LoadSemsAsync(string course, string examMY)
        {
            var sql = "SELECT DISTINCT cast(SEM as varchar(250)) SEM" +
                      " FROM tbl_sh WHERE COURSE=@Course AND EXAMMY=@ExamMY ORDER BY SEM";

            var parameters = new[]
            {
                new SqlParameter("@Course", SqlDbType.VarChar, 30) { Value = course ?? string.Empty },
                new SqlParameter("@ExamMY", SqlDbType.VarChar, 20) { Value = examMY ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Get Branch-wise Section Percentage data (btnView_Click)
        /// SP: sp_PAP_SEC_PERCENT  params (4): @Course, @ExamMY, @Regu, @Sem
        /// No ChkIsrv on this page — no RV toggle
        /// btnDownLoad (Download) is Visible=false in ASPX — Crystal Report viewer only
        /// BAL method: BranchWiseSectionPercentage_and_Chart (DataAccessLayer.dll ASCII offset 0x1c691)
        /// Confirmed: DataAccessLayer.dll UTF-16LE US heap offset 0x27132:
        ///   "sp_PAP_SEC_PERCENT ..." in the SP lookup table, following sp_PAP_PERCENT entry
        /// </summary>
        public async Task<IEnumerable<object>> GetDataAsync(
            string course, string examMY, string regu, string sem)
        {
            // SP param order: @REGULATION, @EXAMMY, @COURSE, @USERID, @RV, @sem
            // @REGULATION = regu as-is (frontend sends full 'R20' from appData.regulation)
            // @USERID = fixed 'DBS' (original used session user)
            var sql = StoredProcSql.Exec(StoredProcedures.sp_PAP_SEC_PERCENT,
                "@REGULATION", "@EXAMMY", "@COURSE", "@USERID", "@RV", "@Sem");

            var parameters = new[]
            {
                new SqlParameter("@REGULATION", SqlDbType.VarChar, 20)  { Value = regu ?? string.Empty },
                new SqlParameter("@EXAMMY",     SqlDbType.VarChar, 10)  { Value = examMY ?? string.Empty },
                new SqlParameter("@COURSE",     SqlDbType.VarChar, 20)  { Value = course ?? string.Empty },
                new SqlParameter("@USERID",     SqlDbType.VarChar, 150) { Value = "DBS" },
                new SqlParameter("@RV",         SqlDbType.VarChar, 20)  { Value = "N" },
                new SqlParameter("@Sem",        SqlDbType.VarChar, 2)   { Value = sem ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }
    }
}
