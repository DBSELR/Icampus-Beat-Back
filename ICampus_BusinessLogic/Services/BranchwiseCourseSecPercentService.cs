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
    public class BranchwiseCourseSecPercentService : IBranchwiseCourseSecPercentService
    {
        private readonly IGenericRepository<object> _repo;

        public BranchwiseCourseSecPercentService(IGenericRepository<object> repo)
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
        /// Get Branch-wise Course Section Percent data (btnView_Click)
        /// SP: PROC_CLASSWISE_COUNT  params (5): @Course, @ExamMY, @Regu, @Sem, @IsRv
        /// @IsRv: 'N' = regular (ChkIsrv unchecked), 'Y' = after RV/SM (ChkIsrv checked)
        /// Crystal Report: ClassWiseCnt.rpt
        /// btnDownLoad (Download) is Visible=false in ASPX — Crystal Report viewer only
        /// BAL method: BranchwiseCourseSecPercent_and_Chart (DataAccessLayer.dll ASCII offset 0x1c6c0)
        /// Confirmed: App_Web_gp3pforx.dll UTF-16LE: "{PROC_CLASSWISE_COUNT.SEM}" Crystal Report
        ///   formula field reference near "ClassWiseCnt.rpt" + "BranchWisePercent"
        /// </summary>
        public async Task<IEnumerable<object>> GetDataAsync(
            string course, string examMY, string regu, string sem, bool isRv)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.PROC_CLASSWISE_COUNT,
                "@Course", "@ExamMY", "@Regu", "@Sem", "@IsRv");

            var parameters = new[]
            {
                new SqlParameter("@Course", SqlDbType.VarChar, 30) { Value = course ?? string.Empty },
                new SqlParameter("@ExamMY", SqlDbType.VarChar, 20) { Value = examMY ?? string.Empty },
                new SqlParameter("@Regu",   SqlDbType.VarChar, 10) { Value = regu   ?? string.Empty },
                new SqlParameter("@Sem",    SqlDbType.VarChar, 5)  { Value = sem    ?? string.Empty },
                new SqlParameter("@IsRv",   SqlDbType.VarChar, 1)  { Value = isRv ? "Y" : "N" }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }
    }
}
