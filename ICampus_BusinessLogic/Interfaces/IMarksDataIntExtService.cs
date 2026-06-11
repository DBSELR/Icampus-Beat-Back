using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IMarksDataIntExtService
    {
        /// <summary>
        /// Load Batch dropdown (ddlbatch — Page_Load)
        /// SQL: SELECT DISTINCT REGU FROM TBL_COURSE ORDER BY REGU
        /// BAL: Bal_Reports_Source (bal_RRS) → Load_Regu
        /// </summary>
        Task<IEnumerable<object>> LoadBatchAsync();

        /// <summary>
        /// Load ExamMY dropdown (cmbExamMY — cascades from Batch)
        /// SQL: SELECT DISTINCT EXAMMY FROM TBL_SH WHERE REGU=@Regu ORDER BY EXAMMY DESC
        /// Pass regu='' or omit to load all exam months
        /// </summary>
        Task<IEnumerable<object>> LoadExammyAsync(string regu);

        /// <summary>
        /// Load Semester dropdown (cmbSemester — cascades from Batch + ExamMY)
        /// SQL: SELECT DISTINCT CAST(SEM AS VARCHAR) SEM FROM TBL_SH WHERE REGU=@Regu AND EXAMMY=@ExamMY ORDER BY SEM
        /// </summary>
        Task<IEnumerable<object>> LoadSemestersAsync(string regu, string examMY);

        /// <summary>
        /// Get marks data for university formats 1–3 and 5 (btnView_Click → loadexcel)
        /// SP: PROC_EXPORT_MARKSDATA (@EXAMMY, @REGULATION, @COURSE, @SEM INT, @REGU)
        /// Confirmed: EXEC PROC_EXPORT_MARKSDATA 'May-2024','R20','B.TECH',8,'20' → rows
        /// </summary>
        Task<IEnumerable<object>> GetShDataAsync(string regulation, string course, string regu, string examMY, string sem);

        /// <summary>
        /// Get marks data for Format 4 — V1, RV, V3 Month &amp; Year Wise (rbtnfinalmarks)
        /// SP: PROC_EXPORT_RES_DATA (@regulation, @course, @exammy, @sem INT)
        /// Confirmed: EXEC PROC_EXPORT_RES_DATA 'R20','B.TECH','May-2024',8 → rows
        /// </summary>
        Task<IEnumerable<object>> GetResultDataAsync(string regulation, string course, string examMY, string sem);
    }
}
