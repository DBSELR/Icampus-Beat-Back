using ICampus_BusinessLogic.Interfaces;
using ICampus_DataAccessLayer.Helpers;
using ICampus_DataAccessLayer.Interfaces;
using ICampus_Models.Requests;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Services
{
    public class ApplySchemaService : IApplySchemaService
    {
        private readonly IGenericRepository<object> _repo;

        public ApplySchemaService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Loads semester dropdown.
        /// SP: Sp_Eval_Load_Sem @Regulation, @Course, @ExamMY  (positional order in SP)
        /// </summary>
        public async Task<IEnumerable<object>> GetSemestersAsync(string course, string regulation, string examMY)
        {
            var sql = StoredProcSql.ExecNamed(StoredProcedures.Sp_Eval_Load_Sem,
                "@Regulation", "@Course", "@ExamMY");

            var parameters = new[]
            {
                new SqlParameter("@Regulation", SqlDbType.VarChar, 1000) { Value = regulation ?? string.Empty },
                new SqlParameter("@Course",     SqlDbType.VarChar, 1000) { Value = course     ?? string.Empty },
                new SqlParameter("@ExamMY",     SqlDbType.VarChar, 1000) { Value = examMY     ?? string.Empty }
            };
            var raw = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return raw ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Loads available papers for a semester from tbl_sh (students must be REGD='Y').
        /// SP: Sp_Eval_Load_Papers @Regulation, @Course, @Exammy, @Sem (@Dept defaults to '')
        /// Returns: pCode (composite "PCode@TempCode@PName@Sem"), PNAME ("PCode_PName"), SEM
        /// </summary>
        public async Task<IEnumerable<object>> GetPapersAsync(string course, string regulation, string sem, string examMY)
        {
            var sql = StoredProcSql.ExecNamed(StoredProcedures.Sp_Eval_Load_Papers,
                "@Regulation", "@Course", "@Exammy", "@Sem");

            var parameters = new[]
            {
                new SqlParameter("@Regulation", SqlDbType.VarChar, 100) { Value = regulation ?? string.Empty },
                new SqlParameter("@Course",     SqlDbType.VarChar, 100) { Value = course     ?? string.Empty },
                new SqlParameter("@Exammy",     SqlDbType.VarChar, 100) { Value = examMY     ?? string.Empty },
                new SqlParameter("@Sem",        SqlDbType.VarChar, 10)  { Value = sem        ?? string.Empty }
            };
            var raw = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return raw ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Returns all papers with SStatus='true' (assigned) or 'false' (not assigned) for a schema.
        /// SP: Sp_Eval_Get_PapData @Regulation, @Course, @Sem, @SchemaName (@Maximum_Marks defaults to '')
        /// Returns: pcode (TempCode), Pname (TempCode_PName), SStatus ('true'/'false')
        /// </summary>
        public async Task<IEnumerable<object>> GetAssignedPapersAsync(string schemaName, string course, string regulation, string sem)
        {
            var sql = StoredProcSql.ExecNamed(StoredProcedures.Sp_Eval_Get_PapData,
                "@Regulation", "@Course", "@Sem", "@SchemaName");

            var parameters = new[]
            {
                new SqlParameter("@Regulation", SqlDbType.VarChar, 1000) { Value = regulation ?? string.Empty },
                new SqlParameter("@Course",     SqlDbType.VarChar, 1000) { Value = course     ?? string.Empty },
                new SqlParameter("@Sem",        SqlDbType.VarChar, 1000) { Value = sem        ?? string.Empty },
                new SqlParameter("@SchemaName", SqlDbType.VarChar, 1000) { Value = schemaName ?? string.Empty }
            };
            var raw = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return raw ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Assigns selected papers to tbl_Eval_UserPapers (one row per paper).
        /// SP: Sp_Eval_Save_UserPapers @Regulation, @Course, @ExamMY, @PCode, @PName, @TEMPCODE, @SEM
        /// papCodes from frontend are composite strings: "PCode@TempCode@PName@Sem"
        /// Splits composite to extract PCode, TempCode, PName, Sem.
        /// </summary>
        public async Task<int> SaveApplySchemaAsync(SaveApplySchemaRequest req)
        {
            int totalSaved = 0;
            if (req.PapCodes == null || req.PapCodes.Count == 0) return totalSaved;

            foreach (var composite in req.PapCodes)
            {
                // composite format: "J211D@J211D@STRENGTH OF MATERIALS-I@3"
                var parts    = (composite ?? string.Empty).Split('@');
                var pCode    = parts.Length > 0 ? parts[0] : string.Empty;
                var tempCode = parts.Length > 1 ? parts[1] : pCode;
                var pName    = parts.Length > 2 ? parts[2] : string.Empty;
                var sem      = parts.Length > 3 ? parts[3] : req.Sem ?? string.Empty;

                var sql = StoredProcSql.ExecNamed(StoredProcedures.Sp_Eval_Save_UserPapers,
                    "@Regulation", "@Course", "@ExamMY", "@PCode", "@PName", "@TEMPCODE", "@SEM");

                var parameters = new[]
                {
                    new SqlParameter("@Regulation", SqlDbType.VarChar, 1000) { Value = req.Regulation ?? string.Empty },
                    new SqlParameter("@Course",     SqlDbType.VarChar, 1000) { Value = req.Course     ?? string.Empty },
                    new SqlParameter("@ExamMY",     SqlDbType.VarChar, 1000) { Value = req.ExamMY     ?? string.Empty },
                    new SqlParameter("@PCode",      SqlDbType.VarChar, 1000) { Value = pCode          },
                    new SqlParameter("@PName",      SqlDbType.VarChar, 1000) { Value = pName          },
                    new SqlParameter("@TEMPCODE",   SqlDbType.VarChar, 1000) { Value = tempCode       },
                    new SqlParameter("@SEM",        SqlDbType.VarChar, 1000) { Value = sem            }
                };

                totalSaved += await _repo.ExecuteStoredProcAsync(sql, (object[])parameters);
            }

            return totalSaved;
        }

        /// <summary>
        /// Removes a paper assignment from tbl_Eval_UserPapers.
        /// SP: Sp_Eval_Remove_UserPaper @Regulation, @Course, @ExamMY, @TempCode, @SEM
        /// tempCode is extracted from composite pCode (parts[1]).
        /// </summary>
        public async Task<int> DeleteAppliedPaperAsync(string regulation, string course, string examMY, string tempCode, string sem)
        {
            var sql = StoredProcSql.ExecNamed(StoredProcedures.Sp_Eval_Remove_UserPaper,
                "@Regulation", "@Course", "@ExamMY", "@TempCode", "@SEM");

            var parameters = new[]
            {
                new SqlParameter("@Regulation", SqlDbType.VarChar, 100) { Value = regulation ?? string.Empty },
                new SqlParameter("@Course",     SqlDbType.VarChar, 100) { Value = course     ?? string.Empty },
                new SqlParameter("@ExamMY",     SqlDbType.VarChar, 100) { Value = examMY     ?? string.Empty },
                new SqlParameter("@TempCode",   SqlDbType.VarChar, 100) { Value = tempCode   ?? string.Empty },
                new SqlParameter("@SEM",        SqlDbType.VarChar, 10)  { Value = sem        ?? string.Empty }
            };
            return await _repo.ExecuteStoredProcAsync(sql, (object[])parameters);
        }
    }
}
