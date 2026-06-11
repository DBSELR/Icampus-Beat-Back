using ICampus_DataAccessLayer.Helpers;
using ICampus_DataAccessLayer.Interfaces;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

public class StudentScreenService : IStudentScreenService
{
    private readonly IGenericRepository<object> _repo;

    public StudentScreenService(IGenericRepository<object> repo)
    {
        _repo = repo;
    }

    // 1. getStudentScreenStudentData  -> inline SQL: SELECT SNAME, COURSE, GRP, PHOTO FROM TBL_STDdATA WHERE REGNO = '{Regno}'
    public async Task<IEnumerable<object>> GetStudentScreenStudentDataAsync(string regno)
    {
        var sql = "SELECT SNAME, COURSE, GRP, PHOTO FROM TBL_STDdATA WHERE REGNO = @REGNO";
        var p = new SqlParameter("@REGNO", SqlDbType.VarChar, 50) { Value = regno ?? string.Empty };
        var raw = await _repo.QueryFromStoredProcAsync(sql, p); // your repo accepts inline SQL as in other services
        return raw ?? Enumerable.Empty<object>();
    }

    // 2. MaxSemetsers -> inline SQL: SELECT MAX(SEM) FROM TBL_SH WHERE REGNO = @REGNO
    public async Task<IEnumerable<object>> GetMaxSemestersAsync(string regno)
    {
        var sql = "SELECT MAX(SEM) AS MaxSem FROM TBL_SH WHERE REGNO = @REGNO";
        var p = new SqlParameter("@REGNO", SqlDbType.VarChar, 50) { Value = regno ?? string.Empty };
        var raw = await _repo.QueryFromStoredProcAsync(sql, p);
        return raw ?? Enumerable.Empty<object>();
    }

    // 3. StudentGrades -> stored proc SPM_REQ_DATA_FOR_EXAMREG @REGNO, @exammy
    public async Task<IEnumerable<object>> GetStudentGradesAsync(string regno, string examMy)
    {
        var p1 = new SqlParameter("@REGNO", SqlDbType.VarChar, 50) { Value = regno ?? string.Empty };
        var p2 = new SqlParameter("@exammy", SqlDbType.VarChar, 30) { Value = examMy ?? string.Empty };

        // StoredProcSql.Exec creates the exec string used across your project (keep consistency)
        var sql = StoredProcSql.Exec(StoredProcedures.SPM_REQ_DATA_FOR_EXAMREG, "@REGNO", "@exammy");
        var raw = await _repo.QueryFromStoredProcAsync(sql, p1, p2);
        return raw ?? Enumerable.Empty<object>();
    }
}
