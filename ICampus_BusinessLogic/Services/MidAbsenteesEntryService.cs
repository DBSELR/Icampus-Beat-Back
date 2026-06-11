using ICampus_BusinessLogic.Interfaces;
using ICampus_DataAccessLayer.Helpers;
using ICampus_DataAccessLayer.Interfaces;
using ICampus_Models.DTOs;
using ICampus_Models.Requests;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Services
{
    public class MidAbsenteesEntryService : IMidAbsenteesEntryService
    {
        private readonly IGenericRepository<object> _repo;

        public MidAbsenteesEntryService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Load papers dropdown for Mid Absentees Entry
        /// SP: PROC_LOADPAPERS_MRKENTRY with TYPE='T'
        /// Parameters confirmed from DLL IL (5 positional params + TYPE='T' hardcoded):
        ///   @Regulation(varchar), @ExamMy(varchar), @Sem(INT — unquoted in IL), @Course(varchar), @GRP(varchar)
        /// Same SP as regular AbsenteesEntry — ExamType is NOT passed here
        /// </summary>
        public async Task<IEnumerable<AbsenteesPaperDto>> LoadPapersAsync(MidAbsenteesPapersRequest request)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.PROC_LOADPAPERS_MRKENTRY,
                "@Regulation", "@ExamMy", "@Sem", "@Course", "@GRP", "@TYPE");

            var parameters = new[]
            {
                new SqlParameter("@Regulation", SqlDbType.VarChar, 10) { Value = request.Regulation ?? string.Empty },
                new SqlParameter("@ExamMy",     SqlDbType.VarChar, 20) { Value = request.ExamMY     ?? string.Empty },
                new SqlParameter("@Sem",        SqlDbType.Int)         { Value = int.TryParse(request.Sem, out var semInt) ? semInt : (object)DBNull.Value },  // INT — unquoted in DLL IL
                new SqlParameter("@Course",     SqlDbType.VarChar, 30) { Value = request.Course     ?? string.Empty },
                new SqlParameter("@GRP",        SqlDbType.VarChar, 30) { Value = request.GRP        ?? string.Empty },
                new SqlParameter("@TYPE",       SqlDbType.Char,    1)  { Value = "T" }   // T = Theory/External (confirmed from DLL IL)
            };

            var raw = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            if (raw == null) return Enumerable.Empty<AbsenteesPaperDto>();

            return raw.Select(row =>
            {
                var ci = row is IDictionary<string, object> rd
                    ? new Dictionary<string, object>(rd, StringComparer.OrdinalIgnoreCase)
                    : new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

                return new AbsenteesPaperDto
                {
                    PCODE = ci.TryGetValue("PCODE", out var pc) && pc != null && pc != DBNull.Value ? pc.ToString()! : string.Empty,
                    PName = ci.TryGetValue("PName", out var pn) && pn != null && pn != DBNull.Value ? pn.ToString()! : string.Empty
                };
            });
        }

        /// <summary>
        /// Load student list for Mid Absentees Entry grid
        /// SP: PROC_LOADMARKS_MRKENTRY_MID with TYPE='T' (Mid-specific SP)
        /// Parameters confirmed from DLL IL (8 positional params in this exact order):
        ///   @Regulation(varchar), @ExamMy(varchar), @Sem(varchar), @Course(varchar),
        ///   @Branch(varchar), @PaperCode(varchar), @TYPE='T'(hardcoded), @ExamType(varchar)
        /// Returns: aSHID, RegNo, grp, PCODE, MIDCODE (current AB/MP status)
        /// ExamType: "1" = MID-I, "2" = MID-II
        /// </summary>
        public async Task<IEnumerable<MidAbsenteesStudentDto>> LoadStudentsAsync(MidAbsenteesStudentsRequest request)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.PROC_LOADMARKS_MRKENTRY_MID,
                "@Regulation", "@ExamMy", "@Sem", "@Course", "@Branch", "@PaperCode", "@TYPE", "@ExamType");

            var parameters = new[]
            {
                new SqlParameter("@Regulation", SqlDbType.VarChar, 10)  { Value = request.Regulation ?? string.Empty },
                new SqlParameter("@ExamMy",     SqlDbType.VarChar, 20)  { Value = request.ExamMY     ?? string.Empty },
                new SqlParameter("@Sem",        SqlDbType.VarChar, 2)   { Value = request.Sem        ?? string.Empty },
                new SqlParameter("@Course",     SqlDbType.VarChar, 30)  { Value = request.Course     ?? string.Empty },
                new SqlParameter("@Branch",     SqlDbType.VarChar, 20)  { Value = string.IsNullOrWhiteSpace(request.GRP)   ? (object)DBNull.Value : request.GRP },
                new SqlParameter("@PaperCode",  SqlDbType.VarChar, 20)  { Value = string.IsNullOrWhiteSpace(request.PCode) ? (object)DBNull.Value : request.PCode },
                new SqlParameter("@TYPE",       SqlDbType.Char,    1)   { Value = "T" },   // T = Theory/External (confirmed from DLL IL)
                new SqlParameter("@ExamType",   SqlDbType.VarChar, 5)   { Value = request.ExamType  ?? string.Empty }   // "1" = MID-I, "2" = MID-II
            };

            var raw = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            if (raw == null) return Enumerable.Empty<MidAbsenteesStudentDto>();

            return raw.Select(row =>
            {
                var ci = row is IDictionary<string, object> rd
                    ? new Dictionary<string, object>(rd, StringComparer.OrdinalIgnoreCase)
                    : new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

                string? Str(string key) => ci.TryGetValue(key, out var v) && v != null && v != DBNull.Value ? v.ToString() : null;
                long    Lng(string key) => ci.TryGetValue(key, out var v) && v != null && v != DBNull.Value ? Convert.ToInt64(v) : 0;

                return new MidAbsenteesStudentDto
                {
                    aSHID   = Lng("aSHID"),
                    RegNo   = Str("RegNo")  ?? string.Empty,
                    GRP     = Str("grp")    ?? string.Empty,
                    PCODE   = Str("PCODE")  ?? string.Empty,
                    MIDCODE = Str("MIDCODE")
                };
            });
        }

        /// <summary>
        /// Save a single student's Mid absentee code (AB or MP)
        /// SP: PROC_UPDATE_MID_MARKS_INT_S_T with TYPE='T' (Mid-specific SP)
        /// Parameters confirmed from DLL IL (4 positional params in this exact order):
        ///   @Marks, @AshId, @TYPE='T'(hardcoded), @ExamType
        /// ExamType: "1" = MID-I, "2" = MID-II
        /// </summary>
        public async Task<int> SaveCodeAsync(MidAbsenteesSaveRequest request)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.PROC_UPDATE_MID_MARKS_INT_S_T,
                "@Marks", "@AshId", "@TYPE", "@ExamType");

            var parameters = new[]
            {
                new SqlParameter("@Marks",    SqlDbType.VarChar, 10) { Value = request.Code?.ToUpper() ?? string.Empty },
                new SqlParameter("@AshId",    SqlDbType.BigInt)      { Value = request.ASHID },
                new SqlParameter("@TYPE",     SqlDbType.Char,    1)  { Value = "T" },   // T = Theory/External (confirmed from DLL IL)
                new SqlParameter("@ExamType", SqlDbType.VarChar, 5)  { Value = request.ExamType ?? string.Empty }   // "1" = MID-I, "2" = MID-II
            };

            return await _repo.ExecuteStoredProcAsync(sql, (object[])parameters);
        }
    }
}
