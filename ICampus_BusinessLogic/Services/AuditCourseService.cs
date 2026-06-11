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
    public class AuditCourseService : IAuditCourseService
    {
        private readonly IGenericRepository<object> _repo;

        public AuditCourseService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Load Batch/Regulation dropdown (Page_Load → load_audit_regu)
        /// SQL: SELECT DISTINCT REGU FROM tbl_audit_course WHERE COURSE=@Course
        /// Source: AuditCourse.aspx — ddlbatch
        /// Confirmed: DataAccessLayer.dll UTF-16LE 0x27358:
        ///   "SELECT  DISTINCT REGU 伀 FROM tbl_audit_course WHERE COURSE = '㼁"
        ///   (garbled chars are US-heap boundary artifacts; actual SQL confirmed)
        /// </summary>
        public async Task<IEnumerable<object>> LoadBatchAsync(string course)
        {
            var sql = "SELECT DISTINCT REGU FROM tbl_audit_course WHERE COURSE=@Course";

            var p = new SqlParameter("@Course", SqlDbType.VarChar, 20) { Value = course ?? string.Empty };
            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])new[] { p });
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Load Semester dropdown (AuditCourse_data_LoadSemesters → proc_load_audit_sem)
        /// SP: proc_load_audit_sem
        /// Params (2): @Course, @Regu
        /// Source: AuditCourse.aspx — ddlSemester
        /// Confirmed: DataAccessLayer.dll UTF-16LE 0x27e93: "proc_load_audit_sem '⤁"
        ///   immediately after Proc_SubjectData_LoadSem in the SP table sequence
        /// </summary>
        public async Task<IEnumerable<object>> LoadSemAsync(string course, string regu)
        {
            // SP params: @Course, @Regulation
            var sql = StoredProcSql.Exec(StoredProcedures.proc_load_audit_sem,
                "@Course", "@Regulation");

            var ps = new object[]
            {
                new SqlParameter("@Course",     SqlDbType.VarChar, 50) { Value = course ?? string.Empty },
                new SqlParameter("@Regulation", SqlDbType.VarChar, 20) { Value = regu   ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, ps);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Load Academic Year dropdown (Page_Load → Load_AcademicYear)
        /// SQL: SELECT DISTINCT Academic_year FROM tbl_audit_course
        ///      (filtered by COURSE if available — matching the page pattern)
        /// Source: AuditCourse.aspx — ddlyear labeled "Academic_year"
        /// Confirmed: DataAccessLayer.dll UTF-16LE 0x273c6:
        ///   "SELECT  DISTINCT Academic_year ⼀ FROM tbl_audit_course"
        ///   (⼀ is heap boundary artifact; trailing WHERE COURSE=@Course inferred from pattern)
        /// </summary>
        public async Task<IEnumerable<object>> LoadAcademicYearAsync(string course)
        {
            var sql = "SELECT DISTINCT Academic_year FROM tbl_audit_course WHERE COURSE=@Course ORDER BY Academic_year";

            var p = new SqlParameter("@Course", SqlDbType.VarChar, 20) { Value = course ?? string.Empty };
            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])new[] { p });
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Get Audit Course data (btnview_Click / btnDownLoad_Click → Audit_CourseData → PROC_AuditCourse_Data)
        /// SP: PROC_AuditCourse_Data
        /// Params (4): @Course, @Regu, @Sem, @Academic_year
        /// Source: AuditCourse.aspx — "View" and "Download" buttons
        ///   App_Web_gp3pforx.dll: load_audit_regu → ddlbatch (Regu),
        ///                          AuditCourse_data_LoadSemesters → ddlSemester (Sem),
        ///                          Load_AcademicYear → ddlyear (Academic_year)
        ///   Crystal: CrystalReportViewer2 (chkIsPrint checkbox for print/export option)
        /// Confirmed: DataAccessLayer.dll UTF-16LE 0x26c34: "[PROC_AuditCourse_Data] '✁"
        ///   adjacent to proc_LOAD_MID_DATES and proc_moderation_new in DAL SP table
        /// </summary>
        public async Task<IEnumerable<object>> GetDataAsync(
            string course, string regu, string sem, string academicYear)
        {
            // SP params: @regu, @sem, @academic_year (no @Course)
            var sql = StoredProcSql.Exec(StoredProcedures.PROC_AuditCourse_Data,
                "@regu", "@sem", "@academic_year");

            var ps = new object[]
            {
                new SqlParameter("@regu",         SqlDbType.VarChar, 30) { Value = regu         ?? string.Empty },
                new SqlParameter("@sem",          SqlDbType.VarChar, 20) { Value = sem          ?? string.Empty },
                new SqlParameter("@academic_year",SqlDbType.VarChar, 30) { Value = academicYear ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, ps);
            return result ?? Enumerable.Empty<object>();
        }
    }
}
