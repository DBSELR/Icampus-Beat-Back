using ICampus_DataAccessLayer.Helpers;
using ICampus_DataAccessLayer.Interfaces;
using ICampus_Models.Requests;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

public class MasterService : IMasterService
{
    private readonly IGenericRepository<object> _repo;

    public MasterService(IGenericRepository<object> repo)
    {
        _repo = repo;
    }

    // 1) PAP_CHECK_MASTERCREATION -> SPM_PAP_CHECK_MASTERCREATION
    public async Task<IEnumerable<object>> GetRegularDataAsync(string course, string examMy, string regulation)
    {
        var p1 = new SqlParameter("@COURSE", SqlDbType.VarChar, 30) { Value = course ?? string.Empty };
        var p2 = new SqlParameter("@EXAMMY", SqlDbType.VarChar, 20) { Value = examMy ?? string.Empty };
        var p3 = new SqlParameter("@REGULATION", SqlDbType.VarChar, 10) { Value = regulation ?? string.Empty };

        var sql = StoredProcSql.Exec(StoredProcedures.SPM_PAP_CHECK_MASTERCREATION, "@COURSE", "@EXAMMY", "@REGULATION");
        var raw = await _repo.QueryFromStoredProcAsync(sql, p1, p2, p3);
        return raw ?? Enumerable.Empty<object>();
    }

    // 2) Update paper data -> PROC_UPDATE_PAP
    public async Task<int> UpdatePapDataAsync(UpdatePapRequest req)
    {
        var ps = new[]
        {
            new SqlParameter("@PNAME", SqlDbType.VarChar, 150) { Value = req.PName ?? string.Empty },
            new SqlParameter("@MAXMRK", SqlDbType.VarChar, 10)  { Value = req.MaxMrk ?? string.Empty },
            new SqlParameter("@SMAX", SqlDbType.VarChar, 150)   { Value = req.SMax ?? string.Empty },
            new SqlParameter("@TMAX", SqlDbType.VarChar, 10)    { Value = req.TMax ?? string.Empty },
            new SqlParameter("@PMAX", SqlDbType.VarChar, 10)    { Value = req.PMax ?? string.Empty },
            new SqlParameter("@TPASS", SqlDbType.VarChar, 10)   { Value = req.TPass ?? string.Empty },
            new SqlParameter("@PASS", SqlDbType.VarChar, 150)   { Value = req.Pass ?? string.Empty },
            new SqlParameter("@Credits", SqlDbType.VarChar, 10) { Value = req.Credits ?? string.Empty },
            new SqlParameter("@SPass", SqlDbType.VarChar, 150)  { Value = req.SPass ?? string.Empty },
            new SqlParameter("@PPass", SqlDbType.VarChar, 10)   { Value = req.PPass ?? string.Empty },
            new SqlParameter("@PID", SqlDbType.VarChar, 10)     { Value = req.PID ?? string.Empty }
        };

        var sql = StoredProcSql.Exec(StoredProcedures.PROC_UPDATE_PAP,
            "@PNAME", "@MAXMRK", "@SMAX", "@TMAX", "@PMAX", "@TPASS", "@PASS", "@Credits", "@SPass", "@PPass", "@PID");

        return await _repo.ExecuteStoredProcAsync(sql, ps);
    }

    // 3) RegularMasterData -> SPM_RegularMasterData_LOAD
    public async Task<IEnumerable<object>> LoadMasterDataAsync(string course, string examMy, string regulation)
    {
        var p1 = new SqlParameter("@COURSE", SqlDbType.VarChar, 30) { Value = course ?? string.Empty };
        var p2 = new SqlParameter("@EXAMMY", SqlDbType.VarChar, 15) { Value = examMy ?? string.Empty };
        var p3 = new SqlParameter("@REGULATION", SqlDbType.VarChar, 10) { Value = regulation ?? string.Empty };

        var sql = StoredProcSql.Exec(StoredProcedures.SPM_RegularMasterData_LOAD, "@COURSE", "@EXAMMY", "@REGULATION");
        var raw = await _repo.QueryFromStoredProcAsync(sql, p1, p2, p3);
        return raw ?? Enumerable.Empty<object>();
    }

