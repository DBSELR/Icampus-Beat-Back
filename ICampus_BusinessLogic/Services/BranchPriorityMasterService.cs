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

public class BranchPriorityMasterService : IBranchPriorityMasterService
{
    private readonly IGenericRepository<object> _repo;

    public BranchPriorityMasterService(IGenericRepository<object> repo)
    {
        _repo = repo;
    }

    // Load branches for a course + regu (maps to Sp_RoomAllot_Load_Course)
    public async Task<IEnumerable<object>> LoadCourseBranchesAsync(string course, string regu)
    {
        var p1 = new SqlParameter("@COURSE", SqlDbType.VarChar) { Value = course ?? string.Empty };
        var p2 = new SqlParameter("@REGULATION", SqlDbType.VarChar) { Value = regu ?? string.Empty };

        var sql = StoredProcSql.Exec(StoredProcedures.Sp_RoomAllot_Load_Course, "@COURSE", "@REGULATION");
        var raw = await _repo.QueryFromStoredProcAsync(sql, p1, p2);
        return raw ?? Enumerable.Empty<object>();
    }

    // Save branch (maps to Branchmaster(B) in legacy code)
    public async Task<int> SaveBranchMasterAsync(BranchSaveRequest req)
    {
        // Build parameter list matching the stored procedure exactly
        var ps = new[]
        {
        new SqlParameter("@PRIORITY", SqlDbType.Int)
        {
            Value = (object?)req.Priority ?? 0  // 0 → SP auto-generates if <=0
        },
        new SqlParameter("@SEM", SqlDbType.Int)
        {
            Value = (object?)req.Sem ?? DBNull.Value
        },
        new SqlParameter("@Branch", SqlDbType.VarChar, 10)
        {
            Value = (object)(req.Branch ?? string.Empty)
        },
        new SqlParameter("@DaySession", SqlDbType.VarChar, 50)
        {
            Value = (object)(req.DaySession ?? string.Empty)
        },
        new SqlParameter("@Course", SqlDbType.VarChar, 50)
        {
            Value = (object)(req.Course ?? string.Empty)
        },
        new SqlParameter("@ExamMy", SqlDbType.VarChar, 50)
        {
            Value = (object)(req.ExamMy ?? string.Empty)
        }
    };

        // Call stored procedure (no EDATE now)
        var sql = StoredProcSql.Exec(
            StoredProcedures.SPM_BRANCHPRIORITY_SAVE,
            "@PRIORITY", "@SEM", "@Branch", "@DaySession", "@Course", "@ExamMy"
        );

        return await _repo.ExecuteStoredProcAsync(sql, ps);
    }


    // check priority (maps to checkRoomPriority)
    public async Task<IEnumerable<object>> LoadRoomMasterAsync(RoomMasterListRequest req)
    {
        var p1 = new SqlParameter("@ID", SqlDbType.Int) { Value = (object?)req.Id ?? 0 };
        var p2 = new SqlParameter("@Session", SqlDbType.VarChar, 10) { Value = (object)(req.Session ?? string.Empty) };

        var sql = StoredProcSql.Exec(StoredProcedures.SPM_RoomMaster_List, "@ID", "@Session");
        var raw = await _repo.QueryFromStoredProcAsync(sql, p1, p2);
        return raw ?? Enumerable.Empty<object>();
    }

    // 2) LoadBranchPriority -> SPM_BranchPriority_List @ID INT = 0, @Session varchar(10)
    public async Task<IEnumerable<object>> LoadBranchPriorityAsync(RoomMasterListRequest req)
    {
        var p1 = new SqlParameter("@ID", SqlDbType.Int) { Value = (object?)req.Id ?? 0 };
        var p2 = new SqlParameter("@Session", SqlDbType.VarChar, 10) { Value = (object)(req.Session ?? string.Empty) };

        var sql = StoredProcSql.Exec(StoredProcedures.SPM_BranchPriority_List, "@ID", "@Session");
        var raw = await _repo.QueryFromStoredProcAsync(sql, p1, p2);
        return raw ?? Enumerable.Empty<object>();
    }

