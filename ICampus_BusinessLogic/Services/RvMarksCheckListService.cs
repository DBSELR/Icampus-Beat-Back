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
    public class RvMarksCheckListService : IRvMarksCheckListService
    {
        private readonly IGenericRepository<object> _repo;

        public RvMarksCheckListService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Load Semester dropdown — ddlSemester (Page_Load + ddlSemester_SelectedIndexChanged)
        /// SQL: SELECT DISTINCT cast(SEM as varchar(250)) SEM FROM tbl_sh WHERE COURSE=@Course
        /// 1 param: @Course (no EXAMMY/Regu filter — broader scope)
        /// Confirmed: DataAccessLayer.dll UTF-16LE US heap byte 159892
        /// </summary>
        public async Task<IEnumerable<object>> LoadSemsAsync(string course)
        {
            var sql = "SELECT DISTINCT cast(SEM as varchar(250)) SEM FROM tbl_sh WHERE COURSE=@Course";

            var parameters = new[]
            {
                new SqlParameter("@Course", SqlDbType.VarChar, 30) { Value = course ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Get RV Marks Check List / Result Sheet data (btnExport_Click)
        /// isReadmit=false → PROC_RV_REPDATA (4 params): @Course, @ExamMY, @Regu, @Sem
        /// isReadmit=true  → PROC_RV_REPDATA_Readmit (5 params): @Course, @ExamMY, @Regu, @Sem, @ReadmitRegu
        /// reportType (frontend-only — same SP regardless):
        ///   1 = Check List-I  → Crystal Report: RVMarksChecklist.rpt (title: "RESULT CHECK LIST - I")
        ///   2 = Check List-II → Crystal Report: RVMarksChecklist.rpt (title: "RESULT CHECK LIST - II")
        ///   3 = Result Sheet  → Crystal Report: RVResultSheet.rpt    (title: "RV RESULT SHEET")
        ///   Readmit versions: "READMIT RESULT CHECK LIST - I/II", "READMIT RV RESULT SHEET"
        /// Confirmed: DataAccessLayer.dll UTF-16LE US heap byte 159892/159934
        /// </summary>
        public async Task<IEnumerable<object>> GetDataAsync(
            string course, string examMY, string regu, string sem,
            bool isReadmit, string readmitRegu)
        {
            object[] parameters;
            string sql;

            // SP param order: @COURSE, @EXAMMY, @SEM, @REGULATION
            // Service was sending @Regu before @Sem — SWAPPED. Fixed below.
            if (isReadmit)
            {
                sql = StoredProcSql.Exec(StoredProcedures.PROC_RV_REPDATA_Readmit,
                    "@Course", "@ExamMY", "@Sem", "@Regu", "@ReadmitRegu");

                parameters = new object[]
                {
                    new SqlParameter("@Course",      SqlDbType.VarChar, 30) { Value = course      ?? string.Empty },
                    new SqlParameter("@ExamMY",      SqlDbType.VarChar, 20) { Value = examMY      ?? string.Empty },
                    new SqlParameter("@Sem",         SqlDbType.VarChar, 5)  { Value = sem         ?? string.Empty },
                    new SqlParameter("@Regu",        SqlDbType.VarChar, 10) { Value = regu        ?? string.Empty },
                    new SqlParameter("@ReadmitRegu", SqlDbType.VarChar, 10) { Value = readmitRegu ?? string.Empty }
                };
            }
            else
            {
                sql = StoredProcSql.Exec(StoredProcedures.PROC_RV_REPDATA,
                    "@Course", "@ExamMY", "@Sem", "@Regu");

                parameters = new object[]
                {
                    new SqlParameter("@Course", SqlDbType.VarChar, 30) { Value = course ?? string.Empty },
                    new SqlParameter("@ExamMY", SqlDbType.VarChar, 20) { Value = examMY ?? string.Empty },
                    new SqlParameter("@Sem",    SqlDbType.VarChar, 5)  { Value = sem    ?? string.Empty },
                    new SqlParameter("@Regu",   SqlDbType.VarChar, 10) { Value = regu   ?? string.Empty }
                };
            }

            var result = await _repo.QueryFromStoredProcAsync(sql, parameters);
            return result ?? Enumerable.Empty<object>();
        }
    }
}
