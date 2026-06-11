using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface ICgpaYearWiseService
    {
        /// <summary>
        /// Load ExamMY dropdown (ddlexammy on Page_Load)
        /// SP: SPM_EXAMS_ExamMY_Load (@Course) — pass empty string for all exam months
        /// BAL method: loadExammy (BAL_CGPA_Yearwise)
        /// </summary>
        Task<IEnumerable<object>> LoadExammyAsync(string course, string regulation);

        /// <summary>
        /// Load Batch dropdown (ddlbatch — cascades from ExamMY)
        /// SQL: SELECT DISTINCT REGU FROM TBL_SH WHERE EXAMMY=@ExamMY ORDER BY REGU
        /// BAL method: loadbatch (BAL_CGPA_Yearwise)
        /// </summary>
        Task<IEnumerable<object>> LoadBatchAsync(string examMY);

        /// <summary>
        /// Load Semester dropdown (ddlsemester — cascades from ExamMY + Batch)
        /// SQL: SELECT DISTINCT CAST(SEM AS VARCHAR) SEM FROM TBL_SH
        ///      WHERE EXAMMY=@ExamMY AND REGU=@Batch ORDER BY SEM
        /// BAL method: LoadSem (BAL_CGPA_Yearwise)
        /// </summary>
        Task<IEnumerable<object>> LoadSemestersAsync(string examMY, string batch);

        /// <summary>
        /// Excel download data (btnDownLoad_Click → Excel export)
        /// SP: sp_cgpa_excel (@ExamMY, @Batch)  — 2 params confirmed from App_Web DLL
        /// BAL: BAL_CGPA_Yearwise → DAL_CGPA_Yearwise
        /// Validation: ExamMY must not be empty ("Select ExamMY" message)
        /// Output filename: CGPA_YearWise.xlsx
        /// </summary>
        Task<IEnumerable<object>> DownloadAsync(
            string regulation, string course, string examMY, string regu, string sem);
    }
}
