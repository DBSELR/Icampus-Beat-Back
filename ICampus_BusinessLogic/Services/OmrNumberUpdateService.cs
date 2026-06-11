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
    public class OmrNumberUpdateService : IOmrNumberUpdateService
    {
        private readonly IGenericRepository<object> _repo;

        public OmrNumberUpdateService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Load OMR grid for a given exam
        /// SP: PROC_REGNOVSOMR (@REGULATION, @COURSE, @EXAMMY)
        /// Returns: aSHID, REGNO, GRP, SEM, TEMPCODE, PCODE, PNAME, OMRNUMBER, PKTNO, SCANNED_SNO, SNO
        /// BAL: BAL_StudentWiseMasterCreation → Get_loadOmrNumUpdate
        /// Confirmed: EXEC PROC_REGNOVSOMR 'R20','B.TECH','May-2024' → 2723 rows
        /// </summary>
        public async Task<IEnumerable<object>> LoadOmrGridAsync(string regulation, string course, string exammy)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.PROC_REGNOVSOMR, "@REGULATION", "@COURSE", "@EXAMMY");
            var parameters = new[]
            {
                new SqlParameter("@REGULATION", SqlDbType.VarChar, 20) { Value = regulation ?? string.Empty },
                new SqlParameter("@COURSE",     SqlDbType.VarChar, 20) { Value = course     ?? string.Empty },
                new SqlParameter("@EXAMMY",     SqlDbType.VarChar, 20) { Value = exammy     ?? string.Empty }
            };
            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Update OMR number using composite key (REGNO + PCODE + EXAMMY)
        /// SP does not return aSHID, so composite key is used instead
        /// </summary>
        public async Task<int> UpdateOmrNumAsync(string regno, string pcode, string exammy, string omrNo)
        {
            var sql = "UPDATE TBL_SH SET OMRNUMBER = @OmrNo WHERE REGNO = @Regno AND PCODE = @PCode AND EXAMMY = @ExamMY";
            var parameters = new[]
            {
                new SqlParameter("@OmrNo",  SqlDbType.VarChar, 50) { Value = omrNo  ?? string.Empty },
                new SqlParameter("@Regno",  SqlDbType.VarChar, 20) { Value = regno  ?? string.Empty },
                new SqlParameter("@PCode",  SqlDbType.VarChar, 20) { Value = pcode  ?? string.Empty },
                new SqlParameter("@ExamMY", SqlDbType.VarChar, 20) { Value = exammy ?? string.Empty }
            };
            return await _repo.ExecuteStoredProcAsync(sql, (object[])parameters);
        }
    }
}
