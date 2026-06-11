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
    public class IndividualMarksMemoService : IIndividualMarksMemoService
    {
        private readonly IGenericRepository<object> _repo;

        public IndividualMarksMemoService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Load Semester dropdown — ddlSemester (Page_Load)
        /// SQL: SELECT DISTINCT CAST(SEM AS VARCHAR(250)) SEM FROM tbl_sh
        ///      WHERE COURSE=@Course AND Exammy=@ExamMY AND Regulation=@Regu ORDER BY SEM
        /// 3 params: @Course, @ExamMY, @Regu
        /// Confirmed: DataAccessLayer.dll UTF-16LE US heap adjacent to SP_MRK_MEMO_REGNO context
        /// </summary>
        public async Task<IEnumerable<object>> LoadSemsAsync(string course, string examMY, string regu)
        {
            var sql = "SELECT DISTINCT CAST(SEM AS VARCHAR(250)) SEM FROM tbl_sh" +
                      " WHERE COURSE=@Course AND Exammy=@ExamMY AND Regulation=@Regu ORDER BY SEM";

            var parameters = new[]
            {
                new SqlParameter("@Course", SqlDbType.VarChar, 30) { Value = course ?? string.Empty },
                new SqlParameter("@ExamMY", SqlDbType.VarChar, 20) { Value = examMY ?? string.Empty },
                new SqlParameter("@Regu",   SqlDbType.VarChar, 10) { Value = regu   ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Load Branch dropdown — ddlBranch (Page_Load)
        /// SQL: SELECT DISTINCT grp FROM tbl_sh WHERE COURSE=@Course ORDER BY grp
        /// 1 param: @Course
        /// Confirmed: DataAccessLayer.dll UTF-16LE US heap
        ///   "SELECT DISTINCT grp   FROM tbl_sh  WHERE COURSE = '@" adjacent to SP_MRK_MEMO_REGNO
        /// </summary>
        public async Task<IEnumerable<object>> LoadBranchesAsync(string course)
        {
            var sql = "SELECT DISTINCT grp FROM tbl_sh WHERE COURSE=@Course ORDER BY grp";

            var parameters = new[]
            {
                new SqlParameter("@Course", SqlDbType.VarChar, 30) { Value = course ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Diagnostic: find valid REGNOs for testing
        /// SQL: SELECT TOP 10 DISTINCT REGNO FROM tbl_sh
        ///      WHERE COURSE=@Course AND Exammy=@ExamMY AND Regulation=@Regu AND SEM=@Sem AND GRP=@Branch
        /// </summary>
        public async Task<IEnumerable<object>> GetValidRegnosAsync(
            string course, string examMY, string regulation, string semester, string branch)
        {
            var whereClause = " WHERE COURSE=@Course AND Exammy=@ExamMY AND Regulation=@Regu";
            if (!string.IsNullOrWhiteSpace(semester))   whereClause += " AND SEM=@Sem";
            if (!string.IsNullOrWhiteSpace(branch))     whereClause += " AND GRP=@Branch";

            var sql = "SELECT TOP 10 DISTINCT REGNO FROM tbl_sh" + whereClause + " ORDER BY REGNO";

            var paramList = new System.Collections.Generic.List<SqlParameter>
            {
                new SqlParameter("@Course", SqlDbType.VarChar, 30) { Value = course      ?? string.Empty },
                new SqlParameter("@ExamMY", SqlDbType.VarChar, 20) { Value = examMY      ?? string.Empty },
                new SqlParameter("@Regu",   SqlDbType.VarChar, 10) { Value = regulation  ?? string.Empty }
            };
            if (!string.IsNullOrWhiteSpace(semester))
                paramList.Add(new SqlParameter("@Sem",    SqlDbType.VarChar, 5)  { Value = semester });
            if (!string.IsNullOrWhiteSpace(branch))
                paramList.Add(new SqlParameter("@Branch", SqlDbType.VarChar, 20) { Value = branch });

            var result = await _repo.QueryFromStoredProcAsync(sql, paramList.ToArray<object>());
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Auto-fill student info on RegNo entry — txtRegNo_TextChanged equivalent
        /// SQL: SELECT SNAME, COURSE, GRP, PHOTO FROM TBL_STDDATA WHERE REGNO=@RegNo
        /// 1 param: @RegNo
        /// Confirmed: DataAccessLayer.dll UTF-16LE US heap adjacent to sp_getdetails_student sequence
        /// </summary>
        public async Task<IEnumerable<object>> GetStudentInfoAsync(string regNo)
        {
            var sql = "SELECT SNAME, COURSE, GRP, PHOTO FROM TBL_STDDATA WHERE REGNO=@RegNo";

            var parameters = new[]
            {
                new SqlParameter("@RegNo", SqlDbType.VarChar, 20) { Value = regNo ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Get Individual MarksMemo / GradeCard data (btnview_Click / btnDownLoad_Click)
        /// SP: SP_MRK_MEMO_REGNO (@REGULATION, @EXAMMY, @Course, @SEMESTER, @BRANCH, @REGNO)
        /// @RV='N' is hardcoded inside the SP — not a parameter
        /// chkgcard is frontend-only (same SP for both modes):
        ///   Checked  → MarksMemo_Regno.rpt
        ///   Unchecked → GradeCard_btech_Regno.rpt / GradeCard_Regno.rpt
        /// Confirmed: DataAccessLayer.dll UTF-16LE US heap ~0x26e6d:
        ///   "[SP_MRK_MEMO_REGNO] 'ᔁ ,'N',  '⬁"
        /// </summary>
        public async Task<IEnumerable<object>> GetDataAsync(
            string regulation, string examMY, string course, string semester,
            string branch, string regNo)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.SP_MRK_MEMO_REGNO,
                "@REGULATION", "@EXAMMY", "@Course", "@SEMESTER", "@BRANCH", "@REGNO");

            var parameters = new object[]
            {
                new SqlParameter("@REGULATION", SqlDbType.VarChar, 10) { Value = regulation ?? string.Empty },
                new SqlParameter("@EXAMMY",      SqlDbType.VarChar, 20) { Value = examMY     ?? string.Empty },
                new SqlParameter("@Course",      SqlDbType.VarChar, 30) { Value = course     ?? string.Empty },
                new SqlParameter("@SEMESTER",    SqlDbType.VarChar, 5)  { Value = semester   ?? string.Empty },
                new SqlParameter("@BRANCH",      SqlDbType.VarChar, 20) { Value = branch     ?? string.Empty },
                new SqlParameter("@REGNO",       SqlDbType.VarChar, 20) { Value = regNo      ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, parameters);
            return result ?? Enumerable.Empty<object>();
        }
    }
}
