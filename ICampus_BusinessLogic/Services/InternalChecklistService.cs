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
    public class InternalChecklistService : IInternalChecklistService
    {
        private readonly IGenericRepository<object> _repo;

        public InternalChecklistService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Generate Internal Check List report data
        /// SP: SP_REP_INTERNAL_CHKLIST — populates TBL_REPORT_PERIODICAL (no final SELECT in SP)
        /// Step 1: Execute SP to populate TBL_REPORT_PERIODICAL
        /// Step 2: SELECT from TBL_REPORT_PERIODICAL with same filter params
        /// Confirmed from DLL IL: old project calls SP then reads TBL_REPORT_PERIODICAL separately
        /// Parameters:
        ///   Param 1: @Course     (varchar)    — REQUIRED
        ///   Param 2: @ExamMY     (varchar)    — REQUIRED
        ///   Param 3: @Regulation (varchar)    — REQUIRED (full e.g. "R20")
        ///   Param 4: @REGU       (varchar, 2) — REQUIRED (2-char batch year e.g. "20")
        ///   Param 5: @GRP        (varchar)    — OPTIONAL, NULL = all branches
        ///   Param 6: @SEM        (varchar)    — OPTIONAL, NULL = all semesters
        /// </summary>
        public async Task<IEnumerable<object>> GetReportDataAsync(InternalChecklistRequest request)
        {
            // Step 1: Execute SP — populates TBL_REPORT_PERIODICAL
            var spSql = StoredProcSql.Exec(StoredProcedures.SP_REP_INTERNAL_CHKLIST,
                "@Course", "@ExamMY", "@Regulation", "@REGU", "@GRP", "@SEM");

            var spParameters = new[]
            {
                new SqlParameter("@Course",     SqlDbType.VarChar, 30) { Value = request.Course     ?? string.Empty },
                new SqlParameter("@ExamMY",     SqlDbType.VarChar, 15) { Value = request.ExamMY     ?? string.Empty },
                new SqlParameter("@Regulation", SqlDbType.VarChar, 10) { Value = request.Regulation ?? string.Empty },
                new SqlParameter("@REGU",       SqlDbType.VarChar, 2)  { Value = request.Regu       ?? string.Empty },
                new SqlParameter("@GRP",        SqlDbType.VarChar, 30) { Value = string.IsNullOrWhiteSpace(request.GRP) ? (object)DBNull.Value : request.GRP },
                new SqlParameter("@SEM",        SqlDbType.VarChar, 2)  { Value = string.IsNullOrWhiteSpace(request.SEM) ? (object)DBNull.Value : request.SEM }
            };

            await _repo.ExecuteStoredProcAsync(spSql, (object[])spParameters);

            // Step 2: Read from TBL_REPORT_PERIODICAL
            // GRP is NOT filtered here — TBL_REPORT_PERIODICAL.GRP stores full name (e.g. 'CIVIL ENGINEERING')
            // but SP was called with short code (e.g. 'CE'). SP already filtered when populating the table.
            // SEM is filtered since it stores the numeric value matching what we passed.
            var selectSql = @"SELECT * FROM TBL_REPORT_PERIODICAL
                              WHERE EXAMMY = @ExamMY
                                AND COURSE = @Course
                                AND REGU   = @REGU
                                AND (@SEM IS NULL OR CAST(SEM AS VARCHAR(2)) = @SEM)";

            var selectParameters = new[]
            {
                new SqlParameter("@ExamMY", SqlDbType.VarChar, 15) { Value = request.ExamMY ?? string.Empty },
                new SqlParameter("@Course", SqlDbType.VarChar, 30) { Value = request.Course ?? string.Empty },
                new SqlParameter("@REGU",   SqlDbType.VarChar, 2)  { Value = request.Regu   ?? string.Empty },
                new SqlParameter("@SEM",    SqlDbType.VarChar, 2)  { Value = string.IsNullOrWhiteSpace(request.SEM) ? (object)DBNull.Value : request.SEM }
            };

            var raw = await _repo.QueryFromStoredProcAsync(selectSql, (object[])selectParameters);
            return raw ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Load semester list for InternalChecklist dropdown cascade
        /// Confirmed from DLL IL: DataAccessLayer.dll method InternalCheckList_Semester
        /// Triggered by ddlBatch_SelectedIndexChanged in InternalChecklist.aspx
        /// </summary>
        public async Task<IEnumerable<object>> GetSemestersAsync(string course, string regulation)
        {
            var sql = @"SELECT DISTINCT CAST(SEM AS VARCHAR(250)) SEM
                        FROM TBL_GPAP
                        WHERE COURSE = @Course AND REGU = @Regulation
                        ORDER BY SEM";

            var parameters = new[]
            {
                new SqlParameter("@Course",     SqlDbType.VarChar, 30) { Value = course     ?? string.Empty },
                new SqlParameter("@Regulation", SqlDbType.VarChar, 10) { Value = regulation ?? string.Empty }
            };

            var raw = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return raw ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Load branch list for InternalChecklist dropdown cascade
        /// Confirmed from DLL IL: DataAccessLayer.dll method InternalCheckList_Branch
        /// Triggered by ddlSemester_SelectedIndexChanged in InternalChecklist.aspx
        /// </summary>
        public async Task<IEnumerable<object>> GetBranchesAsync(string course, string regulation)
        {
            var sql = @"SELECT DISTINCT GSUB GRP, GRP ID
                        FROM TBL_COURSE
                        WHERE COURSE = @Course AND REGU = @Regulation
                        ORDER BY ID";

            var parameters = new[]
            {
                new SqlParameter("@Course",     SqlDbType.VarChar, 30) { Value = course     ?? string.Empty },
                new SqlParameter("@Regulation", SqlDbType.VarChar, 10) { Value = regulation ?? string.Empty }
            };

            var raw = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return raw ?? Enumerable.Empty<object>();
        }
    }
}
