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

namespace ICampus_BusinessLogic.Services
{
    public class AbsenteesListService : IAbsenteesListService
    {
        private readonly IGenericRepository<object> _repo;

        public AbsenteesListService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Load semester list for AbsenteesList dropdown
        /// Inline SQL confirmed from DLL IL: DataAccessLayer.dll
        /// 1 param: Course
        /// </summary>
        public async Task<IEnumerable<object>> GetSemestersAsync(string course)
        {
            var sql = @"SELECT DISTINCT sem
                        FROM tbl_sh
                        WHERE course = @Course
                        ORDER BY sem";

            var parameters = new[]
            {
                new SqlParameter("@Course", SqlDbType.VarChar, 30) { Value = course ?? string.Empty }
            };

            var raw = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return raw ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Load exam date list — cascade from semester selection
        /// Inline SQL confirmed from DLL IL: DataAccessLayer.dll method Get_Exams_Edates
        /// 3 params: COURSE (varchar), SEM (numeric), EXAMMY (varchar)
        /// </summary>
        public async Task<IEnumerable<object>> GetEdatesAsync(string course, string examMY, string sem)
        {
            var sql = @"SELECT DISTINCT
                            CONVERT(NVARCHAR, EDate, 105) edate1,
                            CONVERT(NVARCHAR, EDate, 105) EDATE
                        FROM tbl_sh
                        WHERE EDATE IS NOT NULL
                          AND COURSE  = @Course
                          AND sem     = @Sem
                          AND ExamMY  = @ExamMY";

            var parameters = new[]
            {
                new SqlParameter("@Course", SqlDbType.VarChar, 30) { Value = course  ?? string.Empty },
                new SqlParameter("@Sem",    SqlDbType.Int)          { Value = int.TryParse(sem, out var s) ? s : (object)DBNull.Value },
                new SqlParameter("@ExamMY", SqlDbType.VarChar, 20) { Value = examMY  ?? string.Empty }
            };

            var raw = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return raw ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Generate Theory Absentees List report
        /// SP: SP_REP_ABLIST
        /// Parameters confirmed from DLL IL (App_Web_gp3pforx.dll, String.Concat at offset 0x000128e7):
        ///   Param 1: @Regulation (varchar, quoted)  — REQUIRED
        ///   Param 2: @Course     (varchar, quoted)  — REQUIRED
        ///   Param 3: @ExamMY     (varchar, quoted)  — REQUIRED
        ///   Param 4: @SEM        (INT, unquoted)    — OPTIONAL (only appended if ddlSemester.SelectedIndex > 0)
        ///   Param 5: @EDate      (varchar, quoted)  — OPTIONAL (format: yyyy-MM-dd)
        /// </summary>
        public async Task<IEnumerable<object>> GetReportDataAsync(AbsenteesListRequest request)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.SP_REP_ABLIST,
                "@Regulation", "@Course", "@ExamMY", "@SEM", "@EDate");

            var parameters = new[]
            {
                new SqlParameter("@Regulation", SqlDbType.VarChar, 10) { Value = request.Regulation ?? string.Empty },
                new SqlParameter("@Course",     SqlDbType.VarChar, 30) { Value = request.Course     ?? string.Empty },
                new SqlParameter("@ExamMY",     SqlDbType.VarChar, 20) { Value = request.ExamMY     ?? string.Empty },
                new SqlParameter("@SEM",        SqlDbType.Int)         { Value = request.SEM.HasValue ? (object)request.SEM.Value : DBNull.Value },  // INT unquoted in original
                new SqlParameter("@EDate",      SqlDbType.VarChar, 20) { Value = string.IsNullOrWhiteSpace(request.EDate) ? (object)DBNull.Value : request.EDate }
            };

            var raw = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return raw ?? Enumerable.Empty<object>();
        }
    }
}
