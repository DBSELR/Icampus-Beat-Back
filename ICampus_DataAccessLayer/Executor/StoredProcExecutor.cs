// StoredProcExecutor.cs
using ICampus_DataAccessLayer.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

public class StoredProcExecutor : IStoredProcExecutor
{
    private readonly ICampusDbContext _context;

    public StoredProcExecutor(ICampusDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Dictionary<string, object>>> QueryStoredProcAsync(string sql, params DbParameter[] parameters)
    {
        var results = new List<Dictionary<string, object>>();

        var conn = _context.Database.GetDbConnection();
        // NOTE: do not Dispose connection here if context will reuse it — but awaiting using is safe
        if (conn.State != ConnectionState.Open)
            await conn.OpenAsync();

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "SET ARITHABORT ON; " + sql;
        cmd.CommandType = CommandType.Text; // you can also use CommandType.StoredProcedure if you modify call pattern
        cmd.CommandTimeout = 300;

        if (parameters != null)
        {
            foreach (var p in parameters)
                cmd.Parameters.Add(p);
        }

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var row = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < reader.FieldCount; i++)
            {
                var val = await reader.IsDBNullAsync(i) ? null : reader.GetValue(i);
                row[reader.GetName(i)] = val;
            }
            results.Add(row);
        }

        return results;
    }

    public async Task<int> ExecuteStoredProcAsync(string sql, params DbParameter[] parameters)
    {
        var conn = _context.Database.GetDbConnection();
        if (conn.State != ConnectionState.Open)
            await conn.OpenAsync();

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "SET ARITHABORT ON; " + sql;
        cmd.CommandType = CommandType.Text;
        cmd.CommandTimeout = 300;

        if (parameters != null)
        {
            foreach (var p in parameters)
                cmd.Parameters.Add(p);
        }

        return await cmd.ExecuteNonQueryAsync();
    }
}
