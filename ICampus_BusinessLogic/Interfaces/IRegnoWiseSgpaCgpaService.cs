using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IRegnoWiseSgpaCgpaService
    {
        /// <summary>
        /// Load Batch dropdown (ddlBatch → ddlBatch_SelectedIndexChanged)
        /// SQL: SELECT DISTINCT REGU,'20'+REGU+'-'+CAST(REGU+MAXSEM/2 AS VARCHAR) BATCH
        ///      FROM TBL_COURSE WHERE COURSE=@Course ORDER BY REGU
        /// BAL method: SgpaCgpaList_Batch (Bal_Reports_Results)
        /// </summary>
        Task<IEnumerable<object>> LoadBatchAsync(string course);

        /// <summary>
        /// Load Semester dropdown (ddlSemester)
        /// SQL: SELECT DISTINCT CAST(SEM AS VARCHAR) SEM FROM TBL_SH
        ///      WHERE COURSE=@Course AND REGU=@REGU ORDER BY SEM
        /// BAL method: SgpaCgpaList_Semester (Bal_Reports_Results)
        /// </summary>
        Task<IEnumerable<object>> LoadSemestersAsync(string course, string regu);

        /// <summary>
        /// Load SGPA and CGPA grid — Regular AND Supply exams considered (RegularAndSup_Click)
        /// SP: PROC_GET_CGPA_SGPA_EXCEL (@Regulation, @regu, @course, @exammy, @sem INT)
        /// BAL method: loadgridview → get_rgnowise_SgpaCgpa_Avg (Bal_Reports_Results)
        /// </summary>
        Task<IEnumerable<object>> GetListAsync(string regulation, string course, string regu, string exammy, string sem);

        /// <summary>
        /// Load SGPA and CGPA grid — Regular exams only, Supply NOT considered (RegularOnly_Click)
        /// SP: PROC_SGPA_AVERAGE (@REGULATION, @REGU, @COURSE, @EXAMMY, @SEM INT)
        /// BAL method: loadgridview_WithOutSupply → get_rgnowise_SgpaCgpa_Avg_RegularOnly (Bal_Reports_Results)
        /// </summary>
        Task<IEnumerable<object>> GetListRegularOnlyAsync(string regulation, string course, string regu, string exammy, string sem);
    }
}
