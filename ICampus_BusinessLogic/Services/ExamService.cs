using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using ICampus_DataAccessLayer.Helpers;
using ICampus_DataAccessLayer.Interfaces;
using ICampus_Models.DTOs;
using ICampus_Models.Requests;

public class ExamService : IExamService
{
    private readonly IGenericRepository<CourseListDto> _courseRepo;  // to use for courses query result (or reuse CourseListDto)
    private readonly IGenericRepository<string> _stringRepo; // not used often - keep pattern
    private readonly IGenericRepository<ExistingExamDto> _existingRepo;
    private readonly IGenericRepository<object> _execRepo;   // execute sp returns int
    private readonly IGenericRepository<ExamNotificationDto> _notifRepo;
    private readonly IGenericRepository<ExamMasterDto> _masterRepo;
    private readonly IGenericRepository<object> _repo;

    public ExamService(
        IGenericRepository<CourseListDto> courseRepo,
        IGenericRepository<ExistingExamDto> existingRepo,
        IGenericRepository<object> execRepo,
        IGenericRepository<ExamNotificationDto> notifRepo,
        IGenericRepository<ExamMasterDto> masterRepo,
        IGenericRepository<object> repo)
    {
        _courseRepo = courseRepo ?? throw new ArgumentNullException(nameof(courseRepo));
        _existingRepo = existingRepo ?? throw new ArgumentNullException(nameof(existingRepo));
        _execRepo = execRepo ?? throw new ArgumentNullException(nameof(execRepo));
        _notifRepo = notifRepo ?? throw new ArgumentNullException(nameof(notifRepo));
        _masterRepo = masterRepo ?? throw new ArgumentNullException(nameof(masterRepo));
        _repo = repo;
    }

    // 1. Regulations (inline SQL like PaperService)
    public async Task<IEnumerable<string>> LoadRegulationsAsync()
    {
        var sql = "SELECT DISTINCT REGULATION FROM TBL_COURSE WHERE REGULATION IS NOT NULL";
        // reuse CourseListDto to fetch rows, then project REGULATION
        var rows = await _courseRepo.QueryFromStoredProcAsync(sql);
        return rows?.Select(r => r.COURSE).Distinct().ToList() ?? Enumerable.Empty<string>();
        // Note: if you prefer a RegulationDto, replace accordingly. The DAL example used a RegulationDto earlier.
    }

    // 2. Courses by regulation
    public async Task<IEnumerable<string>> LoadCoursesByRegulationAsync(string regulation)
    {
        var sql = "SELECT DISTINCT UPPER(COURSE) COURSE FROM TBL_COURSE WHERE REGULATION = @Regulation";
        var p = new SqlParameter("@Regulation", SqlDbType.VarChar) { Value = regulation ?? string.Empty };
        var rows = await _courseRepo.QueryFromStoredProcAsync(sql, p);
        return rows?.Select(r => r.COURSE).ToList() ?? Enumerable.Empty<string>();
    }

    // 3. Load existing exams (SPM_EXISTING_EXAMS_LOAD)
    public async Task<IEnumerable<ExistingExamDto>> LoadExistingExamsAsync(string regulation, string examMy, string course)
    {
        var sql = StoredProcSql.Exec(StoredProcedures.SPM_EXISTING_EXAMS_LOAD, "@REGULATION", "@EXAMMY", "@COURSE");
        var p1 = new SqlParameter("@REGULATION", SqlDbType.VarChar) { Value = regulation ?? string.Empty };
        var p2 = new SqlParameter("@EXAMMY", SqlDbType.VarChar) { Value = examMy ?? string.Empty };
        var p3 = new SqlParameter("@COURSE", SqlDbType.VarChar) { Value = course ?? string.Empty };
        return (await _existingRepo.QueryFromStoredProcAsync(sql, p1, p2, p3)).ToList();
    }

