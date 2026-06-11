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
    public class BtechCmmService : IBtechCmmService
    {
        private readonly IGenericRepository<object> _repo;

        public BtechCmmService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Load Batch dropdown (Page_Load)
        /// SQL: SELECT DISTINCT CAST(REGU AS VARCHAR) AS REGU,
        ///      '20'+CAST(REGU AS VARCHAR)+'-'+CAST(REGU+MAXSEM/2 AS VARCHAR) BATCH
        ///      FROM TBL_COURSE WHERE COURSE=@Course ORDER BY REGU
        /// BAL: getBatch_Course (BusinessAccessLayer.dll ASCII offset 40407)
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
        /// BAL: Load_Branch (BusinessAccessLayer.dll ASCII offset 41781)
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
        /// Get CGC All Programmes data (btnView_Click)
        /// SP selection logic (matches BTECHCMM ASPX code-behind):
        ///   isGracing=true  + regu ends with "R16" → SP_CMM_AddGracing_R16  params: @Course, @ExamMY, @Regu, @Batch, @Branch, @RegNo
        ///   isGracing=true                         → SP_CMM_AddGracing       params: @Course, @ExamMY, @Regu, @Batch, @Branch, @RegNo
        ///   regu == "R18"                          → SP_CMM_R18              params: @Course, @ExamMY, @Regu
        ///   regu == "R11" (PG)                     → SP_CMM_R11_PG           params: @Course, @ExamMY, @Regu
        ///   default                                → SP_CMM                  params: @Course, @ExamMY, @Regu
        /// Confirmed: DataAccessLayer.dll UTF-16LE US heap offset 161251
        ///   BAL: Load_Btech_CMM_AddGracing, Load_Btech_CMM_AddGracing_R16
        ///        (BusinessAccessLayer.dll ASCII offsets 41586/35178)
        /// </summary>
        public async Task<IEnumerable<object>> GetDataAsync(
            string course, string examMY, string regu,
            string batch, string branch, string regNo,
            bool isGracing, bool isLateral)
        {
            // Drop permanent tables so SP_CMM recompiles with deferred resolution (avoids stale column errors)
            await _repo.ExecuteStoredProcAsync(
                "IF OBJECT_ID('TBL_CMM')    IS NOT NULL DROP TABLE TBL_CMM; " +
                "IF OBJECT_ID('TEMP_CMM')   IS NOT NULL DROP TABLE TEMP_CMM; " +
                "IF OBJECT_ID('marks_del_1') IS NOT NULL DROP TABLE marks_del_1");

            var reguUpper = (regu ?? string.Empty).ToUpper();
            // @RLE: 'REG' = regular students only, '' = lateral students only (B.TECH only)
            var rle = isLateral ? string.Empty : "REG";

            string sql;
            SqlParameter[] parameters;

            if (isGracing && reguUpper.Contains("R16"))
            {
                sql = StoredProcSql.Exec(StoredProcedures.SP_CMM_AddGracing_R16,
                    "@Regulation", "@Course", "@Regu", "@GRP", "@ExamMY", "@REGNO", "@RLE");
                parameters = BuildParams(course, examMY, regu, branch, regNo, rle);
            }
            else if (isGracing)
            {
                sql = StoredProcSql.Exec(StoredProcedures.SP_CMM_AddGracing,
                    "@Regulation", "@Course", "@Regu", "@GRP", "@ExamMY", "@REGNO", "@RLE");
                parameters = BuildParams(course, examMY, regu, branch, regNo, rle);
            }
            else if (reguUpper == "R18")
            {
                sql = StoredProcSql.Exec(StoredProcedures.SP_CMM_R18,
                    "@Regulation", "@Course", "@Regu", "@GRP", "@ExamMY", "@REGNO", "@RLE");
                parameters = BuildParams(course, examMY, regu, branch, regNo, rle);
            }
            else if (reguUpper.StartsWith("R11"))
            {
                // SP_CMM_R11_PG has no @RLE param
                sql = StoredProcSql.Exec(StoredProcedures.SP_CMM_R11_PG,
                    "@Regulation", "@Course", "@Regu", "@GRP", "@ExamMY", "@REGNO");
                parameters = BuildParamsNoRle(course, examMY, regu, branch, regNo);
            }
            else
            {
                sql = StoredProcSql.Exec(StoredProcedures.SP_CMM,
                    "@Regulation", "@Course", "@Regu", "@GRP", "@ExamMY", "@REGNO", "@RLE");
                parameters = BuildParams(course, examMY, regu, branch, regNo, rle);
            }

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        // All CMM SPs: @Regulation(full), @Course, @Regu(numeric), @GRP, @EXAMMY, @REGNO, @RLE
        private static SqlParameter[] BuildParams(
            string course, string examMY, string regu,
            string grp, string regNo, string rle)
        {
            var reguNumeric = (regu ?? string.Empty).TrimStart('R', 'r');
            return new[]
            {
                new SqlParameter("@Regulation", SqlDbType.VarChar, 20) { Value = regu        ?? string.Empty },
                new SqlParameter("@Course",     SqlDbType.VarChar, 15) { Value = course      ?? string.Empty },
                new SqlParameter("@Regu",       SqlDbType.VarChar, 20) { Value = reguNumeric },
                new SqlParameter("@GRP",        SqlDbType.VarChar, 20) { Value = grp         ?? string.Empty },
                new SqlParameter("@ExamMY",     SqlDbType.VarChar, 15) { Value = examMY      ?? string.Empty },
                new SqlParameter("@REGNO",      SqlDbType.VarChar, 20) { Value = regNo       ?? string.Empty },
                new SqlParameter("@RLE",        SqlDbType.VarChar, 5)  { Value = rle         ?? string.Empty }
            };
        }

        // SP_CMM_R11_PG has no @RLE
        private static SqlParameter[] BuildParamsNoRle(
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
                new SqlParameter("@ExamMY",     SqlDbType.VarChar, 15) { Value = examMY      ?? string.Empty },
                new SqlParameter("@REGNO",      SqlDbType.VarChar, 20) { Value = regNo       ?? string.Empty }
            };
        }
    }
}
