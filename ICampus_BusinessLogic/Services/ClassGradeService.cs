using ICampus_DataAccessLayer.Helpers;
using ICampus_DataAccessLayer.Interfaces;
using ICampus_Models.Requests;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

public class ClassGradeService : IClassGradeService
{
    private readonly IGenericRepository<object> _repo;
    public ClassGradeService(IGenericRepository<object> repo) => _repo = repo;

    // 1. Load batches => PROC_BATCH_LOAD @course
    public async Task<IEnumerable<object>> LoadBatchAsync(string course)
    {
        var p1 = new SqlParameter("@course", SqlDbType.VarChar) { Value = course ?? string.Empty };
        var sql = StoredProcSql.Exec(StoredProcedures.PROC_BATCH_LOAD, "@course");
        var raw = await _repo.QueryFromStoredProcAsync(sql, p1);
        return raw ?? Enumerable.Empty<object>();
    }

    // 2. Load class grid => sp_LOAD_CLASSMASTER_GRIDS 'SEMGR', @COURSE, @REGU
    public async Task<IEnumerable<object>> LoadClassGradeGridAsync(string course, string regu)
    {
        // stored proc in your SQL expects @TYPE, @COURSE, @REGU
        var p1 = new SqlParameter("@TYPE", SqlDbType.VarChar) { Value = "SEMGR" };
        var p2 = new SqlParameter("@COURSE", SqlDbType.VarChar) { Value = course ?? string.Empty };
        // pass 'REGU' default or actual value: the existing SP accepts a default 'REGU' text; but in DAL pattern they pass the value,
        // above ASPX passes either blank or a regu value. We'll pass the regu or "REGU" to match original behavior.
        var p3 = new SqlParameter("@REGU", SqlDbType.VarChar) { Value = string.IsNullOrWhiteSpace(regu) ? "REGU" : regu };

        var sql = StoredProcSql.Exec(StoredProcedures.sp_LOAD_CLASSMASTER_GRIDS, "@TYPE", "@COURSE", "@REGU");
        var raw = await _repo.QueryFromStoredProcAsync(sql, p1, p2, p3);
        return raw ?? Enumerable.Empty<object>();
    }

    // 3. Save class grade => SP_CLASSGRADE_SAVE @ID,@REGU,@MRKFROM,@MRKTO,@Class,@Course
    public async Task<int> SaveClassGradeAsync(ClassGradeSaveRequest req)
    {
        var ps = new[]
        {
            new SqlParameter("@ID", SqlDbType.VarChar, 10) { Value = req.Id ?? string.Empty },
            new SqlParameter("@REGU", SqlDbType.VarChar, 2) { Value = req.Regu ?? string.Empty },
            new SqlParameter("@MRKFROM", SqlDbType.VarChar, 6) { Value = Convert.ToString(req.SgpaFrom) ?? string.Empty },
            new SqlParameter("@MRKTO", SqlDbType.VarChar, 6) { Value = Convert.ToString(req.SgpaTo) ?? string.Empty },
            new SqlParameter("@Class", SqlDbType.VarChar, 50) { Value = req.ClassName?.ToUpper() ?? string.Empty },
            new SqlParameter("@Course", SqlDbType.VarChar, 15) { Value = req.Course ?? string.Empty },
        };

        var sql = StoredProcSql.Exec(StoredProcedures.SP_CLASSGRADE_SAVE, "@ID", "@REGU", "@MRKFROM", "@MRKTO", "@Class", "@Course");
        return await _repo.ExecuteStoredProcAsync(sql, ps);
    }

    // 4. Delete class => inline SQL (DELETE FROM TBL_CLASS WHERE ID = {id})
    public async Task<int> DeleteClassGradeAsync(IdDeleteRequest req)
    {
        // preserve the original behavior: DAL used a raw DELETE string. But here we will call an inline statement.
        var sql = $"DELETE FROM TBL_CLASS WHERE ID = @ID";
        var p = new SqlParameter("@ID", SqlDbType.Int) { Value = req.Id };
        // reuse repository ExecuteStoredProcAsync to run inline SQL — existing Generic repo wraps that.
        return await _repo.ExecuteStoredProcAsync(sql, p);
    }

    // 5. Copy class grade from prev regu => PROC_COPY_GRADE_Class_DATA @REGU,@PREGU,@COURSE
    public async Task<int> CopyClassGradeFromPrevReguAsync(CopyClassGradeRequest req)
    {
        var ps = new[]
        {
            new SqlParameter("@REGU", SqlDbType.VarChar, 2) { Value = req.ToRegu ?? string.Empty },
            new SqlParameter("@PREGU", SqlDbType.VarChar, 2) { Value = req.FromRegu ?? string.Empty },
            new SqlParameter("@COURSE", SqlDbType.VarChar, 20) { Value = req.Course ?? string.Empty }
        };

        var sql = StoredProcSql.Exec(StoredProcedures.PROC_COPY_GRADE_Class_DATA, "@REGU", "@PREGU", "@COURSE");
        return await _repo.ExecuteStoredProcAsync(sql, ps);
    }
}
