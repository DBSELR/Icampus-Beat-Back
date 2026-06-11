using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface ITranscriptService
    {
        /// <summary>
        /// Load Semester dropdown — ddlSemester (Page_Load)
        /// SQL: SELECT DISTINCT cast(SEM as varchar(250)) SEM
        ///      FROM tbl_sh WHERE COURSE=@Course AND Exammy=@ExamMY AND Regulation=@Regu ORDER BY SEM
        /// Confirmed: DataAccessLayer.dll UTF-16LE US heap offset ~0x26ddb context
        /// </summary>
        Task<IEnumerable<object>> LoadSemsAsync(string course, string examMY, string regu);

        /// <summary>
        /// Load Branch dropdown — ddlBranch (Page_Load)
        /// SQL: SELECT DISTINCT grp FROM tbl_sh WHERE COURSE=@Course ORDER BY grp
        /// Confirmed: DataAccessLayer.dll UTF-16LE context near Transcript sequence
        /// </summary>
        Task<IEnumerable<object>> LoadBranchesAsync(string course);

        /// <summary>
        /// Get Transcript / MarksMemo data (btnview_Click and btnDownLoad_Click)
        /// isMarksMemo=true  (chkgcard checked, default): SP_MRK_MEMO
        ///   params (8): @REGULATION, @EXAMMY, @Course, @SEMESTER, @RV='N', @BRANCH, @REGNO, @Date=''
        ///   Confirmed: DupCertificateService / DataAccessLayer.dll
        /// isMarksMemo=false (chkgcard unchecked): SP_Transcript_New
        ///   params (6): @Course, @ExamMY, @Regu, @Sem, @Branch, @RegNo
        ///   Confirmed: DataAccessLayer.dll UTF-16LE offset 0x26dd5 "[SP_Transcript_New]"
        /// Crystal Reports (course-dependent):
        ///   Transcript_R14_MBAMCA.rpt, Transcript.rpt, Transcript_Gr_btech.rpt, Transcript_Gr.rpt
        /// branch and regNo can be empty string to return all branches/students
        /// </summary>
        Task<IEnumerable<object>> GetDataAsync(
            string course, string examMY, string regu, string sem,
            string branch, string regNo, bool isMarksMemo);
    }
}
