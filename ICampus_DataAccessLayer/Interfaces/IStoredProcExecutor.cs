// IStoredProcExecutor.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.Common;

public interface IStoredProcExecutor
{
    Task<IEnumerable<Dictionary<string, object>>> QueryStoredProcAsync(string sql, params DbParameter[] parameters);
    Task<int> ExecuteStoredProcAsync(string sql, params DbParameter[] parameters);
}
