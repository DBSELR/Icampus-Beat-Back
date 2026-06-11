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
    public class CancelReceiptListService : ICancelReceiptListService
    {
        private readonly IGenericRepository<object> _repo;

        public CancelReceiptListService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Get list of courses for dropdown
        /// Stored Procedure: SPM_COURSE_LIST
        /// Parameters: @Regulation
        /// </summary>
        public async Task<IEnumerable<object>> GetCoursesAsync(string regulation)
        {
            var parameter = new SqlParameter("@REGULATION", SqlDbType.VarChar) { Value = regulation ?? string.Empty };
            var sql = StoredProcSql.Exec(StoredProcedures.SPM_COURSE_LIST, "@REGULATION");
            var raw = await _repo.QueryFromStoredProcAsync(sql, parameter);
            return raw ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Get list of exam month-years for dropdown
        /// Stored Procedure: SPM_EXAMS_ExamMY_Load
        /// Parameters: @Regulation, @Course
        /// </summary>
        public async Task<IEnumerable<object>> GetExamMYsAsync(string regulation, string course)
        {
            var parameters = new[]
            {
                new SqlParameter("@Regulation", SqlDbType.VarChar) { Value = regulation ?? string.Empty },
                new SqlParameter("@Course", SqlDbType.VarChar) { Value = course ?? string.Empty }
            };
            var sql = StoredProcSql.Exec(StoredProcedures.SPM_EXAMS_ExamMY_Load, "@Regulation", "@Course");
            var raw = await _repo.QueryFromStoredProcAsync(sql, parameters);
            return raw ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Get cancel receipt list data
        /// Stored Procedure: SP_Cancel_Receipt
        /// Parameters: @Course, @ExamMy
        /// </summary>
        public async Task<IEnumerable<object>> GetCancelReceiptListDataAsync(CancelReceiptListRequest request)
        {
            var parameters = new[]
            {
                new SqlParameter("@Course", SqlDbType.VarChar) { Value = request.Course ?? string.Empty },
                new SqlParameter("@ExamMy", SqlDbType.VarChar) { Value = request.ExamMY ?? string.Empty }
            };
            var sql = StoredProcSql.Exec(StoredProcedures.SP_Cancel_Receipt, "@Course", "@ExamMy");
            var raw = await _repo.QueryFromStoredProcAsync(sql, parameters);
            return raw ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Get cancel receipt list data for Excel export
        /// Stored Procedure: SP_Cancel_Receipt (same as data endpoint)
        /// Parameters: @Course, @ExamMy
        /// </summary>
        public async Task<IEnumerable<object>> GetCancelReceiptListExportDataAsync(CancelReceiptListRequest request)
        {
            // Same as GetCancelReceiptListDataAsync - returns data for export
            return await GetCancelReceiptListDataAsync(request);
        }
    }
}

