using ICampus_BusinessLogic.Interfaces;
using ICampus_DataAccessLayer.Helpers;
using ICampus_DataAccessLayer.Interfaces;
using ICampus_Models.Requests;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Services
{
    public class DFormMidService : IDFormMidService
    {
        private readonly IGenericRepository<object> _repo;

        public DFormMidService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Load semester list for DFORM_MID ddlSemester (Page_Load)
        /// Inline SQL confirmed from DLL IL: DataAccessLayer.dll offset 0x26c8f
        /// 3 params: Course, ExamMY, Regulation — all varchar
        /// Different from regular DFORM which only uses Course + Regulation (no ExamMY filter)
        /// </summary>
        public async Task<IEnumerable<object>> GetSemestersAsync(string course, string regulation, string examMY)
        {
            var sql = @"SELECT DISTINCT CAST(SEM AS VARCHAR(250)) SEM
                        FROM tbl_sh
                        WHERE COURSE     = @Course
                          AND Exammy     = @ExamMY
                          AND Regulation = @Regulation
                        ORDER BY SEM";

            var parameters = new[]
            {
                new SqlParameter("@Course",     SqlDbType.VarChar, 30) { Value = course     ?? string.Empty },
                new SqlParameter("@ExamMY",     SqlDbType.VarChar, 20) { Value = examMY     ?? string.Empty },
                new SqlParameter("@Regulation", SqlDbType.VarChar, 10) { Value = regulation ?? string.Empty }
            };

            var raw = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return raw ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Load exam date list for DFORM_MID — ddlSemester_SelectedIndexChanged cascade
        /// SP: proc_LOAD_MID_DATES
        /// Parameters confirmed from DLL IL (DataAccessLayer.dll DForm_Edate_MID, US heap 0x9d61,
        ///   11-element newarr String.Concat):
        ///   "proc_LOAD_MID_DATES '" + Regulation + "','" + Course + "','" + ExamMy + "','" + Sem + "','" + ExamType + "'"
        ///   All 5 parameters are varchar (all quoted in IL string concat)
        /// Returns column MDATE (used as both value and display in original loadCombo call)
        /// </summary>
        public async Task<IEnumerable<object>> GetEdatesAsync(string course, string regulation, string examMY, string sem, string examType)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.proc_LOAD_MID_DATES,
                "@Regulation", "@Course", "@ExamMY", "@Sem", "@ExamType");

            var parameters = new[]
            {
                new SqlParameter("@Regulation", SqlDbType.VarChar, 10) { Value = regulation ?? string.Empty },
                new SqlParameter("@Course",     SqlDbType.VarChar, 30) { Value = course     ?? string.Empty },
                new SqlParameter("@ExamMY",     SqlDbType.VarChar, 20) { Value = examMY     ?? string.Empty },
                new SqlParameter("@Sem",        SqlDbType.VarChar, 5)  { Value = sem        ?? string.Empty },
                new SqlParameter("@ExamType",   SqlDbType.VarChar, 10) { Value = examType   ?? string.Empty }  // "1"=MID-I, "2"=MID-II
            };

            var raw = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return raw ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Generate D Form Mid report data
        /// SP: sp_rep_deform_MID (normal) or sp_rep_deform_Readmit_MID (readmit)
        /// Parameters confirmed from DLL IL (App_Web_gp3pforx.dll, 6-phase String.Concat at offset 0xc4f1):
        ///   Param 1: @Regulation (varchar) — REQUIRED
        ///   Param 2: @Course     (varchar) — REQUIRED
        ///   Param 3: @ExamMY     (varchar) — REQUIRED
        ///   Param 4: @SEM        (INT — unquoted in Phase 3!) — OPTIONAL (DBNull when null)
        ///   Param 5: @EDate      (varchar, yyyy-MM-dd) — OPTIONAL (empty = all dates)
        ///   Param 6: @ExamType   (varchar, "1"=MID-I, "2"=MID-II) — OPTIONAL (empty = all types)
        /// IsReadmit: selects between the two SP variants (ChkIsreadmitresult checkbox in original)
        /// </summary>
        public async Task<IEnumerable<object>> GetReportDataAsync(DFormMidRequest request)
        {
            var sp = request.IsReadmit
                ? StoredProcedures.sp_rep_deform_Readmit_MID
                : StoredProcedures.sp_rep_deform_MID;

            var sql = StoredProcSql.Exec(sp,
                "@Regulation", "@Course", "@ExamMY", "@SEM", "@EDate", "@ExamType");

            var parameters = new[]
            {
                new SqlParameter("@Regulation", SqlDbType.VarChar, 10) { Value = request.Regulation ?? string.Empty },
                new SqlParameter("@Course",     SqlDbType.VarChar, 30) { Value = request.Course     ?? string.Empty },
                new SqlParameter("@ExamMY",     SqlDbType.VarChar, 20) { Value = request.ExamMY     ?? string.Empty },
                new SqlParameter("@SEM",        SqlDbType.Int)         { Value = request.SEM.HasValue ? (object)request.SEM.Value : DBNull.Value },  // INT — unquoted in original (Phase 3)
                new SqlParameter("@EDate",      SqlDbType.VarChar, 20) { Value = string.IsNullOrWhiteSpace(request.EDate)    ? (object)DBNull.Value : request.EDate },
                new SqlParameter("@ExamType",   SqlDbType.VarChar, 10) { Value = string.IsNullOrWhiteSpace(request.ExamType) ? (object)DBNull.Value : request.ExamType }
            };

            var raw = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return raw ?? Enumerable.Empty<object>();
        }
    }
}
