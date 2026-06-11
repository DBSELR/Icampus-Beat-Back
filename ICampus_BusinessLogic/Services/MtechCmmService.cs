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
    public class MtechCmmService : IMtechCmmService
    {
        private readonly IGenericRepository<object> _repo;

        public MtechCmmService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Get MTECH CMM data (Page_Load / btnDownLoad_Click)
        /// SP: SP_CMM (@Course, @ExamMY, @Regu)
        /// No user-facing filters — all params from session
        /// Confirmed: DataAccessLayer.dll UTF-16LE US heap offsets 159387/159411
        /// BAL: Load_Btech_CMM (DataAccessLayer.dll ASCII offset 102636)
        /// </summary>
        public async Task<IEnumerable<object>> GetDataAsync(string course, string examMY, string regu, string grp)
        {
            // Drop TBL_CMM/TEMP_CMM before calling SP so WITH RECOMPILE uses deferred resolution
            // (avoids "Invalid column name" errors from stale cached schema)
            await _repo.ExecuteStoredProcAsync(
                "IF OBJECT_ID('TBL_CMM')  IS NOT NULL DROP TABLE TBL_CMM; " +
                "IF OBJECT_ID('TEMP_CMM') IS NOT NULL DROP TABLE TEMP_CMM; " +
                "IF OBJECT_ID('marks_del_1') IS NOT NULL DROP TABLE marks_del_1");

            // SP_CMM param order: @Regulation, @Course, @Regu (numeric), @GRP, @EXAMMY, @REGNO, @RLE
            // @Regulation = full regu string (e.g. 'R20'), @Regu = numeric part (e.g. '20')
            var reguNumeric = regu?.TrimStart('R', 'r') ?? string.Empty;

            var sql = StoredProcSql.Exec(StoredProcedures.SP_CMM,
                "@Regulation", "@Course", "@Regu", "@GRP", "@ExamMY", "@REGNO", "@RLE");

            var parameters = new[]
            {
                new SqlParameter("@Regulation", SqlDbType.VarChar, 20) { Value = regu        ?? string.Empty },
                new SqlParameter("@Course",     SqlDbType.VarChar, 15) { Value = course      ?? string.Empty },
                new SqlParameter("@Regu",       SqlDbType.VarChar, 20) { Value = reguNumeric },
                new SqlParameter("@GRP",        SqlDbType.VarChar, 20) { Value = grp         ?? string.Empty },
                new SqlParameter("@ExamMY",     SqlDbType.VarChar, 15) { Value = examMY      ?? string.Empty },
                new SqlParameter("@REGNO",      SqlDbType.VarChar, 20) { Value = string.Empty },
                new SqlParameter("@RLE",        SqlDbType.VarChar, 5)  { Value = string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }
    }
}
