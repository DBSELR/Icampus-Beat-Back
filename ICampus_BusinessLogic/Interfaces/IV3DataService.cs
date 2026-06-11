using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IV3DataService
    {
        // ── Results/V3_Data.aspx ─────────────────────────────────────────────────
        // Title: "V3 Data"  (Second Revaluation / RV2 data)
        // Controls: ddlSem (Semester), txtdiffmarks (Marks Difference),
        //           chkReadmit (IsReadmit checkbox), btnView (View), btnDownLoad (Export)
        // Modal popup for readmit: txtreadmireulation (Readmit Regulation)
        // BAL: iCampus_Results_V3_Data class (App_Web_m2jhophz.dll)

        /// <summary>
        /// Load Semester dropdown (Page_Load)
        /// Inline SQL: SELECT DISTINCT SEM FROM tbl_sh
        ///   WHERE COURSE=@Course AND REGULATION=@Regulation AND EXAMMY=@ExamMY
        ///   ORDER BY SEM
        /// </summary>
        Task<IEnumerable<object>> LoadSemestersAsync(string course, string regulation, string examMY);

        /// <summary>
        /// Get V3 (Second Revaluation) data (btnView_Click)
        /// SP: PROC_DATAFOR_V3          — when isReadmit = false
        ///   params (5): @Course, @Regulation, @ExamMY, @Sem, @DiffMarks
        /// SP: PROC_DATAFOR_V3_READMIT  — when isReadmit = true
        ///   params (6): @Course, @Regulation, @ExamMY, @Sem, @DiffMarks, @ReadmitReg
        /// @DiffMarks: minimum marks difference threshold (txtdiffmarks)
        /// @ReadmitReg: readmit student regulation (txtreadmireulation from modal popup)
        /// Returns: AutoGenerateColumns grid — column list varies by SP version
        /// Confirmed: DataAccessLayer.dll UTF-16LE US heap exec templates
        /// </summary>
        Task<IEnumerable<object>> GetV3DataAsync(
            string course, string regulation, string examMY,
            string sem, string diffMarks,
            bool isReadmit, string readmitReg);
    }
}
