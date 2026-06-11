using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IStudentHistoryService
    {
        /// <summary>Load student personal details (name, course, branch) by RegNo</summary>
        Task<IEnumerable<object>> LoadStudentDetailsAsync(string regNo);

        /// <summary>Load the full subject-wise history grid for a student</summary>
        Task<IEnumerable<object>> LoadHistoryAsync(string regNo);

        /// <summary>Load SGPA/CGPA per-semester summary for the student</summary>
        Task<IEnumerable<object>> LoadSgpaCgpaAsync(string regNo);

        /// <summary>Load a single TBL_SH record for the edit modal (by ASHID)</summary>
        Task<IEnumerable<object>> GetMarksByAshIdAsync(string ashId);

        /// <summary>Update marks for a TBL_SH record (SetStudentMarks)</summary>
        Task<int> UpdateMarksAsync(string ashId, string pName, string sMarks, string tMarks,
            string mMarks, string rvMarks, string v3, string pMarks);

        /// <summary>Delete a TBL_SH record by ASHID (PROC_DEL_ASHID)</summary>
        Task<int> DeleteByAshIdAsync(string ashId);

        /// <summary>Get the latest ExamMY for a student (SPM_Student_MaxExamMY)</summary>
        Task<string> GetMaxExamMyAsync(string regNo);
    }
}
