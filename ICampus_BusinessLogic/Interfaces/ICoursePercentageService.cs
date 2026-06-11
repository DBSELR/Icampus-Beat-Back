using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface ICoursePercentageService
    {
        // ── Reports/CoursePercentage.aspx ────────────────────────────────────────
        // Title: Course Percentage Report
        // Controls: ddlSemester, ChkIsrv (RV checkbox), Chkregsup (Sup checkbox), btnView, btnDownLoad
        // Course and ExamMY come from session (user context)
        // Crystal Report: App_Data/Reports/Results/CoursePercent.rpt
        // BAL: CoursePercentage_and_Chart, Branch_Wise_Course_Percentage_LoadSemesters
        // Confirmed: DataAccessLayer.dll ASCII offset 116342

        /// <summary>
        /// Load Semester dropdown (ddlSemester — Page_Load)
        /// Inline SQL: SELECT DISTINCT cast(SEM as varchar(250)) SEM
        ///   FROM tbl_sh WHERE COURSE=@Course AND EXAMMY=@ExamMY ORDER BY SEM
        /// Confirmed: DataAccessLayer.dll UTF-16LE US heap offset 159480
        /// BAL: Branch_Wise_Course_Percentage_LoadSemesters
        /// </summary>
        Task<IEnumerable<object>> LoadSemsAsync(string course, string examMY);

        /// <summary>
        /// Get Course Percentage data (btnView_Click / btnDownLoad_Click)
        /// Regular/RV mode → SP: sp_PAP_PERCENT (@Course, @ExamMY, @Regu, @Sem)
        /// Supply mode     → SP: SP_Pap_Percent_Sup (@Course, @ExamMY, @Regu, @Sem)
        /// Returns: PAP_STAT rows — PCODE, PNAME, GRP, SEM, appeared, passed, % per paper
        /// Confirmed: DataAccessLayer.dll UTF-16LE US heap offsets 160036 (regular), 160090 (supply)
        /// BAL: CoursePercentage_and_Chart
        /// </summary>
        Task<IEnumerable<object>> GetDataAsync(string course, string examMY, string regu, string sem, bool isSup, bool isRv);
    }
}
