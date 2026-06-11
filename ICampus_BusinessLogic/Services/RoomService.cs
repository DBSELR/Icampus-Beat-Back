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

public class RoomService : IRoomService
{
    private readonly IGenericRepository<object> _repo;
    public RoomService(IGenericRepository<object> repo) => _repo = repo;

    // Load room list SP: SPM_RoomMaster_List (@ID INT = 0, @Session VARCHAR(10))
    public async Task<IEnumerable<object>> LoadRoomMasterAsync(int id, string session)
    {
        var p1 = new SqlParameter("@ID", SqlDbType.Int) { Value = id };
        var p2 = new SqlParameter("@Session", SqlDbType.VarChar) { Value = session ?? string.Empty };

        var sql = StoredProcSql.Exec(StoredProcedures.SPM_RoomMaster_List, "@ID", "@Session");
        var raw = await _repo.QueryFromStoredProcAsync(sql, p1, p2);
        return raw ?? Enumerable.Empty<object>();
    }

    // Save room SP: SPM_ROOMMASTER_SAVE (params mapped below)
    public async Task<int> SaveRoomAsync(RoomSaveRequest req)
    {
        var ps = new[]
        {
            new SqlParameter("@ROOMNO", SqlDbType.VarChar, 10) { Value = req.RoomNo ?? string.Empty },
            new SqlParameter("@NOOFCOLUMNS", SqlDbType.TinyInt) { Value = (object)req.NoOfColumns ?? DBNull.Value },
            new SqlParameter("@NOOFROWS", SqlDbType.TinyInt) { Value = (object)req.NoOfRows ?? DBNull.Value },
            new SqlParameter("@PRIORITY", SqlDbType.Int) { Value = req.Priority },
            new SqlParameter("@CAPACITY", SqlDbType.Int) { Value = req.Capacity },
            new SqlParameter("@SEM", SqlDbType.Int) { Value = req.Sem ?? (object)DBNull.Value },
            new SqlParameter("@TotalBranches", SqlDbType.Int) { Value = req.TotalBranches },
            new SqlParameter("@DaySession", SqlDbType.VarChar, 50) { Value = req.DaySession ?? string.Empty },
            new SqlParameter("@Course", SqlDbType.VarChar, 50) { Value = req.Course ?? string.Empty },
            new SqlParameter("@ExamMy", SqlDbType.VarChar, 50) { Value = req.ExamMy ?? string.Empty },
            new SqlParameter("@RType", SqlDbType.VarChar, 50) { Value = req.RoomType ?? string.Empty }
        };

        var sql = StoredProcSql.Exec(StoredProcedures.SPM_ROOMMASTER_SAVE,
            "@ROOMNO", "@NOOFCOLUMNS", "@NOOFROWS", "@PRIORITY", "@CAPACITY", "@SEM", "@TotalBranches", "@DaySession", "@Course", "@ExamMy", "@RType");

        return await _repo.ExecuteStoredProcAsync(sql, ps);
    }

    public async Task<int> CheckRoomPriorityAsync(int priority)
    {
        var p1 = new SqlParameter("@PRIORITY", SqlDbType.Int) { Value = priority };
        var sql = StoredProcSql.Exec(StoredProcedures.SPM_CheckRoomPriority, "@PRIORITY");
        var raw = await _repo.QueryFromStoredProcAsync(sql, p1);

        // parse scalar from result similar to FeeService
        try
        {
            // raw -> JSON -> List<Dictionary<string, object>>
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(raw);
            var list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(json);
            if (list != null && list.Count > 0)
            {
                var first = list[0];
                if (first.Count > 0)
                {
                    var val = first.Values.FirstOrDefault();
                    if (val != null && int.TryParse(val.ToString(), out int parsed)) return parsed;
                }
            }
        }
        catch { }

        return 0;
    }

