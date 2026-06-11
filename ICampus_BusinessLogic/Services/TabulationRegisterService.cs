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
    public class TabulationRegisterService : ITabulationRegisterService
    {
        private readonly IGenericRepository<object> _repo;

        public TabulationRegisterService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Load Batch (Regulation) dropdown — ddlBatch AutoPostBack
        /// SQL: SELECT DISTINCT CAST(REGU AS VARCHAR) AS REGU,
        ///      '20'+CAST(REGU AS VARCHAR)+'-'+CAST(REGU+MAXSEM/2 AS VARCHAR) BATCH
        ///      FROM TBL_COURSE WHERE COURSE=@Course ORDER BY REGU
        /// Confirmed: DataAccessLayer.dll UTF-16LE US heap (same pattern as BTECHCMM, PC_All_Courses)
        /// </summary>
        public async Task<IEnumerable<object>> LoadBatchesAsync(string course)
        {
            var sql = "SELECT DISTINCT CAST(REGU AS VARCHAR) AS REGU," +
                      "'20'+CAST(REGU AS VARCHAR)+'-'+CAST(REGU+MAXSEM/2 AS VARCHAR) BATCH" +
                      " FROM TBL_COURSE WHERE COURSE=@Course ORDER BY REGU";

            var parameters = new[]
            {
                new SqlParameter("@Course", SqlDbType.VarChar, 30) { Value = course ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Load Branch dropdown — ddlBranch (loaded on ddlBatch_SelectedIndexChanged)
        /// SQL: SELECT DISTINCT GRP FROM TBL_COURSE WHERE COURSE=@Course AND REGU=@Batch ORDER BY GRP
        /// Confirmed: DataAccessLayer.dll UTF-16LE US heap (same pattern as BTECHCMM branch load)
        /// </summary>
        public async Task<IEnumerable<object>> LoadBranchesAsync(string course, string regu)
        {
            var sql = "SELECT DISTINCT GRP FROM TBL_COURSE" +
                      " WHERE COURSE=@Course AND REGU=@Regu ORDER BY GRP";

            var parameters = new[]
            {
                new SqlParameter("@Course", SqlDbType.VarChar, 30) { Value = course ?? string.Empty },
                new SqlParameter("@Regu",   SqlDbType.VarChar, 10) { Value = regu   ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Load ExamMY dropdown — cmbExamMY
        /// SQL: SELECT DISTINCT EXAMMY, AEXAMID FROM TBL_EXAMS
        ///      WHERE COURSE=@Course and REGULATION=@Regu ORDER BY AEXAMID DESC
        /// Confirmed: DataAccessLayer.dll UTF-16LE US heap offset 158384
        /// </summary>
        public async Task<IEnumerable<object>> LoadExamMYsAsync(string course, string regu)
        {
            // TBL_EXAMS.REGULATION stores full string e.g. 'R20', but regu from frontend is numeric '20'.
            // Prepend 'R' so '20' → 'R20'.
            var sql = "SELECT DISTINCT EXAMMY, AEXAMID FROM TBL_EXAMS" +
                      " WHERE COURSE=@Course AND REGULATION='R'+@Regu ORDER BY AEXAMID DESC";

            var parameters = new[]
            {
                new SqlParameter("@Course", SqlDbType.VarChar, 30) { Value = course ?? string.Empty },
                new SqlParameter("@Regu",   SqlDbType.VarChar, 10) { Value = regu   ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Get Tabulation Register data (btnView_Click / btnexport_Click)
        /// SP selection (TabulationRegister.aspx):
        ///   isReadmit=false → SP_TABULATION_REGISTER  params: @Course, @ExamMY, @Regu, @Branch, @RegNo
        ///   isReadmit=true  → SP_TABULATION_REGISTER_Readmit  params: + @ReadmitRegu
        /// Crystal reports: TabulationRegister_btech.rpt, TabulationRegister_mca.rpt, TabulationRegister_mba.rpt
        ///   (selected by Course on frontend — frontend responsibility)
        /// Confirmed: DataAccessLayer.dll UTF-16LE US heap byte 161101/161161
        ///   Labels: 'READMIT TABULATION REGISTER' / 'TABULATION REGISTER'
        /// </summary>
        public async Task<IEnumerable<object>> GetDataAsync(
            string course, string examMY, string regu, string branch, string regNo,
            bool isReadmit, string readmitRegu)
        {
            string sql;
            SqlParameter[] parameters;

            // SP param order: @Regulation, @COURSE, @REGU, @GRP, @exammy, @REGNO
            // regu from frontend is numeric e.g. '20'; SP needs @Regulation='R20'.
            var regulation = "R" + (regu ?? string.Empty);

            if (isReadmit)
            {
                // SP_TABULATION_REGISTER_Readmit param order:
                // @Regulation, @COURSE, @REGU, @GRP, @exammy, @REGNO, @READMIT_REGULATION
                sql = StoredProcSql.Exec(StoredProcedures.SP_TABULATION_REGISTER_Readmit,
                    "@Regulation", "@Course", "@Regu", "@GRP", "@ExamMY", "@RegNo", "@ReadmitRegu");
                parameters = new[]
                {
                    new SqlParameter("@Regulation", SqlDbType.VarChar, 20) { Value = regulation   },
                    new SqlParameter("@Course",      SqlDbType.VarChar, 20) { Value = course      ?? string.Empty },
                    new SqlParameter("@Regu",        SqlDbType.VarChar, 20) { Value = regu        ?? string.Empty },
                    new SqlParameter("@GRP",         SqlDbType.VarChar, 20) { Value = branch      ?? string.Empty },
                    new SqlParameter("@ExamMY",      SqlDbType.VarChar, 20) { Value = examMY      ?? string.Empty },
                    new SqlParameter("@RegNo",       SqlDbType.VarChar, 20) { Value = regNo       ?? string.Empty },
                    new SqlParameter("@ReadmitRegu", SqlDbType.VarChar, 10) { Value = readmitRegu ?? string.Empty }
                };
            }
            else
            {
                // SP_TABULATION_REGISTER param order:
                // @Regulation, @COURSE, @REGU, @GRP, @exammy, @REGNO
                sql = StoredProcSql.Exec(StoredProcedures.SP_TABULATION_REGISTER,
                    "@Regulation", "@Course", "@Regu", "@GRP", "@ExamMY", "@RegNo");
                parameters = new[]
                {
                    new SqlParameter("@Regulation", SqlDbType.VarChar, 20) { Value = regulation   },
                    new SqlParameter("@Course",      SqlDbType.VarChar, 20) { Value = course      ?? string.Empty },
                    new SqlParameter("@Regu",        SqlDbType.VarChar, 20) { Value = regu        ?? string.Empty },
                    new SqlParameter("@GRP",         SqlDbType.VarChar, 20) { Value = branch      ?? string.Empty },
                    new SqlParameter("@ExamMY",      SqlDbType.VarChar, 20) { Value = examMY      ?? string.Empty },
                    new SqlParameter("@RegNo",       SqlDbType.VarChar, 20) { Value = regNo       ?? string.Empty }
                };
            }

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }
    }
}
