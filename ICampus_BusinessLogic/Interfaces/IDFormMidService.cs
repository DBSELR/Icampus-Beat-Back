using ICampus_Models.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IDFormMidService
    {
        /// <summary>
        /// Load semester list for DFORM_MID ddlSemester (Page_Load)
        /// Inline SQL on tbl_sh — 3 params: Course, ExamMY, Regulation (all varchar)
        /// Confirmed from DLL IL: DataAccessLayer.dll offset 0x26c8f
        ///   SQL: SELECT DISTINCT CAST(SEM AS VARCHAR(250)) SEM
        ///         FROM tbl_sh WHERE COURSE=@Course AND Exammy=@ExamMY AND Regulation=@Regulation
        ///         ORDER BY SEM
        /// NOTE: uses ExamMY as a filter (unlike regular DFORM which uses only Course + Regulation)
        /// </summary>
        Task<IEnumerable<object>> GetSemestersAsync(string course, string regulation, string examMY);

        /// <summary>
        /// Load exam date list for DFORM_MID ddlEdate (ddlSemester_SelectedIndexChanged cascade)
        /// SP: proc_LOAD_MID_DATES
        /// Parameters confirmed from DLL IL (DataAccessLayer.dll DForm_Edate_MID, US heap 0x9d61):
        ///   param 1: Regulation (varchar) — from Session["Regulation"]
        ///   param 2: Course     (varchar) — from Session["Course"]
        ///   param 3: ExamMY     (varchar) — from Session["ExamMY"]
        ///   param 4: Sem        (varchar) — from ddlSemester.SelectedValue
        ///   param 5: ExamType   (varchar) — from ddlExamType.SelectedValue ("1"=MID-I, "2"=MID-II)
        /// Returns column: MDATE (both value and display field)
        /// </summary>
        Task<IEnumerable<object>> GetEdatesAsync(string course, string regulation, string examMY, string sem, string examType);

        /// <summary>
        /// Generate D Form Mid report data
        /// SP: sp_rep_deform_MID (IsReadmit=false) or sp_rep_deform_Readmit_MID (IsReadmit=true)
        /// Parameters confirmed from DLL IL (App_Web_gp3pforx.dll, 6-phase String.Concat at offset 0xc4f1):
        ///   param 1: Regulation (varchar) — REQUIRED
        ///   param 2: Course     (varchar) — REQUIRED
        ///   param 3: ExamMY     (varchar) — REQUIRED
        ///   param 4: SEM        (INT — unquoted!) — OPTIONAL (DBNull when null)
        ///   param 5: EDate      (varchar, yyyy-MM-dd) — OPTIONAL (empty = all dates)
        ///   param 6: ExamType   (varchar, "1" or "2") — OPTIONAL (empty = all types)
        /// </summary>
        Task<IEnumerable<object>> GetReportDataAsync(DFormMidRequest request);
    }
}
