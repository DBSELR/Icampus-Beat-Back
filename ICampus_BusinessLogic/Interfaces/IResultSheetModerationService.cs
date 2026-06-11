using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IResultSheetModerationService
    {
        /// <summary>
        /// Load Semester dropdown — ddlSem (Page_Load)
        /// SQL: SELECT DISTINCT SEM FROM TBL_SH WHERE REGULATION=@Regu
        ///      AND EXAMMY=@ExamMY AND COURSE=@Course ORDER BY SEM
        /// Confirmed: App_Web_gp3pforx.dll UTF-16LE offset 0x2e10b (Moderation page)
        /// </summary>
        Task<IEnumerable<object>> LoadSemsAsync(string course, string examMY, string regu);

        /// <summary>
        /// Get Result Sheet - Subject Moderation data (btnview_Click)
        /// isReadmit=false → SP_REP_MRK_CHKLIST_SMFLAG (4 params): @Course, @ExamMY, @Regu, @Sem
        /// isReadmit=true  → SP_REP_MRK_CHKLIST_Readmit_SMFLAG (5 params): @Course, @ExamMY, @Regu, @Sem, @ReadmitRegu
        /// Crystal Report: ResTR_SMFLAG.rpt
        /// Confirmed: App_Web_gp3pforx.dll offset 0x2e10b
        /// </summary>
        Task<IEnumerable<object>> GetDataAsync(
            string course, string examMY, string regu, string sem,
            bool isReadmit, string readmitRegu);
    }
}
