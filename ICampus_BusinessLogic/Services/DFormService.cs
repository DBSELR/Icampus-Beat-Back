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
    public class DFormService : IDFormService
    {
        private readonly IGenericRepository<object> _repo;

        public DFormService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Generate D Form report data
        /// SP: sp_rep_deform (normal) or sp_rep_deform_Readmit (readmit)
        /// Parameters confirmed from DLL IL (App_Web_gp3pforx.dll, method loadingdform, IL offset 0x0000c3be):
        ///   Param 1: @Regulation (varchar) — REQUIRED, from Session["Regulation"]
        ///   Param 2: @Course     (varchar) — REQUIRED, from Session["Course"]
        ///   Param 3: @ExamMY     (varchar) — REQUIRED, from Session["ExamMY"]
        ///   Param 4: @SEM        (varchar) — OPTIONAL, from ddlSemester (empty = all)
        ///   Param 5: @EDate      (varchar) — OPTIONAL, from ddlEdate formatted yyyy-MM-dd (empty = all)
        /// </summary>
        public async Task<IEnumerable<object>> GetReportDataAsync(DFormRequest request)
        {
            // Select SP based on readmit flag — mirrors ChkIsreadmitresult checkbox in DFORM.aspx
            var sp = request.IsReadmit
                ? StoredProcedures.sp_rep_deform_Readmit
                : StoredProcedures.sp_rep_deform;

            var sql = StoredProcSql.Exec(sp,
                "@Regulation", "@Course", "@ExamMY", "@SEM", "@EDate");

            var parameters = new[]
            {
                new SqlParameter("@Regulation", SqlDbType.VarChar, 10) { Value = request.Regulation ?? string.Empty },
                new SqlParameter("@Course",     SqlDbType.VarChar, 30) { Value = request.Course     ?? string.Empty },
                new SqlParameter("@ExamMY",     SqlDbType.VarChar, 20) { Value = request.ExamMY     ?? string.Empty },
                new SqlParameter("@SEM",   SqlDbType.VarChar, 5)  { Value = string.IsNullOrWhiteSpace(request.SEM)   ? (object)DBNull.Value : request.SEM },
                new SqlParameter("@EDate", SqlDbType.VarChar, 20) { Value = string.IsNullOrWhiteSpace(request.EDate) ? (object)DBNull.Value : request.EDate }
            };

            var raw = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return raw ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Load semester list for DForm — uses TBL_SH (NOT TBL_GPAP like InternalChecklist)
        /// Confirmed from DLL IL: DataAccessLayer.dll method DForm_Semester
        /// Triggered by Page_Load in DFORM.aspx
        /// </summary>
        public async Task<IEnumerable<object>> GetSemestersAsync(string course, string regulation)
        {
            var sql = @"SELECT DISTINCT SEM, CAST(sem AS INT) sem1
                        FROM TBL_SH
                        WHERE COURSE = @Course AND Regulation = @Regulation
                        ORDER BY sem1";

            var parameters = new[]
            {
                new SqlParameter("@Course",     SqlDbType.VarChar, 30) { Value = course     ?? string.Empty },
                new SqlParameter("@Regulation", SqlDbType.VarChar, 10) { Value = regulation ?? string.Empty }
            };

            var raw = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return raw ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Load exam date list — critical cascade triggered by ddlSemester_SelectedIndexChanged in DFORM.aspx
        /// Confirmed from DLL IL: DataAccessLayer.dll method DForm_Edate
        /// Returns edate1 (dd-MM-yyyy display) and EDATE (raw) for use in the report call
        /// When calling GET /api/dform/report, pass eDate in yyyy-MM-dd format
        /// (original code reformats the selected date via .ToString("yyyy-MM-dd") before SP call)
        /// </summary>
        public async Task<IEnumerable<object>> GetEdatesAsync(string course, string examMY, string sem)
        {
            var sql = @"SELECT DISTINCT
                            CONVERT(NVARCHAR, EDate, 105) edate1,
                            CAST(EDATE AS VARCHAR) EDATE
                        FROM tbl_sh
                        WHERE EDATE IS NOT NULL
                          AND COURSE = @Course
                          AND sem    = @Sem
                          AND ExamMY = @ExamMY";

            var parameters = new[]
            {
                new SqlParameter("@Course", SqlDbType.VarChar, 30) { Value = course  ?? string.Empty },
                new SqlParameter("@Sem",    SqlDbType.VarChar, 5)  { Value = sem     ?? string.Empty },
                new SqlParameter("@ExamMY", SqlDbType.VarChar, 20) { Value = examMY  ?? string.Empty }
            };

            var raw = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return raw ?? Enumerable.Empty<object>();
        }
    }
}
