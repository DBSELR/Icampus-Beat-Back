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
    public class PcAllCoursesService : IPcAllCoursesService
    {
        private readonly IGenericRepository<object> _repo;

        public PcAllCoursesService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Load Batch dropdown (Page_Load)
        /// SQL: SELECT DISTINCT CAST(REGU AS VARCHAR) AS REGU,
        ///      '20'+CAST(REGU AS VARCHAR)+'-'+CAST(REGU+MAXSEM/2 AS VARCHAR) BATCH
        ///      FROM TBL_COURSE WHERE COURSE=@Course ORDER BY REGU
        /// </summary>
        public async Task<IEnumerable<object>> LoadBatchAsync(string course)
        {
            var sql = "SELECT DISTINCT CAST(REGU AS VARCHAR) AS REGU, " +
                      "'20'+CAST(REGU AS VARCHAR)+'-'+CAST(REGU + MAXSEM/2 AS VARCHAR) BATCH " +
                      "FROM TBL_COURSE WHERE COURSE=@Course ORDER BY REGU";

            var p = new SqlParameter("@Course", SqlDbType.VarChar, 20) { Value = course ?? string.Empty };
            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])new[] { p });
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Load Branch dropdown (ddlBatch_SelectedIndexChanged)
        /// SQL: SELECT DISTINCT GRP FROM TBL_COURSE WHERE COURSE=@Course AND REGU=@Batch ORDER BY GRP
        /// </summary>
        public async Task<IEnumerable<object>> LoadBranchAsync(string course, string batch)
        {
            var sql = "SELECT DISTINCT GRP FROM TBL_COURSE WHERE COURSE=@Course AND REGU=@Batch ORDER BY GRP";

            var ps = new[]
            {
                new SqlParameter("@Course", SqlDbType.VarChar, 20) { Value = course ?? string.Empty },
                new SqlParameter("@Batch",  SqlDbType.VarChar, 10) { Value = batch  ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])ps);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Get PC All Courses data (btnView_Click)
        /// SP selection logic (matches PC_All_Courses.aspx code-behind):
        ///   isGracing=true  + regu ends with "R16" → proc_pc_rep_AddGracing_R16  params: @Course, @ExamMY, @Regu, @Batch, @Branch, @RegNo
        ///   isGracing=true                         → proc_pc_rep_AddGracing       params: @Course, @ExamMY, @Regu, @Batch, @Branch, @RegNo
        ///   regu == "R18"                          → proc_pc_rep_R18              params: @Course, @ExamMY, @Regu
        ///   default                                → proc_pc_rep_AllCourse        params: @Course, @ExamMY, @Regu
        /// Confirmed: DataAccessLayer.dll UTF-16LE US heap offsets 161643, 162903
        ///   BAL: Allcourses_Pc_R18, Load_Mtech_Pc_AddGracing, Load_Mtech_Pc_AddGracing_R16
        ///        (BusinessAccessLayer.dll ASCII offsets 35189/41597/35208)
        /// </summary>
        public async Task<IEnumerable<object>> GetDataAsync(
            string course, string examMY, string regu,
            string batch, string branch, string regNo,
            bool isGracing)
        {
            string sql;

            var reguUpper = (regu ?? string.Empty).ToUpper();
            int Rbatch = int.Parse(batch);
            // All 4 SPs share the same 6-param signature:
            // @REGULATION, @Course, @Regu(numeric), @GRP, @Exammy, @REGNO
            StoredProcedures spName;
            if (isGracing && reguUpper.Contains("R16"))
                spName = StoredProcedures.proc_pc_rep_AddGracing_R16;
            else if (isGracing)
                spName = StoredProcedures.proc_pc_rep_AddGracing;
            // else if (reguUpper == "R18")
           else if(Rbatch>=18)
                spName = StoredProcedures.proc_pc_rep_R18;
            else
                spName = StoredProcedures.proc_pc_rep_AllCourse;

            sql = StoredProcSql.Exec(spName,
                "@Regulation", "@Course", "@Regu", "@GRP", "@Exammy", "@REGNO");

            var parameters = BuildParams(course, examMY, regu, branch, regNo);

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        // All 4 SPs: @REGULATION(full), @Course, @Regu(numeric), @GRP, @Exammy, @REGNO
        private static SqlParameter[] BuildParams(
            string course, string examMY, string regu,
            string grp, string regNo)
        {
            var reguNumeric = (regu ?? string.Empty).TrimStart('R', 'r');
            return new[]
            {
                new SqlParameter("@Regulation", SqlDbType.VarChar, 20) { Value = regu        ?? string.Empty },
                new SqlParameter("@Course",     SqlDbType.VarChar, 15) { Value = course      ?? string.Empty },
                new SqlParameter("@Regu",       SqlDbType.VarChar, 20) { Value = reguNumeric },
                new SqlParameter("@GRP",        SqlDbType.VarChar, 20) { Value = grp         ?? string.Empty },
                new SqlParameter("@Exammy",     SqlDbType.VarChar, 20) { Value = examMY      ?? string.Empty },
                new SqlParameter("@REGNO",      SqlDbType.VarChar, 20) { Value = regNo       ?? string.Empty }
            };
        }
    }
}
