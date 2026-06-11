using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IStudentResultService
    {
        /// <summary>
        /// Load student header details (txtCourse, txtStudentName, txtGRP auto-fill)
        /// SP: SPM_STUDENT_DETAILS @RegNo
        /// </summary>
        Task<IEnumerable<object>> LoadStudentDetailsAsync(string regNo);

        /// <summary>
        /// Load SGPA/CGPA grid (gvSGPA_CGPA)
        /// SP: SP_STUDENT_SGPA_CGPA @REGNO
        /// Returns: SEM, TCR, SCR, REGNO, SGPA, CGPA, RNO, BACKLOGS
        /// </summary>
        Task<IEnumerable<object>> LoadSgpaCgpaAsync(string regNo);

        /// <summary>
        /// Passed papers list (RBPassedList)
        /// SP: SPM_REQ_DATA_FOR_PASSEDPAPLIST @REGNO, @EXAMMY, @CON
        /// @con='true' = include papers up to @EXAMMY; 'false' = all semesters
        /// Returns: REGNO, SEM, PNO, PCODE, PNAME, GR, EXAMMY, PAPRES, REGD, REGSUP, CR, Course, SName, GSUB
        /// </summary>
        Task<IEnumerable<object>> GetPassedPapersAsync(string regNo, string examMY, string con);

        /// <summary>
        /// Failed papers list (RBFailedList)
        /// SP: SPM_REQ_DATA_FOR_FAILEDPAPLIST @REGNO, @EXAMMY, @CON
        /// @con='true' = include papers up to @EXAMMY; 'false' = all semesters
        /// Returns: REGNO, SEM, PNO, PCODE, PNAME, GR, EXAMMY, PAPRES, REGD, REGSUP, CR, SNAME, Course, GSUB
        /// </summary>
        Task<IEnumerable<object>> GetFailedPapersAsync(string regNo, string examMY, string con);

        /// <summary>
        /// All papers list (Rbtn_allFP) — both passed and failed
        /// Raw SQL on TBL_SH with optional EXAMMY filter (CBCurrentMonthYear)
        /// Returns: REGNO, SEM, PCODE, PNAME, GR, EXAMMY, PAPRES, REGSUP, CR
        /// </summary>
        Task<IEnumerable<object>> GetAllPapersAsync(string regNo, string examMY);
    }
}
