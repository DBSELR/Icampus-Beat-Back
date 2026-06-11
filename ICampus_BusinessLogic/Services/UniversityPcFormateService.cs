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
    public class UniversityPcFormateService : IUniversityPcFormateService
    {
        private readonly IGenericRepository<object> _repo;

        public UniversityPcFormateService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Load Batch dropdown (Page_Load / ddlBatch)
        /// SQL: SELECT DISTINCT CAST(REGU AS VARCHAR) AS REGU,
        ///      '20'+CAST(REGU AS VARCHAR)+'-'+CAST(REGU+MAXSEM/2 AS VARCHAR) BATCH
        ///      FROM TBL_COURSE WHERE COURSE=@Course ORDER BY REGU
        /// Source: University_PC_Formate.aspx → same batch-load pattern as PC_All_Courses
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
        /// Get JNTUH University Provisional Certificate data (btnDownLoad_Click)
        /// Source: University_PC_Formate.aspx → App_Web_gp3pforx.dll
        ///   Methods: Get_Univeristy_Formate, Get_Univeristy_Formate_R18, Get_Univeristy_Formate_Gracing
        ///
        /// SP selection logic:
        ///   isGracing=true  + regu contains "R16" → SP_Jntu_Award_JBIET_AddGracing_R16  params: @Course, @ExamMY, @Regu, @Batch, @Branch
        ///   isGracing=true                         → SP_Jntu_Award_JBIET_AddGracing       params: @Course, @ExamMY, @Regu, @Batch, @Branch
        ///   default (incl. R18)                    → SP_Jntu_Award_JBIET                  params: @Course, @ExamMY, @Regu
        ///
        /// isLateral: ChkLateral checkbox — handled inside the SP (passed as part of the data filter)
        ///   Not a separate SP variant; SP_Jntu_Award_JBIET internally uses @Lateral if needed
        ///   (University_PC_Formate only has Lateral checkbox for non-gracing path)
        ///
        /// Confirmed: DataAccessLayer.dll UTF-16LE:
        ///   0x276af: "[SP_Jntu_Award_JBIET]  '✁"
        ///   0x27aef: "[SP_Jntu_Award_JBIET_AddGracing]  '儁"
        ///            "[SP_Jntu_Award_JBIET_AddGracing_R16]  '儁"
        ///
        /// Crystal Reports: Btech_Pc.rpt / MCA_Pc.rpt / MBA_Pc.rpt / Mtech_Pc.rpt
        ///   QR path: ~/QRimages/PC/
        /// </summary>
        public async Task<IEnumerable<object>> GetDataAsync(
            string course, string examMY, string regu,
            string batch, string branch,
            bool isGracing, bool isLateral)
        {
            string sql;
            SqlParameter[] parameters;

            var reguUpper = (regu ?? string.Empty).ToUpper();

            if (isGracing && reguUpper.Contains("R16"))
            {
                sql = StoredProcSql.ExecNamed(StoredProcedures.SP_Jntu_Award_JBIET_AddGracing_R16,
                    "@Regulation", "@Course", "@Regu", "@GRP", "@EXAMMY", "@REGNO", "@RLE");
                parameters = BuildBaseParams(course, examMY, regu, branch, isLateral);
            }
            else if (isGracing)
            {
                sql = StoredProcSql.ExecNamed(StoredProcedures.SP_Jntu_Award_JBIET_AddGracing,
                    "@Regulation", "@Course", "@Regu", "@GRP", "@EXAMMY", "@REGNO", "@RLE");
                parameters = BuildBaseParams(course, examMY, regu, branch, isLateral);
            }
            else
            {
                // SP_Jntu_Award_JBIET needs all 7 params:
                // @Regulation (full e.g. "R20"), @Course, @Regu (numeric e.g. "20"),
                // @GRP, @EXAMMY, @REGNO (empty = all), @RLE ("REG"=regular / "LAT"=lateral)
                sql = StoredProcSql.ExecNamed(StoredProcedures.SP_Jntu_Award_JBIET,
                    "@Regulation", "@Course", "@Regu", "@GRP", "@EXAMMY", "@REGNO", "@RLE");
                parameters = BuildBaseParams(course, examMY, regu, branch, isLateral);
            }

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        private static SqlParameter[] BuildBaseParams(
            string course, string examMY, string regu, string grp, bool isLateral)
        {
            // @Regulation = full string e.g. "R20"
            // @Regu       = numeric part e.g. "20" (strip leading alpha chars)
            var reguNumeric = System.Text.RegularExpressions.Regex.Replace(regu ?? string.Empty, "[^0-9]", "");
            var rle = isLateral ? "LAT" : "REG";

            return new[]
            {
                new SqlParameter("@Regulation", SqlDbType.VarChar, 20) { Value = regu        ?? string.Empty },
                new SqlParameter("@Course",     SqlDbType.VarChar, 15) { Value = course      ?? string.Empty },
                new SqlParameter("@Regu",       SqlDbType.VarChar, 20) { Value = reguNumeric },
                new SqlParameter("@GRP",        SqlDbType.VarChar, 20) { Value = grp         ?? string.Empty },
                new SqlParameter("@EXAMMY",     SqlDbType.VarChar, 15) { Value = examMY      ?? string.Empty },
                new SqlParameter("@REGNO",      SqlDbType.VarChar, 20) { Value = string.Empty },
                new SqlParameter("@RLE",        SqlDbType.VarChar, 5)  { Value = rle }
            };
        }

        public async Task<IEnumerable<object>> GetSpParamsAsync(string spName)
        {
            var sql = "SELECT p.parameter_id, p.name, t.name AS type_name, p.has_default_value " +
                      "FROM sys.parameters p " +
                      "JOIN sys.types t ON p.user_type_id = t.user_type_id " +
                      "WHERE OBJECT_NAME(p.object_id) = @SpName " +
                      "ORDER BY p.parameter_id";
            var ps = new[] { new SqlParameter("@SpName", SqlDbType.VarChar, 200) { Value = spName ?? string.Empty } };
            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])ps);
            return result ?? Enumerable.Empty<object>();
        }

    }
}