    // 3) checkRoomPriority -> SPM_CheckRoomPriority @PRIORITY INT
    public async Task<int> CheckRoomPriorityAsync(int priority)
    {
        var p = new SqlParameter("@PRIORITY", SqlDbType.Int) { Value = priority };
        var sql = StoredProcSql.Exec(StoredProcedures.SPM_CheckRoomPriority, "@PRIORITY");
        var raw = await _repo.QueryFromStoredProcAsync(sql, p);

        // original webform returned first column of first row or 0
        try
        {
            var dict = raw.Cast<Dictionary<string, object>>().FirstOrDefault();
            if (dict != null && dict.Count > 0)
            {
                var val = dict.Values.First();
                if (int.TryParse(val?.ToString(), out int parsed)) return parsed;
            }
        }
        catch { /* ignore and return 0 */ }

        return 0;
    }

    // 4) UpdateBranchPriority -> original replaced tokens:
    // Q = RM.Up_Q.Replace("SETPRIORITY", "UPDATE [TBL_BRANCHPRIORITY] SET ");
    // Q = Q.Replace("CONDITION", " WHERE ");
    // return dal.InsertData(Q);
    public async Task<int> UpdateBranchPriorityAsync(RawUpdateRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.UpdateQuery))
            return 0;

        var q = req.UpdateQuery
                   .Replace("SETPRIORITY", "UPDATE [TBL_BRANCHPRIORITY] SET ", StringComparison.OrdinalIgnoreCase)
                   .Replace("CONDITION", " WHERE ", StringComparison.OrdinalIgnoreCase);

        // Execute raw SQL. Use your repo's method name for executing non-SP SQL.
        // IGenericRepository: ExecuteSqlAsync(string sql, params SqlParameter[] parms)
        return await _repo.ExecuteStoredProcAsync(q);
    }

    // 5) UpdateRoomPriority -> same pattern but updates [tbl_RoomMaster]
    public async Task<int> UpdateRoomPriorityAsync(RawUpdateRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.UpdateQuery))
            return 0;

        var q = req.UpdateQuery
                   .Replace("SETPRIORITY", "UPDATE [tbl_RoomMaster] SET ", StringComparison.OrdinalIgnoreCase)
                   .Replace("CONDITION", " WHERE ", StringComparison.OrdinalIgnoreCase);

        return await _repo.ExecuteStoredProcAsync(q);
    }

    // 6) DeleteRoom -> DELETE FROM [tbl_RoomMaster] WHERE ROOMNO = '...'
    public async Task<int> DeleteRoomAsync(DeleteRoomRequest req)
    {
        // parameterized delete to avoid SQL injection
        var sql = "DELETE FROM [tbl_RoomMaster] WHERE ROOMNO = @ROOMNO";
        var p = new SqlParameter("@ROOMNO", SqlDbType.VarChar, 50) { Value = (object)(req.RoomNo ?? string.Empty) };

        return await _repo.ExecuteStoredProcAsync(sql, p);
    }

    // 7) DeleteBranch_Priority -> DELETE FROM TBL_BRANCHPRIORITY WHERE PRIORITY='..' AND BRANCH='..' AND DAYSESSION='..'
    public async Task<int> DeleteBranchPriorityAsync(DeleteBranchPriorityRequest req)
    {
        var sql = "DELETE FROM TBL_BRANCHPRIORITY WHERE PRIORITY = @PRIORITY AND Branch = @BRANCH AND DaySession = @DAYSESSION";
        var ps = new[]
        {
            new SqlParameter("@PRIORITY", SqlDbType.Int) { Value = req.Priority },
            new SqlParameter("@BRANCH", SqlDbType.VarChar, 10) { Value = (object)(req.Branch ?? string.Empty) },
            new SqlParameter("@DAYSESSION", SqlDbType.VarChar, 50) { Value = (object)(req.DaySession ?? string.Empty) }
        };

        return await _repo.ExecuteStoredProcAsync(sql, ps);
    }
}
