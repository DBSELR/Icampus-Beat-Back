// RevaluationService.cs
using ICampus_DataAccessLayer.Helpers;
using ICampus_DataAccessLayer.Interfaces;
using ICampus_Models.Requests; // place request dto namespace appropriately
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

public class RevaluationService : IRevaluationService
{
    private readonly IGenericRepository<object> _repo;
    public RevaluationService(IGenericRepository<object> repo) => _repo = repo;

    public async Task<IEnumerable<object>> LoadSemsAsync(string course, string examMy, string regulation)
    {
        var p1 = new SqlParameter("@EXAMMY", SqlDbType.VarChar) { Value = examMy ?? string.Empty };
        var p2 = new SqlParameter("@COURSE", SqlDbType.VarChar) { Value = course ?? string.Empty };
        var p3 = new SqlParameter("@REGULATION", SqlDbType.VarChar) { Value = regulation ?? string.Empty };

        var sql = StoredProcSql.Exec(StoredProcedures.SPM_LOAD_SEMS_FOR_RV, "@EXAMMY", "@COURSE", "@REGULATION");
        var raw = await _repo.QueryFromStoredProcAsync(sql, p1, p2, p3);
        return raw ?? Enumerable.Empty<object>();
    }

    public async Task<IEnumerable<object>> GetPapersForRvAsync(string examMy, string regno, string sem)
    {
        var p1 = new SqlParameter("@EXAMMY", SqlDbType.VarChar) { Value = examMy ?? string.Empty };
        var p2 = new SqlParameter("@REGNO", SqlDbType.VarChar) { Value = regno ?? string.Empty };
        var p3 = new SqlParameter("@SEM", SqlDbType.VarChar) { Value = sem ?? string.Empty };

        var sql = StoredProcSql.Exec(StoredProcedures.SPM_GETPAPERS_FOR_RV, "@EXAMMY", "@REGNO", "@SEM");
        var raw = await _repo.QueryFromStoredProcAsync(sql, p1, p2, p3);
        return raw ?? Enumerable.Empty<object>();
    }

    public async Task<IEnumerable<object>> GetOptedPapersAsync(string examMy, string regno, string sem)
    {
        var p1 = new SqlParameter("@EXAMMY", SqlDbType.VarChar) { Value = examMy ?? string.Empty };
        var p2 = new SqlParameter("@REGNO", SqlDbType.VarChar) { Value = regno ?? string.Empty };
        var p3 = new SqlParameter("@SEM", SqlDbType.VarChar) { Value = sem ?? string.Empty };

        var sql = StoredProcSql.Exec(StoredProcedures.SPM_GET_OPT_RV_PAPERS, "@EXAMMY", "@REGNO", "@SEM");
        var raw = await _repo.QueryFromStoredProcAsync(sql, p1, p2, p3);
        return raw ?? Enumerable.Empty<object>();
    }

    public async Task<IEnumerable<object>> CheckRvCloseDateAsync(string regulation, string course, string examMy, string sem, string regno)
    {
        var p1 = new SqlParameter("@REGULATION", SqlDbType.VarChar) { Value = regulation ?? string.Empty };
        var p2 = new SqlParameter("@COURSE", SqlDbType.VarChar) { Value = course ?? string.Empty };
        var p3 = new SqlParameter("@EXAMMY", SqlDbType.VarChar) { Value = examMy ?? string.Empty };
        var p4 = new SqlParameter("@SEM", SqlDbType.VarChar) { Value = sem ?? string.Empty };
        var p5 = new SqlParameter("@REGNO", SqlDbType.VarChar) { Value = regno ?? string.Empty };

        // stored proc used in DAL: SP_RV_CLOSINGDATES (per uploaded DAL)
        var sql = StoredProcSql.Exec(StoredProcedures.SP_RV_CLOSINGDATES, "@REGULATION", "@COURSE", "@EXAMMY", "@SEM", "@REGNO");
        var raw = await _repo.QueryFromStoredProcAsync(sql, p1, p2, p3, p4, p5);
        return raw ?? Enumerable.Empty<object>();
    }

    public async Task<IEnumerable<object>> GetRvFeeAsync(string regulation, string course, string examMy, string sem, string rvType)
    {
        var p1 = new SqlParameter("@Regulation", SqlDbType.VarChar) { Value = regulation ?? string.Empty };
        var p2 = new SqlParameter("@Course", SqlDbType.VarChar) { Value = course ?? string.Empty };
        var p3 = new SqlParameter("@ExamMY", SqlDbType.VarChar) { Value = examMy ?? string.Empty };
        var p4 = new SqlParameter("@Sem", SqlDbType.VarChar) { Value = sem ?? string.Empty };
        var p5 = new SqlParameter("@RvType", SqlDbType.VarChar) { Value = rvType ?? string.Empty };

        var sql = StoredProcSql.Exec(StoredProcedures.SP_RV_FEE_DATA, "@Regulation", "@Course", "@ExamMY", "@Sem", "@RvType");
        var raw = await _repo.QueryFromStoredProcAsync(sql, p1, p2, p3, p4, p5);
        return raw ?? Enumerable.Empty<object>();
    }

