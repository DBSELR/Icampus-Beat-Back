using ICampus_Models.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IDFormService
    {
        /// <summary>
        /// Generate D Form report data
        /// Calls sp_rep_deform (IsReadmit=false) or sp_rep_deform_Readmit (IsReadmit=true)
        /// 5 positional params confirmed from DLL IL (App_Web_gp3pforx.dll loadingdform, offset 0x0000c3be):
        ///   Regulation(1), Course(2), ExamMY(3), SEM(4 optional), EDate(5 optional, yyyy-MM-dd)
        /// Returns raw rows — Crystal Reports dataset output
        /// </summary>
        Task<IEnumerable<object>> GetReportDataAsync(DFormRequest request);

        /// <summary>
        /// Load semester list for DForm — uses TBL_SH (NOT TBL_GPAP like InternalChecklist)
        /// Inline SQL: SELECT DISTINCT SEM, cast(sem as int) sem1 FROM TBL_SH WHERE COURSE=@Course AND Regulation=@Regulation ORDER BY sem1
        /// Confirmed from DLL IL: DataAccessLayer.dll method DForm_Semester
        /// Triggered by Page_Load in DFORM.aspx
        /// </summary>
        Task<IEnumerable<object>> GetSemestersAsync(string course, string regulation);

        /// <summary>
        /// Load exam date list for DForm — critical cascade after semester selection
        /// Inline SQL: SELECT DISTINCT convert(nVarchar,EDate,105) edate1, CAST(EDATE AS VARCHAR) EDATE
        ///             FROM tbl_sh WHERE EDATE IS NOT NULL AND COURSE=@Course AND sem=@Sem AND ExamMY=@ExamMY
        /// Confirmed from DLL IL: DataAccessLayer.dll method DForm_Edate
        /// Triggered by ddlSemester_SelectedIndexChanged in DFORM.aspx
        /// EDate is returned formatted as dd-MM-yyyy (SQL style 105) for display,
        /// but the SP call uses yyyy-MM-dd — the original code reformats it before calling the report SP
        /// </summary>
        Task<IEnumerable<object>> GetEdatesAsync(string course, string examMY, string sem);
    }
}
