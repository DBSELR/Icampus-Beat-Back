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
    public class MiscFeeService : IMiscFeeService
    {
        private readonly IGenericRepository<object> _repo;
        public MiscFeeService(IGenericRepository<object> repo) => _repo = repo;

        // Load fee head rows for a student -> maps to Sp_LoadFee_Data @Regno
        public async Task<IEnumerable<object>> LoadFeeDataAsync(string regno)
        {
            var p1 = new SqlParameter("@Regno", SqlDbType.VarChar) { Value = regno ?? string.Empty };
            var sql = StoredProcSql.Exec(StoredProcedures.Sp_LoadFee_Data, "@Regno");
            var raw = await _repo.QueryFromStoredProcAsync(sql, p1);
            return raw ?? Enumerable.Empty<object>();
        }

        // Load next receipt no -> PROC_LOADRECEIPT / PROC_LOADRECEIPT (named PROC_LOADRECEIPT in DAL is PROC_LOADRECEIPT / PROC_LOADRECEIPT)
        public async Task<object> LoadReceiptNoAsync()
        {
            var sql = "PROC_LOADRECEIPT"; // match your DAL proc name: PROC_LOADRECEIPT (your DAL called PROC_LOADRECEIPT)
            // if you use StoredProcSql.Exec helper:
            var sqlExec = StoredProcSql.Exec(StoredProcedures.PROC_LOADRECEIPT);
            var raw = await _repo.QueryFromStoredProcAsync(sqlExec);
            return raw;
        }

        // Save misc fee items -> iterate items, call PROC_MISC_FEEPAY for each item (same RECPTNO)
        public async Task<IEnumerable<object>> SaveMiscFeeAsync(MiscFeeSaveRequest request)
        {
            if (request == null || request.Items == null || !request.Items.Any())
                return Enumerable.Empty<object>();

            var results = new List<object>();

            foreach (var it in request.Items)
            {
                var ps = new[]
                {
                    new SqlParameter("@RECPTNO", SqlDbType.VarChar, 15) { Value = request.ReceiptNo ?? string.Empty },
                    new SqlParameter("@DATE", SqlDbType.VarChar, 20) { Value = string.IsNullOrWhiteSpace(request.Date) ? (object)DBNull.Value : request.Date },
                    new SqlParameter("@REGNO", SqlDbType.VarChar, 20) { Value = request.Regno ?? string.Empty },
                    new SqlParameter("@SEM", SqlDbType.Int) { Value = string.IsNullOrWhiteSpace(request.Sem) ? (object)DBNull.Value : Convert.ToInt32(request.Sem) },
                    new SqlParameter("@FEENAME", SqlDbType.VarChar, 30) { Value = it.FeeName ?? string.Empty },
                    new SqlParameter("@FEETYPE", SqlDbType.VarChar, 30) { Value = it.FeeType ?? string.Empty },
                    new SqlParameter("@JNTUK_FEE", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = it.JntukFee },
                    new SqlParameter("@LBRCE_FEE", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = it.LbrceFee },
                    new SqlParameter("@CONCESSION", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = request.Concession },
                    new SqlParameter("@REMARK", SqlDbType.VarChar, 50) { Value = request.Remark ?? string.Empty },
                    new SqlParameter("@CREATEDID", SqlDbType.VarChar, 25) { Value = request.UserId ?? string.Empty }
                };

                var sql = StoredProcSql.Exec(StoredProcedures.PROC_MISC_FEEPAY,
                    "@RECPTNO", "@DATE", "@REGNO", "@SEM", "@FEENAME", "@FEETYPE", "@JNTUK_FEE", "@LBRCE_FEE", "@CONCESSION", "@REMARK", "@CREATEDID");

                var raw = await _repo.QueryFromStoredProcAsync(sql, ps);
                if (raw != null) results.AddRange(raw);
            }

            return results;
        }

        // get misc receipt rows: SELECT * FROM [TBL_MISCFEE_DETAILS] where RRRECIPTNO = ...
        public async Task<IEnumerable<object>> GetMiscReceiptAsync(string recptno)
        {
            // inline SQL used in DAL originally:
            var sql = $"select * from [TBL_MISCFEE_DETAILS] where RRRECIPTNO = '{recptno}'";
            var raw = await _repo.QueryFromStoredProcAsync(sql); // QueryFromStoredProcAsync handles inline SQL as in other services
            return raw ?? Enumerable.Empty<object>();
        }

        // Delete receipt (DAL used delete from TBL_MISCFEE_DETAILS where RRRECIPTNO = @RECPTNO)
        public async Task<int> DeleteReceiptAsync(string recptno)
        {
            var sql = $"delete from [TBL_MISCFEE_DETAILS] where RRRECIPTNO = @RECPTNO";
            var p = new SqlParameter("@RECPTNO", SqlDbType.VarChar) { Value = recptno ?? string.Empty };
            return await _repo.ExecuteStoredProcAsync(sql, p);
        }

        // Export data (SP_Export_MiscFee)
        public async Task<IEnumerable<object>> ExportDataAsync()
        {
            var sql = StoredProcSql.Exec(StoredProcedures.SP_Export_MiscFee);
            var raw = await _repo.QueryFromStoredProcAsync(sql);
            return raw ?? Enumerable.Empty<object>();
        }
    }
}
