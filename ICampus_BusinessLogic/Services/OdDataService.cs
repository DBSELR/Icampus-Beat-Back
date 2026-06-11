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
    public class OdDataService : IOdDataService
    {
        private readonly IGenericRepository<object> _repo;

        public OdDataService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Load Batch dropdown (Page_Load — shared by both OD pages)
        /// SQL: SELECT DISTINCT CAST(REGU AS VARCHAR) AS REGU,
        ///      '20'+CAST(REGU AS VARCHAR)+'-'+CAST(REGU+MAXSEM/2 AS VARCHAR) BATCH
        ///      FROM TBL_COURSE WHERE COURSE=@Course ORDER BY REGU
        /// BAL: BAL_MRF / BAL_StudentHistory → Load_Batch
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
        /// Load Branch dropdown (ddlBatch AutoPostBack → cascade)
        /// SQL: SELECT DISTINCT GRP, GRP+' - '+GSUB BRANCH FROM TBL_COURSE
        ///      WHERE COURSE=@Course AND REGU=@Regu ORDER BY GRP
        /// BAL: BAL_MRF / BAL_StudentHistory → Load_Branch
        /// </summary>
        public async Task<IEnumerable<object>> LoadBranchAsync(string course, string regu)
        {
            var sql = "SELECT DISTINCT GRP, GRP+' - '+GSUB BRANCH FROM TBL_COURSE " +
                      "WHERE COURSE=@Course AND REGU=@Regu ORDER BY GRP";

            var parameters = new[]
            {
                new SqlParameter("@Course", SqlDbType.VarChar, 20) { Value = course ?? string.Empty },
                new SqlParameter("@Regu",   SqlDbType.VarChar, 10) { Value = regu   ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Get OD (On Duty) subject list — btnExcel_Click on both pages
        /// DAL method: ODLIST_REGNO_SUBJECTS (DAL_MRF)
        /// Confirmed: DAL_MRF.ODLIST_REGNO_SUBJECTS method name in DataAccessLayer.dll ASCII heap
        /// Both ODLIST_JBIET.aspx and OD_RegnoWise_subjectData.aspx call this via Load_OD_Data
        /// SQL: SELECT S.REGNO, D.SNAME, S.SEM, S.GRP BRANCH, S.PCODE, S.PNAME,
        ///           CONVERT(VARCHAR,S.EDATE,106) EDATE, S.EXAMMY, S.REGU, S.COURSE
        ///      FROM TBL_SH S LEFT JOIN TBL_STDDATA D ON S.REGNO=D.REGNO
        ///      WHERE S.CODE='OD' AND S.COURSE=@Course AND S.REGU=@REGU
        ///        AND (@Branch='' OR S.GRP=@Branch)
        ///        AND (@RegNo='' OR S.REGNO=@RegNo)
        ///        AND (@IsLateral=0 OR D.LATERAL='Y')
        ///      ORDER BY S.REGNO, S.SEM, S.PCODE
        /// ChkLateral (ODLIST_JBIET): D.LATERAL='Y' filter for lateral entry students
        /// </summary>
        public async Task<IEnumerable<object>> GetOdListAsync(string course, string regu, string branch, string regNo, bool isLateral)
        {
            // TODO: confirm actual column name for lateral filter in TBL_STDDATA (column 'LATERAL' does not exist)
            var sql = "SELECT S.REGNO, D.SNAME, S.SEM, S.GRP BRANCH, S.PCODE, S.PNAME, " +
                      "CONVERT(VARCHAR, S.EDATE, 106) EDATE, S.EXAMMY, S.REGU, S.COURSE " +
                      "FROM TBL_SH S LEFT JOIN TBL_STDDATA D ON S.REGNO = D.REGNO " +
                      "WHERE S.CODE = 'OD' AND S.COURSE = @Course AND S.REGU = @REGU " +
                      "AND (@Branch = '' OR S.GRP = @Branch) " +
                      "AND (@RegNo = '' OR S.REGNO = @RegNo) " +
                      "ORDER BY S.REGNO, S.SEM, S.PCODE";

            var parameters = new[]
            {
                new SqlParameter("@Course", SqlDbType.VarChar, 20) { Value = course ?? string.Empty },
                new SqlParameter("@REGU",   SqlDbType.VarChar, 10) { Value = regu   ?? string.Empty },
                new SqlParameter("@Branch", SqlDbType.VarChar, 10) { Value = branch ?? string.Empty },
                new SqlParameter("@RegNo",  SqlDbType.VarChar, 20) { Value = regNo  ?? string.Empty },
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Apply OD gracing — Load_Btech_OD_AddGracing_R16 (BAL_MRF)
        /// Runs 3 SPs in sequence. @EDATE = '01-' + ExamMY (e.g. '01-Nov-2022').
        /// </summary>
        public async Task<int> RunGracingAsync(string course, string regu, string branch, string examMy, string userId, string regLetrl)
        {
            var edate = "01-" + examMy;

            // SP 1: PROC_GRACING_GRAFTING (6 params)
            var sql1 = StoredProcSql.Exec(StoredProcedures.PROC_GRACING_GRAFTING,
                "@Course", "@REGU", "@BRANCH", "@EXAMMY", "@UserID", "@Reg_Letrl");
            var p1 = new[]
            {
                new SqlParameter("@Course",    SqlDbType.VarChar, 30) { Value = course    ?? string.Empty },
                new SqlParameter("@REGU",      SqlDbType.VarChar, 10) { Value = regu      ?? string.Empty },
                new SqlParameter("@BRANCH",    SqlDbType.VarChar, 10) { Value = branch    ?? string.Empty },
                new SqlParameter("@EXAMMY",    SqlDbType.VarChar, 20) { Value = examMy    ?? string.Empty },
                new SqlParameter("@UserID",    SqlDbType.VarChar, 50) { Value = userId    ?? string.Empty },
                new SqlParameter("@Reg_Letrl", SqlDbType.VarChar, 5)  { Value = regLetrl  ?? string.Empty }
            };
            await _repo.ExecuteStoredProcAsync(sql1, p1);

            // SP 2: PROC_GRACING_GRAFTING_MRK_UPDATE (6 params)
            var sql2 = StoredProcSql.Exec(StoredProcedures.PROC_GRACING_GRAFTING_MRK_UPDATE,
                "@Course", "@REGU", "@BRANCH", "@EXAMMY", "@UserID", "@Reg_Letrl");
            var p2 = new[]
            {
                new SqlParameter("@Course",    SqlDbType.VarChar, 30) { Value = course    ?? string.Empty },
                new SqlParameter("@REGU",      SqlDbType.VarChar, 10) { Value = regu      ?? string.Empty },
                new SqlParameter("@BRANCH",    SqlDbType.VarChar, 10) { Value = branch    ?? string.Empty },
                new SqlParameter("@EXAMMY",    SqlDbType.VarChar, 20) { Value = examMy    ?? string.Empty },
                new SqlParameter("@UserID",    SqlDbType.VarChar, 50) { Value = userId    ?? string.Empty },
                new SqlParameter("@Reg_Letrl", SqlDbType.VarChar, 5)  { Value = regLetrl  ?? string.Empty }
            };
            await _repo.ExecuteStoredProcAsync(sql2, p2);

            // SP 3: PROC_GRACING_GRAFTING_SH_UPDATE (7 params — includes @EDATE = '01-'+ExamMY)
            var sql3 = StoredProcSql.Exec(StoredProcedures.PROC_GRACING_GRAFTING_SH_UPDATE,
                "@Course", "@REGU", "@BRANCH", "@EXAMMY", "@UserID", "@Reg_Letrl", "@EDATE");
            var p3 = new[]
            {
                new SqlParameter("@Course",    SqlDbType.VarChar, 30) { Value = course    ?? string.Empty },
                new SqlParameter("@REGU",      SqlDbType.VarChar, 10) { Value = regu      ?? string.Empty },
                new SqlParameter("@BRANCH",    SqlDbType.VarChar, 10) { Value = branch    ?? string.Empty },
                new SqlParameter("@EXAMMY",    SqlDbType.VarChar, 20) { Value = examMy    ?? string.Empty },
                new SqlParameter("@UserID",    SqlDbType.VarChar, 50) { Value = userId    ?? string.Empty },
                new SqlParameter("@Reg_Letrl", SqlDbType.VarChar, 5)  { Value = regLetrl  ?? string.Empty },
                new SqlParameter("@EDATE",     SqlDbType.VarChar, 30) { Value = edate }
            };
            return await _repo.ExecuteStoredProcAsync(sql3, p3);
        }
    }
}
