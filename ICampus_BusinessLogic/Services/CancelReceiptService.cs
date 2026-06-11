using ICampus_BusinessLogic.Interfaces;
using ICampus_DataAccessLayer.Helpers;
using ICampus_DataAccessLayer.Interfaces;
using ICampus_Models.Requests;
using Microsoft.Data.SqlClient;
using System.Linq;

public class CancelReceiptService : ICancelReceiptService
{
    private readonly IGenericRepository<object> _repo;
    public CancelReceiptService(IGenericRepository<object> repo) => _repo = repo;

    // 1) Student details -> SPM_STUDENT_DETAILS
    public async Task<object> GetStudentDetailsAsync(string regno)
    {
        var p = new SqlParameter[] {
            new SqlParameter("@REGNO", System.Data.SqlDbType.VarChar) { Value = regno ?? string.Empty }
        };

        var sql = StoredProcSql.Exec(StoredProcedures.SPM_STUDENT_DETAILS, "@REGNO");
        var raw = await _repo.QueryFromStoredProcAsync(sql, p);
        return raw?.FirstOrDefault();
    }

    // 2) Receipt subjects -> PROC_FEERECEIPT_SUBJECTS
    public async Task<IEnumerable<object>> LoadReceiptSubjectsAsync(string regno, string examMy)
    {
        var p = new SqlParameter[] {
            new SqlParameter("@REGNO", System.Data.SqlDbType.VarChar) { Value = regno ?? string.Empty },
            new SqlParameter("@EXAMMY", System.Data.SqlDbType.VarChar) { Value = examMy ?? string.Empty }
        };

        var sql = StoredProcSql.Exec(StoredProcedures.PROC_FEERECEIPT_SUBJECTS, "@REGNO", "@EXAMMY");
        var raw = await _repo.QueryFromStoredProcAsync(sql, p);
        return raw ?? Enumerable.Empty<object>();
    }

    // 3) Cancel receipt -> SPM_RECEIPT_CANCELED
    // Note: original WebForms DAL calls "[SPM_RECEIPT_CANCELED] " + ER.ReceiptNO + ", '"+ER.Regno+"'";
    public async Task<int> CancelReceiptAsync(CancelReceiptRequest req)
    {
        // SP signature: SPM_RECEIPT_CANCELED @RECEIPTNO, @USERID
        var ps = new SqlParameter[] {
            new SqlParameter("@RECEIPTNO", System.Data.SqlDbType.VarChar) { Value = req.ReceiptNo ?? string.Empty },
            new SqlParameter("@USERID",   System.Data.SqlDbType.VarChar) { Value = req.UserId ?? string.Empty } // pass user initiating cancel
        };

        var sql = StoredProcSql.Exec(StoredProcedures.SPM_RECEIPT_CANCELED, "@RECEIPTNO", "@USERID");
        return await _repo.ExecuteStoredProcAsync(sql, ps);
    }
}
