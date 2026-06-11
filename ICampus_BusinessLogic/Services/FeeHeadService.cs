// ICampus_BusinessLogic.Services/FeeHeadService.cs
using ICampus_BusinessLogic.Interfaces;
using ICampus_DataAccessLayer.Helpers;
using ICampus_DataAccessLayer.Interfaces;
using ICampus_Models.Requests;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Services
{
    public class FeeHeadService : IFeeHeadService
    {
        private readonly IGenericRepository<object> _repo;

        public FeeHeadService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        // GET heads by course (uses inline SQL)
        public async Task<IEnumerable<object>> GetHeadsAsync(string course)
        {
            var sql = $"SELECT ID, COURSE, FEETYPE, FEENAME, SHORTNAME, JNTUK_AMOUNT AS JNTUAMOUNT, LBRCE_AMOUNT AS LBRAMOUNT FROM TBL_FEE_HEADS WHERE COURSE = @COURSE";
            var p1 = new SqlParameter("@COURSE", SqlDbType.VarChar) { Value = course ?? string.Empty };
            // Note: QueryFromStoredProcAsync accepts SQL too in your adapter - it will run raw query
            var raw = await _repo.QueryFromStoredProcAsync(sql, p1);
            return raw ?? Array.Empty<object>();
        }

        // Save (INSERT/UPDATE) using stored proc PROCFEEHEADS_SAVE
        public async Task<int> SaveHeadAsync(FeeHeadRequest req)
        {
            var ps = new[]
            {
                new SqlParameter("@ID", SqlDbType.Int) { Value = req.ID.HasValue ? (object)req.ID.Value : 0 },
                new SqlParameter("@COURSE", SqlDbType.VarChar, 50) { Value = req.COURSE ?? string.Empty },
                new SqlParameter("@FEETYPE", SqlDbType.VarChar, 225) { Value = req.FEETYPE ?? string.Empty },
                new SqlParameter("@FEENAME", SqlDbType.VarChar, 225) { Value = req.FEENAME ?? string.Empty },
                new SqlParameter("@SHORTNAME", SqlDbType.VarChar, 225) { Value = req.SHORTNAME ?? string.Empty },
                new SqlParameter("@JNTUK_AMOUNT", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = req.JNTUAMOUNT },
                new SqlParameter("@LBRCE_AMOUNT", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = req.LBRAMOUNT },
            };

            var sql = StoredProcSql.Exec(StoredProcedures.PROCFEEHEADS_SAVE,
                                         "@ID", "@COURSE", "@FEETYPE", "@FEENAME", "@SHORTNAME", "@JNTUK_AMOUNT", "@LBRCE_AMOUNT");

            return await _repo.ExecuteStoredProcAsync(sql, ps);
        }

        // Delete by ID (previous raw query used DELETE FROM TBL_FEE_HEADS WHERE ID = '{id}'; implement as parameterized delete)
        public async Task<int> DeleteHeadAsync(int id)
        {
            var sql = "DELETE FROM [TBL_FEE_HEADS] WHERE ID = @ID";
            var p = new SqlParameter("@ID", SqlDbType.Int) { Value = id };
            return await _repo.ExecuteStoredProcAsync(sql, p); // ExecuteStoredProcAsync executes SQL too in your adapter
        }
    }
}