    public async Task<int> UpdateRoomPriorityAsync(UpdatePriorityRequest req)
    {
        // DAL used a raw SQL string in Up_Q — replicate that behaviour: pass UP_Q in a single param
        // Historically DAL used RM.Up_Q.Replace(...) and dal.InsertData(Q). Here we expect client to build the SQL partial similarly.
        var ps = new[] { new SqlParameter("@UPQ", SqlDbType.VarChar) { Value = req.UpdateQuery ?? string.Empty } };
        // We'll execute inline SQL (not recommended) — but to keep consistency, execute as text (not stored proc)
        // Implementation note: replace this with a parametrized stored proc if possible.
        var sql = req.UpdateQuery ?? string.Empty;
        return await _repo.ExecuteStoredProcAsync(sql); // ensure your repo has ExecuteSqlAsync for raw SQL; if not, use ExecuteStoredProcAsync with a small wrapper proc.
    }

    public async Task<int> DeleteRoomAsync(string roomNo)
    {
        var sql = $"DELETE FROM [tbl_RoomMaster] WHERE ROOMNO = @ROOMNO";
        var p = new SqlParameter("@ROOMNO", SqlDbType.VarChar, 10) { Value = roomNo ?? string.Empty };
        return await _repo.ExecuteStoredProcAsync(sql, p);
    }

    // Branch priority list
    public async Task<IEnumerable<object>> LoadBranchPriorityAsync(int id, string session)
    {
        var p1 = new SqlParameter("@ID", SqlDbType.Int) { Value = id };
        var p2 = new SqlParameter("@Session", SqlDbType.VarChar) { Value = session ?? string.Empty };
        var sql = StoredProcSql.Exec(StoredProcedures.SPM_BranchPriority_List, "@ID", "@Session");
        var raw = await _repo.QueryFromStoredProcAsync(sql, p1, p2);
        return raw ?? Enumerable.Empty<object>();
    }

    public async Task<int> SaveBranchPriorityAsync(BranchPrioritySaveRequest req)
    {
        // DAL had: SPM_BRANCHPRIORITY_SAVE priority, SEM, Branch, Session, Course, ExamMy
        var ps = new[]
        {
            new SqlParameter("@PRIORITY", SqlDbType.Int) { Value = req.Priority },
            new SqlParameter("@SEM", SqlDbType.Int) { Value = req.Sem ?? (object)DBNull.Value },
            new SqlParameter("@BRANCH", SqlDbType.VarChar, 20) { Value = req.Branch ?? string.Empty },
            new SqlParameter("@SESSION", SqlDbType.VarChar, 50) { Value = req.Session ?? string.Empty },
            new SqlParameter("@COURSE", SqlDbType.VarChar, 50) { Value = req.Course ?? string.Empty },
            new SqlParameter("@EXAMMY", SqlDbType.VarChar, 50) { Value = req.ExamMy ?? string.Empty },
        };

        // There's no stored proc signature in your sample file for SPM_BRANCHPRIORITY_SAVE; but DAL used the name without params.
        // We'll execute an inline exec if the stored proc exists on DB: Exec(StoredProcedures.SPM_BRANCHPRIORITY_SAVE,...)
        var sql = StoredProcSql.Exec(StoredProcedures.SPM_BRANCHPRIORITY_SAVE,
            "@PRIORITY", "@SEM", "@BRANCH", "@SESSION", "@COURSE", "@EXAMMY");

        return await _repo.ExecuteStoredProcAsync(sql, ps);
    }

    public async Task<int> DeleteBranchPriorityAsync(string priority, string branch, string session)
    {
        var sql = "DELETE FROM TBL_BRANCHPRIORITY WHERE PRIORITY=@PRIORITY AND BRANCH=@BRANCH AND DAYSESSION=@SESSION";
        var ps = new[]
        {
            new SqlParameter("@PRIORITY", SqlDbType.VarChar) { Value = priority ?? string.Empty },
            new SqlParameter("@BRANCH", SqlDbType.VarChar) { Value = branch ?? string.Empty },
            new SqlParameter("@SESSION", SqlDbType.VarChar) { Value = session ?? string.Empty }
        };
        return await _repo.ExecuteStoredProcAsync(sql, ps);
    }
}
