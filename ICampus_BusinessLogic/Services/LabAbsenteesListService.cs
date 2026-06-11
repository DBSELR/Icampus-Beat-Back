using ICampus_BusinessLogic.Interfaces;
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
    public class LabAbsenteesListService : ILabAbsenteesListService
    {
        private readonly IGenericRepository<object> _repo;

        public LabAbsenteesListService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Load semester list for LabAbsenteesList dropdown
        /// Same inline SQL pattern as AbsenteesList — 1 param: Course
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
        /// Load branch list — cascade from ddlSemester_SelectedIndexChanged in LabAbsenteesList.aspx
        /// 2 params: Course, Regulation
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

        /// <summary>
        /// Load subject list — cascade from ddlBranch_SelectedIndexChanged in LabAbsenteesList.aspx
        /// Inline SQL confirmed from DLL IL: DataAccessLayer.dll offset 0x25AF7
        /// 4 params: Course(varchar), Regulation(varchar), GRP(varchar), SEM(INT unquoted in original)
        /// Returns pcode (value field) and pname (display: "CS301-Data Structures")
        /// </summary>
        public async Task<IEnumerable<object>> GetSubjectsAsync(string course, string regulation, string grp, string sem)
        {
            var sql = @"SELECT DISTINCT pno, pcode, pcode + '-' + pname pname
                        FROM tbl_sh
                        WHERE Course = @Course
                          AND Regu   = @Regulation
                          AND grp    = @GRP
                          AND sem    = @SEM
                        ORDER BY pno";

            var parameters = new[]
            {
                new SqlParameter("@Course",     SqlDbType.VarChar, 30) { Value = course     ?? string.Empty },
                new SqlParameter("@Regulation", SqlDbType.VarChar, 10) { Value = regulation ?? string.Empty },
                new SqlParameter("@GRP",        SqlDbType.VarChar, 30) { Value = grp        ?? string.Empty },
                new SqlParameter("@SEM",        SqlDbType.Int)         { Value = int.TryParse(sem, out var s) ? s : (object)DBNull.Value }  // numeric/unquoted in original
            };

            var raw = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return raw ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Generate Lab Absentees List report
        /// No stored procedure — inline SQL equivalent of Crystal Reports selection formula
        /// confirmed from DLL IL (App_Web_gp3pforx.dll):
        ///   WHERE Course=@Course AND ExamMY=@ExamMY AND PMARKS IN ('ab','sm')
        ///   AND PCode=@PCode AND grp=@GRP
        /// 'ab' = Absent, 'sm' = Supplementary-Malpractice (confirmed from IL string pool)
        /// </summary>
        public async Task<IEnumerable<object>> GetReportDataAsync(LabAbsenteesListRequest request)
        {
            var sql = @"SELECT *
                        FROM tbl_SH
                        WHERE Course  = @Course
                          AND ExamMY  = @ExamMY
                          AND PMARKS  IN ('ab', 'sm')";

            // Append optional filters — mirrors Crystal Reports conditional selection formula
            var paramList = new System.Collections.Generic.List<SqlParameter>
            {
                new SqlParameter("@Course", SqlDbType.VarChar, 30) { Value = request.Course  ?? string.Empty },
                new SqlParameter("@ExamMY", SqlDbType.VarChar, 20) { Value = request.ExamMY  ?? string.Empty }
            };

            if (!string.IsNullOrWhiteSpace(request.PCode))
            {
                sql += " AND PCode = @PCode";
                paramList.Add(new SqlParameter("@PCode", SqlDbType.VarChar, 20) { Value = request.PCode });
            }

            if (!string.IsNullOrWhiteSpace(request.GRP))
            {
                sql += " AND grp = @GRP";
                paramList.Add(new SqlParameter("@GRP", SqlDbType.VarChar, 30) { Value = request.GRP });
            }

            var raw = await _repo.QueryFromStoredProcAsync(sql, paramList.Cast<object>().ToArray());
            return raw ?? Enumerable.Empty<object>();
        }
    }
}
