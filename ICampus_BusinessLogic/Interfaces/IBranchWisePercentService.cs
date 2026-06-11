using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IBranchWisePercentService
    {
        // ── Reports/BranchWisePercent.aspx ──────────────────────────────────────
        // Title: "Branch wise Percent"
        // Class: iCampus_Results_BranchWisePercent, App_Web_gp3pforx
        // Controls: ddlSemester, ChkIsrv (RV/SM/GR), Chkregsup (Sup), btnView, btnDownLoad
        // Crystal Report: BranchWisePercent.rpt
        //   Title: 'Before RV/SM/GR' or 'After RV/SM/GR' based on ChkIsrv
        // BAL: BRANCHWISE_PERCENT_Chart
        // Confirmed: DataAccessLayer.dll ASCII offset 116317, BusinessAccessLayer.dll ASCII offset 49113

        /// <summary>
        /// Load Semester dropdown (Page_Load)
        /// Inline SQL: SELECT DISTINCT cast(SEM as varchar(250)) SEM
        ///   FROM tbl_sh WHERE COURSE=@Course AND EXAMMY=@ExamMY ORDER BY SEM
        /// Confirmed: DataAccessLayer.dll UTF-16LE US heap offset 159480
        /// </summary>
        Task<IEnumerable<object>> LoadSemsAsync(string course, string examMY);

        /// <summary>
        /// Get Branch-wise Pass Percentage data (btnView_Click / btnDownLoad_Click)
        /// SP: sp_COURSE_STAT (@Course, @ExamMY, @Regu, @Sem, @IsRv)
        /// @IsRv: 'N' = regular (ChkIsrv unchecked), 'Y' = after RV/SM/GR (ChkIsrv checked)
        /// Returns: branch-wise pass percentage per semester
        /// Confirmed: DataAccessLayer.dll UTF-16LE US heap offset 159742
        ///   pattern "sp_COURSE_STAT '" + "', 'N','" → IsRv flag
        /// BAL: BRANCHWISE_PERCENT_Chart
        /// </summary>
        Task<IEnumerable<object>> GetDataAsync(string course, string examMY, string regu, string sem, bool isRv, bool isSup);
    }
}
