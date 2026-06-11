using ICampus_BusinessLogic.Interfaces;
using ICampus_DataAccessLayer.Helpers;
using ICampus_DataAccessLayer.Interfaces;
using ICampus_Models.Requests;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

public class SemesterGradeService : ISemesterGradeService
{
    private readonly IGenericRepository<object> _repo;
    public SemesterGradeService(IGenericRepository<object> repo) => _repo = repo;

    // 1. Load batches -> PROC_BATCH_LOAD @course
    public async Task<IEnumerable<object>> LoadBatchesAsync(string course)
    {
        var p1 = new SqlParameter("@course", SqlDbType.VarChar) { Value = course ?? string.Empty };
        var sql = StoredProcSql.Exec(StoredProcedures.PROC_BATCH_LOAD, "@course");
        var raw = await _repo.QueryFromStoredProcAsync(sql, p1);
        return raw ?? Enumerable.Empty<object>();
    }

    // 2. Load sem grade grid -> sp_LOAD_GRADEMASTER_GRIDS 'SEMGR', @course, @REGU
    public async Task<IEnumerable<object>> LoadSemGradeGridAsync(string course, string regu)
    {
        // SP expects first param TYPE (SEMGR), then course and optional REGU. We'll pass REGU as string literal in exec.
        // Using the same StoredProcSql.Exec style: list parameters in order used by Exec helper.
        var p1 = new SqlParameter("@TYPE", SqlDbType.Char) { Value = "SEMGR" };
        var p2 = new SqlParameter("@course", SqlDbType.VarChar) { Value = course ?? string.Empty };
        var p3 = new SqlParameter("@REGU", SqlDbType.VarChar) { Value = string.IsNullOrWhiteSpace(regu) ? " REGU" : regu };

        var sql = StoredProcSql.Exec(StoredProcedures.SP_LOAD_GRADEMASTER_GRIDS, "@TYPE", "@course", "@REGU");
        var raw = await _repo.QueryFromStoredProcAsync(sql, p1, p2, p3);
        return raw ?? Enumerable.Empty<object>();
    }

    // 3. Save sem grade -> SP_SEMGRADE_SAVE
    public async Task<int> SaveSemGradeAsync(SemGradeSaveRequest req)
    {
        var ps = new[]
        {
            new SqlParameter("@ID", SqlDbType.VarChar) { Value = req.Id ?? string.Empty },
            new SqlParameter("@REGU", SqlDbType.VarChar) { Value = req.Regu ?? string.Empty },
            new SqlParameter("@MRKFROM", SqlDbType.VarChar) { Value = req.SgpaFrom ?? string.Empty },
            new SqlParameter("@MRKTO", SqlDbType.VarChar) { Value = req.SgpaTo ?? string.Empty },
            new SqlParameter("@GR", SqlDbType.VarChar) { Value = (req.Grade ?? string.Empty).ToUpper() },
            new SqlParameter("@course", SqlDbType.VarChar) { Value = req.Course ?? string.Empty }
        };

        var sql = StoredProcSql.Exec(StoredProcedures.SP_SEMGRADE_SAVE, "@ID", "@REGU", "@MRKFROM", "@MRKTO", "@GR", "@course");
        return await _repo.ExecuteStoredProcAsync(sql, ps);
    }

    // 4. Delete sem grade -> inline DELETE (or use the same SP if exists). We'll keep same behaviour as old DAL:
    public async Task<int> DeleteSemGradeAsync(DeleteRequest req)
    {
        // The legacy DAL did: DELETE FROM TBL_SEMGRADE WHERE ID = {ID}
        // We'll run inline SQL (the repo supports executing SQL via QueryFromStoredProcAsync)
        var sql = $"DELETE FROM TBL_SEMGRADE WHERE ID = {req.Id}";
        return await _repo.ExecuteStoredProcAsync(sql);
    }

    // 5. Check sem grade -> SELECT COUNT(*) query used in DAL (returns table)
    public async Task<IEnumerable<object>> CheckSemGradeAsync(string course, string toBatch, string type)
    {
        // maps to: SELECT COUNT(*) FROM tbl_SEMGRADE WHERE REGU = @To_Batch AND COURSE = @Course
        string sql;
        if (type == "TBL_GRADE")
            sql = $"SELECT COUNT(*) AS Cnt FROM TBL_GRADE WHERE REGU = '{toBatch}' AND COURSE = '{course}'";
        else
            sql = $"SELECT COUNT(*) AS Cnt FROM TBL_SEMGRADE WHERE REGU = '{toBatch}' AND COURSE = '{course}'";

        var raw = await _repo.QueryFromStoredProcAsync(sql);
        return raw ?? Enumerable.Empty<object>();
    }

    // 6. Copy grade -> PROC_COPY_GRADE_DATA @REGU, @PREGU, @COURSE, @TYPE
    public async Task<int> CopyGradeAsync(CopySemesterGradeRequest req)
    {
        var ps = new[]
        {
            new SqlParameter("@REGU", SqlDbType.Char, 2) { Value = req.ToBatch ?? string.Empty },
            new SqlParameter("@PREGU", SqlDbType.Char, 2) { Value = req.FromBatch ?? string.Empty },
            new SqlParameter("@COURSE", SqlDbType.VarChar, 20) { Value = req.Course ?? string.Empty },
            new SqlParameter("@TYPE", SqlDbType.VarChar, 20) { Value = string.IsNullOrWhiteSpace(req.Type) ? "TBL_SEMGRADE" : req.Type }
        };

        var sql = StoredProcSql.Exec(StoredProcedures.PROC_COPY_GRADE_DATA, "@REGU", "@PREGU", "@COURSE", "@TYPE");
        return await _repo.ExecuteStoredProcAsync(sql, ps);
    }

    // 7. Load regu (distinct regu, batch) similar to DAL.Load_Regu
    public async Task<IEnumerable<object>> LoadReguAsync(string course, string type)
    {
        // Compose inline SQL similar to DAL.Load_Regu
        string sql;
        if (type == "TBL_GRADE")
            sql = $"SELECT distinct G.regu, C.BATCH FROM tbl_GRADE G INNER JOIN TBL_GRP C ON G.REGU = C.REGU AND G.COURSE = C.COURSE WHERE c.COURSE = '{course}'";
        else
            sql = $"SELECT distinct G.regu, C.BATCH FROM TBL_SEMGRADE G INNER JOIN TBL_GRP C ON G.REGU = C.REGU AND G.COURSE = C.COURSE WHERE c.COURSE = '{course}'";

        var raw = await _repo.QueryFromStoredProcAsync(sql);
        return raw ?? Enumerable.Empty<object>();
    }
}
