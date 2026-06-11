using ICampus_BusinessLogic.Interfaces;
using ICampus_DataAccessLayer.Helpers;
using ICampus_DataAccessLayer.Interfaces;
using ICampus_Models.Requests;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Services
{
    public class FeeService : IFeeService
    {
        private readonly IGenericRepository<object> _repo;

        public FeeService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        // 1. Load fee-structure grid (SPM_FEESTUCTURE_LOAD)
        public async Task<IEnumerable<object>> LoadFeeStructureGridAsync(string course, string examMy, string regulation)
        {
            var p1 = new SqlParameter("@REGULATION", SqlDbType.VarChar) { Value = regulation ?? string.Empty };
            var p2 = new SqlParameter("@EXAMMY", SqlDbType.VarChar) { Value = examMy ?? string.Empty };
            var p3 = new SqlParameter("@COURSE", SqlDbType.VarChar) { Value = course ?? string.Empty };

            var sql = StoredProcSql.Exec(StoredProcedures.SPM_FEESTUCTURE_LOAD, "@REGULATION", "@EXAMMY", "@COURSE");

            var raw = await _repo.QueryFromStoredProcAsync(sql, p1, p2, p3);
            return raw ?? Enumerable.Empty<object>();
        }

        // 2. Filtered fee structure (inline SQL)
        public async Task<IEnumerable<object>> LoadFeeStructureFilterAsync(string course, string examMy, string regulation, string regu, string grp)
        {
            var whereParts = new List<string>();
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@COURSE", SqlDbType.VarChar) { Value = course ?? string.Empty },
                new SqlParameter("@EXAMMY", SqlDbType.VarChar) { Value = examMy ?? string.Empty },
                new SqlParameter("@REGULATION", SqlDbType.VarChar) { Value = regulation ?? string.Empty }
            };

            if (!string.IsNullOrWhiteSpace(regu))
            {
                whereParts.Add("c.regu = @REGU");
                parameters.Add(new SqlParameter("@REGU", SqlDbType.VarChar) { Value = regu });
            }

            if (!string.IsNullOrWhiteSpace(grp))
            {
                whereParts.Add("c.grp = @GRP");
                parameters.Add(new SqlParameter("@GRP", SqlDbType.VarChar) { Value = grp });
            }

            var where = whereParts.Count > 0 ? " AND " + string.Join(" AND ", whereParts) : string.Empty;

            var sql = $@"
                SELECT F.REGU, F.BATCH, DBO.ToRoman(F.SEM) SEM, F.GRP, F.AMOUNT, F.GRP + '-' + C.GSUB BRANCH
                FROM TBL_FEESTRUCTURE F
                LEFT JOIN TBL_COURSE C ON C.REGU = F.REGU AND C.GRP = F.GRP AND F.COURSE = C.COURSE
                WHERE F.COURSE = @COURSE AND F.EXAMMY = @EXAMMY AND C.Regulation = @REGULATION AND STAT IS NOT NULL {where}
                ORDER BY F.REGU, F.SEM, F.FROMPAP
            ";

            var raw = await _repo.QueryFromStoredProcAsync(sql, parameters.ToArray());
            return raw ?? Enumerable.Empty<object>();
        }

        // 3. Load sems/branches/fine rows (PROC_LOAD_SEMS_FOR_FEE)
        public async Task<IEnumerable<object>> LoadBatchBranchFineAsync(string course, string examMy, string regulation, string type)
        {
            var p1 = new SqlParameter("@REGULATION", SqlDbType.VarChar) { Value = regulation ?? string.Empty };
            var p2 = new SqlParameter("@EXAMMY", SqlDbType.VarChar) { Value = examMy ?? string.Empty };
            var p3 = new SqlParameter("@COURSE", SqlDbType.VarChar) { Value = course ?? string.Empty };
            var p4 = new SqlParameter("@TYPE", SqlDbType.VarChar) { Value = type ?? "SEMS" };

            var sql = StoredProcSql.Exec(StoredProcedures.PROC_LOAD_SEMS_FOR_FEE, "@REGULATION", "@EXAMMY", "@COURSE", "@TYPE");

            var raw = await _repo.QueryFromStoredProcAsync(sql, p1, p2, p3, p4);
            return raw ?? Enumerable.Empty<object>();
        }

        // 4. Supply grid (PROC_SUPPLY_FEE_GRIDLOAD)
        public async Task<IEnumerable<object>> LoadSupplyGridAsync(string course, string examMy, string regulation, string type)
        {
            var p1 = new SqlParameter("@REGULATION", SqlDbType.VarChar) { Value = regulation ?? string.Empty };
            var p2 = new SqlParameter("@EXAMMY", SqlDbType.VarChar) { Value = examMy ?? string.Empty };
            var p3 = new SqlParameter("@COURSE", SqlDbType.VarChar) { Value = course ?? string.Empty };
            var p4 = new SqlParameter("@TYPE", SqlDbType.VarChar) { Value = type ?? "S_FEE" };

            var sql = StoredProcSql.Exec(StoredProcedures.PROC_SUPPLY_FEE_GRIDLOAD, "@REGULATION", "@EXAMMY", "@COURSE", "@TYPE");

            var raw = await _repo.QueryFromStoredProcAsync(sql, p1, p2, p3, p4);
            return raw ?? Enumerable.Empty<object>();
        }

        // 5. Save regular fee (SP_FEE_STRUCTURE_SAVE)
        public async Task<int> SaveFeeRegularAsync(FeeSaveRegularRequest req)
        {
            // Convert.ToString works whether req.Sem is string or numeric
            var ps = new[]
            {
        new SqlParameter("@BATCH", SqlDbType.VarChar)     { Value = Convert.ToString(req.Batch) ?? string.Empty },
        new SqlParameter("@REGU", SqlDbType.VarChar)      { Value = Convert.ToString(req.Regu) ?? string.Empty },
        new SqlParameter("@SEM", SqlDbType.VarChar)       { Value = Convert.ToString(req.Sem) ?? string.Empty },
        new SqlParameter("@COURSE", SqlDbType.VarChar)    { Value = Convert.ToString(req.Course) ?? string.Empty },
        new SqlParameter("@GRP", SqlDbType.VarChar)       { Value = Convert.ToString(req.Grp) ?? string.Empty },
        new SqlParameter("@FROMPAP", SqlDbType.Int)       { Value = req.FromPap },
        new SqlParameter("@TOPAP", SqlDbType.Int)         { Value = req.ToPap },
        new SqlParameter("@AMOUNT", SqlDbType.Decimal)    { Precision = 18, Scale = 2, Value = req.Amount },
        new SqlParameter("@STAT", SqlDbType.VarChar)      { Value = string.IsNullOrWhiteSpace(Convert.ToString(req.Stat)) ? "R" : Convert.ToString(req.Stat) },
        new SqlParameter("@ExamMy", SqlDbType.VarChar)    { Value = Convert.ToString(req.ExamMy) ?? string.Empty },
        new SqlParameter("@REGULATION", SqlDbType.VarChar) { Value = Convert.ToString(req.Regulation) ?? string.Empty },
        new SqlParameter("@ALLGRP", SqlDbType.Char)       { Value = req.AllGrp ? "Y" : "N" }
    };

            var sql = StoredProcSql.Exec(
                StoredProcedures.SP_FEESTRUCTURE_SAVE,
                "@BATCH", "@REGU", "@SEM", "@COURSE", "@GRP", "@FROMPAP", "@TOPAP", "@AMOUNT", "@STAT", "@ExamMy", "@REGULATION", "@ALLGRP");

            return await _repo.ExecuteStoredProcAsync(sql, ps);
        }

        // 6. Save supply fee (PROC_SUPPLY_FEE_SAVE)
        public async Task<int> SaveSupplyFeeAsync(SupplyFeeSaveRequest req)
        {
            // keep inputs as-is (no string-length validation). Convert empty grp -> NULL for SP.
            object grpValue = string.IsNullOrWhiteSpace(req.Grp) ? (object)DBNull.Value : req.Grp;

            var ps = new[]
            {
        // set sizes to match table/SP expectations (adjust if your DB differs)
        new SqlParameter("@GRP", SqlDbType.VarChar, 15) { Value = grpValue },
        // PTYPE in proc declared CHAR(3) — can use VarChar(3) here
        new SqlParameter("@PTYPE", SqlDbType.VarChar, 3) { Value = req.PType ?? string.Empty },
        new SqlParameter("@EXAMMY", SqlDbType.VarChar, 20) { Value = req.ExamMy ?? string.Empty },
        new SqlParameter("@FCOUNT", SqlDbType.Int) { Value = req.FCount },
        new SqlParameter("@AMOUNT", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = req.Amount },
        new SqlParameter("@S_TYPE", SqlDbType.VarChar, 20) { Value = string.IsNullOrWhiteSpace(req.SType) ? "SAVE_FEE" : req.SType },
        new SqlParameter("@COURSE", SqlDbType.VarChar, 30) { Value = req.Course ?? string.Empty },
        new SqlParameter("@REGULATION", SqlDbType.VarChar, 10) { Value = req.Regulation ?? string.Empty }
    };

            var sql = StoredProcSql.Exec(StoredProcedures.PROC_SUPPLY_FEE_SAVE,
                "@GRP", "@PTYPE", "@EXAMMY", "@FCOUNT", "@AMOUNT", "@S_TYPE", "@COURSE", "@REGULATION");

            return await _repo.ExecuteStoredProcAsync(sql, ps);
        }


        // 7. Failed-subjects count (SP_FAILD_SUBJECTS_COUNT) -> returns scalar
        public async Task<int> GetFailedSubjectsCountAsync(string examMy, string sems)
        {
            var p1 = new SqlParameter("@EXAMMY", SqlDbType.VarChar) { Value = examMy ?? string.Empty };
            var p2 = new SqlParameter("@SEMS", SqlDbType.VarChar) { Value = sems ?? string.Empty };

            var sql = StoredProcSql.Exec(StoredProcedures.SP_FAILD_SUBJECTS_COUNT, "@EXAMMY", "@SEMS");

            var raw = await _repo.QueryFromStoredProcAsync(sql, p1, p2);

            // extract first scalar value safely
            try
            {
                var json = JsonConvert.SerializeObject(raw);
                var list = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(json);
                if (list != null && list.Count > 0)
                {
                    var first = list[0];
                    if (first.Count > 0)
                    {
                        var val = first.Values.FirstOrDefault();
                        if (val != null && int.TryParse(val.ToString(), out int parsed)) return parsed;
                    }
                }
            }
            catch
            {
                // ignore parse errors
            }

            // fallback default (preserve previous behaviour)
            return 10;
        }

        // 8. Load fine list (PROC_SUPPLY_FEE_GRIDLOAD with TYPE = 'F_FEE')
        public async Task<IEnumerable<object>> LoadFineListAsync(string course, string examMy, string regulation)
        {
            var p1 = new SqlParameter("@REGULATION", SqlDbType.VarChar) { Value = regulation ?? string.Empty };
            var p2 = new SqlParameter("@EXAMMY", SqlDbType.VarChar) { Value = examMy ?? string.Empty };
            var p3 = new SqlParameter("@COURSE", SqlDbType.VarChar) { Value = course ?? string.Empty };
            var p4 = new SqlParameter("@TYPE", SqlDbType.VarChar) { Value = "F_FEE" };

            var sql = StoredProcSql.Exec(StoredProcedures.PROC_SUPPLY_FEE_GRIDLOAD, "@REGULATION", "@EXAMMY", "@COURSE", "@TYPE");

            var raw = await _repo.QueryFromStoredProcAsync(sql, p1, p2, p3, p4);
            return raw ?? Enumerable.Empty<object>();
        }

        // 9. Check fine (PROC_FINE_FEE_SAVE S_TYPE = CHK_FEE)
        public async Task<IEnumerable<object>> CheckFineAsync(FineSaveRequest request)
        {
            var ps = new[]
            {
                new SqlParameter("@COURSE", SqlDbType.VarChar) { Value = request.Course ?? string.Empty },
                new SqlParameter("@EXAMMY", SqlDbType.VarChar) { Value = request.ExamMy ?? string.Empty },
                new SqlParameter("@SEM", SqlDbType.Int) { Value = request.Sem },
                new SqlParameter("@FINE_AMT", SqlDbType.Decimal) { Value = request.FineAmt },
                new SqlParameter("@FROM_DATE", SqlDbType.VarChar) { Value = request.FromDate == DateTime.MinValue ? (object)DBNull.Value : request.FromDate.ToString("yyyy-MM-dd") },
                new SqlParameter("@TO_DATE", SqlDbType.VarChar) { Value = request.ToDate == DateTime.MinValue ? (object)DBNull.Value : request.ToDate.ToString("yyyy-MM-dd") },
                new SqlParameter("@FID", SqlDbType.Int) { Value = request.Fid.HasValue ? (object)request.Fid.Value : DBNull.Value },
                new SqlParameter("@S_TYPE", SqlDbType.VarChar) { Value = "CHK_FEE" },
                new SqlParameter("@REGULATION", SqlDbType.VarChar) { Value = request.Regulation ?? string.Empty }
            };

            var sql = StoredProcSql.Exec(StoredProcedures.PROC_FINE_FEE_SAVE,
                "@COURSE", "@EXAMMY", "@SEM", "@FINE_AMT", "@FROM_DATE", "@TO_DATE", "@FID", "@S_TYPE", "@REGULATION");

            var raw = await _repo.QueryFromStoredProcAsync(sql, ps);
            return raw ?? Enumerable.Empty<object>();
        }

        // 10. Save fine (PROC_FINE_FEE_SAVE S_TYPE = SAVE_FEE)
        public async Task<int> SaveFineAsync(FineSaveRequest request)
        {
            var ps = new[]
            {
        // SP signature: @COURSE VARCHAR(30), @EXAMMY VARCHAR(30), @SEM INT, @FINE_AMT MONEY, @FROM_DATE VARCHAR(30), @TO_DATE VARCHAR(30), @FID INT, @S_TYPE VARCHAR(20), @REGULATION VARCHAR(10)
        new SqlParameter("@COURSE", SqlDbType.VarChar, 30) { Value = request.Course ?? string.Empty },
        new SqlParameter("@EXAMMY", SqlDbType.VarChar, 30) { Value = request.ExamMy ?? string.Empty },

        // SP expects INT for @SEM
        new SqlParameter("@SEM", SqlDbType.Int) { Value = request.Sem },

        // Use MONEY for @FINE_AMT
        new SqlParameter("@FINE_AMT", SqlDbType.Money) { Value = request.FineAmt },

        // SP expects VARCHAR(30) for dates; we pass yyyy-MM-dd or DBNull
        new SqlParameter("@FROM_DATE", SqlDbType.VarChar, 30)
        {
            Value = request.FromDate == DateTime.MinValue ? (object)DBNull.Value : request.FromDate.ToString("yyyy-MM-dd")
        },
        new SqlParameter("@TO_DATE", SqlDbType.VarChar, 30)
        {
            Value = request.ToDate == DateTime.MinValue ? (object)DBNull.Value : request.ToDate.ToString("yyyy-MM-dd")
        },

        // FID nullable for insert
        new SqlParameter("@FID", SqlDbType.Int) { Value = request.Fid.HasValue ? (object)request.Fid.Value : DBNull.Value },

        // Save action (this method handles SAVE only)
        new SqlParameter("@S_TYPE", SqlDbType.VarChar, 20) { Value = "SAVE_FEE" },

        new SqlParameter("@REGULATION", SqlDbType.VarChar, 10) { Value = request.Regulation ?? string.Empty }
    };

            var sql = StoredProcSql.Exec(
                StoredProcedures.PROC_FINE_FEE_SAVE,
                "@COURSE", "@EXAMMY", "@SEM", "@FINE_AMT", "@FROM_DATE", "@TO_DATE", "@FID", "@S_TYPE", "@REGULATION");

            return await _repo.ExecuteStoredProcAsync(sql, ps);
        }


        // 11. Delete fine (PROC_FINE_FEE_SAVE S_TYPE = DEL_FEE)
        public async Task<int> DeleteFineAsync(FineSaveRequest request)
        {
            var ps = new[]
            {
        // match SP param types/sizes
        new SqlParameter("@COURSE", SqlDbType.VarChar, 30) { Value = request.Course ?? string.Empty },
        new SqlParameter("@EXAMMY", SqlDbType.VarChar, 30) { Value = request.ExamMy ?? string.Empty },

        // not used for delete; pass DBNull
        new SqlParameter("@SEM", SqlDbType.Int) { Value = DBNull.Value },

        // amount not used for delete; pass 0 as money
        new SqlParameter("@FINE_AMT", SqlDbType.Money) { Value = 0m },

        new SqlParameter("@FROM_DATE", SqlDbType.VarChar, 30) { Value = DBNull.Value },
        new SqlParameter("@TO_DATE", SqlDbType.VarChar, 30)   { Value = DBNull.Value },

        // FID must be provided to delete the right row; pass DBNull if missing
        new SqlParameter("@FID", SqlDbType.Int) { Value = request.Fid.HasValue ? (object)request.Fid.Value : DBNull.Value },

        // Force delete action
        new SqlParameter("@S_TYPE", SqlDbType.VarChar, 20) { Value = "DEL_FEE" },

        new SqlParameter("@REGULATION", SqlDbType.VarChar, 10) { Value = request.Regulation ?? string.Empty }
    };

            var sql = StoredProcSql.Exec(
                StoredProcedures.PROC_FINE_FEE_SAVE,
                "@COURSE", "@EXAMMY", "@SEM", "@FINE_AMT", "@FROM_DATE", "@TO_DATE", "@FID", "@S_TYPE", "@REGULATION");

            return await _repo.ExecuteStoredProcAsync(sql, ps);
        }

        // 12. Condination sems
        public async Task<IEnumerable<object>> LoadCondinationSemsAsync(string regulation, string examMy, string course)
        {
            var p1 = new SqlParameter("@REGULATION", SqlDbType.VarChar) { Value = regulation ?? string.Empty };
            var p2 = new SqlParameter("@EXAMMY", SqlDbType.VarChar) { Value = examMy ?? string.Empty };
            var p3 = new SqlParameter("@COURSE", SqlDbType.VarChar) { Value = course ?? string.Empty };

            var sql = StoredProcSql.Exec(StoredProcedures.SP_CONDINATION_SEMS, "@REGULATION", "@EXAMMY", "@COURSE");
            var raw = await _repo.QueryFromStoredProcAsync(sql, p1, p2, p3);
            return raw ?? Enumerable.Empty<object>();
        }

        // 13. Condination dates load
        public async Task<IEnumerable<object>> LoadCondinationDatesAsync(string regulation, string examMy, string course)
        {
            var p1 = new SqlParameter("@REGULATION", SqlDbType.VarChar) { Value = regulation ?? string.Empty };
            var p2 = new SqlParameter("@EXAMMY", SqlDbType.VarChar) { Value = examMy ?? string.Empty };
            var p3 = new SqlParameter("@COURSE", SqlDbType.VarChar) { Value = course ?? string.Empty };

            var sql = StoredProcSql.Exec(StoredProcedures.SP_CONDINATION_DATES_LOAD, "@REGULATION", "@EXAMMY", "@COURSE");
            var raw = await _repo.QueryFromStoredProcAsync(sql, p1, p2, p3);
            return raw ?? Enumerable.Empty<object>();
        }

        // 14. Save condination dates
        public async Task<int> SaveCondinationDatesAsync(CondinationDateSaveRequest req)
        {
            var ps = new[]
            {
                new SqlParameter("@COURSE", SqlDbType.VarChar) { Value = req.Course ?? string.Empty },
                new SqlParameter("@REGULATION", SqlDbType.VarChar) { Value = req.Regulation ?? string.Empty },
                new SqlParameter("@EXAMMY", SqlDbType.VarChar) { Value = req.ExamMy ?? string.Empty },
                new SqlParameter("@SEM", SqlDbType.Int) { Value = req.Sem },
                new SqlParameter("@FROMDATE", SqlDbType.VarChar) { Value = req.FromDate == DateTime.MinValue ? (object)DBNull.Value : req.FromDate.ToString("yyyy-MM-dd") },
                new SqlParameter("@TODATE", SqlDbType.VarChar) { Value = req.ToDate == DateTime.MinValue ? (object)DBNull.Value : req.ToDate.ToString("yyyy-MM-dd") }
            };

            var sql = StoredProcSql.Exec(StoredProcedures.SP_CONDINATION_DATES_SAVE, "@COURSE", "@REGULATION", "@EXAMMY", "@SEM", "@FROMDATE", "@TODATE");
            return await _repo.ExecuteStoredProcAsync(sql, ps);
        }

        // 15. Delete condination date
        public async Task<int> DeleteCondinationDateAsync(int fid)
        {
            var p1 = new SqlParameter("@FID", SqlDbType.Int) { Value = fid };
            var sql = StoredProcSql.Exec(StoredProcedures.SP_CONDINATION_DATES_DELETE, "@FID");
            return await _repo.ExecuteStoredProcAsync(sql, p1);
        }
    }
}
