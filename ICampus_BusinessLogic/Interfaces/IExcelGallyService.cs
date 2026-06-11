using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IExcelGallyService
    {
        // ── EXCEL_GALLY.aspx (Results/EXCEL_GALLY.aspx) ─────────────────────
        // Title: "Result Sheet Excel Export"

        /// <summary>
        /// Load Semester dropdown (Page_Load)
        /// SQL: SELECT DISTINCT CAST(SEM AS VARCHAR(250)) SEM FROM TBL_GPAP
        ///      WHERE COURSE = @Course ORDER BY SEM
        /// BAL: BAL_CGPA_Yearwise → Load_Sems_Data (DAL method)
        /// Confirmed: DataAccessLayer.dll UTF-16LE "TBL_GPAP WHERE   COURSE = '"
        /// </summary>
        Task<IEnumerable<object>> LoadSemsAsync(string course);

        /// <summary>
        /// Load Branch dropdown (ddlSemester AutoPostBack → Page_Load cascade)
        /// SQL: SELECT DISTINCT GRP FROM TBL_GPAP
        ///      WHERE COURSE = @Course AND SEM = @Sem ORDER BY GRP
        /// </summary>
        Task<IEnumerable<object>> LoadBranchesAsync(string course, string sem);

        /// <summary>
        /// Export Result Sheet data (btnExcelExort_Click)
        /// SP: PROC_EXCEL_GALLY (@Regulation, @Course, @ExamMy, @Grp, @REGSUP, @Sem)
        /// @REGSUP = 'Reg' | 'Sup' (from ddlSemType)
        /// Confirmed: EXEC PROC_EXCEL_GALLY 'R20','B.TECH','May-2024','CE','Reg','8' → 79 rows
        /// </summary>
        Task<IEnumerable<object>> GetExcelGallyAsync(string regulation, string course, string examMY, string branch, string regsup, string sem);

        /// <summary>
        /// Export Backlogs (failed subjects with marks) data (btnBacklogsExcelExport_Click)
        /// SP: PROC_GETfiledasubwithmarks — NOT FOUND in DB, returns empty
        /// </summary>
        Task<IEnumerable<object>> GetBacklogsExcelAsync(string regulation, string course, string examMY, string branch, string regsup, string sem);
    }
}
