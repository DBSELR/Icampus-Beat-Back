using ICampus_BusinessLogic.Interfaces;
using ICampus_DataAccessLayer.Helpers;
using ICampus_DataAccessLayer.Interfaces;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Services
{
    public class UniversityDataService : IUniversityDataService
    {
        private readonly IGenericRepository<object> _repo;

        public UniversityDataService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        // ── Shared Dropdowns ─────────────────────────────────────────────────────

        /// <summary>
        /// Load Batch dropdown
        /// SQL: SELECT DISTINCT CAST(REGU AS VARCHAR) AS REGU,
        ///      '20'+CAST(REGU AS VARCHAR)+'-'+CAST(REGU+MAXSEM/2 AS VARCHAR) BATCH
        ///      FROM TBL_COURSE WHERE COURSE=@Course ORDER BY REGU
        /// </summary>
        public async Task<IEnumerable<object>> LoadBatchAsync(string course)
        {
            var sql = "SELECT DISTINCT CAST(REGU AS VARCHAR) AS REGU, " +
                      "'20'+CAST(REGU AS VARCHAR)+'-'+CAST(REGU + MAXSEM/2 AS VARCHAR) BATCH " +
                      "FROM TBL_COURSE WHERE COURSE=@Course ORDER BY REGU";

            var p = new SqlParameter("@Course", SqlDbType.VarChar, 20) { Value = course ?? string.Empty };
            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])new[] { p });
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Load Branch dropdown (ddlBatch AutoPostBack → cascade for PC Format page)
        /// SQL: SELECT DISTINCT GRP, GRP+' - '+GSUB BRANCH FROM TBL_COURSE
        ///      WHERE COURSE=@Course AND REGU=@Regu ORDER BY GRP
        /// </summary>
        public async Task<IEnumerable<object>> LoadBranchAsync(string course, string regu)
        {
            var sql = "SELECT DISTINCT GRP, GRP+' - '+GSUB BRANCH FROM TBL_COURSE " +
                      "WHERE COURSE=@Course AND REGU=@Regu ORDER BY GRP";

            var parameters = new[]
            {
                new SqlParameter("@Course", SqlDbType.VarChar, 20) { Value = course ?? string.Empty },
                new SqlParameter("@Regu",   SqlDbType.VarChar, 10) { Value = regu   ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Load Semester dropdown
        /// SQL: SELECT DISTINCT cast(SEM as varchar(250)) SEM FROM tbl_sh
        ///      WHERE COURSE=@Course and exammy=@ExamMY ORDER BY SEM
        /// Confirmed from DataAccessLayer.dll UTF-16LE US heap fragment
        /// </summary>
        public async Task<IEnumerable<object>> LoadSemesterAsync(string course, string exammy)
        {
            var sql = "SELECT DISTINCT cast(SEM as varchar(250)) SEM FROM tbl_sh " +
                      "WHERE COURSE=@Course and exammy=@ExamMY ORDER BY SEM";

            var parameters = new[]
            {
                new SqlParameter("@Course", SqlDbType.VarChar, 20) { Value = course ?? string.Empty },
                new SqlParameter("@ExamMY", SqlDbType.VarChar, 20) { Value = exammy ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        // ── UniversityData.aspx — 3 format buttons ───────────────────────────────
        // All call PROC_EXPORT_MARKSDATA; RegSup distinguishes the output variant.
        // BAL: Bal_Reports_Results → Get_Univeristy_Formate / Get_Univeristy_Formate_R18 / UniversityData_format2
        // Course + REGU + ExamMY from master page session (passed as query params).
        // @IsRv = 0 (no revaluation filter for these format buttons).

        /// <summary>btnview — "Result Format" (all students, RegSup='')</summary>
        public async Task<IEnumerable<object>> GetResultFormatAsync(
            string course, string regu, string sem, string exammy)
            => await ExecExportMarksdata(course, regu, sem, regSup: string.Empty, exammy, isRv: false);

        /// <summary>btnView2 — "Registred Format" (regular students, RegSup='REG')</summary>
        public async Task<IEnumerable<object>> GetRegisteredFormatAsync(
            string course, string regu, string sem, string exammy)
            => await ExecExportMarksdata(course, regu, sem, regSup: "REG", exammy, isRv: false);

        /// <summary>btnview3 — "Registred Format2" (supplementary students, RegSup='SUP')</summary>
        public async Task<IEnumerable<object>> GetRegisteredFormat2Async(
            string course, string regu, string sem, string exammy)
            => await ExecExportMarksdata(course, regu, sem, regSup: "SUP", exammy, isRv: false);

        // ── University_Formate_Data.aspx ─────────────────────────────────────────

        /// <summary>
        /// btnDownLoad — Univeristy_Formate_Data (BAL, App_Web_m2jhophz)
        /// SP: PROC_EXPORT_MARKSDATA(@Course,@REGU,@Sem,@RegSup,@ExamMY,@IsRv=0)
        /// </summary>
        public async Task<IEnumerable<object>> GetFormateDataAsync(
            string course, string regu, string sem, string regSup, string exammy)
            => await ExecExportMarksdata(course, regu, sem, regSup, exammy, isRv: false);

        // ── University_SubjectData.aspx ───────────────────────────────────────────

        /// <summary>btnSubjectList — "Subjects Data"</summary>
        public async Task<IEnumerable<object>> GetSubjectListAsync(
            string course, string regu, string sem, string regSup, string exammy, bool isRv)
            => await ExecExportMarksdata(course, regu, sem, regSup, exammy, isRv);

        /// <summary>btnSubjectData — "Students Data"</summary>
        public async Task<IEnumerable<object>> GetSubjectDataAsync(
            string course, string regu, string sem, string regSup, string exammy, bool isRv)
            => await ExecExportMarksdata(course, regu, sem, regSup, exammy, isRv);

        // ── University_CD_Data.aspx ───────────────────────────────────────────────

        /// <summary>btnSubjectList — "Subjects Data" (CD variant)</summary>
        public async Task<IEnumerable<object>> GetCdSubjectListAsync(
            string course, string regu, string sem, string regSup, string exammy, bool isRv)
            => await ExecExportMarksdata(course, regu, sem, regSup, exammy, isRv);

        /// <summary>btnStudentData — "Students Data" (CD variant)</summary>
        public async Task<IEnumerable<object>> GetCdStudentDataAsync(
            string course, string regu, string sem, string regSup, string exammy, bool isRv)
            => await ExecExportMarksdata(course, regu, sem, regSup, exammy, isRv);

        // ── University_PC_Formate.aspx ────────────────────────────────────────────

        /// <summary>
        /// btnDownLoad — PC Format
        /// SP: PROC_AWARD_DEGREE_UNIVERSITY(@regulation,@course,@exammy,@regu,@sem,@type)
        /// @type: 'CourseComplete' | 'All' | 'Regnowise' | 'JNTUK CE'
        /// </summary>
        public async Task<IEnumerable<object>> GetPcFormatAsync(
            string course, string regu, string exammy, string sem, string type)
        {
            var reguNumeric = regu?.Length > 1 && (regu[0] == 'R' || regu[0] == 'r')
                ? regu.Substring(1) : regu ?? string.Empty;
            var regulation  = reguNumeric.Length > 0 ? "R" + reguNumeric : string.Empty;

            var sql = StoredProcSql.Exec(StoredProcedures.PROC_AWARD_DEGREE_UNIVERSITY,
                "@regulation", "@course", "@exammy", "@regu", "@sem", "@type");

            var parameters = new[]
            {
                new SqlParameter("@regulation", SqlDbType.VarChar, 20) { Value = regulation },
                new SqlParameter("@course",     SqlDbType.VarChar, 20) { Value = course      ?? string.Empty },
                new SqlParameter("@exammy",     SqlDbType.VarChar, 20) { Value = exammy      ?? string.Empty },
                new SqlParameter("@regu",       SqlDbType.VarChar, 20) { Value = reguNumeric },
                new SqlParameter("@sem",        SqlDbType.VarChar, 20) { Value = sem         ?? string.Empty },
                new SqlParameter("@type",       SqlDbType.VarChar, 20) { Value = type        ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        // ── Private helper ────────────────────────────────────────────────────────

        private async Task<IEnumerable<object>> ExecExportMarksdata(
            string course, string regu, string sem, string regSup, string exammy, bool isRv)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.PROC_EXPORT_MARKSDATA,
                "@EXAMMY", "@REGULATION", "@COURSE", "@SEM", "@REGU", "@RegSup", "@IsRv");

            // SP uses REGU (numeric, e.g. "20") and REGULATION (e.g. "R20") separately
            // Accept either "20" or "R20" from frontend
            var reguNumeric = regu?.Length > 1 && (regu[0] == 'R' || regu[0] == 'r')
                ? regu.Substring(1) : regu ?? string.Empty;
            var regulation  = reguNumeric.Length > 0 ? "R" + reguNumeric : string.Empty;

            var parameters = new[]
            {
                new SqlParameter("@EXAMMY",     SqlDbType.VarChar, 20) { Value = exammy     ?? string.Empty },
                new SqlParameter("@REGULATION", SqlDbType.VarChar, 20) { Value = regulation },
                new SqlParameter("@COURSE",     SqlDbType.VarChar, 20) { Value = course     ?? string.Empty },
                new SqlParameter("@SEM",        SqlDbType.VarChar, 10) { Value = sem        ?? string.Empty },
                new SqlParameter("@REGU",       SqlDbType.VarChar, 20) { Value = reguNumeric },
                new SqlParameter("@RegSup",     SqlDbType.VarChar, 10) { Value = regSup     ?? string.Empty },
                new SqlParameter("@IsRv",       SqlDbType.Bit)         { Value = isRv }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }
    }
}
