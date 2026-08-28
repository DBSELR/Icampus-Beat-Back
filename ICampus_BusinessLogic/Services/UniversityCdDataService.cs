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
    public class UniversityCdDataService : IUniversityCdDataService
    {
        private readonly IGenericRepository<object> _repo;

        public UniversityCdDataService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Load Batch dropdown (Page_Load → loadCombo)
        /// SQL: SELECT DISTINCT EXAMMY,AEXAMID FROM TBL_EXAMS
        ///      WHERE COURSE=@Course AND REGULATION=@Regulation ORDER BY AEXAMID DESC
        /// Source: University_CD_Data.aspx — ddlbatch
        /// </summary>
        public async Task<IEnumerable<object>> LoadBatchAsync(string course, string regulation)
        {
            //var sql = "SELECT DISTINCT EXAMMY, AEXAMID FROM TBL_EXAMS " +
            //          "WHERE COURSE=@Course AND REGULATION=@Regulation ORDER BY AEXAMID DESC";
            var sql = "SELECT  DISTINCT REGU,'20' + REGU + '-20' + CAST(CAST(REGU AS INT) + (MAXSEM/2) AS VARCHAR) AS BATCH FROM TBL_COURSE   ";
                      

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
        /// SP: Proc_SubjectData_LoadSem  (shared with University_SubjectData page)
        /// Params (2): @Course, @ExamMY
        /// Source: University_CD_Data.aspx — ddlSemester
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
        /// Get Subject List (btnSubjectList_Click → Load_SubjectList_CD → Proc_University_SubjectList_CD)
        /// SP: Proc_University_SubjectList_CD
        /// Params (4): @Course, @Regulation, @ExamMY, @Sem
        /// Source: University_CD_Data.aspx — "Subjects Data" button
        ///   App_Web_gp3pforx.dll: set_Batch → set_Sem → Load_SubjectList_CD
        /// Confirmed: DataAccessLayer.dll UTF-16LE 0x27d5b: "[Proc_University_SubjectList_CD] '㤁"
        ///   adjacent to Proc_University_SubjectList
        /// </summary>
        public async Task<IEnumerable<object>> GetSubjectListAsync(
            string course, string regulation, string examMY, string sem)
        {
            var sql = StoredProcSql.ExecNamed(StoredProcedures.Proc_University_SubjectList_CD,
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
        /// Get Students Data (btnStudentData_Click → Load_CD_Data → Proc_University_CD_Data)
        /// SP: Proc_University_CD_Data
        /// Params (5): @Course, @Regulation, @ExamMY, @Sem, @RegSup
        /// Source: University_CD_Data.aspx — "Students Data" button
        ///   App_Web_gp3pforx.dll: set_Batch → set_Sem → set_RegSup (ddlregsup) → Load_CD_Data
        /// Confirmed: DataAccessLayer.dll UTF-16LE 0x27de5: "[Proc_University_CD_Data] '䄁"
        /// </summary>
        public async Task<IEnumerable<object>> GetStudentsDataAsync(
            string course, string regulation, string examMY, string sem, string regSup)
        {
            var sql = StoredProcSql.ExecNamed(StoredProcedures.Proc_University_CD_Data,
                "@Course", "@Regulation", "@ExamMY", "@Regu", "@RegSup", "@Sem");

            // SP expects @Regu as numeric (e.g. "20" from "R20")
            var reguNumeric = regulation?.Length > 1 && (regulation[0] == 'R' || regulation[0] == 'r')
                ? regulation.Substring(1) : regulation ?? string.Empty;

            var ps = new object[]
            {
                new SqlParameter("@Course",     SqlDbType.VarChar, 20)  { Value = course     ?? string.Empty },
                new SqlParameter("@Regulation", SqlDbType.VarChar, 10)  { Value = regulation ?? string.Empty },
                new SqlParameter("@ExamMY",     SqlDbType.VarChar, 20)  { Value = examMY     ?? string.Empty },
                new SqlParameter("@Regu",       SqlDbType.VarChar, 20)  { Value = reguNumeric },
                new SqlParameter("@RegSup",     SqlDbType.VarChar, 5)   { Value = regSup     ?? string.Empty },
                new SqlParameter("@Sem",        SqlDbType.VarChar, 10)  { Value = sem        ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, ps);
            return result ?? Enumerable.Empty<object>();
        }
    }
}
