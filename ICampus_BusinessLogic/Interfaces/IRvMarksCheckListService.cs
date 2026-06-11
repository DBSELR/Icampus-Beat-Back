using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IRvMarksCheckListService
    {
        /// <summary>
        /// Load Semester dropdown — ddlSemester (Page_Load + ddlSemester_SelectedIndexChanged)
        /// SQL: SELECT DISTINCT cast(SEM as varchar(250)) SEM FROM tbl_sh WHERE COURSE=@Course
        /// Note: Only @Course filter (no EXAMMY/Regu) — confirmed from enum comment
        /// Confirmed: DataAccessLayer.dll UTF-16LE US heap byte 159892
        /// </summary>
        Task<IEnumerable<object>> LoadSemsAsync(string course);

        /// <summary>
        /// Get RV Marks Check List / Result Sheet data (btnExport_Click)
        /// isReadmit=false → PROC_RV_REPDATA (4 params): @Course, @ExamMY, @Regu, @Sem
        /// isReadmit=true  → PROC_RV_REPDATA_Readmit (5 params): @Course, @ExamMY, @Regu, @Sem, @ReadmitRegu
        /// reportType (frontend-only, same SP): 1=Check List-I, 2=Check List-II, 3=Result Sheet
        ///   → used by frontend to select Crystal Report title and template
        ///   Crystal Reports: RVMarksChecklist.rpt (type 1/2), RVResultSheet.rpt (type 3)
        /// Confirmed: DataAccessLayer.dll UTF-16LE US heap byte 159892/159934
        /// </summary>
        Task<IEnumerable<object>> GetDataAsync(
            string course, string examMY, string regu, string sem,
            bool isReadmit, string readmitRegu);
    }
}
