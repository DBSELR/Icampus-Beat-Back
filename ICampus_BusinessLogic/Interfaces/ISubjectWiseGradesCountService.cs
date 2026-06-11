using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface ISubjectWiseGradesCountService
    {
        /// <summary>
        /// Load Batch (Regulation) dropdown — ddlbatch (Page_Load)
        /// SQL: SELECT DISTINCT CAST(REGU AS VARCHAR) AS REGU,
        ///      '20'+CAST(REGU AS VARCHAR)+'-'+CAST(REGU+MAXSEM/2 AS VARCHAR) BATCH
        ///      FROM TBL_COURSE WHERE COURSE=@Course ORDER BY REGU
        /// </summary>
        Task<IEnumerable<object>> LoadBatchesAsync(string course);

        /// <summary>
        /// Load Semester dropdown — ddlSemester (Page_Load)
        /// ddlSemester has AutoPostBack → ddlSemester_SelectedIndexChanged auto-reloads CR
        /// SQL: SELECT DISTINCT cast(SEM as varchar(250)) SEM FROM tbl_sh
        ///      WHERE COURSE=@Course AND EXAMMY=@ExamMY
        /// </summary>
        Task<IEnumerable<object>> LoadSemsAsync(string course, string examMY);

        /// <summary>
        /// Get Results Grade Analysis Crystal Report data (btnview_Click)
        /// SP: PROC_GRADE_CNT  params (4): @Course, @ExamMY, @Regu, @Sem
        /// Crystal Report: ResGradesecwise.rpt
        /// Title: "RESULTS GRADE ANALYSIS" (or "READMIT RESULTS GRADE ANALYSIS" when ChkIsreadmitresult checked)
        /// Note: ChkIsreadmitresult is frontend-only — same SP called regardless
        /// Confirmed: DataAccessLayer.dll UTF-16LE US heap offset 162245
        /// </summary>
        Task<IEnumerable<object>> GetViewDataAsync(
            string course, string examMY, string regu, string sem);

        /// <summary>
        /// Get Results Grade Analysis Excel export data (btnexcel_Click)
        /// SP: PROC_GRADE_CNT_EXCEL  params (4): @Course, @ExamMY, @Regu, @Sem
        /// Confirmed: App_Web_gp3pforx.dll UTF-16LE offset 191010
        /// </summary>
        Task<IEnumerable<object>> GetExcelDataAsync(
            string course, string examMY, string regu, string sem);
    }
}
