using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IResultGradeSheetService
    {
        // ── Reports/ResultGradeSheet.aspx (Grade Card) ───────────────────────────
        // Title: "Results Sheet" / "Revaluation Results Sheet" / "(Re-admitted)" variants
        // Class: iCampus_Reports_ResultGradeSheet, App_Web_oxqewfcs
        // Controls: ddlSemester, chkRv (Revaluation), ChkIsreadmitresult (Is Readmit Result),
        //           btnView, btnDownLoad, txtreadmitReulation (modal popup for readmit reg),
        //           btnreadmitok (confirm readmit)
        // Crystal Report: GradeCheckList.rpt
        // SP selection based on flags:
        //   isRv=false, isReadmit=false → SP_REP_GRADE_CHKLIST
        //   isRv=true,  isReadmit=false → SP_REP_GRADE_CHKLIST_RV
        //   isRv=false, isReadmit=true  → SP_REP_GRADE_CHKLIST_Readmit + readmitRegu
        //   isRv=true,  isReadmit=true  → SP_REP_GRADE_CHKLIST_RV_Readmit + readmitRegu
        // Confirmed: App_Web_oxqewfcs.dll UTF-16LE US heap offsets 48901/48841/49063/48993

        /// <summary>
        /// Load Semester dropdown (Page_Load)
        /// Inline SQL: SELECT DISTINCT cast(SEM as varchar(250)) SEM
        ///   FROM tbl_sh WHERE COURSE=@Course AND EXAMMY=@ExamMY ORDER BY SEM
        /// </summary>
        Task<IEnumerable<object>> LoadSemsAsync(string course, string examMY);

        /// <summary>
        /// Get Grade Sheet data (btnView_Click / btnreadmitok_Click / btnDownLoad_Click)
        /// SP selected by isRv + isReadmit flags:
        ///   SP_REP_GRADE_CHKLIST         — regular
        ///   SP_REP_GRADE_CHKLIST_RV      — after revaluation
        ///   SP_REP_GRADE_CHKLIST_Readmit — readmit students
        ///   SP_REP_GRADE_CHKLIST_RV_Readmit — RV + readmit
        /// @ReadmitRegu required when isReadmit=true (from txtreadmitReulation modal)
        /// </summary>
        Task<IEnumerable<object>> GetDataAsync(
            string course, string examMY, string regu, string sem,
            bool isRv, bool isReadmit, string readmitRegu);
    }
}
