using ICampus_Models.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IInternalChecklistService
    {
        /// <summary>
        /// Generate Internal Check List report data
        /// Calls SP_REP_INTERNAL_CHKLIST
        /// 5 positional params confirmed from DLL IL (App_Web_oxqewfcs.dll loadingInternalCheckList):
        ///   Course(1), ExamMY(2), Regulation(3), GRP(4, optional), SEM(5, optional)
        /// Returns raw rows — column names depend on SP output (Crystal Reports dataset)
        /// </summary>
        Task<IEnumerable<object>> GetReportDataAsync(InternalChecklistRequest request);

        /// <summary>
        /// Load semester list for InternalChecklist (and PracticalChecklist — same DAL method)
        /// Inline SQL on TBL_GPAP: SELECT DISTINCT CAST(SEM AS VARCHAR(250)) SEM FROM TBL_GPAP WHERE COURSE=@Course AND REGU=@Regulation
        /// Confirmed from DLL IL: DataAccessLayer.dll method InternalCheckList_Semester
        /// Triggered by ddlBatch_SelectedIndexChanged in InternalChecklist.aspx
        /// </summary>
        Task<IEnumerable<object>> GetSemestersAsync(string course, string regulation);

        /// <summary>
        /// Load branch list for InternalChecklist (and PracticalChecklist — same DAL method)
        /// Inline SQL on TBL_COURSE: SELECT DISTINCT GSUB GRP, GRP ID FROM TBL_COURSE WHERE COURSE=@Course AND REGU=@Regulation ORDER BY ID
        /// Confirmed from DLL IL: DataAccessLayer.dll method InternalCheckList_Branch
        /// Triggered by ddlSemester_SelectedIndexChanged in InternalChecklist.aspx
        /// </summary>
        Task<IEnumerable<object>> GetBranchesAsync(string course, string regulation);
    }
}
