// ICampus_BusinessLogic.Services/SubjectService.cs
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

public class CourseReportService : ICourseReportService
{
    private readonly IGenericRepository<object> _repo;
    public CourseReportService(IGenericRepository<object> repo) { _repo = repo; }

    public async Task<IEnumerable<object>> LoadSubjectListAsync()
    {
        // SPM_SUBJECTLIST stored proc
        var sql = StoredProcSql.Exec(StoredProcedures.SPM_SUBJECTLIST, null);
        var raw = await _repo.QueryFromStoredProcAsync(sql);
        return raw ?? Enumerable.Empty<object>();
    }

    public async Task<IEnumerable<object>> LoadBatchesAsync(string course, string regulation)
    {
        // inline SQL (same logic as Dal.SubjectList_Batch)
        var sql = @"
            SELECT DISTINCT REGU, '20' + REGU + '-20' + CAST(CAST(REGU AS INT) + (MAXSEM/2) AS VARCHAR) AS BATCH
            FROM TBL_COURSE
            WHERE COURSE = @COURSE AND Regulation = @REGULATION
        ";
        var p1 = new SqlParameter("@COURSE", SqlDbType.VarChar) { Value = course ?? string.Empty };
        var p2 = new SqlParameter("@REGULATION", SqlDbType.VarChar) { Value = regulation ?? string.Empty };
        var raw = await _repo.QueryFromStoredProcAsync(sql, p1, p2);
        return raw ?? Enumerable.Empty<object>();
    }

    public async Task<IEnumerable<object>> LoadBranchesAsync(string course, string regulation, string batch)
    {
        var sql = @"
            SELECT DISTINCT GRP
            FROM TBL_COURSE
            WHERE Regulation = @REGULATION AND COURSE = @COURSE AND REGU = @BATCH
        ";
        var p1 = new SqlParameter("@REGULATION", SqlDbType.VarChar) { Value = regulation ?? string.Empty };
        var p2 = new SqlParameter("@COURSE", SqlDbType.VarChar) { Value = course ?? string.Empty };
        var p3 = new SqlParameter("@BATCH", SqlDbType.VarChar) { Value = batch ?? string.Empty };
        var raw = await _repo.QueryFromStoredProcAsync(sql, p1, p2, p3);
        return raw ?? Enumerable.Empty<object>();
    }

    public async Task<IEnumerable<object>> LoadSemsAsync(string course, string regulation, string batch)
    {
        var sql = @"
            SELECT DISTINCT CAST(SEM AS VARCHAR(250)) SEM, SEM SEM1
            FROM TBL_GPAP
            WHERE COURSE = @COURSE AND REGU = @BATCH
        ";
        var p1 = new SqlParameter("@COURSE", SqlDbType.VarChar) { Value = course ?? string.Empty };
        var p2 = new SqlParameter("@BATCH", SqlDbType.VarChar) { Value = batch ?? string.Empty };
        var raw = await _repo.QueryFromStoredProcAsync(sql, p1, p2);
        return raw ?? Enumerable.Empty<object>();
    }

    public async Task<IEnumerable<object>> LoadPapersListAsync(PapersListRequest request)
    {
        var ps = new[]
        {
            new SqlParameter("@Course", SqlDbType.VarChar){ Value = request.Course ?? string.Empty },
            new SqlParameter("@Regulation", SqlDbType.VarChar){ Value = request.Regulation ?? string.Empty },
            new SqlParameter("@Batch", SqlDbType.VarChar){ Value = string.IsNullOrWhiteSpace(request.Batch) ? (object)DBNull.Value : request.Batch },
            new SqlParameter("@Branch", SqlDbType.VarChar){ Value = string.IsNullOrWhiteSpace(request.Branch) ? (object)DBNull.Value : request.Branch },
            new SqlParameter("@Sem", SqlDbType.VarChar){ Value = string.IsNullOrWhiteSpace(request.Sem) ? (object)DBNull.Value : request.Sem }
        };

        var sql = StoredProcSql.Exec(StoredProcedures.PROC_LOAD_PAPERSLIST, "@Course", "@Regulation", "@Batch", "@Branch", "@Sem");
        var raw = await _repo.QueryFromStoredProcAsync(sql, ps);
        return raw ?? Enumerable.Empty<object>();
    }

    public async Task<int> RunIasReportAsync()
    {
        // DAL called PROC_IS_IASUPDATE via IAS_Report earlier: using inline stored proc string
        var sql = StoredProcSql.Exec(StoredProcedures.SP_CONDINATION_DATES_LOAD); // placeholder, change to correct if you have a named proc
        // If your DAL used "PROC_IS_IASUPDATE" do add it to enum and call as below:
        // var p = ...; return await _repo.ExecuteStoredProcAsync(StoredProcSql.Exec(StoredProcedures.PROC_IS_IASUPDATE));
        // For now return 0 if not needed
        return 0;
    }

    public async Task<int> RunRegnoResultProcessAsync(SubjectProcessRequest request)
    {
        // Map to DB call in Dal.REGNORESULTPROCESS: expects Regno, ExamMy, PrevExamy, ResultType maybe
        // Because Dal used different proc signatures depending on DB, we'll call inline stored proc exec string:
        var q = $"[proc_resultprocess_alldata]'{request.Regno}','{request.ExamMy}','{request.PrevExamy}','{request.ResultType}'";
        // Use repository execute - repository expects a sql string and SqlParameters. If executor expects a full string:
        return await _repo.ExecuteStoredProcAsync(q);
    }
}
