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
    public class InternalMarksService : IInternalMarksService
    {
        private readonly IGenericRepository<object> _repo;

        public InternalMarksService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Load papers dropdown for Internal Marks Entry
        /// SP: PROC_LOADPAPERS_MRKENTRY
        /// Parameters: @Regulation, @EXAMMY, @Sem, @Course, @GRP, @TYPE='I'
        /// Returns: PCODE, PName (PCODE + '-' + PNAME)
        /// Only returns papers where SMAX > 0 (has internal marks)
        /// </summary>
        public async Task<IEnumerable<InternalMarksPaperDto>> LoadPapersAsync(InternalMarksPapersRequest request)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.PROC_LOADPAPERS_MRKENTRY,
                "@Regulation", "@EXAMMY", "@Sem", "@Course", "@GRP", "@TYPE");

            var parameters = new[]
            {
                new SqlParameter("@Regulation", SqlDbType.VarChar, 10)  { Value = request.Regulation ?? string.Empty },
                new SqlParameter("@EXAMMY",     SqlDbType.VarChar, 20)  { Value = request.ExamMY     ?? string.Empty },
                new SqlParameter("@Sem",        SqlDbType.VarChar, 2)   { Value = request.Sem        ?? string.Empty },
                new SqlParameter("@Course",     SqlDbType.VarChar, 30)  { Value = request.Course     ?? string.Empty },
                new SqlParameter("@GRP",        SqlDbType.VarChar, 30)  { Value = request.GRP        ?? string.Empty },
                new SqlParameter("@TYPE",       SqlDbType.Char,    1)   { Value = "I" }   // I = Internal (SMAX > 0)
            };

            var raw = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            if (raw == null) return Enumerable.Empty<InternalMarksPaperDto>();

            return raw.Select(row =>
            {
                var d = row as IDictionary<string, object>;
                return new InternalMarksPaperDto
                {
                    PCODE = d?["PCODE"]?.ToString() ?? string.Empty,
                    PName = d?["PName"]?.ToString() ?? string.Empty
                };
            });
        }

        /// <summary>
        /// Load student marks grid for Internal Marks Entry
        /// SP: PROC_LOADMARKS_MRKENTRY
        /// Parameters: @Regulation, @EXAMMY, @Sem, @Course, @GRP, @PCode, @TYPE='I'
        /// Returns: aSHID, RegNo, PCODE, SMARKS, SMAX, GRP
        /// PCode can be null to load all papers (but normally one paper is selected)
        /// </summary>
        public async Task<IEnumerable<InternalMarksStudentDto>> LoadStudentsAsync(InternalMarksStudentsRequest request)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.PROC_LOADMARKS_MRKENTRY,
                "@Regulation", "@EXAMMY", "@Sem", "@Course", "@GRP", "@PCode", "@TYPE");

            var parameters = new[]
            {
                new SqlParameter("@Regulation", SqlDbType.VarChar, 10)  { Value = request.Regulation ?? string.Empty },
                new SqlParameter("@EXAMMY",     SqlDbType.VarChar, 20)  { Value = request.ExamMY     ?? string.Empty },
                new SqlParameter("@Sem",        SqlDbType.Int)          { Value = int.TryParse(request.Sem, out var s) ? s : (object)DBNull.Value },
                new SqlParameter("@Course",     SqlDbType.VarChar, 30)  { Value = request.Course     ?? string.Empty },
                new SqlParameter("@GRP",        SqlDbType.VarChar, 20)  { Value = string.IsNullOrWhiteSpace(request.GRP)   ? (object)DBNull.Value : request.GRP },
                new SqlParameter("@PCode",      SqlDbType.VarChar, 20)  { Value = string.IsNullOrWhiteSpace(request.PCode) ? (object)DBNull.Value : request.PCode },
                new SqlParameter("@TYPE",       SqlDbType.Char,    1)   { Value = "I" }   // I = Internal (SMARKS + SMAX)
            };

            var raw = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            if (raw == null) return Enumerable.Empty<InternalMarksStudentDto>();

            return raw.Select(row =>
            {
                var d = row as IDictionary<string, object>;
                return new InternalMarksStudentDto
                {
                    aSHID  = d?["aSHID"]  != null && d["aSHID"]  != DBNull.Value ? Convert.ToInt64(d["aSHID"])  : 0,
                    RegNo  = d?["RegNo"]?.ToString()  ?? string.Empty,
                    PCODE  = d?["PCODE"]?.ToString()  ?? string.Empty,
                    SMARKS = d?["SMARKS"] != null && d["SMARKS"] != DBNull.Value ? (int?)Convert.ToInt32(d["SMARKS"]) : null,
                    SMAX   = d?["SMAX"]   != null && d["SMAX"]   != DBNull.Value ? Convert.ToInt32(d["SMAX"])   : 0,
                    GRP    = d?["GRP"]?.ToString()    ?? string.Empty
                };
            });
        }

        /// <summary>
        /// Save a single student's internal mark
        /// SP: PROC_UPDATE_MARKS_INT_S_T
        /// Parameters: @MARKS, @ASHID, @TYPE='S'
        /// TYPE='S' updates tbl_SH.SMARKS
        /// Marks value: numeric string (e.g. "18") or "AB" (Absent)
        /// </summary>
        public async Task<int> SaveMarkAsync(InternalMarksSaveRequest request)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.PROC_UPDATE_MARKS_INT_S_T,
                "@MARKS", "@ASHID", "@TYPE");

            var parameters = new[]
            {
                new SqlParameter("@MARKS", SqlDbType.VarChar, 10) { Value = request.Marks ?? string.Empty },
                new SqlParameter("@ASHID", SqlDbType.BigInt)      { Value = request.ASHID },
                new SqlParameter("@TYPE",  SqlDbType.Char,    1)  { Value = "S" }   // S = SMARKS (internal/sessional)
            };

            return await _repo.ExecuteStoredProcAsync(sql, (object[])parameters);
        }
    }
}
