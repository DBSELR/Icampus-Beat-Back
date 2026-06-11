using ICampus_BusinessLogic.Interfaces;
using ICampus_DataAccessLayer.Helpers;
using ICampus_DataAccessLayer.Interfaces;
using ICampus_Models.Requests;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Linq;

namespace ICampus_BusinessLogic.Services
{
    public class SubjectGradeService : ISubjectGradeService
    {
        private readonly IGenericRepository<object> _repo;
        public SubjectGradeService(IGenericRepository<object> repo) => _repo = repo;

        // 1. Load batches (PROC_BATCH_LOAD @course)
        public async Task<IEnumerable<object>> LoadBatchAsync(string course)
        {
            var p1 = new SqlParameter("@course", SqlDbType.VarChar) { Value = course ?? string.Empty };
            var sql = StoredProcSql.Exec(StoredProcedures.PROC_BATCH_LOAD, "@course");
            var raw = await _repo.QueryFromStoredProcAsync(sql, p1);
            return raw ?? Enumerable.Empty<object>();
        }

        // 2. Load subject grade grid (PEOC_LAOD_GRADEMASTER_GRIDS 'SUBGR' , course, regu)
        public async Task<IEnumerable<object>> LoadSubGradeGridAsync(string course, string regu)
        {
            var pType = new SqlParameter("@TYPE", SqlDbType.VarChar) { Value = "SUBGR" };
            var pCourse = new SqlParameter("@course", SqlDbType.VarChar) { Value = course ?? string.Empty };
            // the proc expects @REGU default ' REGU' but we pass actual value or the placeholder
            var pRegu = new SqlParameter("@REGU", SqlDbType.VarChar) { Value = string.IsNullOrWhiteSpace(regu) ? " REGU" : regu };

            var sql = StoredProcSql.Exec(StoredProcedures.PEOC_LAOD_GRADEMASTER_GRIDS, "@TYPE", "@course", "@REGU");
            var raw = await _repo.QueryFromStoredProcAsync(sql, pType, pCourse, pRegu);
            return raw ?? Enumerable.Empty<object>();
        }

        // 3. Save subject grade (SP_GRADE_SAVE)
        public async Task<int> SaveSubGradeAsync(SubjectGradeSaveRequest req)
        {
            var ps = new[]
            {
                new SqlParameter("@ID", SqlDbType.VarChar, 10) { Value = req.Id ?? string.Empty },
                new SqlParameter("@REGU", SqlDbType.VarChar, 2) { Value = req.Regu ?? string.Empty },
                new SqlParameter("@MRKFROM", SqlDbType.VarChar, 10) { Value = req.MrkFrom ?? string.Empty },
                new SqlParameter("@MRKTO", SqlDbType.VarChar, 10) { Value = req.MrkTo ?? string.Empty },
                new SqlParameter("@GR", SqlDbType.VarChar, 10) { Value = (req.Grade ?? string.Empty).ToUpper() },
                new SqlParameter("@GRPTS", SqlDbType.VarChar, 5) { Value = req.GradePoint ?? string.Empty },
                new SqlParameter("@course", SqlDbType.VarChar, 15) { Value = req.Course ?? string.Empty }
            };

            var sql = StoredProcSql.Exec(StoredProcedures.SP_GRADE_SAVE,
                "@ID", "@REGU", "@MRKFROM", "@MRKTO", "@GR", "@GRPTS", "@course");

            return await _repo.ExecuteStoredProcAsync(sql, ps);
        }

        // 4. Delete subject grade (DELETE FROM TBL_GRADE WHERE ID = @ID)
        public async Task<int> DeleteSubGradeAsync(string id)
        {
            var sql = "DELETE FROM TBL_GRADE WHERE ID = @ID";
            var p = new SqlParameter("@ID", SqlDbType.Int) { Value = string.IsNullOrWhiteSpace(id) ? (object)DBNull.Value : Convert.ToInt32(id) };
            // QueryFromStoredProcAsync can execute inline SQL in your repo implementation; use ExecuteStoredProcAsync for non-query.
            return await _repo.ExecuteStoredProcAsync(sql, p);
        }

        // 5. Copy grade data (PROC_COPY_GRADE_DATA @REGU, @PREGU, @COURSE, @TYPE)
        public async Task<int> CopyGradeAsync(CopyGradeRequest req)
        {
            var ps = new[]
            {
                new SqlParameter("@REGU", SqlDbType.Char, 2) { Value = req.ToBatch ?? string.Empty },
                new SqlParameter("@PREGU", SqlDbType.Char, 2) { Value = req.FromBatch ?? string.Empty },
                new SqlParameter("@COURSE", SqlDbType.VarChar, 20) { Value = req.Course ?? string.Empty },
                new SqlParameter("@TYPE", SqlDbType.VarChar, 20) { Value = req.ProcType ?? "TBL_GRADE" }
            };

            var sql = StoredProcSql.Exec(StoredProcedures.PROC_COPY_GRADE_DATA, "@REGU", "@PREGU", "@COURSE", "@TYPE");
            return await _repo.ExecuteStoredProcAsync(sql, ps);
        }

        // 6. Load regu list (used when copying grades) -- matches DAL.Load_Regu
        public async Task<IEnumerable<object>> LoadReguListAsync(string course, string procType)
        {
            // Build inline SQL similar to DAL.Load_Regu
            string sqlStr;
            if (procType == "TBL_GRADE")
            {
                sqlStr = $"SELECT distinct G.regu,C.BATCH FROM tbl_GRADE G INNER JOIN TBL_GRP C ON G.REGU = C.REGU AND G.COURSE = C.COURSE where c.COURSE = @COURSE";
            }
            else
            {
                sqlStr = $"SELECT distinct G.regu,C.BATCH FROM TBL_SEMGRADE G INNER JOIN TBL_GRP C ON G.REGU = C.REGU AND G.COURSE = C.COURSE where c.COURSE = @COURSE";
            }

            var p = new SqlParameter("@COURSE", SqlDbType.VarChar) { Value = course ?? string.Empty };
            var raw = await _repo.QueryFromStoredProcAsync(sqlStr, p);
            return raw ?? Enumerable.Empty<object>();
        }
    }
}
