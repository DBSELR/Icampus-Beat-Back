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
    public class RvClosingDatesService : IRvClosingDatesService
    {
        private readonly IGenericRepository<object> _repo;

        public RvClosingDatesService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Load RV Closing Dates grid (Page_Load → gvRvCloseDate)
        /// SP: SP_RV_CLOSINGDATES_LIST (@REGULATION, @COURSE, @EXAMMY)
        /// BAL method: rvClosingDates_List (BAL_RVRegistrations)
        /// Returns: REGULATION, COURSE, EXAMMY, SEM, RV_CLOSEDATE, RV_CDATE_SUP
        /// Confirmed: EXEC SP_RV_CLOSINGDATES_LIST 'R20','B.TECH','May-2024' → 2 rows
        /// </summary>
        public async Task<IEnumerable<object>> GetClosingDatesAsync(string regulation, string course, string examMY)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.SP_RV_CLOSINGDATES_LIST,
                "@REGULATION", "@COURSE", "@EXAMMY");

            var parameters = new[]
            {
                new SqlParameter("@REGULATION", SqlDbType.VarChar, 10) { Value = regulation ?? string.Empty },
                new SqlParameter("@COURSE",     SqlDbType.VarChar, 30) { Value = course     ?? string.Empty },
                new SqlParameter("@EXAMMY",     SqlDbType.VarChar, 12) { Value = examMY     ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Save RV Closing Dates for one row (btnSave_Click per grid row)
        /// SP: SP_RV_CLOSINGDATES_Update (6 params)
        /// BAL method: rvClosingDates_Update (BAL_RVRegistrations)
        /// Row key: @Regulation + @Course + @ExamMY + @Sem
        /// Editable: @RV_CLOSEDATE (txtReg_DATE), @RV_CDATE_SUP (txtSup_DATE)
        /// Date format: yyyy-MM-dd (confirmed from App_Web DLL string table)
        /// Success message: "Revaluation date(s) updated successfully"
        /// </summary>
        public async Task<int> UpdateClosingDateAsync(
            string regulation, string course, string examMY, string sem,
            string rvCloseDate, string rvCDateSup)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.SP_RV_CLOSINGDATES_Update,
                "@Regulation", "@Course", "@ExamMY", "@Sem", "@RV_CLOSEDATE", "@RV_CDATE_SUP");

            var parameters = new[]
            {
                new SqlParameter("@Regulation",  SqlDbType.VarChar, 20) { Value = regulation ?? string.Empty },
                new SqlParameter("@Course",       SqlDbType.VarChar, 20) { Value = course     ?? string.Empty },
                new SqlParameter("@ExamMY",       SqlDbType.VarChar, 10) { Value = examMY     ?? string.Empty },
                new SqlParameter("@Sem",          SqlDbType.VarChar, 5)  { Value = sem        ?? string.Empty },
                new SqlParameter("@RV_CLOSEDATE", SqlDbType.VarChar, 20) { Value = rvCloseDate ?? string.Empty },
                new SqlParameter("@RV_CDATE_SUP", SqlDbType.VarChar, 20) { Value = rvCDateSup  ?? string.Empty }
            };

            return await _repo.ExecuteStoredProcAsync(sql, parameters);
        }
    }
}
