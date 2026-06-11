using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IOdDataService
    {
        // ── ODLIST_JBIET.aspx & OD_RegnoWise_subjectData.aspx ─────────────────
        // Title: "OD DATA" / "OD List" / "OD RegnoWise Subject Data"
        // Both pages share identical controls (ddlBatch, ddlBranch, txtRegno, ChkLateral)

        /// <summary>
        /// Load Batch dropdown (Page_Load)
        /// SQL: SELECT DISTINCT REGU, '20'+REGU+'-'+CAST(REGU+MAXSEM/2 AS VARCHAR) BATCH
        ///      FROM TBL_COURSE WHERE COURSE=@Course ORDER BY REGU
        /// BAL: BAL_MRF / BAL_StudentHistory → Load_Batch
        /// </summary>
        Task<IEnumerable<object>> LoadBatchAsync(string course);

        /// <summary>
        /// Load Branch dropdown (ddlBatch AutoPostBack → cascade)
        /// SQL: SELECT DISTINCT GRP, GRP+' - '+GSUB BRANCH FROM TBL_COURSE
        ///      WHERE COURSE=@Course AND REGU=@Regu ORDER BY GRP
        /// BAL: BAL_MRF / BAL_StudentHistory → Load_Branch
        /// </summary>
        Task<IEnumerable<object>> LoadBranchAsync(string course, string regu);

        /// <summary>
        /// Get OD (On Duty) subject list — both pages (btnExcel_Click)
        /// DAL method: ODLIST_REGNO_SUBJECTS (DAL_MRF)
        /// SQL: SELECT S.REGNO, D.SNAME, S.SEM, S.GRP BRANCH, S.PCODE, S.PNAME,
        ///           CONVERT(VARCHAR,S.EDATE,106) EDATE, S.EXAMMY, S.REGU, S.COURSE
        ///      FROM TBL_SH S LEFT JOIN TBL_STDDATA D ON S.REGNO=D.REGNO
        ///      WHERE S.CODE='OD' AND S.COURSE=@Course AND S.REGU=@REGU
        ///        AND (@Branch='' OR S.GRP=@Branch)
        ///        AND (@RegNo='' OR S.REGNO=@RegNo)
        ///        AND (@IsLateral=0 OR D.LATERAL='Y')
        ///      ORDER BY S.REGNO, S.SEM, S.PCODE
        /// BAL: BAL_MRF → Load_OD_Data; BAL_StudentHistory → Load_OD_Data
        /// Confirmed: DAL method name ODLIST_REGNO_SUBJECTS in DataAccessLayer.dll ASCII heap
        /// </summary>
        Task<IEnumerable<object>> GetOdListAsync(string course, string regu, string branch, string regNo, bool isLateral);

        /// <summary>
        /// Apply OD gracing — ODLIST_JBIET.aspx chkgracing / btnExcel_Click (Load_Btech_OD_AddGracing_R16)
        /// Calls 3 SPs in sequence:
        ///   1. PROC_GRACING_GRAFTING         — applies gracing to OD students (main marks)
        ///   2. PROC_GRACING_GRAFTING_MRK_UPDATE — updates marks table for graced students
        ///   3. PROC_GRACING_GRAFTING_SH_UPDATE  — updates TBL_SH exam-date ('01-'+ExamMY)
        /// BAL: BAL_MRF → Load_Btech_OD_AddGracing_R16
        /// App_Web setters: set_Batch(REGU), set_BRANCH, set_ExamMy, set_UserID, set_Reg_Letrl
        /// Confirmed: DAL methods Flotation_GRACING_GRAFTING + Flotation_GRACING_MRK_UPDATE +
        ///            Flotation_GRACING_GRAFTING_SH_UPDATE in DataAccessLayer.dll ASCII heap
        /// </summary>
        Task<int> RunGracingAsync(string course, string regu, string branch, string examMy, string userId, string regLetrl);
    }
}
