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

    public class AbsenteesEntryService : IAbsenteesEntryService
    {
        private readonly IGenericRepository<object> _repo;

        public AbsenteesEntryService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Load papers dropdown for Absentees Entry
        /// SP: PROC_LOADPAPERS_MRKENTRY with TYPE='T' (Theory/External)
        /// Parameters confirmed from DLL IL (5 positional params + TYPE='T' hardcoded):
        ///   @Regulation(varchar), @ExamMy(varchar), @Sem(INT — unquoted in IL), @Course(varchar), @GRP(varchar)
        /// </summary>
        public async Task<IEnumerable<AbsenteesPaperDto>> LoadPapersAsync(AbsenteesPapersRequest request)
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
                var d = row as IDictionary<string, object>;
                return new AbsenteesPaperDto
                {
                    PCODE = d?["PCODE"]?.ToString() ?? string.Empty,
                    PName = d?["PName"]?.ToString() ?? string.Empty
                };
            });
        }

        /// <summary>
        /// Load student list for Absentees Entry grid
        /// SP: PROC_LOADMARKS_MRKENTRY with TYPE='T' (Theory/External)
        /// Parameters confirmed from DLL IL (6 positional params in this exact order + TYPE='T' hardcoded):
        ///   @Regulation(varchar), @ExamMy(varchar), @Sem(varchar), @Course(varchar), @Branch(varchar), @PaperCode(varchar)
        /// Returns: aSHID, RegNo, grp, PCODE, CODE (current AB/MP status)
        /// </summary>
        public async Task<IEnumerable<AbsenteesStudentDto>> LoadStudentsAsync(AbsenteesStudentsRequest request)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.PROC_LOADMARKS_MRKENTRY,
                "@Regulation", "@ExamMy", "@Sem", "@Course", "@Branch", "@PaperCode", "@TYPE");

            var parameters = new[]
            {
                new SqlParameter("@Regulation", SqlDbType.VarChar, 10)  { Value = request.Regulation ?? string.Empty },
                new SqlParameter("@ExamMy",     SqlDbType.VarChar, 20)  { Value = request.ExamMY     ?? string.Empty },
                new SqlParameter("@Sem",        SqlDbType.VarChar, 2)   { Value = request.Sem        ?? string.Empty },
                new SqlParameter("@Course",     SqlDbType.VarChar, 30)  { Value = request.Course     ?? string.Empty },
                new SqlParameter("@Branch",     SqlDbType.VarChar, 20)  { Value = string.IsNullOrWhiteSpace(request.GRP)   ? (object)DBNull.Value : request.GRP },
                new SqlParameter("@PaperCode",  SqlDbType.VarChar, 20)  { Value = string.IsNullOrWhiteSpace(request.PCode) ? (object)DBNull.Value : request.PCode },
                new SqlParameter("@TYPE",       SqlDbType.Char,    1)   { Value = "T" }   // T = Theory/External (confirmed from DLL IL)
            };

            var raw = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            if (raw == null) return Enumerable.Empty<AbsenteesStudentDto>();

            return raw.Select(row =>
            {
                var d = row as IDictionary<string, object>;
                return new AbsenteesStudentDto
                {
                    aSHID = d?["aSHID"] != null && d["aSHID"] != DBNull.Value ? Convert.ToInt64(d["aSHID"]) : 0,
                    RegNo = d?["RegNo"]?.ToString() ?? string.Empty,
                    GRP   = d?["grp"]?.ToString()   ?? string.Empty,
                    PCODE = d?["PCODE"]?.ToString()  ?? string.Empty,
                    CODE  = d?["CODE"] != null && d["CODE"] != DBNull.Value ? d["CODE"].ToString() : null
                };
            });
        }

        /// <summary>
        /// Save a single student's absentee code (AB or MP)
        /// SP: PROC_UPDATE_MARKS_INT_S_T with TYPE='T' (Theory/External)
        /// Parameters confirmed from DLL IL (2 positional params + TYPE='T' hardcoded):
        ///   @Marks, @AshId
        /// Valid codes: "AB" (Absent), "MP" (Malpractice)
        /// </summary>
        public async Task<int> SaveCodeAsync(AbsenteesSaveRequest request)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.PROC_UPDATE_MARKS_INT_S_T,
                "@Marks", "@AshId", "@TYPE");

            var parameters = new[]
            {
                new SqlParameter("@Marks", SqlDbType.VarChar, 10) { Value = request.Code?.ToUpper() ?? string.Empty },
                new SqlParameter("@AshId", SqlDbType.BigInt)      { Value = request.ASHID },
                new SqlParameter("@TYPE",  SqlDbType.Char,    1)  { Value = "T" }   // T = Theory — SP sets both TMARKS and CODE for absentee codes (AB/MP)
            };

            return await _repo.ExecuteStoredProcAsync(sql, (object[])parameters);
        }
    }
}
