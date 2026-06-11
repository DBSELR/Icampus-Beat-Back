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
    public class SupplyLabRegisteredService : ISupplyLabRegisteredService
    {
        private readonly IGenericRepository<object> _repo;

        public SupplyLabRegisteredService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        // Get semesters for dropdown
        // Query: select distinct sem from tbl_sh where course='{Course}' order by sem
        public async Task<IEnumerable<object>> GetSemestersAsync(string course)
        {
            var sql = "SELECT DISTINCT sem FROM tbl_sh WHERE course = @Course ORDER BY sem";
            var p = new SqlParameter("@Course", SqlDbType.VarChar) { Value = course ?? string.Empty };
            var raw = await _repo.QueryFromStoredProcAsync(sql, p);
            return raw ?? Enumerable.Empty<object>();
        }

        // Get batches/regulations for dropdown
        // Query: select distinct regu from tbl_sh s order by s.regu desc
        public async Task<IEnumerable<object>> GetBatchesAsync()
        {
            var sql = "SELECT DISTINCT regu FROM tbl_sh s ORDER BY s.regu DESC";
            var raw = await _repo.QueryFromStoredProcAsync(sql);
            return raw ?? Enumerable.Empty<object>();
        }

        // Get supply lab registered data for export
        // Stored Procedure: sp_supplylabregistereddata
        // Parameters: @ExamMY, @Course, @REGU, @Sem
        public async Task<IEnumerable<object>> GetSupplyLabDataAsync(string examMy, string course, string regu, string sem)
        {
            var ps = new[]
            {
                new SqlParameter("@ExamMY", SqlDbType.VarChar, 20) { Value = examMy ?? string.Empty },
                new SqlParameter("@Course", SqlDbType.VarChar, 20) { Value = course ?? string.Empty },
                new SqlParameter("@REGU", SqlDbType.VarChar, 20) { Value = regu ?? string.Empty },
                new SqlParameter("@Sem", SqlDbType.VarChar, 20) { Value = sem ?? string.Empty }
            };

            var sql = StoredProcSql.Exec(StoredProcedures.SP_SUPPLYLABREGISTEREDDATA, "@ExamMY", "@Course", "@REGU", "@Sem");
            var raw = await _repo.QueryFromStoredProcAsync(sql, ps);
            return raw ?? Enumerable.Empty<object>();
        }
    }
}

