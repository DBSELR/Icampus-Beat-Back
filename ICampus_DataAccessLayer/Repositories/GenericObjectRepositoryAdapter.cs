// GenericObjectRepositoryAdapter.cs
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using ICampus_DataAccessLayer.Interfaces;

public class GenericObjectRepositoryAdapter : IGenericRepository<object>
{
    private readonly IStoredProcExecutor _executor;

    public GenericObjectRepositoryAdapter(IStoredProcExecutor executor)
    {
        _executor = executor;
    }

    // Return a sequence of boxed objects (Dictionary<string,object>) so existing callers expecting IEnumerable<object> keep working
    public async Task<IEnumerable<object>> QueryFromStoredProcAsync(string sql, params object[] parameters)
    {
        // Convert incoming parameters (which in existing code are likely SqlParameter or DbParameter) into DbParameter[]
        var dbParams = parameters?
            .Select(p => p as DbParameter)
            .Where(p => p != null)
            .ToArray() ?? Array.Empty<DbParameter>();

        var rows = await _executor.QueryStoredProcAsync(sql, dbParams);
        // box each dictionary as object to satisfy old signature
        return rows.Cast<object>();
    }

    public async Task<int> ExecuteStoredProcAsync(string sql, params object[] parameters)
    {
        var dbParams = parameters?
            .Select(p => p as DbParameter)
            .Where(p => p != null)
            .ToArray() ?? Array.Empty<DbParameter>();

        return await _executor.ExecuteStoredProcAsync(sql, dbParams);
    }
}
