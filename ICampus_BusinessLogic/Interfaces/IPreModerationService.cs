using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IPreModerationService
    {
        // ── Reports/PREMODERATION.aspx ───────────────────────────────────────
        // Title: "PREMODERATION REPORT"
        // Controls: ddlsem (Semester + AutoPostBack), btnExport (Export)
        // Course and ExamMY come from session (user context)
        // Crystal Report: App_Data/Reports/Results/PreModeration.rpt
        // BAL: Bal_Reports_Results → PreModeration()
        // BAL method confirmed in DataAccessLayer.dll ASCII heap adjacent to
        //   Update_Moderation and PROC_MODERATION_REG_SEM_GRP_PAP

        /// <summary>
        /// Load Semester dropdown (ddlsem — Page_Load / ddlsem_SelectedIndexChanged)
        /// Inline SQL: SELECT DISTINCT SEM FROM TBL_SH
        ///   WHERE REGULATION=@Regulation AND EXAMMY=@ExamMY AND COURSE=@Course
        ///   ORDER BY SEM
        /// Confirmed: App_Web_gp3pforx.dll UTF-16LE US heap (shared SELECT DISTINCT SEM pattern)
        /// </summary>
        Task<IEnumerable<object>> LoadSemsAsync(string course, string examMY, string regulation);

        /// <summary>
        /// Get PreModeration data (btnExport_Click — data fed into Crystal Report)
        /// SP: PROC_MODERATION_REG_SEM_GRP_PAP (@Course, @ExamMY, @Regu, @Sem, @Grp='', @PapCode='')
        /// Passing @Grp='' and @PapCode='' returns ALL branches and papers for the semester
        /// BAL: Bal_Reports_Results → PreModeration()
        /// Confirmed: BAL PreModeration is adjacent to Update_Moderation (PROC_MODERATION_REG_SEM_GRP_PAP)
        ///   in BusinessAccessLayer.dll ASCII heap
        /// Returns: REGNO, SNAME, GRP, PCODE, PNAME, SEM, SMARKS, TMARKS, MMARKS, PMARKS
        /// </summary>
        Task<IEnumerable<object>> GetDataAsync(string course, string examMY, string regu, string sem);
    }
}
