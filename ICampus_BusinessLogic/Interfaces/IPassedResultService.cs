using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IPassedResultService
    {
        // ── Reports/PassedResult.aspx ────────────────────────────────────────────
        // Title: "PASSED RESULT" (label text)
        // Class: iCampus_Reports_PassedResult, App_Web_gp3pforx
        // Controls: ddlsem (AutoPostBack + OnSelectedIndexChanged), btnDownLoad (hidden), btnPrint (hidden)
        // No View button — data loads automatically on ddlsem_SelectedIndexChanged
        // Crystal Report: PassedResult.rpt (label "PassedResultList")
        // BAL: PassedResult (DataAccessLayer.dll ASCII offset 115765)

        /// <summary>
        /// Load Semester dropdown (Page_Load)
        /// Inline SQL: SELECT DISTINCT CAST(SEM AS VARCHAR(250)) SEM
        ///   FROM tbl_sh WHERE COURSE=@Course AND Exammy=@ExamMY AND Regulation=@Regu ORDER BY SEM
        /// Confirmed: DataAccessLayer.dll UTF-16LE US heap offset 158863
        /// </summary>
        Task<IEnumerable<object>> LoadSemsAsync(string course, string examMY, string regu);

        /// <summary>
        /// Get Passed Result data (ddlsem_SelectedIndexChanged / btnDownLoad_Click)
        /// SP: SP_PASSEDLIST_NEW (@Course, @ExamMY, @Regu, @Sem)
        /// Returns: passed students list for the selected semester
        /// Confirmed: DataAccessLayer.dll UTF-16LE US heap offset 158822
        /// BAL: PassedResult
        /// </summary>
        Task<IEnumerable<object>> GetDataAsync(string course, string examMY, string regu, string sem);
    }
}
