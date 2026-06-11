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
    public class RvSummeryReportService : IRvSummeryReportService
    {
        private readonly IGenericRepository<object> _repo;

        public RvSummeryReportService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// RV Summary report data (btnExport_Click "Summary of RV Report" and btnRVExportExcel_Click)
        /// SP: Proc_RV_Summary
        /// Params (2): @Regulation, @ExamMY
        /// Source: RV_Summery_Report.aspx → App_Web_zjsy5om4.dll → loadingRVSummery → RV_Summary()
        ///   Session["Regulation"] + Session["ExamMY"] → now API query params
        /// Crystal: no fixed name (rendered inline in CrystalReportViewer2)
        /// Confirmed: DataAccessLayer.dll UTF-16LE 0x2699e: "[Proc_RV_Summary] '⼁"
        /// </summary>
        public async Task<IEnumerable<object>> GetRvSummaryAsync(string regulation, string course, string examMY)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.Proc_RV_Summary,
                "@regulation", "@Course", "@Exammy");

            var parameters = new object[]
            {
                new SqlParameter("@regulation", SqlDbType.VarChar, 50) { Value = regulation ?? string.Empty },
                new SqlParameter("@Course",     SqlDbType.VarChar, 50) { Value = course     ?? string.Empty },
                new SqlParameter("@Exammy",     SqlDbType.VarChar, 50) { Value = examMY     ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Supply Summary Excel export data (ExcelExport_Click "Summary of Supply Excel")
        /// SP: Proc_Supply_Summary
        /// Params (2): @Regulation, @ExamMY
        /// Source: RV_Summery_Report.aspx → App_Web_zjsy5om4.dll → Supply_Summary()
        ///   Session["Regulation"] + Session["ExamMY"] → now API query params
        /// Confirmed: DataAccessLayer.dll UTF-16LE 0x2699e: "[Proc_Supply_Summary] '㔁"
        ///   adjacent to Proc_RV_Summary in the same BAL class method sequence
        /// </summary>
        public async Task<IEnumerable<object>> GetSupplySummaryAsync(string regulation, string course, string examMY)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.Proc_Supply_Summary,
                "@regulation", "@Course", "@Exammy");

            var parameters = new object[]
            {
                new SqlParameter("@regulation", SqlDbType.VarChar, 50) { Value = regulation ?? string.Empty },
                new SqlParameter("@Course",     SqlDbType.VarChar, 50) { Value = course     ?? string.Empty },
                new SqlParameter("@Exammy",     SqlDbType.VarChar, 50) { Value = examMY     ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, parameters);
            return result ?? Enumerable.Empty<object>();
        }
    }
}
