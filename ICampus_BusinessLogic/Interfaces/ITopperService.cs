using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface ITopperService
    {
        /// <summary>
        /// Load Batch dropdown (DDLBatch)
        /// Raw SQL: SELECT DISTINCT REGU, BATCH FROM TBL_COURSE WHERE COURSE = @Course
        /// </summary>
        Task<IEnumerable<object>> LoadBatchAsync(string course);

        /// <summary>
        /// Load max semester for selected batch (DDLBatch_SelectedIndexChanged → auto-fills txtSemTo)
        /// Raw SQL: SELECT DISTINCT max(SEM) FROM tbl_sh WHERE REGU = @REGU
        /// </summary>
        Task<IEnumerable<object>> LoadMaxSemAsync(string regu);

        /// <summary>
        /// Get Toppers List — overall (PROC_TOPPERSLIST_NEW)
        /// Called when chksemwise is NOT checked (btnToppersList_Click)
        /// params: @Course, @REGU, @Sem, @NoOfToppers, @Branch, @Caste, @Gender, @WithRv
        /// Returns: REGNO, SNAME, COURSE, BRANCH, CASTE, SEM, GENDER,
        ///          TOTAL GRADE POINTS, TOTAL CREDITS, SGPA, CGPA, RANK, exammy, regsup
        /// </summary>
        Task<IEnumerable<object>> GetToppersListAsync(
            string regulation, string course, string regu, string sem,
            string exammy, string rank, string branch, string caste, string gender, string rv);

        Task<IEnumerable<object>> GetToppersListSemWiseAsync(
            string regulation, string course, string regu, string sem,
            string exammy, string rank, string branch, string caste, string gender, string rv);
    }
}