    public async Task<int> RegisterRvPaperAsync(RegisterRvPaperRequest request)
    {
        // This mirrors DAL update query: UPDATE TBL_SH SET RV='Y', RV_TYPE = @RegistrationType WHERE REGNO=@Regno AND EXAMMY=@ExamMY AND SEM=@Sem AND PCODE=@PCode
        var sql = $"UPDATE TBL_SH SET RV = 'Y', RV_TYPE = @RegistrationType WHERE REGNO = @Regno AND EXAMMY = @ExamMY AND SEM = @Sem AND PCODE = @PCode";

        var p1 = new SqlParameter("@RegistrationType", SqlDbType.VarChar) { Value = request.RegistrationType ?? string.Empty };
        var p2 = new SqlParameter("@Regno", SqlDbType.VarChar) { Value = request.Regno ?? string.Empty };
        var p3 = new SqlParameter("@ExamMY", SqlDbType.VarChar) { Value = request.ExamMy ?? string.Empty };
        var p4 = new SqlParameter("@Sem", SqlDbType.VarChar) { Value = request.Sem ?? string.Empty };
        var p5 = new SqlParameter("@PCode", SqlDbType.VarChar) { Value = request.PCode ?? string.Empty };

        return await _repo.ExecuteStoredProcAsync(sql, p1, p2, p3, p4, p5);
    }

    public async Task<int> ResetRvPapersAsync(ResetRvPaperRequest request)
    {
        var sql = $"UPDATE TBL_SH SET RV = NULL WHERE REGNO = @Regno AND EXAMMY = @ExamMY AND SEM = @Sem";
        var p1 = new SqlParameter("@Regno", SqlDbType.VarChar) { Value = request.Regno ?? string.Empty };
        var p2 = new SqlParameter("@ExamMY", SqlDbType.VarChar) { Value = request.ExamMy ?? string.Empty };
        var p3 = new SqlParameter("@Sem", SqlDbType.VarChar) { Value = request.Sem ?? string.Empty };

        return await _repo.ExecuteStoredProcAsync(sql, p1, p2, p3);
    }

    public async Task<IEnumerable<object>> RvExamFeePayAsync(RvFeePayRequest request)
    {
        var p1 = new SqlParameter("@Q", SqlDbType.NVarChar) { Value = request.ExFeePay ?? string.Empty };
        var p2 = new SqlParameter("@APPFEE", SqlDbType.Decimal) { Precision = 7, Scale = 2, Value = request.AppFee ?? 0m };
        var p3 = new SqlParameter("@CONCESSION", SqlDbType.Decimal) { Precision = 7, Scale = 2, Value = request.Concession ?? 0m };

        var sql = StoredProcSql.Exec(StoredProcedures.SPM_EXAMFEE_PAY, "@Q", "@APPFEE", "@CONCESSION");
        var raw = await _repo.QueryFromStoredProcAsync(sql, p1, p2, p3);
        return raw ?? Enumerable.Empty<object>();
    }

    public async Task<IEnumerable<object>> GetRvBundleScriptsAsync(string regulation, string examMy, string course, string userId)
    {
        var p1 = new SqlParameter("@REGULATION", SqlDbType.VarChar) { Value = regulation ?? string.Empty };
        var p2 = new SqlParameter("@EXAMMY", SqlDbType.VarChar) { Value = examMy ?? string.Empty };
        var p3 = new SqlParameter("@COURSE", SqlDbType.VarChar) { Value = course ?? string.Empty };
        var p4 = new SqlParameter("@USERID", SqlDbType.VarChar) { Value = userId ?? string.Empty };

        var sql = StoredProcSql.Exec(StoredProcedures.SPM_GET_RV_BUNDLE_SCRIPTS, "@REGULATION", "@EXAMMY", "@COURSE", "@USERID");
        var raw = await _repo.QueryFromStoredProcAsync(sql, p1, p2, p3, p4);
        return raw ?? Enumerable.Empty<object>();
    }

    public async Task<IEnumerable<object>> GetReceiptDataAsync(string receiptNo)
    {
        // DAL: SELECT REGNO, SEM, PCOUNT, PAPERS, USERID, PDATE FROM TBL_EXAMFEE WHERE RECEIPTNO = {receiptNo}
        // Use parameterized query
        var sql = "SELECT REGNO, SEM, PCOUNT, PAPERS, USERID, PDATE FROM TBL_EXAMFEE WHERE RECEIPTNO = @ReceiptNo";
        var p1 = new SqlParameter("@ReceiptNo", SqlDbType.VarChar) { Value = receiptNo ?? string.Empty };
        var raw = await _repo.QueryFromStoredProcAsync(sql, p1);
        return raw ?? Enumerable.Empty<object>();
    }
}
