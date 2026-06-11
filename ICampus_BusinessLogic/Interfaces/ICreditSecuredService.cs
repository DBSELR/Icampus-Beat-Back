using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface ICreditSecuredService
    {
        /// <summary>
        /// Load ExamMY dropdown (ddlexammy — Page_Load)
        /// SP: SPM_EXAMS_ExamMY_Load (@regulation, @COURSE) — 2 params confirmed
        /// </summary>
        Task<IEnumerable<object>> LoadExammyAsync(string regulation, string course);

        /// <summary>
        /// Load Batch dropdown (DDLBatch — Page_Load)
        /// SQL: SELECT DISTINCT REGU, '20'+REGU+'-'+CAST(REGU+MAXSEM/2 AS VARCHAR) BATCH
        ///      FROM TBL_COURSE WHERE COURSE=@Course ORDER BY REGU
        /// </summary>
        Task<IEnumerable<object>> LoadBatchAsync(string course);

        /// <summary>
        /// Load Branch dropdown (ddlbranch — cascades from DDLBatch)
        /// SQL: SELECT DISTINCT GRP, GRP+' - '+GSUB BRANCH FROM TBL_COURSE
        ///      WHERE COURSE=@Course AND REGU=@Regu ORDER BY GRP
        /// </summary>
        Task<IEnumerable<object>> LoadBranchAsync(string course, string regu);

        /// <summary>
        /// Get Credit Secured data (btnCredit_Click → Get_CreditList)
        /// SP: SP_Credit_Secured (@Regulation, @Course, @Branch, @Regu, @Exammy, @noofcr INT)
        /// Confirmed: EXEC SP_Credit_Secured 'R20','B.TECH','CE','20','May-2024',100 → rows
        /// </summary>
        Task<IEnumerable<object>> GetCreditSecuredAsync(string regulation, string course, string regu, string examMY, string branch, int noofcredits);

        /// <summary>
        /// Get Total Credit Secured data (btnCredittotal_Click → Get_CreditList_total)
        /// SP: SP_Credit_Secured_Total (@Regulation, @Course, @Branch, @Regu, @Exammy, @Sem INT)
        /// Confirmed: EXEC SP_Credit_Secured_Total 'R20','B.TECH','CE','20','May-2024',8 → rows
        /// </summary>
        Task<IEnumerable<object>> GetCreditSecuredTotalAsync(string regulation, string course, string regu, string examMY, string branch, string sem);
    }
}
