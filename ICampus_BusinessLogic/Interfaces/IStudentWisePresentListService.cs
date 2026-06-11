using ICampus_Models.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IStudentWisePresentListService
    {
        /// <summary>
        /// Load semester list for Studentwise_presentlist.aspx ddlSemester (Page_Load)
        /// Inline SQL on TBL_SH — 3 params: ExamMY, Course, Regulation (all varchar)
        /// Confirmed from DLL IL: BusinessAccessLayer.dll BAL_StudentWiseMasterCreation::Load_Sems_Data
        ///   SQL: SELECT DISTINCT SH.SEM FROM TBL_SH SH
        ///         WHERE SH.EXAMMY=@ExamMY AND SH.course=@Course AND SH.Regulation=@Regulation
        ///         ORDER BY SEM
        /// Note: uses ExamMY from session (unlike most modules that use regulation+course only)
        /// </summary>
        Task<IEnumerable<object>> GetSemestersAsync(string course, string regulation, string examMY);

        /// <summary>
        /// Generate Subject Wise Present List report data
        /// SP: SP_Pcode_Presentlist
        /// Parameters confirmed from DLL IL (DataAccessLayer.dll DAL_StudentHistory::Pcode_PresentList,
        ///   US heap US[0xc0c1], 11-element String.Concat):
        ///   param 1: Course     (varchar) — REQUIRED
        ///   param 2: Regulation (varchar) — REQUIRED
        ///   param 3: ExamMY     (varchar) — REQUIRED
        ///   param 4: Sem        (varchar) — REQUIRED (ddlSemester.SelectedItem)
        ///   param 5: Regsup     (varchar) — REQUIRED: "REG" or "Sup" (ddlregsup.SelectedItem)
        /// Excel export in original: worksheet "REGNO_VS_OMR"
        /// </summary>
        Task<IEnumerable<object>> GetReportDataAsync(StudentWisePresentListRequest request);
    }
}
