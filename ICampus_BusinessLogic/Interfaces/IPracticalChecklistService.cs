using ICampus_Models.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IPracticalChecklistService
    {
        /// <summary>
        /// Generate Practical Check List report data
        /// Confirmed from DLL IL: uses same InternalCheckList_data DAL method as InternalChecklist
        ///   - SP: SP_REP_INTERNAL_CHKLIST (NOT SP_REP_PRAC_CHKLIST — not found in any DLL)
        ///   - Table: TBL_REPORT_PERIODICAL
        /// Step 1: Execute SP to populate TBL_REPORT_PERIODICAL
        /// Step 2: SELECT from TBL_REPORT_PERIODICAL
        /// </summary>
        Task<IEnumerable<object>> GetReportDataAsync(PracticalChecklistRequest request);

        /// <summary>
        /// Load semester list for PracticalChecklist — uses same DAL method as InternalChecklist
        /// Inline SQL on TBL_GPAP: SELECT DISTINCT CAST(SEM AS VARCHAR(250)) SEM FROM TBL_GPAP WHERE COURSE=@Course AND REGU=@Regulation
        /// Confirmed from DLL IL: DataAccessLayer.dll method InternalCheckList_Semester (shared)
        /// Triggered by ddlBatch_SelectedIndexChanged in PracticalChecklist.aspx
        /// </summary>
        Task<IEnumerable<object>> GetSemestersAsync(string course, string regulation);

        /// <summary>
        /// Load branch list for PracticalChecklist — uses same DAL method as InternalChecklist
        /// Inline SQL on TBL_COURSE: SELECT DISTINCT GSUB GRP, GRP ID FROM TBL_COURSE WHERE COURSE=@Course AND REGU=@Regulation ORDER BY ID
        /// Confirmed from DLL IL: DataAccessLayer.dll method InternalCheckList_Branch (shared)
        /// Triggered by ddlSemester_SelectedIndexChanged in PracticalChecklist.aspx
        /// </summary>
        Task<IEnumerable<object>> GetBranchesAsync(string course, string regulation);
    }
}
