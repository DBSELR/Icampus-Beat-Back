using ICampus_BusinessLogic.Interfaces;
using ICampus_DataAccessLayer.Helpers;
using ICampus_DataAccessLayer.Interfaces;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Services
{
    public class ResultProcessService : IResultProcessService
    {
        private readonly IGenericRepository<object> _repo;

        public ResultProcessService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Load ExamMY dropdown for Result Process page
        /// SP: SPM_EXAMS_ExamMY_Load(@regulation, @COURSE)
        /// Confirmed: EXEC SPM_EXAMS_ExamMY_Load 'R20','B.Tech' → 23 rows
        /// Note: @Regulation must NOT be empty — filters tbl_sh WHERE REGULATION = @regulation
        /// </summary>
        public async Task<IEnumerable<object>> LoadExamMyAsync(string course, string regulation)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.SPM_EXAMS_ExamMY_Load,
                "@Regulation", "@Course");

            var parameters = new[]
            {
                new SqlParameter("@Regulation", SqlDbType.VarChar, 20) { Value = regulation ?? string.Empty },
                new SqlParameter("@Course",     SqlDbType.VarChar, 30) { Value = course     ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Prepare the 'testee' table expected by SP_RESULT_PROCESS_REGNOWISE.
        /// The original web app created this permanent table and populated it
        /// with the target regno(s) before calling the SP.
        /// CREATE if not exists → DELETE all → INSERT @REGNO
        /// Also drops TLB_RESULT_PROCESS_TEMP if it exists from a stale/partial run,
        /// since the SP does SELECT INTO and will fail if the table already exists
        /// with mismatched columns (e.g. GRFLAG added to TBL_SH after old table was created).
        /// </summary>
        private async Task PrepareTesteeTableAsync(string regNo)
        {
            var sql = @"
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'testee')
    CREATE TABLE testee (regno VARCHAR(20));
DELETE FROM testee;
INSERT INTO testee (regno) VALUES (@REGNO);
IF OBJECT_ID('TLB_RESULT_PROCESS_TEMP', 'U') IS NOT NULL
    DROP TABLE TLB_RESULT_PROCESS_TEMP;";

            var p = new SqlParameter("@REGNO", SqlDbType.VarChar, 20) { Value = regNo ?? string.Empty };
            await _repo.ExecuteStoredProcAsync(sql, new[] { p });
        }

        /// <summary>
        /// Run result process for a student (regno-wise)
        /// SP: SP_RESULT_PROCESS_REGNOWISE(@Regulation, @COURSE, @EXAMMY1, @SEM, @GRP, @REGNO, @flag)
        /// Confirmed: EXEC SP_RESULT_PROCESS_REGNOWISE 'R20','B.Tech','May-2024','8','CE','20671A0101','NORMAL'
        /// The SP reads from the permanent 'testee' table to filter by regno.
        /// </summary>
        public async Task<int> RunResultProcessAsync(
            string regNo, string examMy, string regulation, string course, string sem, string grp)
        {
            await PrepareTesteeTableAsync(regNo);

            var sql = StoredProcSql.Exec(StoredProcedures.SP_RESULT_PROCESS_REGNOWISE,
                "@Regulation", "@COURSE", "@EXAMMY1", "@SEM", "@GRP", "@REGNO", "@flag");

            var parameters = new[]
            {
                new SqlParameter("@Regulation", SqlDbType.VarChar, 100) { Value = regulation ?? string.Empty },
                new SqlParameter("@COURSE",     SqlDbType.VarChar, 100) { Value = course     ?? string.Empty },
                new SqlParameter("@EXAMMY1",    SqlDbType.VarChar, 20)  { Value = examMy     ?? string.Empty },
                new SqlParameter("@SEM",        SqlDbType.VarChar, 100) { Value = sem        ?? string.Empty },
                new SqlParameter("@GRP",        SqlDbType.VarChar, 100) { Value = grp        ?? string.Empty },
                new SqlParameter("@REGNO",      SqlDbType.VarChar, 20)  { Value = regNo      ?? string.Empty },
                new SqlParameter("@flag",       SqlDbType.VarChar, 30)  { Value = "NORMAL" }
            };

            return await _repo.ExecuteStoredProcAsync(sql, parameters);
        }

        /// <summary>
        /// Run readmit result process for a student
        /// SP: SP_RESULT_PROCESS_readmit(@Regulation, @COURSE, @EXAMMY1, @SEM, @flag, @GRP, @READMIT_REGULATION)
        /// </summary>
        public async Task<int> RunReadmitResultProcessAsync(
            string regNo, string examMy, string regulation, string course, string sem, string grp, string readmitRegulation)
        {
            await PrepareTesteeTableAsync(regNo);

            var sql = StoredProcSql.Exec(StoredProcedures.SP_RESULT_PROCESS_readmit,
                "@Regulation", "@COURSE", "@EXAMMY1", "@SEM", "@flag", "@GRP", "@READMIT_REGULATION");

            var parameters = new[]
            {
                new SqlParameter("@Regulation",        SqlDbType.VarChar, 100) { Value = regulation        ?? string.Empty },
                new SqlParameter("@COURSE",            SqlDbType.VarChar, 100) { Value = course            ?? string.Empty },
                new SqlParameter("@EXAMMY1",           SqlDbType.VarChar, 20)  { Value = examMy            ?? string.Empty },
                new SqlParameter("@SEM",               SqlDbType.VarChar, 100) { Value = sem               ?? string.Empty },
                new SqlParameter("@flag",              SqlDbType.VarChar, 30)  { Value = "NORMAL" },
                new SqlParameter("@GRP",               SqlDbType.VarChar, 100) { Value = grp               ?? string.Empty },
                new SqlParameter("@READMIT_REGULATION",SqlDbType.VarChar, 20)  { Value = readmitRegulation ?? string.Empty }
            };

            return await _repo.ExecuteStoredProcAsync(sql, parameters);
        }
    }
}
