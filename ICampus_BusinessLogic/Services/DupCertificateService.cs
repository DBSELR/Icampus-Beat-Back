using ICampus_BusinessLogic.Interfaces;
using ICampus_DataAccessLayer.Helpers;
using ICampus_DataAccessLayer.Interfaces;
using ICampus_Models.Requests;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

public class DupCertificateService : IDupCertificateService
{
    private readonly IGenericRepository<object> _repo;

    public DupCertificateService(IGenericRepository<object> repo)
    {
        _repo = repo;
    }

    // Load student/receipt data (was LoadStddata_REceiptWise in old DAL)
    public async Task<IEnumerable<object>> LoadReceiptStudentDataAsync(string receiptNo)
    {
        var p = new SqlParameter("@RECPTNO", SqlDbType.VarChar) { Value = receiptNo ?? string.Empty };
        var sql = $"SELECT M.*,S.SName,S.Course,S.GRP FROM TBL_MISCFEE_DETAILS M INNER JOIN TBL_STDDATA S ON M.REGNO = S.REGNO WHERE M.RECPTNO = @RECPTNO AND CAST([DATE] AS DATE) >= CAST(GETDATE() AS DATE)";
        // old DAL used inline SQL — keep it as inline query for parity
        var raw = await _repo.QueryFromStoredProcAsync(sql, p);
        return raw ?? Enumerable.Empty<object>();
    }

    // HallTicket DataLoad -> calls SPM_HT_LBRCE (original DAL string call). Keep as stored proc exec.
    public async Task<IEnumerable<object>> LoadHallTicketAsync(string examMy, string course, string regno, string regulation)
    {
        var p1 = new SqlParameter("@ExamMY", SqlDbType.VarChar) { Value = examMy ?? string.Empty };
        var p2 = new SqlParameter("@Course", SqlDbType.VarChar) { Value = course ?? string.Empty };
        var p3 = new SqlParameter("@REGNO", SqlDbType.VarChar) { Value = regno ?? (object)DBNull.Value };
        var p4 = new SqlParameter("@regulation", SqlDbType.VarChar) { Value = regulation ?? string.Empty };
        var sql = StoredProcSql.Exec(StoredProcedures.SPM_HT_LBRCE, "@ExamMY", "@Course", "@REGNO", "@regulation");
        var raw = await _repo.QueryFromStoredProcAsync(sql, p1, p2, p3, p4);
        return raw ?? Enumerable.Empty<object>();
    }

    // MarksMemo DataLoad -> SP_MRK_MEMO (old DAL used SP_MRK_MEMO)
    public async Task<IEnumerable<object>> LoadMarksMemoAsync(MarksMemoRequest req)
    {
        // Convert empty strings -> DBNull.Value where SP treats empty as NULL (DAL did similar)
        object reguVal = string.IsNullOrWhiteSpace(req.Regulation) ? (object)DBNull.Value : req.Regulation;
        object regnoVal = string.IsNullOrWhiteSpace(req.RegNo) ? (object)DBNull.Value : req.RegNo;
        object branchVal = string.IsNullOrWhiteSpace(req.Branch) ? (object)DBNull.Value : req.Branch;
        object dateVal = string.IsNullOrWhiteSpace(req.Date) ? (object)DBNull.Value : req.Date;

        var ps = new[]
        {
        new SqlParameter("@REGULATION", SqlDbType.VarChar, 10) { Value = reguVal },
        new SqlParameter("@EXAMMY", SqlDbType.VarChar, 15)     { Value = req.ExamMy ?? string.Empty },
        new SqlParameter("@Course", SqlDbType.VarChar, 20)     { Value = req.Course ?? string.Empty },
        new SqlParameter("@SEMESTER", SqlDbType.VarChar, 2)    { Value = req.Semester ?? string.Empty },
        new SqlParameter("@RV", SqlDbType.VarChar, 2)          { Value = string.IsNullOrWhiteSpace(req.RV) ? "N" : req.RV },
        new SqlParameter("@BRANCH", SqlDbType.VarChar, 20)     { Value = branchVal },
        new SqlParameter("@REGNO", SqlDbType.VarChar, 20)      { Value = regnoVal },
        new SqlParameter("@Date", SqlDbType.VarChar, 50)       { Value = dateVal }
    };

        var sql = StoredProcSql.Exec(StoredProcedures.SP_MRK_MEMO,
            "@REGULATION", "@EXAMMY", "@Course", "@SEMESTER", "@RV", "@BRANCH", "@REGNO", "@Date");

        var raw = await _repo.QueryFromStoredProcAsync(sql, ps);
        return raw ?? Enumerable.Empty<object>();
    }


