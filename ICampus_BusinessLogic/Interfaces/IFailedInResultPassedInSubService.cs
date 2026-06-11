using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IFailedInResultPassedInSubService
    {
        // ── Reports/Failed_inResult_Passed_inSub.aspx ────────────────────────────
        // Title: (label "PASSED RESULT") — students who failed overall but passed in subjects
        // Class: iCampus_Reports_Failed_inResult_Passed_inSub, App_Web_gp3pforx
        // Controls: btnDownLoad (hidden), btnPrint (hidden)
        // NO filter controls — data loads on Page_Load with session Course+ExamMY
        // Crystal Report: FailedResult.rpt (label "Passed_List Subjectwise")
        //   CR selection: {tbl_SH.PMARKS} in ['ab','sm'] — absent/subject-marked filter
        // Confirmed: App_Web_gp3pforx.dll UTF-16LE US heap offset 200341

        /// <summary>
        /// Get students who failed in semester result but passed in individual subjects (Page_Load)
        /// SP: SP_SubJ_FAILEDLIST_NEW (@regulation, @course, @EXAMMY, @sem='')
        /// No semester filter — returns all semesters for the exam
        /// </summary>
        Task<IEnumerable<object>> GetDataAsync(string regulation, string course, string examMY);
    }
}
