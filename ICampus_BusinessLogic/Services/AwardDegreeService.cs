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
    public class AwardDegreeService : IAwardDegreeService
    {
        private readonly IGenericRepository<object> _repo;

        public AwardDegreeService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Load Batch (Regulation) dropdown — ddlbatch (Page_Load)
        /// SQL: SELECT DISTINCT CAST(REGU AS VARCHAR) AS REGU,
        ///      '20'+CAST(REGU AS VARCHAR)+'-'+CAST(REGU+MAXSEM/2 AS VARCHAR) BATCH
        ///      FROM TBL_COURSE WHERE COURSE=@Course ORDER BY REGU
        /// AutoPostBack: ddlbatch_SelectedIndexChanged → loads ExamMY dropdown
        /// </summary>
        public async Task<IEnumerable<object>> LoadBatchesAsync(string course)
        {
            var sql = "SELECT DISTINCT CAST(REGU AS VARCHAR) AS REGU," +
                      "'20'+CAST(REGU AS VARCHAR)+'-'+CAST(REGU+MAXSEM/2 AS VARCHAR) BATCH" +
                      " FROM TBL_COURSE WHERE COURSE=@Course ORDER BY REGU";

            var parameters = new[]
            {
                new SqlParameter("@Course", SqlDbType.VarChar, 30) { Value = course ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Load ExamMY dropdown — cmbExamMY (ddlbatch_SelectedIndexChanged)
        /// SQL: SELECT DISTINCT EXAMMY, AEXAMID FROM TBL_EXAMS
        ///      WHERE COURSE=@Course AND REGULATION=@Regu ORDER BY AEXAMID DESC
        /// AutoPostBack: cmbExamMY_SelectedIndexChanged → auto-loads Crystal Report
        /// </summary>
        public async Task<IEnumerable<object>> LoadExamMYsAsync(string course, string regu)
        {
            // TBL_EXAMS.REGULATION stores 'R20'; regu from frontend is numeric '20' — prepend 'R'
            var sql = "SELECT DISTINCT EXAMMY, AEXAMID FROM TBL_EXAMS" +
                      " WHERE COURSE=@Course AND REGULATION='R'+@Regu ORDER BY AEXAMID DESC";

            var parameters = new[]
            {
                new SqlParameter("@Course", SqlDbType.VarChar, 30) { Value = course ?? string.Empty },
                new SqlParameter("@Regu",   SqlDbType.VarChar, 10) { Value = regu   ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Get Award Degree List data (cmbExamMY_SelectedIndexChanged auto-loads Crystal Report /
        /// btnDownLoad_Click for Excel export)
        /// SP: PROC_AWARD_DEGREE  params (2): @Regu, @ExamMY
        /// Crystal Report: AwardDegreeList.rpt
        /// Excel filename: AwardDegreeList_[regu].xlsx
        /// Note: No @Course param — SP works across all courses
        /// Confirmed: App_Web_gp3pforx.dll UTF-16LE offset 0x31a6e:
        ///   "AwardDegreeList.rpt" + "PROC_AWARD_DEGREE 'x','y'" (2 param placeholders)
        /// </summary>
        public async Task<IEnumerable<object>> GetDataAsync(string regu, string examMY, string course)
        {
            // SP param order: @Regulation, @Course, @Exammy, @Regu, [@Regno optional]
            // @Regulation = 'R' + regu (e.g. 'R20')
            var regulation = "R" + (regu ?? string.Empty);

            var sql = StoredProcSql.Exec(StoredProcedures.PROC_AWARD_DEGREE,
                "@Regulation", "@Course", "@ExamMY", "@Regu");

            var parameters = new[]
            {
                new SqlParameter("@Regulation", SqlDbType.VarChar, 20) { Value = regulation },
                new SqlParameter("@Course",     SqlDbType.VarChar, 20) { Value = course ?? string.Empty },
                new SqlParameter("@ExamMY",     SqlDbType.VarChar, 20) { Value = examMY ?? string.Empty },
                new SqlParameter("@Regu",       SqlDbType.VarChar, 20) { Value = regu   ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }
    }
}
