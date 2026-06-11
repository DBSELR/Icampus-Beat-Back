using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IGraceDataService
    {
        // ── Results/Grace_Eligible_Data.aspx ─────────────────────────────────────
        // Title: "Grace Eligible Data"
        // Controls: ddlbatch (REGU), ddlSemester (SEM), ChkIsLE ("LE"), btnGetData ("Get Data")
        // BAL: Bal_Reports_Results (bal_RR, App_Web_m2jhophz.dll)
        // DAL method: Load_GraceData
        // Confirmed: App_Web_m2jhophz.dll ASCII "ChkIsLE\x00btnGetData\x00bal_RR\x00btnGetData_Click"

        /// <summary>
        /// Load Batch dropdown (Page_Load)
        /// SQL: SELECT DISTINCT CAST(REGU AS VARCHAR) AS REGU,
        ///      '20'+CAST(REGU AS VARCHAR)+'-'+CAST(REGU+MAXSEM/2 AS VARCHAR) BATCH
        ///      FROM TBL_COURSE WHERE COURSE=@Course ORDER BY REGU
        /// </summary>
        Task<IEnumerable<object>> LoadBatchAsync(string course);

        /// <summary>
        /// Load Semester dropdown (Page_Load)
        /// SP: proc_load_audit_sem @Course, @REGU
        ///   params (2): @Course, @REGU
        /// Confirmed: DataAccessLayer.dll UTF-16LE US heap — template "proc_load_audit_sem '"
        ///   (string-concat DAL pattern; 2 params inferred from page context)
        /// </summary>
        Task<IEnumerable<object>> LoadSemesterAsync(string course, string regu);

        /// <summary>
        /// Get Grace Eligible Data (btnGetData_Click)
        /// SP: proc_Grace_Data @Course, @REGU, @Sem, @IsLE
        ///   params (4): @Course, @REGU, @Sem, @IsLE
        ///   @IsLE: 1 = Lateral Entry students (ChkIsLE checked), 0 = all students
        /// Confirmed: DataAccessLayer.dll UTF-16LE US heap — template "[proc_Grace_Data] '"
        ///   (string-concat DAL pattern; 4 params inferred from 4 page controls)
        /// BAL: Bal_Reports_Results → Load_GraceData; output filename: GraceData.xlsx
        /// </summary>
        Task<IEnumerable<object>> GetGraceDataAsync(string course, string regu, string exammy, string sem, bool isLE);
    }
}
