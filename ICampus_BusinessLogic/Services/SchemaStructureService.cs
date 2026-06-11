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
    public class SchemaStructureService : ISchemaStructureService
    {
        private readonly IGenericRepository<object> _repo;

        public SchemaStructureService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Returns schema names for a given course/regulation/sem.
        /// Inline SQL on TBL_EVAL_SCHEMAMASTER — confirmed from DataAccessLayer.dll analysis.
        /// </summary>
        public async Task<IEnumerable<object>> GetSchemaListAsync(string course, string regulation, string sem)
        {
            var sql = @"SELECT DISTINCT SCHEMANAME FROM TBL_EVAL_SCHEMAMASTER
                        WHERE COURSE = @Course AND REGULATION = @Regulation
                        AND (@Sem = '' OR CAST(SEM AS VARCHAR) = @Sem)
                        ORDER BY SCHEMANAME";

            var parameters = new[]
            {
                new SqlParameter("@Course",     SqlDbType.VarChar, 100) { Value = course     ?? string.Empty },
                new SqlParameter("@Regulation", SqlDbType.VarChar, 100) { Value = regulation ?? string.Empty },
                new SqlParameter("@Sem",        SqlDbType.VarChar, 50)  { Value = sem        ?? string.Empty }
            };

            var raw = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return raw ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Checks if a schema name already exists in TBL_EVAL_SCHEMAMASTER.
        /// SP: Sp_Eval_Check_Schema @SchemaName
        /// Confirmed from DataAccessLayer.dll: "Select COUNT(*) from TBL_EVAL_SCHEMAMASTER where SCHEMANAME='...'"
        /// </summary>
        public async Task<IEnumerable<object>> CheckSchemaAsync(string schemaName)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.Sp_Eval_Check_Schema, "@SchemaName");
            var parameters = new[]
            {
                new SqlParameter("@SchemaName", SqlDbType.VarChar, 200) { Value = schemaName ?? string.Empty }
            };
            var raw = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return raw ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Loads schema master + structure rows for edit form.
        /// Direct SQL join on tbl_Eval_SchemaMaster + TBL_EVAL_SCHEMASTRUCTURE.
        /// (SP_EVAL_LOAD_STRUCTURE_Edit returns grid dimension arrays, not form data.)
        /// </summary>
        public async Task<IEnumerable<object>> LoadStructureForEditAsync(string schemaName)
        {
            var sql = @"SELECT m.SchemaName AS SCHEMANAME, m.COURSE, m.REGULATION,
                               CAST(m.SEM AS VARCHAR) AS SEM,
                               CAST(m.NoofQuestions AS VARCHAR) AS MaxNoofQuestions,
                               m.MaxSections,
                               s.QNO, CAST(s.MAXMRK AS VARCHAR) AS MaxMrk,
                               s.QStatus,
                               s.MaxNoofQuestions AS QMaxNoofQuestions,
                               s.MaxSections      AS QMaxSections
                        FROM tbl_Eval_SchemaMaster m
                        LEFT JOIN TBL_EVAL_SCHEMASTRUCTURE s
                            ON s.SCHEMANAME = m.SchemaName
                           AND s.COURSE     = m.COURSE
                           AND s.REGULATION = m.REGULATION
                        WHERE m.SchemaName = @SchemaName
                        ORDER BY s.QNO";

            var parameters = new[]
            {
                new SqlParameter("@SchemaName", SqlDbType.VarChar, 200) { Value = schemaName ?? string.Empty }
            };
            var raw = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return raw ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Loads schema structure for display/view.
        /// SP: SP_EVAL_LOAD_STRUCTURE @SchemaName
        /// Confirmed from DataAccessLayer.dll string analysis.
        /// </summary>
        public async Task<IEnumerable<object>> LoadStructureAsync(string schemaName)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.SP_EVAL_LOAD_STRUCTURE, "@SchemaName");
            var parameters = new[]
            {
                new SqlParameter("@SchemaName", SqlDbType.VarChar, 200) { Value = schemaName ?? string.Empty }
            };
            var raw = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return raw ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Saves schema master header then all question rows.
        /// SP_EVAL_SCHEMAMASTER_SAVE: @SCHEMANAME, @COURSE, @REGULATION, @SEM, @MaxNoofQuestions, @MaxSections (@PCODE defaults to '')
        /// SP_EVAL_SCHEMASTRUCTURE_SAVE: @COURSE, @REGULATION, @SEM, @PCODE='', @QNO, @MAXMRK INT, @SCHEMANAME, @Mandatory, @MaxNoofQuestions, @MaxSections
        /// </summary>
        public async Task<int> SaveSchemaStructureAsync(SaveSchemaStructureRequest req)
        {
            // Step 1: Save schema master header
            var masterSql = StoredProcSql.ExecNamed(
                StoredProcedures.SP_EVAL_SCHEMAMASTER_SAVE,
                "@SCHEMANAME", "@COURSE", "@REGULATION", "@SEM", "@MaxNoofQuestions", "@MaxSections");

            var masterParams = new[]
            {
                new SqlParameter("@SCHEMANAME",       SqlDbType.VarChar, 1000) { Value = req.SchemaName       ?? string.Empty },
                new SqlParameter("@COURSE",           SqlDbType.VarChar, 20)   { Value = req.Course           ?? string.Empty },
                new SqlParameter("@REGULATION",       SqlDbType.VarChar, 10)   { Value = req.Regulation       ?? string.Empty },
                new SqlParameter("@SEM",              SqlDbType.VarChar, 10)   { Value = req.Sem              ?? string.Empty },
                new SqlParameter("@MaxNoofQuestions", SqlDbType.VarChar, 10)   { Value = req.MaxNoofQuestions ?? "0"          },
                new SqlParameter("@MaxSections",      SqlDbType.VarChar, 10)   { Value = req.MaxSections      ?? string.Empty }
            };

            await _repo.ExecuteStoredProcAsync(masterSql, (object[])masterParams);

            // Step 2: Save each question row
            int totalSaved = 0;
            if (req.Questions != null)
            {
                foreach (var q in req.Questions)
                {
                    var structSql = StoredProcSql.ExecNamed(
                        StoredProcedures.SP_EVAL_SCHEMASTRUCTURE_SAVE,
                        "@COURSE", "@REGULATION", "@SEM", "@PCODE", "@QNO", "@MAXMRK",
                        "@SCHEMANAME", "@Mandatory", "@MaxNoofQuestions", "@MaxSections");

                    _ = int.TryParse(q.MaxMrk, out var maxMrkInt);
                    var structParams = new[]
                    {
                        new SqlParameter("@COURSE",           SqlDbType.VarChar, 20)   { Value = req.Course           ?? string.Empty },
                        new SqlParameter("@REGULATION",       SqlDbType.VarChar, 10)   { Value = req.Regulation       ?? string.Empty },
                        new SqlParameter("@SEM",              SqlDbType.VarChar, 10)   { Value = req.Sem              ?? string.Empty },
                        new SqlParameter("@PCODE",            SqlDbType.VarChar, 20)   { Value = string.Empty         },
                        new SqlParameter("@QNO",              SqlDbType.VarChar, 5)    { Value = q.Qno                ?? string.Empty },
                        new SqlParameter("@MAXMRK",           SqlDbType.Int)           { Value = maxMrkInt            },
                        new SqlParameter("@SCHEMANAME",       SqlDbType.VarChar, 1000) { Value = req.SchemaName       ?? string.Empty },
                        new SqlParameter("@Mandatory",        SqlDbType.VarChar, 2)    { Value = q.QStatus            ?? "O"          },
                        new SqlParameter("@MaxNoofQuestions", SqlDbType.VarChar, 10)   { Value = q.MaxNoofQuestions   ?? string.Empty },
                        new SqlParameter("@MaxSections",      SqlDbType.VarChar, 10)   { Value = q.MaxSections        ?? string.Empty }
                    };

                    totalSaved += await _repo.ExecuteStoredProcAsync(structSql, (object[])structParams);
                }
            }

            return totalSaved;
        }

        /// <summary>
        /// Deletes a schema and all its question structure rows.
        /// SP: Sp_Eval_Delete_Pap_Data @SchemaName
        /// Confirmed from DataAccessLayer.dll string analysis.
        /// </summary>
        public async Task<int> DeleteSchemaAsync(string schemaName)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.Sp_Eval_Delete_Pap_Data, "@SchemaName");
            var parameters = new[]
            {
                new SqlParameter("@SchemaName", SqlDbType.VarChar, 200) { Value = schemaName ?? string.Empty }
            };
            return await _repo.ExecuteStoredProcAsync(sql, (object[])parameters);
        }
    }
}
