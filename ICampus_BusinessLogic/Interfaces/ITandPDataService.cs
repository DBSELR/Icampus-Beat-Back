using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface ITandPDataService
    {
        // ── Results/TandPdata.aspx ────────────────────────────────────────────────
        // Title: "Training and Placement Data"
        // Controls: ddlbatch (REGU selector), btnDownLoad ("Excel Download")
        // Course comes from master page session (client passes it for batch loading)
        // BAL/DAL: iCampus_Results_TandPdata (App_Web_m2jhophz.dll)

        /// <summary>
        /// Load Batch dropdown (Page_Load)
        /// SQL: SELECT DISTINCT CAST(REGU AS VARCHAR) AS REGU,
        ///      '20'+CAST(REGU AS VARCHAR)+'-'+CAST(REGU+MAXSEM/2 AS VARCHAR) BATCH
        ///      FROM TBL_COURSE WHERE COURSE=@Course ORDER BY REGU
        /// </summary>
        Task<IEnumerable<object>> LoadBatchAsync(string course);

        /// <summary>
        /// Download T&P data (btnDownLoad_Click)
        /// SP: sp_t_and_p_data(@Regulation, @Course, @batch, @EXAMMY)
        /// </summary>
        Task<IEnumerable<object>> GetTandPDataAsync(string course, string regu, string exammy);
    }
}
