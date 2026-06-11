// ICampus_BusinessLogic.Services/RoomMasterReportService.cs
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

public class RoomMasterReportService : IRoomMasterReportService
{
    private readonly IGenericRepository<object> _repo;
    public RoomMasterReportService(IGenericRepository<object> repo) => _repo = repo;

    public async Task<IEnumerable<object>> LoadSubjectListAsync()
    {
        var sql = StoredProcSql.Exec(StoredProcedures.SPM_SUBJECTLIST);
        var raw = await _repo.QueryFromStoredProcAsync(sql);
        return raw ?? Enumerable.Empty<object>();
    }

    public async Task<IEnumerable<object>> LoadBatchesAsync(string course, string regulation)
    {
        var sql = @"SELECT DISTINCT REGU, '20' + REGU + '-20' + CAST(CAST(REGU AS INT) + (MAXSEM/2) AS VARCHAR) AS BATCH
                    FROM TBL_COURSE
                    WHERE COURSE = @COURSE AND Regulation = @REGULATION";
        var p1 = new SqlParameter("@COURSE", SqlDbType.VarChar) { Value = course ?? string.Empty };
        var p2 = new SqlParameter("@REGULATION", SqlDbType.VarChar) { Value = regulation ?? string.Empty };
        var raw = await _repo.QueryFromStoredProcAsync(sql, p1, p2);
        return raw ?? Enumerable.Empty<object>();
    }

    public async Task<IEnumerable<object>> LoadBranchesAsync(string course, string regulation, string batch)
    {
        var sql = @"SELECT DISTINCT GRP FROM TBL_COURSE WHERE Regulation = @REGULATION AND COURSE = @COURSE AND REGU = @BATCH";
        var p1 = new SqlParameter("@REGULATION", SqlDbType.VarChar) { Value = regulation ?? string.Empty };
        var p2 = new SqlParameter("@COURSE", SqlDbType.VarChar) { Value = course ?? string.Empty };
        var p3 = new SqlParameter("@BATCH", SqlDbType.VarChar) { Value = batch ?? string.Empty };
        var raw = await _repo.QueryFromStoredProcAsync(sql, p1, p2, p3);
        return raw ?? Enumerable.Empty<object>();
    }

    public async Task<IEnumerable<object>> LoadSemsAsync(string course, string regulation, string batch)
    {
        var sql = @"SELECT DISTINCT CAST(SEM AS VARCHAR(250)) SEM, SEM SEM1 FROM TBL_GPAP WHERE COURSE = @COURSE AND REGU = @BATCH";
        var p1 = new SqlParameter("@COURSE", SqlDbType.VarChar) { Value = course ?? string.Empty };
        var p2 = new SqlParameter("@BATCH", SqlDbType.VarChar) { Value = batch ?? string.Empty };
        var raw = await _repo.QueryFromStoredProcAsync(sql, p1, p2);
        return raw ?? Enumerable.Empty<object>();
    }

    public async Task<IEnumerable<object>> LoadExammyAsync(string course, string regulation)
    {
        var p1 = new SqlParameter("@Course", SqlDbType.VarChar) { Value = course ?? string.Empty };
        var p2 = new SqlParameter("@Regulation", SqlDbType.VarChar) { Value = regulation ?? string.Empty };
        var sql = StoredProcSql.Exec(StoredProcedures.SP_REGNO_EXAMMY, "@Course", "@Regulation");
        var raw = await _repo.QueryFromStoredProcAsync(sql, p1, p2);
        return raw ?? Enumerable.Empty<object>();
    }

    public async Task<IEnumerable<object>> LoadPapersListAsync(RoomMasterPapersRequest request)
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
        var sql = StoredProcSql.Exec(StoredProcedures.PROC_IS_IASUPDATE);
        return await _repo.ExecuteStoredProcAsync(sql);
    }

    //public async Task<int> RunRegnoResultProcessAsync(RoomResultProcessRequest request)
    //{
    //    // Preserve existing DAL behavior (drop temp table for a specific DB then call proc)
    //    var dbName = SQLConn.SQLConnStr.SQLdb?.ToUpper() ?? string.Empty;
    //    if (dbName == "DB_ICAMPUS_EXAMS_JBIET")
    //    {
    //        await _repo.ExecuteStoredProcAsync("drop table TLB_RESULT_PROCESS_TEMP");
    //        var q = $"[proc_resultprocess_alldata]'{request.Regno}','{request.ExamMy}','{request.PrevExamy}','{request.ResultType}'";
    //        return await _repo.ExecuteStoredProcAsync(q);
    //    }
    //    else
    //    {
    //        var q = $"[proc_resultprocess_alldataa]'{request.Regno}','{request.ExamMy}','{request.PrevExamy}'";
    //        return await _repo.ExecuteStoredProcAsync(q);
    //    }
    //}
}
