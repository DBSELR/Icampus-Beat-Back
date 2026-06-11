using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IMtechCmmService
    {
        // ── Reports/MTECHCMM.aspx ───────────────────────────────────────────────
        // Title: "MTECH CMM REPORT"
        // Class: iCampus_Reports_MTECHCMM, App_Web_oxqewfcs
        // Controls: btnDownLoad (hidden), btnPrint — NO filter controls
        // NO filter controls — data loads on Page_Load with session Course+ExamMY+Regu
        // Crystal Report: MTECHCMM.rpt
        // BAL: MTECHCMM → Load_Btech_CMM (DAL method shared with BTECHCMM/MBACMM)
        //   Confirmed: DataAccessLayer.dll ASCII offset 102636
        //   BusinessAccessLayer.dll ASCII offset 36091
        // SP: SP_CMM (@Course, @ExamMY, @Regu)
        //   (BTECHCMM uses variants: SP_CMM_R11_PG, SP_CMM_R18, SP_CMM_AddGracing based on UI;
        //    MTECHCMM has no UI controls → uses SP_CMM directly)
        // Confirmed: DataAccessLayer.dll UTF-16LE US heap offsets 159387/159411

        /// <summary>
        /// Get MTECH Consolidated Marks Memo data (Page_Load / btnDownLoad_Click)
        /// SP: SP_CMM (@Course, @ExamMY, @Regu)
        /// All params from session — no user-facing filter controls
        /// </summary>
        Task<IEnumerable<object>> GetDataAsync(string course, string examMY, string regu, string grp);
    }
}