    // Save dup certificate -> PROC_DUP_CERTIFICATE_DATA
    public async Task<int> SaveDupCertificateAsync(DupCertificateSaveRequest req)
    {
        var ps = new[]
        {
            new SqlParameter("@RECEIPTNO", SqlDbType.VarChar) { Value = req.ReceiptNo ?? string.Empty },
            new SqlParameter("@REGNO", SqlDbType.VarChar) { Value = req.RegNo ?? string.Empty },
            new SqlParameter("@SEM", SqlDbType.Int) { Value = req.Sem },
            new SqlParameter("@EXAMMY", SqlDbType.VarChar) { Value = req.ExamMy ?? string.Empty },
            new SqlParameter("@CERTIFICATE_NAME", SqlDbType.VarChar) { Value = req.CertificateName ?? string.Empty },
            new SqlParameter("@REMARKS", SqlDbType.VarChar) { Value = req.Remarks ?? string.Empty },
            new SqlParameter("@CR_ID", SqlDbType.VarChar) { Value = req.CrId ?? string.Empty }
        };

        var sql = StoredProcSql.Exec(StoredProcedures.PROC_DUP_CERTIFICATE_DATA,
            "@RECEIPTNO", "@REGNO", "@SEM", "@EXAMMY", "@CERTIFICATE_NAME", "@REMARKS", "@CR_ID");

        return await _repo.ExecuteStoredProcAsync(sql, ps);
    }

    // Checks (the DAL returned DataTable COUNT(*) for both)
    public async Task<int> CheckRegWiseDupCountAsync(string regNo, int sem, string examMy, string certificateName)
    {
        // Recreate the count query used in the DAL
        var sql = "SELECT COUNT(*) Cnt FROM TBL_DUP_CERTIFICATE_DATA WHERE EXAMMY = @EXAMMY AND REGNO = @REGNO AND CERTIFICATE_NAME = @CERTIFICATE_NAME AND SEM = @SEM";
        var p1 = new SqlParameter("@EXAMMY", SqlDbType.VarChar) { Value = examMy ?? string.Empty };
        var p2 = new SqlParameter("@REGNO", SqlDbType.VarChar) { Value = regNo ?? string.Empty };
        var p3 = new SqlParameter("@CERTIFICATE_NAME", SqlDbType.VarChar) { Value = certificateName ?? string.Empty };
        var p4 = new SqlParameter("@SEM", SqlDbType.Int) { Value = sem };
        var raw = await _repo.QueryFromStoredProcAsync(sql, p1, p2, p3, p4);
        // parse scalar from returned rows
        try
        {
            var json = JsonConvert.SerializeObject(raw);
            var list = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(json);
            if (list?.Count > 0) { var v = list[0].Values.FirstOrDefault(); if (int.TryParse(v?.ToString(), out int val)) return val; }
        }
        catch { }
        return 0;
    }

    public async Task<int> CheckReceiptWiseDupCountAsync(string receiptNo, string regNo, int sem, string examMy, string certificateName)
    {
        var sql = "SELECT COUNT(*) Cnt FROM TBL_DUP_CERTIFICATE_DATA WHERE EXAMMY = @EXAMMY AND REGNO = @REGNO AND CERTIFICATE_NAME = @CERTIFICATE_NAME AND SEM = @SEM AND RECEIPTNO = @RECEIPTNO";
        var p0 = new SqlParameter("@RECEIPTNO", SqlDbType.VarChar) { Value = receiptNo ?? string.Empty };
        var p1 = new SqlParameter("@EXAMMY", SqlDbType.VarChar) { Value = examMy ?? string.Empty };
        var p2 = new SqlParameter("@REGNO", SqlDbType.VarChar) { Value = regNo ?? string.Empty };
        var p3 = new SqlParameter("@CERTIFICATE_NAME", SqlDbType.VarChar) { Value = certificateName ?? string.Empty };
        var p4 = new SqlParameter("@SEM", SqlDbType.Int) { Value = sem };
        var raw = await _repo.QueryFromStoredProcAsync(sql, p0, p1, p2, p3, p4);
        try
        {
            var json = JsonConvert.SerializeObject(raw);
            var list = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(json);
            if (list?.Count > 0) { var v = list[0].Values.FirstOrDefault(); if (int.TryParse(v?.ToString(), out int val)) return val; }
        }
        catch { }
        return 0;
    }
}
