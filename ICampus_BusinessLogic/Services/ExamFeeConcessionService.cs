using ICampus_DataAccessLayer.Helpers;
using ICampus_DataAccessLayer.Interfaces;
using ICampus_Models.Requests;
using Microsoft.Data.SqlClient;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

public class ExamFeeConcessionService : IExamFeeConcessionService
{
    private readonly IGenericRepository<object> _repo;

    public ExamFeeConcessionService(IGenericRepository<object> repo)
    {
        _repo = repo;
    }

    // 1. Load sems - inline SQL from DAL_Load_Sems in your file
    public async Task<IEnumerable<object>> LoadSemsAsync(string course, string regulation)
    {
        // The old DAL used SELECT DISTINCT SH.SEM FROM TBL_SH SH WHERE SH.course = '{ME.Course}' AND SH.Regulation = '{ME.Regulation}' ORDER BY SEM
        var sql = @"SELECT DISTINCT SH.SEM FROM TBL_SH SH 
                    WHERE SH.course = @COURSE AND SH.Regulation = @REGULATION
                    ORDER BY SEM";
        var p1 = new SqlParameter("@COURSE", SqlDbType.VarChar) { Value = course ?? string.Empty };
        var p2 = new SqlParameter("@REGULATION", SqlDbType.VarChar) { Value = regulation ?? string.Empty };
        var raw = await _repo.QueryFromStoredProcAsync(sql, p1, p2);
        return raw ?? new List<object>();
    }

    // 2. Get details by Regno -> calls SP_GETDETAILS @REGNO
    public async Task<IEnumerable<object>> GetDetailsByRegnoAsync(string regno)
    {
        var p = new SqlParameter("@REGNO", SqlDbType.VarChar) { Value = regno ?? string.Empty };
        var sql = StoredProcSql.Exec(StoredProcedures.SP_GETDETAILS, "@REGNO");
        var raw = await _repo.QueryFromStoredProcAsync(sql, p);
        return raw ?? new List<object>();
    }

    // 3. Grid -> SP_FeeConcession_Grid @Course, @Exammy, @Regno, @Sem
    public async Task<IEnumerable<object>> LoadFeeConcessionGridAsync(string course, string examMy, string regno, int sem)
    {
        var p1 = new SqlParameter("@Course", SqlDbType.VarChar) { Value = course ?? string.Empty };
        var p2 = new SqlParameter("@Exammy", SqlDbType.VarChar) { Value = examMy ?? string.Empty };
        var p3 = new SqlParameter("@Regno", SqlDbType.VarChar) { Value = regno ?? string.Empty };
        var p4 = new SqlParameter("@Sem", SqlDbType.Int) { Value = sem };
        var sql = StoredProcSql.Exec(StoredProcedures.SP_FeeConcession_Grid, "@Course", "@Exammy", "@Regno", "@Sem");
        var raw = await _repo.QueryFromStoredProcAsync(sql, p1, p2, p3, p4);
        return raw ?? new List<object>();
    }

    // Check if semester is Regular or Supply using sp_RegsUp_Check_ExammY
    // Note: User mentioned SP_REGSUP_CHECK_EXAMMY, but using existing sp_RegsUp_Check_ExammY from enum
    public async Task<string> CheckRegSupAsync(string regu, int sem, string course, string examMy)
    {
        // sp_RegsUp_Check_ExammY checks if semester is Regular or Supply
        // Parameters: @Regu (INT), @Sem (INT), @Edate (VarChar), @Course (VarChar)
        // Returns REGSUP field with 'REG' or 'SUP'
        int reguInt = 0;
        if (!string.IsNullOrWhiteSpace(regu) && int.TryParse(regu, out var parsedRegu))
        {
            reguInt = parsedRegu;
        }
        
        var p1 = new SqlParameter("@Regu", SqlDbType.Int) { Value = reguInt };
        var p2 = new SqlParameter("@Sem", SqlDbType.Int) { Value = sem };
        var p3 = new SqlParameter("@Edate", SqlDbType.VarChar, 50) { Value = examMy ?? string.Empty };
        var p4 = new SqlParameter("@Course", SqlDbType.VarChar, 20) { Value = course ?? string.Empty };
        
        var sql = StoredProcSql.Exec(StoredProcedures.sp_RegsUp_Check_ExammY, "@Regu", "@Sem", "@Edate", "@Course");
        var raw = await _repo.QueryFromStoredProcAsync(sql, p1, p2, p3, p4);
        
        if (raw != null && raw.Any())
        {
            // Get the first result and check for REGSUP field
            var first = raw.First();
            if (first is System.Dynamic.ExpandoObject expando)
            {
                var dict = (IDictionary<string, object>)expando;
                if (dict.ContainsKey("REGSUP"))
                {
                    return dict["REGSUP"]?.ToString()?.ToUpper() ?? "REG";
                }
            }
        }
        
        return "REG"; // Default to Regular if not found
    }