    // 4. Save exams (SP_EXAMS_SAVE)
    public async Task<int> SaveExamsAsync(ExamsSaveRequest request)
    {
        var sql = StoredProcSql.Exec(StoredProcedures.SP_EXAMS_SAVE_API,
            "@EXAMMY", "@ETYPE", "@REMARKS", "@SEMS", "@EXSEMS", "@COURSE", "@REGSEM", "@SUPSEM", "@REGULATION");

        var p1 = new SqlParameter("@EXAMMY", SqlDbType.VarChar) { Value = request.ExamMY ?? string.Empty };
        var p2 = new SqlParameter("@ETYPE", SqlDbType.VarChar) { Value = request.EType ?? string.Empty };
        var p3 = new SqlParameter("@REMARKS", SqlDbType.NVarChar) { Value = request.Remarks ?? string.Empty };
        var p4 = new SqlParameter("@SEMS", SqlDbType.VarChar) { Value = request.Sems ?? string.Empty };
        var p5 = new SqlParameter("@EXSEMS", SqlDbType.VarChar) { Value = request.ExSems ?? string.Empty };
        var p6 = new SqlParameter("@COURSE", SqlDbType.VarChar) { Value = request.Course ?? string.Empty };
        var p7 = new SqlParameter("@REGSEM", SqlDbType.VarChar) { Value = request.RegSem ?? string.Empty };
        var p8 = new SqlParameter("@SUPSEM", SqlDbType.VarChar) { Value = request.SupSem ?? string.Empty };
        var p9 = new SqlParameter("@REGULATION", SqlDbType.VarChar) { Value = request.Regulation ?? string.Empty };

        return await _execRepo.ExecuteStoredProcAsync(sql, p1, p2, p3, p4, p5, p6, p7, p8, p9);
    }

    // 5. Load notifications list (SPM_EXAMNOFICATIONS_LIST)
    public async Task<IEnumerable<object>> LoadNotificationsAsync(string examMy, string course, string regulation)
    {
        var sql = StoredProcSql.Exec(StoredProcedures.SPM_EXAMNOFICATIONS_LIST, "@EXAMMY", "@COURSE", "@REGULATION");
        var p1 = new SqlParameter("@EXAMMY", SqlDbType.VarChar) { Value = examMy ?? string.Empty };
        var p2 = new SqlParameter("@COURSE", SqlDbType.VarChar) { Value = course ?? string.Empty };
        var p3 = new SqlParameter("@REGULATION", SqlDbType.VarChar) { Value = regulation ?? string.Empty };
        return (await _repo.QueryFromStoredProcAsync(sql, p1, p2, p3)).ToList();
    }

    // 6. Save single notification (SPM_ExamNofications_Save)
    public async Task<int> SaveNotificationAsync(ExamNotificationSaveRequest request)
    {
        var sql = StoredProcSql.Exec(StoredProcedures.SPM_ExamNofications_Save,
            "@ENID", "@NNUM", "@EXAMMY", "@COURSE", "@SEM", "@NDATE", "@EXREG_DATE", "@EXREG_END_DATE", "@REGULATION");

        var p1 = new SqlParameter("@ENID", SqlDbType.VarChar) { Value = request.ENID ?? "0" };
        var p2 = new SqlParameter("@NNUM", SqlDbType.VarChar) { Value = request.NNumber ?? string.Empty };
        var p3 = new SqlParameter("@EXAMMY", SqlDbType.VarChar) { Value = request.ExamMY ?? string.Empty };
        var p4 = new SqlParameter("@COURSE", SqlDbType.VarChar) { Value = request.Course ?? string.Empty };
        var p5 = new SqlParameter("@SEM", SqlDbType.VarChar) { Value = request.Sem ?? string.Empty };
        var p6 = new SqlParameter("@NDATE", SqlDbType.VarChar) { Value = request.NDate ?? string.Empty };
        var p7 = new SqlParameter("@EXREG_DATE", SqlDbType.VarChar) { Value = request.ExRegDate ?? string.Empty };
        var p8 = new SqlParameter("@EXREG_END_DATE", SqlDbType.VarChar) { Value = request.ExRegEndDate ?? string.Empty };
        var p9 = new SqlParameter("@REGULATION", SqlDbType.VarChar) { Value = request.Regulation ?? string.Empty };

        return await _execRepo.ExecuteStoredProcAsync(sql, p1, p2, p3, p4, p5, p6, p7, p8, p9);
    }

