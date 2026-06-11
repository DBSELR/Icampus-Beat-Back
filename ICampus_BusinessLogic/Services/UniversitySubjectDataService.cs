using ICampus_BusinessLogic.Interfaces;
using ICampus_DataAccessLayer.Helpers;
using ICampus_DataAccessLayer.Interfaces;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Services
{
    public class UniversitySubjectDataService : IUniversitySubjectDataService
    {
        private readonly IGenericRepository<object> _repo;

        public UniversitySubjectDataService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Load Batch dropdown (Page_Load → loadCombo)
        /// SQL: SELECT DISTINCT EXAMMY,AEXAMID FROM TBL_EXAMS
        ///      WHERE COURSE=@Course AND REGULATION=@Regulation ORDER BY AEXAMID DESC
        /// Source: University_SubjectData.aspx — ddlbatch shows ExamMY values
        /// Confirmed: DataAccessLayer.dll UTF-16LE 0x26ad0 (same loader pattern as SubjectList)
        /// </summary>
        public async Task<IEnumerable<object>> LoadBatchAsync(string course, string regulation)
        {
            var sql = "SELECT DISTINCT EXAMMY, AEXAMID FROM TBL_EXAMS " +
                      "WHERE COURSE=@Course AND REGULATION=@Regulation ORDER BY AEXAMID DESC";

            var ps = new[]
            {
                new SqlParameter("@Course",     SqlDbType.VarChar, 20) { Value = course     ?? string.Empty },
                new SqlParameter("@Regulation", SqlDbType.VarChar, 10) { Value = regulation ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])ps);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Load Semester dropdown (SubjectData_LoadSemesters → Proc_SubjectData_LoadSem)
        /// SP: Proc_SubjectData_LoadSem
        /// Params (2): @Course, @ExamMY
        /// Source: University_SubjectData.aspx — ddlSemester
        /// Confirmed: DataAccessLayer.dll UTF-16LE 0x27e5d: "Proc_SubjectData_LoadSem '⬁"
        /// </summary>
        public async Task<IEnumerable<object>> LoadSemAsync(string course, string examMY, string regulation)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.Proc_SubjectData_LoadSem,
                "@Course", "@ExamMY", "@Regulation");

            var ps = new object[]
            {
                new SqlParameter("@Course",     SqlDbType.VarChar, 20) { Value = course     ?? string.Empty },
                new SqlParameter("@ExamMY",     SqlDbType.VarChar, 20) { Value = examMY     ?? string.Empty },
                new SqlParameter("@Regulation", SqlDbType.VarChar, 20) { Value = regulation ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, ps);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Get Subject List (btnSubjectList_Click → Load_SubjectList → Proc_University_SubjectList)
        /// SP: Proc_University_SubjectList
        /// Params (4): @Course, @Regulation, @ExamMY, @Sem
        /// Source: University_SubjectData.aspx — "Subjects Data" button
        ///   set_Batch (ExamMY from ddlbatch) → set_Sem (from ddlSemester) → Load_SubjectList
        /// Confirmed: DataAccessLayer.dll UTF-16LE 0x27d5b: "[Proc_University_SubjectList] '䜁"
        /// </summary>
        public async Task<IEnumerable<object>> GetSubjectListAsync(
            string course, string regulation, string examMY, string sem)
        {
            var sql = StoredProcSql.ExecNamed(StoredProcedures.Proc_University_SubjectList,
                "@Course", "@Regulation", "@Regu", "@Sem");

            // SP expects @Regu as numeric (e.g. "20" from "R20"), not the full regulation string
            var reguNumeric = regulation?.Length > 1 && (regulation[0] == 'R' || regulation[0] == 'r')
                ? regulation.Substring(1) : regulation ?? string.Empty;

            var ps = new object[]
            {
                new SqlParameter("@Course",     SqlDbType.VarChar, 20)  { Value = course     ?? string.Empty },
                new SqlParameter("@Regulation", SqlDbType.VarChar, 10)  { Value = regulation ?? string.Empty },
                new SqlParameter("@Regu",       SqlDbType.VarChar, 20)  { Value = reguNumeric },
                new SqlParameter("@Sem",        SqlDbType.VarChar, 10)  { Value = sem        ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, ps);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Get Students Data (btnSubjectData_Click → Load_SubjectData → Proc_University_SubjectData)
        /// SP: Proc_University_SubjectData
        /// Params (5): @Course, @Regulation, @ExamMY, @Sem, @RegSup
        /// Source: University_SubjectData.aspx — "Students Data" button
        ///   set_Batch → set_Sem → set_RegSup (ddlregsup: REG/SUP) → Load_SubjectData
        ///   ChkIsrv is hidden (display:none) — frontend-only flag, not passed to SP
        /// Confirmed: DataAccessLayer.dll UTF-16LE 0x27e1f: "[Proc_University_SubjectData] '㔁"
        /// </summary>
        public async Task<IEnumerable<object>> GetStudentsDataAsync(
            string course, string regulation, string examMY, string sem, string regSup)
        {
            var sql = StoredProcSql.ExecNamed(StoredProcedures.Proc_University_SubjectData,
                "@Course", "@Regulation", "@ExamMY", "@Regu", "@Sem", "@RegSup");

            // SP expects @Regu as numeric (e.g. "20" from "R20")
            var reguNumeric = regulation?.Length > 1 && (regulation[0] == 'R' || regulation[0] == 'r')
                ? regulation.Substring(1) : regulation ?? string.Empty;

            var ps = new object[]
            {
                new SqlParameter("@Course",     SqlDbType.VarChar, 20)  { Value = course     ?? string.Empty },
                new SqlParameter("@Regulation", SqlDbType.VarChar, 10)  { Value = regulation ?? string.Empty },
                new SqlParameter("@ExamMY",     SqlDbType.VarChar, 20)  { Value = examMY     ?? string.Empty },
                new SqlParameter("@Regu",       SqlDbType.VarChar, 20)  { Value = reguNumeric },
                new SqlParameter("@Sem",        SqlDbType.VarChar, 10)  { Value = sem        ?? string.Empty },
                new SqlParameter("@RegSup",     SqlDbType.VarChar, 5)   { Value = regSup     ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, ps);
            return result ?? Enumerable.Empty<object>();
        }
    }
}
