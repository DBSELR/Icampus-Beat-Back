// StudentMasterService.cs (only core methods shown)
using Microsoft.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using ICampus_DataAccessLayer.Helpers;
using ICampus_DataAccessLayer.Interfaces;
using ICampus_Models.Requests;
using System.Linq;

public class StudentMasterService : IStudentMasterService
{
    private readonly IGenericRepository<object> _repo;
    public StudentMasterService(IGenericRepository<object> repo) => _repo = repo;

    public async Task<IEnumerable<object>> LoadBranchAsync(string course, string examMy, string regulation)
    {
        // DAL used SQL: SELECT DISTINCT BATCH,REGU FROM TBL_GRP WHERE COURSE = '{Course}'
        // We can call a proc or inline SQL; following your DAL `Load_Branch` used inline SQL.
        var sql = "SELECT DISTINCT BATCH,REGU FROM TBL_GRP WHERE COURSE = @COURSE ORDER BY REGU";
        var p = new SqlParameter("@COURSE", SqlDbType.VarChar) { Value = course ?? string.Empty };
        var raw = await _repo.QueryFromStoredProcAsync(sql, p);
        return raw ?? Enumerable.Empty<object>();
    }

    public async Task<IEnumerable<object>> LoadSemsAsync(string course, string examMy, string regulation)
    {
        // use stored proc PROC_LOAD_SEMS_FOR_FEE? Your DAL used a SELECT on TBL_SH. We'll replicate the DAL query:
        var sql = "SELECT DISTINCT SH.SEM FROM TBL_SH SH WHERE SH.EXAMMY = @EXAMMY AND SH.COURSE = @COURSE AND SH.Regulation = @REGULATION AND regsup = 'reg' ORDER BY SEM";
        var p1 = new SqlParameter("@EXAMMY", SqlDbType.VarChar) { Value = examMy ?? string.Empty };
        var p2 = new SqlParameter("@COURSE", SqlDbType.VarChar) { Value = course ?? string.Empty };
        var p3 = new SqlParameter("@REGULATION", SqlDbType.VarChar) { Value = regulation ?? string.Empty };
        var raw = await _repo.QueryFromStoredProcAsync(sql, p1, p2, p3);
        return raw ?? Enumerable.Empty<object>();
    }

    public async Task<IEnumerable<object>> LoadStdMasterAsync(string course, string examMy, string regu, string sem, string regno)
    {
        var p1 = new SqlParameter("@COURSE", SqlDbType.VarChar) { Value = course ?? string.Empty };
        var p2 = new SqlParameter("@EXAMMY", SqlDbType.VarChar) { Value = examMy ?? string.Empty };
        var p3 = new SqlParameter("@REGU", SqlDbType.VarChar) { Value = regu ?? string.Empty };
        var p4 = new SqlParameter("@SEM", SqlDbType.VarChar) { Value = sem ?? string.Empty };
        var p5 = new SqlParameter("@REGNO", SqlDbType.VarChar) { Value = regno ?? string.Empty };

        var sql = StoredProcSql.Exec(StoredProcedures.PROC_GETSUBJTS_REGNO_WISE, "@COURSE", "@EXAMMY", "@REGU", "@SEM", "@REGNO");
        var raw = await _repo.QueryFromStoredProcAsync(sql, p1, p2, p3, p4, p5);
        return raw ?? Enumerable.Empty<object>();
    }

    public async Task<int> GetCountAsync(string examMy, string regu, string regno)
    {
        // DAL getCount used inline SQL SELECT COUNT(*) FROM TBL_SH WHERE EXAMMY=... REGU=... REGSUP='reg' AND REGNO=...
        var sql = "SELECT COUNT(*) AS CNT FROM TBL_SH WHERE EXAMMY = @EXAMMY AND REGU = @REGU AND REGSUP = 'reg' AND REGNO = @REGNO";
        var p1 = new SqlParameter("@EXAMMY", SqlDbType.VarChar) { Value = examMy ?? string.Empty };
        var p2 = new SqlParameter("@REGU", SqlDbType.VarChar) { Value = regu ?? string.Empty };
        var p3 = new SqlParameter("@REGNO", SqlDbType.VarChar) { Value = regno ?? string.Empty };
        var raw = await _repo.QueryFromStoredProcAsync(sql, p1, p2, p3);
        try
        {
            var first = raw.Cast<Dictionary<string, object>>().FirstOrDefault();
            if (first != null && first.Values.FirstOrDefault() != null && int.TryParse(first.Values.FirstOrDefault().ToString(), out int parsed)) return parsed;
        }
        catch { /* ignore */ }
        return 0;
    }