    // 7. Exam master list (PROC_EXAM_MASTERLIST)
    public async Task<IEnumerable<ExamMasterDto>> LoadExamMasterListAsync(string regulation, string course)
    {
        var sql = StoredProcSql.Exec(StoredProcedures.PROC_EXAM_MASTERLIST, "@REGULATION", "@COURSE");
        var p1 = new SqlParameter("@REGULATION", SqlDbType.VarChar) { Value = regulation ?? string.Empty };
        var p2 = new SqlParameter("@COURSE", SqlDbType.VarChar) { Value = course ?? string.Empty };
        return (await _masterRepo.QueryFromStoredProcAsync(sql, p1, p2)).ToList();
    }

    // 8. Delete master (PROC_EXAMMASTER_DELETE)

    public async Task<IEnumerable<object>> DeleteExamMasterAsync(long aExamId)
    {
        var p1 = new SqlParameter("@AEXAMID", SqlDbType.BigInt) { Value = aExamId };
        var sql = StoredProcSql.Exec(StoredProcedures.PROC_EXAMMASTER_DELETE, "@AEXAMID");
        var raw = await _repo.QueryFromStoredProcAsync(sql, p1);
        return raw ?? Enumerable.Empty<object>();
    }

    // 9. Reset RegSup exams (SPM_RESETREGSUP_EXAMS)
    public async Task<int> ResetRegsupExamsAsync(string examMy, string course)
    {
        var sql = StoredProcSql.Exec(StoredProcedures.SPM_RESETREGSUP_EXAMS, "@EXAMMY", "@COURSE");
        var p1 = new SqlParameter("@EXAMMY", SqlDbType.VarChar) { Value = examMy ?? string.Empty };
        var p2 = new SqlParameter("@COURSE", SqlDbType.VarChar) { Value = course ?? string.Empty };
        return await _execRepo.ExecuteStoredProcAsync(sql, p1, p2);
    }

    // 10. sp_RegsUp_Save wrapper
    public async Task<int> RegsUp_SaveAsync(RegsUpSaveRequest request)
    {
        var sql = StoredProcSql.Exec(StoredProcedures.sp_RegsUp_Save, "@Regu", "@Sem", "@Mode", "@ExamMY", "@Batch", "@Course", "@cname");
        var ps = new[]
        {
            new SqlParameter("@Regu", SqlDbType.VarChar) { Value = request.Regu ?? string.Empty },
            new SqlParameter("@Sem", SqlDbType.Int) { Value = request.Sem },
            new SqlParameter("@Mode", SqlDbType.VarChar) { Value = request.Mode ?? string.Empty },
            new SqlParameter("@ExamMY", SqlDbType.VarChar) { Value = request.ExamMy ?? string.Empty },
            new SqlParameter("@Batch", SqlDbType.VarChar) { Value = request.Batch ?? string.Empty },
            new SqlParameter("@Course", SqlDbType.VarChar) { Value = request.Course ?? string.Empty },
            new SqlParameter("@cname", SqlDbType.VarChar) { Value = request.Cname ?? string.Empty }
        };
        return await _execRepo.ExecuteStoredProcAsync(sql, ps);
    }

    // 11. helper: existing exam master list (same as master list; kept for parity)
    public async Task<IEnumerable<ExamMasterDto>> ExistingExamMasterListAsync(string regulation, string course)
    {
        return (await LoadExamMasterListAsync(regulation, course)).ToList();
    }

    // 12. Get Batch by Course (SPM_GETBATCH_COURSE)
    public async Task<IEnumerable<object>> GetBatchByCourseAsync(int regu, string course)
    {
        var ps = new[]
        {
            new SqlParameter("@REGU", SqlDbType.Int) { Value = regu },
            new SqlParameter("@COURSE", SqlDbType.VarChar, 20) { Value = course ?? string.Empty }
        };

        var sql = StoredProcSql.Exec(StoredProcedures.SPM_GETBATCH_COURSE, "@REGU", "@COURSE");
        var raw = await _repo.QueryFromStoredProcAsync(sql, ps);
        return raw ?? Enumerable.Empty<object>();
    }
}
