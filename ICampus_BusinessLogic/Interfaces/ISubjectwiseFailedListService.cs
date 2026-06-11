using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface ISubjectwiseFailedListService
    {
        /// <summary>
        /// Load Batch dropdown (ddlBatch on Page_Load)
        /// Raw SQL: SELECT DISTINCT REGU,'20'+REGU+'-'+CAST(REGU+MAXSEM/2 AS VARCHAR) BATCH
        ///          FROM TBL_COURSE WHERE COURSE=@Course ORDER BY REGU
        /// </summary>
        Task<IEnumerable<object>> LoadBatchAsync(string course);

        /// <summary>
        /// Load Semester dropdown (ddlSemester — ddlBatch_SelectedIndexChanged)
        /// Raw SQL: SELECT DISTINCT CAST(SEM AS VARCHAR) SEM
        ///          FROM TBL_SH WHERE COURSE=@Course AND REGU=@REGU ORDER BY SEM
        /// </summary>
        Task<IEnumerable<object>> LoadSemestersAsync(string course, string regu);

        /// <summary>
        /// Load Branch dropdown (ddlBranch — ddlSemester_SelectedIndexChanged)
        /// Raw SQL: SELECT DISTINCT GRP
        ///          FROM TBL_SH WHERE COURSE=@Course AND REGU=@REGU AND SEM=@Sem ORDER BY GRP
        /// </summary>
        Task<IEnumerable<object>> LoadBranchesAsync(string course, string regu, string sem);

        /// <summary>
        /// Load Subject dropdown (ddlPcode — ddlBranch_SelectedIndexChanged)
        /// Confirmed SQL: select distinct pno, pcode, pcode+'-'+pname pname
        ///                from tbl_sh where Course=@Course and Regu=@REGU and grp=@Branch and sem=@Sem
        ///                order by pno
        /// </summary>
        Task<IEnumerable<object>> LoadSubjectsAsync(string course, string regu, string sem, string branch);

        /// <summary>
        /// Get Subjectwise Failed List grid data (btnView_Click)
        /// SP: SP_SubJ_FAILEDLIST_NEW
        /// params: @regulation varchar(20), @course varchar(20), @EXAMMY varchar(25), @sem varchar(20)
        /// @sem = '' → no sem filter (all semesters)
        /// Returns: COURSE, GRP, REGU, REGNO, PCODE, PNAME, SEM, ORD, EXAMMY, RV, Regulation
        /// </summary>
        Task<IEnumerable<object>> GetFailedListAsync(
            string regulation, string course, string examMY, string sem);
    }
}