    public async Task<int> CreateMasterAsync(SaveStudentMasterRequest req)
    {
        var ps = new[]
        {
            new SqlParameter("@COURSE", SqlDbType.VarChar, 30) { Value = req.Course ?? string.Empty },
            new SqlParameter("@EXAMMY", SqlDbType.VarChar, 15) { Value = req.ExamMy ?? string.Empty },
            new SqlParameter("@REGU", SqlDbType.VarChar, 2) { Value = req.Batch ?? string.Empty },
            new SqlParameter("@SEM", SqlDbType.VarChar, 2) { Value = req.Sem ?? string.Empty },
            new SqlParameter("@REGNO", SqlDbType.VarChar, 20) { Value = req.Regno ?? string.Empty }
        };
        var sql = StoredProcSql.Exec(StoredProcedures.SP_MASTER_CREATE_REGNO, "@COURSE", "@EXAMMY", "@REGU", "@SEM", "@REGNO");
        return await _repo.ExecuteStoredProcAsync(sql, ps);
    }

    public async Task<IEnumerable<object>> LoadOmrNumUpdateAsync(string regno, string course, string regulation, string examMy)
    {
        var p1 = new SqlParameter("@REGNO", SqlDbType.VarChar) { Value = regno ?? string.Empty };
        var p2 = new SqlParameter("@COURSE", SqlDbType.VarChar) { Value = course ?? string.Empty };
        var p3 = new SqlParameter("@REGULATION", SqlDbType.VarChar) { Value = regulation ?? string.Empty };
        var p4 = new SqlParameter("@EXAMMY", SqlDbType.VarChar) { Value = examMy ?? string.Empty };

        var sql = StoredProcSql.Exec(StoredProcedures.PROC_OMRNUM_UPDATE, "@REGNO", "@COURSE", "@REGULATION", "@EXAMMY");
        var raw = await _repo.QueryFromStoredProcAsync(sql, p1, p2, p3, p4);
        return raw ?? Enumerable.Empty<object>();
    }

    public async Task<IEnumerable<object>> GetLoadOmrNumUpdateAsync(string regno, string course, string regulation, string examMy, string regsup)
    {
        var p1 = new SqlParameter("@REGNO", SqlDbType.VarChar) { Value = regno ?? string.Empty };
        var p2 = new SqlParameter("@COURSE", SqlDbType.VarChar) { Value = course ?? string.Empty };
        var p3 = new SqlParameter("@REGULATION", SqlDbType.VarChar) { Value = regulation ?? string.Empty };
        var p4 = new SqlParameter("@EXAMMY", SqlDbType.VarChar) { Value = examMy ?? string.Empty };
        var p5 = new SqlParameter("@REGSUP", SqlDbType.VarChar) { Value = regsup ?? string.Empty };

        var sql = StoredProcSql.Exec(StoredProcedures.PROC_OMRNUM_UPDATE_Get, "@REGNO", "@COURSE", "@REGULATION", "@EXAMMY", "@REGSUP");
        var raw = await _repo.QueryFromStoredProcAsync(sql, p1, p2, p3, p4, p5);
        return raw ?? Enumerable.Empty<object>();
    }

    public async Task<int> UpdateOmrNumAsync(UpdateOmrRequest req)
    {
        var ps = new[]
        {
            new SqlParameter("@COURSE", SqlDbType.VarChar, 15) { Value = req.Course ?? string.Empty },
            new SqlParameter("@REGULATION", SqlDbType.VarChar, 5) { Value = req.Regulation ?? string.Empty },
            new SqlParameter("@EXAMMY", SqlDbType.VarChar, 20) { Value = req.ExamMy ?? string.Empty },
            new SqlParameter("@SEM", SqlDbType.Int) { Value = req.Sem },
            new SqlParameter("@OMRNO", SqlDbType.Int) { Value = req.OmrNo },
            new SqlParameter("@ASHID", SqlDbType.Int) { Value = req.AshId }
        };
        var sql = StoredProcSql.Exec(StoredProcedures.PROC_UPDATE_OMRNO, "@COURSE", "@REGULATION", "@EXAMMY", "@SEM", "@OMRNO", "@ASHID");
        return await _repo.ExecuteStoredProcAsync(sql, ps);
    }

