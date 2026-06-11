using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IBranchwiseCoursePercentService
    {
        // ── Reports/BranchwiseCoursePercent.aspx ────────────────────────────────
        // Title: "Branch wise Course Percent"
        // Class: iCampus_Reports_BranchwiseCoursePercent, App_Web_oxqewfcs
        // Controls: ddlSemester (AutoPostBack), ChkSup (Supply), ChkIsrv (RV), btnView, btnDownLoad
        // Crystal Report: BranchwiseCoursePercent.rpt (selection: {Pap_Stat.SEM} = @Sem)
        // BAL: BranchwiseCoursePercent_and_Chart, BranchwiseCoursePercent_and_Chart_Supply
        //      Branch_Wise_Course_Percentage_LoadSemesters
        // Confirmed: App_Web_oxqewfcs.dll UTF-16LE US heap offset 45014

        /// <summary>
        /// Load Semester dropdown (Page_Load / ddlSemester_SelectedIndexChanged)
        /// Inline SQL: SELECT DISTINCT cast(SEM as varchar(250)) SEM
        ///   FROM tbl_sh WHERE COURSE=@Course AND EXAMMY=@ExamMY ORDER BY SEM
        /// BAL: Branch_Wise_Course_Percentage_LoadSemesters
        /// Confirmed: DataAccessLayer.dll UTF-16LE US heap offset 159480
        /// </summary>
        Task<IEnumerable<object>> LoadSemsAsync(string course, string examMY);

        /// <summary>
        /// Get Branch-wise Course Percentage data (btnView_Click / btnDownLoad_Click)
        /// Regular/RV mode → SP: sp_PAP_PERCENT (@Course, @ExamMY, @Regu, @Sem)
        /// Supply mode     → SP: SP_Pap_Percent_Sup (@Course, @ExamMY, @Regu, @Sem)
        /// Returns: PAP_STAT rows grouped by branch — paper-wise pass % per branch
        /// BAL: BranchwiseCoursePercent_and_Chart / BranchwiseCoursePercent_and_Chart_Supply
        /// Confirmed: App_Web_oxqewfcs.dll ASCII heap
        /// </summary>
        Task<IEnumerable<object>> GetDataAsync(string course, string examMY, string regu, string sem, bool isSup, bool isRv);
    }
}