    // Get fee for Regular semester using SPM_GET_FEEConcession_REG
    // Parameters: @REGU VARCHAR(2), @SEM VARCHAR(2), @COURSE VARCHAR(20), @GRP VARCHAR(15), @EXAMMY VARCHAR(12), @REGULATION VARCHAR(10), @REGNO varchar(10)
    public async Task<IEnumerable<object>> GetFeeConcessionRegAsync(string regu, string sem, string course, string grp, string examMy, string regulation, string regno)
    {
        var p1 = new SqlParameter("@REGU", SqlDbType.VarChar, 2) { Value = regu ?? string.Empty };
        var p2 = new SqlParameter("@SEM", SqlDbType.VarChar, 2) { Value = sem ?? string.Empty };
        var p3 = new SqlParameter("@COURSE", SqlDbType.VarChar, 20) { Value = course ?? string.Empty };
        var p4 = new SqlParameter("@GRP", SqlDbType.VarChar, 15) { Value = grp ?? string.Empty };
        var p5 = new SqlParameter("@EXAMMY", SqlDbType.VarChar, 12) { Value = examMy ?? string.Empty };
        var p6 = new SqlParameter("@REGULATION", SqlDbType.VarChar, 10) { Value = regulation ?? string.Empty };
        var p7 = new SqlParameter("@REGNO", SqlDbType.VarChar, 10) { Value = regno ?? string.Empty };
        
        var sql = StoredProcSql.Exec(StoredProcedures.SPM_GET_FEEConcession_REG, "@REGU", "@SEM", "@COURSE", "@GRP", "@EXAMMY", "@REGULATION", "@REGNO");
        var raw = await _repo.QueryFromStoredProcAsync(sql, p1, p2, p3, p4, p5, p6, p7);
        return raw ?? new List<object>();
    }

    // Get fee for Supply semester using SPM_GET_FEEConcession_SUP
    // Parameters: @COURSE VARCHAR(20), @EXAMMY VARCHAR(12), @REGULATION VARCHAR(10), @Regno varchar(20), @sem int
    public async Task<IEnumerable<object>> GetFeeConcessionSupAsync(string course, string examMy, string regulation, string regno, int sem)
    {
        var p1 = new SqlParameter("@COURSE", SqlDbType.VarChar, 20) { Value = course ?? string.Empty };
        var p2 = new SqlParameter("@EXAMMY", SqlDbType.VarChar, 12) { Value = examMy ?? string.Empty };
        var p3 = new SqlParameter("@REGULATION", SqlDbType.VarChar, 10) { Value = regulation ?? string.Empty };
        var p4 = new SqlParameter("@Regno", SqlDbType.VarChar, 20) { Value = regno ?? string.Empty };
        var p5 = new SqlParameter("@sem", SqlDbType.Int) { Value = sem };
        
        var sql = StoredProcSql.Exec(StoredProcedures.SPM_GET_FEEConcession_SUP, "@COURSE", "@EXAMMY", "@REGULATION", "@Regno", "@sem");
        var raw = await _repo.QueryFromStoredProcAsync(sql, p1, p2, p3, p4, p5);
        return raw ?? new List<object>();
    }

    // 4. Save -> SP_EXAMFEECONCESSION_SAVE(@REGULATION,@REGNO,@SEM,@TOTALAMOUNT,@FINEAMOUNT,@CONCESSION,@TOBEPAID,@Regu,@Exammy)
    // FeeToBePaid = (Total Amount + Fine Amount) − Concession Amount
    public async Task<int> SaveFeeConcessionAsync(FeeConcessionSaveRequest req)
    {
        // Calculate FeeToBePaid: (Total Amount + Fine Amount) - Concession Amount
        decimal feeToBePaid = (req.TotalAmount + req.FineAmount) - req.Concession;
        
        var ps = new[]
        {
            new SqlParameter("@REGULATION", SqlDbType.VarChar) { Value = req.Regulation ?? string.Empty },
            new SqlParameter("@REGNO", SqlDbType.VarChar) { Value = req.Regno ?? string.Empty },
            new SqlParameter("@SEM", SqlDbType.Int) { Value = req.Sem },
            new SqlParameter("@TOTALAMOUNT", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = req.TotalAmount },
            new SqlParameter("@FINEAMOUNT", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = req.FineAmount },
            new SqlParameter("@CONCESSION", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = req.Concession },
            new SqlParameter("@TOBEPAID", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = feeToBePaid },
            new SqlParameter("@Regu", SqlDbType.VarChar) { Value = req.Regu ?? string.Empty },
            new SqlParameter("@Exammy", SqlDbType.VarChar) { Value = req.ExamMy ?? string.Empty }
        };

        var sql = StoredProcSql.Exec(StoredProcedures.SP_EXAMFEECONCESSION_SAVE,
            "@REGULATION", "@REGNO", "@SEM", "@TOTALAMOUNT", "@FINEAMOUNT", "@CONCESSION", "@TOBEPAID", "@Regu", "@Exammy");

        return await _repo.ExecuteStoredProcAsync(sql, ps);
    }

    // 5. Delete -> original code used inline DELETE FROM tbl_ExamFeeConcession WHERE ID = {id}
    public async Task<int> DeleteFeeConcessionAsync(int id)
    {
        // Use parameterized inline SQL to avoid injection
        var sql = "DELETE FROM tbl_ExamFeeConcession WHERE ID = @ID";
        var p = new SqlParameter("@ID", SqlDbType.Int) { Value = id };
        return await _repo.ExecuteStoredProcAsync(sql, p); // assume repo has ExecuteSqlAsync for inline SQL; if not, use ExecuteStoredProcAsync with dynamic SQL
    }
}