    public async Task<IEnumerable<object>> LoadStdUpdateAsync(string regulation, string course, string regu, string grp, string sem)
    {
        var p1 = new SqlParameter("@regulation", SqlDbType.VarChar) { Value = regulation ?? string.Empty };
        var p2 = new SqlParameter("@course", SqlDbType.VarChar) { Value = course ?? string.Empty };
        var p3 = new SqlParameter("@regu", SqlDbType.VarChar) { Value = regu ?? string.Empty };
        var p4 = new SqlParameter("@grp", SqlDbType.VarChar) { Value = grp ?? string.Empty };
        var p5 = new SqlParameter("@sem", SqlDbType.VarChar) { Value = sem ?? string.Empty };

        var sql = StoredProcSql.Exec(StoredProcedures.sp_loadstdexammy_update, "@regulation", "@course", "@regu", "@grp", "@sem");
        var raw = await _repo.QueryFromStoredProcAsync(sql, p1, p2, p3, p4, p5);
        return raw ?? Enumerable.Empty<object>();
    }

    public async Task<int> StdUpdateAsync(StdUpdateRequest req)
    {
        var ps = new[]
        {
            new SqlParameter("@Actexammy", SqlDbType.VarChar) { Value = req.ActExamMy ?? string.Empty },
            new SqlParameter("@regulation", SqlDbType.VarChar) { Value = req.Regulation ?? string.Empty },
            new SqlParameter("@regu", SqlDbType.VarChar) { Value = req.Regu ?? string.Empty },
            new SqlParameter("@course", SqlDbType.VarChar) { Value = req.Course ?? string.Empty },
            new SqlParameter("@grp", SqlDbType.VarChar) { Value = req.Grp ?? string.Empty },
            new SqlParameter("@sem", SqlDbType.VarChar) { Value = req.Sem ?? string.Empty },
            new SqlParameter("@regno", SqlDbType.VarChar) { Value = req.Regno ?? string.Empty },
            new SqlParameter("@exammy", SqlDbType.VarChar) { Value = req.ExamMy ?? string.Empty }
        };

        var sql = StoredProcSql.Exec(StoredProcedures.sp_Exammy_update, "@Actexammy", "@regulation", "@regu", "@course", "@grp", "@sem", "@regno", "@exammy");
        return await _repo.ExecuteStoredProcAsync(sql, ps);
    }

    public async Task<int> MarksUpdateRegnoWiseAsync(MarksUpdateRequest req)
    {
        var ps = new[]
        {
            new SqlParameter("@ASHID", SqlDbType.Int) { Value = req.AshId },
            new SqlParameter("@SMARKS", SqlDbType.VarChar) { Value = req.SMarks ?? string.Empty },
            new SqlParameter("@PMARKS", SqlDbType.VarChar) { Value = req.PMarks ?? string.Empty },
            new SqlParameter("@TMARKS", SqlDbType.VarChar) { Value = req.TMarks ?? string.Empty },
            new SqlParameter("@RVMARKS", SqlDbType.VarChar) { Value = req.RvMarks ?? string.Empty },
            new SqlParameter("@V3MARKS", SqlDbType.VarChar) { Value = req.V3Marks ?? string.Empty },
            new SqlParameter("@mrk_fin", SqlDbType.VarChar) { Value = req.MrkFinal ?? string.Empty }
        };
        var sql = StoredProcSql.Exec(StoredProcedures.PROC_marksupdate_regnowise, "@ASHID", "@SMARKS", "@PMARKS", "@TMARKS", "@RVMARKS", "@V3MARKS", "@mrk_fin");
        return await _repo.ExecuteStoredProcAsync(sql, ps);
    }

    public async Task<int> DeleteAshIdAsync(DeleteAshIdRequest req)
    {
        var p = new SqlParameter("@ASHID", SqlDbType.Int) { Value = req.AshId };
        var sql = StoredProcSql.Exec(StoredProcedures.PROC_DEL_ASHID, "@ASHID");
        return await _repo.ExecuteStoredProcAsync(sql, p);
    }
}
