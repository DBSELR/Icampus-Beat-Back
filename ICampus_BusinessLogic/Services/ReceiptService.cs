using ICampus_BusinessLogic.Interfaces;
using ICampus_DataAccessLayer.Helpers;
using ICampus_DataAccessLayer.Interfaces;
using ICampus_Models.Requests;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ICampus_BusinessLogic.Services
{
    public class ReceiptService : IReceiptService
    {
        private readonly IGenericRepository<object> _repo;

        public ReceiptService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// 1) Load receipt collection (list) using SPM_EXAMFEE_COLLECTION
        /// </summary>
        public async Task<IEnumerable<object>> LoadReceiptCollectionAsync(ReceiptCollectionRequest req)
        {
            // pass empty string when caller leaves values empty so SP receives same shape as WebForms
            var p = new[]
            {
                new SqlParameter("@EXAMMY", req?.ExamMy ?? string.Empty),
                new SqlParameter("@COURSE", req?.Course ?? string.Empty),
                new SqlParameter("@FDate", req?.FDate ?? string.Empty),
                new SqlParameter("@TDate", req?.TDate ?? string.Empty),
                // USERID is optional in your SP; pass DBNull if not provided
                string.IsNullOrWhiteSpace(req?.UserId)
                    ? new SqlParameter("@USERID", DBNull.Value)
                    : new SqlParameter("@USERID", req.UserId)
            };

            string sql = StoredProcSql.Exec(StoredProcedures.SPM_EXAMFEE_COLLECTION,
                "@EXAMMY", "@COURSE", "@FDate", "@TDate", "@USERID");

            return await _repo.QueryFromStoredProcAsync(sql, p);
        }

        /// <summary>
        /// 2) Load detailed receipt information (Sp_Receipt_List -> may internally call SPM_EXAMFEE_RECEIPT)
        /// </summary>
        public async Task<object> LoadReceiptDetailAsync(ReceiptDetailRequest req)
        {
            var p = new[]
            {
                new SqlParameter("@Course", req?.Course ?? string.Empty),
                new SqlParameter("@Exammy", req?.ExamMy ?? string.Empty),
                new SqlParameter("@RECEIPTNO", req?.ReceiptNo ?? string.Empty)
            };

            string sql = StoredProcSql.Exec(StoredProcedures.Sp_Receipt_List,
                "@Course", "@Exammy", "@RECEIPTNO");

            var raw = await _repo.QueryFromStoredProcAsync(sql, p);

            // return first result (matching pattern used in other services) - raw may be IEnumerable<object> or a dataset shaped by repo
            return raw?.FirstOrDefault();
        }

        /// <summary>
        /// 3) Search receipts by registration number (sp_Search_Regno)
        /// </summary>
        public async Task<IEnumerable<object>> SearchByRegnoAsync(string regno)
        {
            var p = new[]
            {
                new SqlParameter("@Searchregno", regno ?? string.Empty)
            };

            string sql = StoredProcSql.Exec(StoredProcedures.sp_Search_Regno, "@Searchregno");

            return await _repo.QueryFromStoredProcAsync(sql, p);
        }

        /// <summary>
        /// Optional helper: Get single receipt by receipt number (if needed elsewhere)
        /// </summary>
        public async Task<object> GetReceiptByNumberAsync(string receiptNo, string course = "", string examMy = "")
        {
            var req = new ReceiptDetailRequest
            {
                ReceiptNo = receiptNo,
                Course = course,
                ExamMy = examMy
            };

            return await LoadReceiptDetailAsync(req);
        }
    }
}