    // 4) MASTER_EXISTS_CHECK -> SPM_MASTER_EXISTS_CHECK (returns single-row with [EXISTS])
    public async Task<int> MasterExistsAsync(string course, string examMy, string batch, string sem)
    {
        var p1 = new SqlParameter("@COURSE", SqlDbType.VarChar, 30) { Value = course ?? string.Empty };
        var p2 = new SqlParameter("@EXAMMY", SqlDbType.VarChar, 15) { Value = examMy ?? string.Empty };
        var p3 = new SqlParameter("@REGU", SqlDbType.VarChar, 2) { Value = batch ?? string.Empty };
        var p4 = new SqlParameter("@SEM", SqlDbType.VarChar, 2) { Value = sem ?? string.Empty };

        var sql = StoredProcSql.Exec(StoredProcedures.SPM_MASTER_EXISTS_CHECK, "@COURSE", "@EXAMMY", "@REGU", "@SEM");
        var raw = await _repo.QueryFromStoredProcAsync(sql, p1, p2, p3, p4);

        // convert to int: raw is IEnumerable<object> -> dictionary -> first value
        try
        {
            var first = raw?.FirstOrDefault();
            if (first != null)
            {
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(first);
                var dict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                if (dict != null && dict.Values.Any() && int.TryParse(dict.Values.First().ToString(), out var v)) return v;
            }
        }
        catch { /* ignore and fallback */ }

        return 0;
    }

    // 5) Create master -> SP_MASTER_CREATE
    public async Task<int> CreateMasterAsync(CreateMasterRequest req)
    {
        var ps = new[]
        {
            new SqlParameter("@COURSE", SqlDbType.VarChar, 30) { Value = req.Course ?? string.Empty },
            new SqlParameter("@EXAMMY", SqlDbType.VarChar, 15) { Value = req.ExamMy ?? string.Empty },
            new SqlParameter("@REGU", SqlDbType.VarChar, 2) { Value = req.Regu ?? string.Empty },
            new SqlParameter("@SEM", SqlDbType.VarChar, 2) { Value = req.Sem ?? string.Empty }
        };

        var sql = StoredProcSql.Exec(StoredProcedures.SP_MASTER_CREATE, "@COURSE", "@EXAMMY", "@REGU", "@SEM");
        return await _repo.ExecuteStoredProcAsync(sql, ps);
    }

    // 6) Export to Excel - replicate original export behaviour: call PAP_CHECK_MASTERCREATION then build Excel bytes
    public async Task<byte[]> ExportPapDataExcelAsync(string course, string examMy, string regulation)
    {
        // call SPM_PAP_CHECK_MASTERCREATION & get DataTable-like object (repo returns objects)
        var data = await GetRegularDataAsync(course, examMy, regulation);
        var list = data?.ToList() ?? new List<object>();

        if (!list.Any()) return Array.Empty<byte[]>().FirstOrDefault(); // empty

        // Build a small DataTable-like Excel file in-memory (ClosedXML or EPPlus)
        // Here we produce an XLSX using ClosedXML if available.
        using (ClosedXML.Excel.XLWorkbook wb = new ClosedXML.Excel.XLWorkbook())
        {
            // Convert list of objects / dictionaries to a DataTable-like structure
            var firstObj = list.First();
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(list);
            var rows = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(json);

            var dt = new System.Data.DataTable();
            foreach (var col in rows.First().Keys) dt.Columns.Add(col);

            foreach (var r in rows)
            {
                var row = dt.NewRow();
                foreach (var kv in r) row[kv.Key] = kv.Value ?? DBNull.Value;
                dt.Rows.Add(row);
            }

            wb.Worksheets.Add(dt, "SubjectsData");
            using (var ms = new System.IO.MemoryStream())
            {
                wb.SaveAs(ms);
                return ms.ToArray();
            }
        }
    }
}
