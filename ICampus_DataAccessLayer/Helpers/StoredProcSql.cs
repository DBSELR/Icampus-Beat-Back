namespace ICampus_DataAccessLayer.Helpers
{
    public static class StoredProcSql
    {
        public static string Exec(StoredProcedures proc, params string[] parameters)
        {
            var paramList = parameters != null && parameters.Length > 0
                ? " " + string.Join(", ", parameters)
                : string.Empty;

            return $"EXEC dbo.{proc}{paramList}";
        }

        // Named-assignment form: EXEC dbo.SP @p1 = @p1, @p2 = @p2, ...
        // Use when SP parameter order is unknown or may differ from call order.
        public static string ExecNamed(StoredProcedures proc, params string[] parameters)
        {
            var paramList = parameters != null && parameters.Length > 0
                ? " " + string.Join(", ", System.Array.ConvertAll(parameters, p => $"{p} = {p}"))
                : string.Empty;

            return $"EXEC dbo.{proc}{paramList}";
        }
    }
}
