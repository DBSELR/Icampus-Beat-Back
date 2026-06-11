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
    public class ExamRegistrationService : IExamRegistrationService
    {
        private readonly IGenericRepository<object> _repo;

        public ExamRegistrationService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<object>> GetSemsForExamRegAsync(string course, string examMy, string regulation)
        {
            var p1 = new SqlParameter("@COURSE", SqlDbType.VarChar) { Value = course ?? string.Empty };
            var p2 = new SqlParameter("@EXAMMY", SqlDbType.VarChar) { Value = examMy ?? string.Empty };
            var p3 = new SqlParameter("@REGULATION", SqlDbType.VarChar) { Value = regulation ?? string.Empty };
            var sql = StoredProcSql.Exec(StoredProcedures.SEMS_FOR_EXAMREG, "@COURSE", "@EXAMMY", "@REGULATION");
            var raw = await _repo.QueryFromStoredProcAsync(sql, p1, p2, p3);
            return raw ?? Enumerable.Empty<object>();
        }

        public async Task<IEnumerable<object>> GetStudentDetailsAsync(string regNo)
        {
            var p = new SqlParameter("@REGNO", SqlDbType.VarChar) { Value = regNo ?? string.Empty };
            var sql = StoredProcSql.Exec(StoredProcedures.SPM_STUDENT_DETAILS, "@REGNO");
            var raw = await _repo.QueryFromStoredProcAsync(sql, p);
            return raw ?? Enumerable.Empty<object>();
        }

        public async Task<IEnumerable<object>> GetDataForExamRegAsync(string regNo, string examMy, string course, string regulation, string sem = "")
        {
            // Fetch ALL papers for this student from SP (returns all sems)
            var p1 = new SqlParameter("@REGNO", SqlDbType.VarChar) { Value = regNo ?? string.Empty };
            var p2 = new SqlParameter("@exammy", SqlDbType.VarChar) { Value = examMy ?? string.Empty };
            var sql = StoredProcSql.Exec(StoredProcedures.SPM_REQ_DATA_FOR_EXAMREG, "@REGNO", "@exammy");
            var raw = await _repo.QueryFromStoredProcAsync(sql, p1, p2);
            if (raw == null || !raw.Any()) return Enumerable.Empty<object>();

            // Issue 1: Get notification sems — only show papers for sems in the exam notification
            var notifSems = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (!string.IsNullOrWhiteSpace(course) && !string.IsNullOrWhiteSpace(regulation) && !string.IsNullOrWhiteSpace(examMy))
            {
                var semsData = await GetSemsForExamRegAsync(course, examMy, regulation);
                foreach (var s in semsData)
                {
                    var sv = ExtractSemValue(s);
                    if (sv != null) notifSems.Add(sv);
                }
            }

            // Issue 2: Cache registration status per sem to avoid repeated DB calls
            var regCache = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);

            var result = new List<object>();
            foreach (var row in raw)
            {
                var paperSem = ExtractSemValue(row);

                // Issue 1: Skip papers for sems not in the exam notification
                if (notifSems.Count > 0 && paperSem != null && !notifSems.Contains(paperSem))
                    continue;

                // Issue 2: Skip papers for sems where student is already registered
                if (!string.IsNullOrWhiteSpace(paperSem))
                {
                    if (!regCache.TryGetValue(paperSem, out var isReg))
                    {
                        isReg = await CheckAlreadyRegisteredAsync(regNo, paperSem, examMy);
                        regCache[paperSem] = isReg;
                    }
                    if (isReg) continue;
                }

                result.Add(row);
            }
            return result;
        }

        // Extract SEM value from a result row using case-insensitive key lookup
        private static string ExtractSemValue(object row)
        {
            if (row is not IDictionary<string, object> dict) return null;
            var ci = new Dictionary<string, object>(dict, StringComparer.OrdinalIgnoreCase);
            foreach (var key in new[] { "SEM", "Sem", "sem", "SEMESTER", "SEM_NO", "SEMNO" })
            {
                if (ci.TryGetValue(key, out var v) && v != null && v != DBNull.Value)
                    return v.ToString().Trim();
            }
            return null;
        }

        // Check if exam notification end date is exceeded
        public async Task<string> CheckExamNotificationAsync(string examMy, string course, string regulation, string sem)
        {
            var p1 = new SqlParameter("@EXAMMY", SqlDbType.VarChar) { Value = examMy ?? string.Empty };
            var p2 = new SqlParameter("@COURSE", SqlDbType.VarChar) { Value = course ?? string.Empty };
            var p3 = new SqlParameter("@REGULATION", SqlDbType.VarChar) { Value = regulation ?? string.Empty };
            var p4 = new SqlParameter("@SEM", SqlDbType.VarChar) { Value = sem ?? string.Empty };
            
            var sql = StoredProcSql.Exec(StoredProcedures.SPM_EXAMNOTIFICATION_CHECK, "@EXAMMY", "@COURSE", "@REGULATION", "@SEM");
            var raw = await _repo.QueryFromStoredProcAsync(sql, p1, p2, p3, p4);
            
            if (raw != null && raw.Any())
            {
                var first = raw.FirstOrDefault();
                if (first != null)
                {
                    // Try to extract status from result
                    if (first is IDictionary<string, object> dict)
                    {
                        // Check for common status field names
                        foreach (var key in new[] { "Status", "STATUS", "status", "EXREG_END_DATE", "ExReg_End_Date" })
                        {
                            if (dict.ContainsKey(key) && dict[key] != null)
                            {
                                var value = dict[key].ToString();
                                // Check if end date is exceeded
                                if (DateTime.TryParse(value, out var endDate))
                                {
                                    if (DateTime.Now > endDate)
                                    {
                                        return "EXCEEDED";
                                    }
                                    return "OPENED";
                                }
                                return value;
                            }
                        }
                    }
                }
            }
            return "NOT OPENED";
        }

        // Check if student is already registered
        public async Task<bool> CheckAlreadyRegisteredAsync(string regNo, string sem, string examMy)
        {
            var p1 = new SqlParameter("@REGNO", SqlDbType.VarChar) { Value = regNo ?? string.Empty };
            var p2 = new SqlParameter("@SEM", SqlDbType.VarChar) { Value = sem ?? string.Empty };
            var p3 = new SqlParameter("@EXAMMY", SqlDbType.VarChar) { Value = examMy ?? string.Empty };
            
            var sql = StoredProcSql.Exec(StoredProcedures.sp_SH_ExamReg_Check, "@REGNO", "@SEM", "@EXAMMY");
            var raw = await _repo.QueryFromStoredProcAsync(sql, p1, p2, p3);
            
            if (raw != null && raw.Any())
            {
                var first = raw.FirstOrDefault();
                if (first != null)
                {
                    // Check if registration exists (REGD = 'Y' or similar)
                    if (first is IDictionary<string, object> dict)
                    {
                        foreach (var key in new[] { "REGD", "Regd", "regd", "Status", "STATUS" })
                        {
                            if (dict.ContainsKey(key) && dict[key] != null)
                            {
                                var value = dict[key].ToString();
                                return value.Equals("Y", StringComparison.OrdinalIgnoreCase) || 
                                       value.Equals("TRUE", StringComparison.OrdinalIgnoreCase) ||
                                       value.Equals("1", StringComparison.OrdinalIgnoreCase);
                            }
                        }
                    }
                }
            }
            return false;
        }

        // ExamRegistrationService.cs (replace existing CheckRegsUpAsync)
        public async Task<string> CheckRegsUpAsync(int regu, int sem, string examMy, string course)
        {
            // Build strongly-typed parameters matching the SP definition
            var p1 = new SqlParameter("@Regu", SqlDbType.Int) { Value = regu };
            var p2 = new SqlParameter("@Sem", SqlDbType.Int) { Value = sem };
            var p3 = new SqlParameter("@Edate", SqlDbType.VarChar, 50) { Value = examMy ?? string.Empty };
            var p4 = new SqlParameter("@Course", SqlDbType.VarChar, 20) { Value = course ?? string.Empty };

            var sql = StoredProcSql.Exec(StoredProcedures.sp_RegsUp_Check_ExammY, "@Regu", "@Sem", "@Edate", "@Course");

            // Use the QueryFromStoredProcAsync you already have (returns IEnumerable<object>)
            var raw = await _repo.QueryFromStoredProcAsync(sql, p1, p2, p3, p4);

            // Defensive extraction of the single column value ("TRUE" or "FALSE")
            if (raw != null)
            {
                var first = raw.FirstOrDefault();
                if (first != null)
                {
                    // Case 1: result row is IDictionary<string, object> (common for Dapper / some mappers)
                    if (first is IDictionary<string, object> dict)
                    {
                        // try common key names (Status, STATUS, status)
                        foreach (var key in new[] { "Status", "STATUS", "status" })
                        {
                            if (dict.ContainsKey(key) && dict[key] != null)
                                return dict[key].ToString();
                        }

                        // fallback to first value
                        var fv = dict.Values.FirstOrDefault();
                        if (fv != null) return fv.ToString();
                    }

                    // Case 2: result row is an object array (object[])
                    if (first is object[] arr && arr.Length > 0)
                    {
                        return arr[0]?.ToString() ?? "FALSE";
                    }

                    // Case 3: anonymous/dynamic object - try reflection to find property named "Status"
                    var t = first.GetType();
                    var prop = t.GetProperty("Status") ?? t.GetProperty("STATUS") ?? t.GetProperty("status");
                    if (prop != null)
                    {
                        var val = prop.GetValue(first);
                        if (val != null) return val.ToString();
                    }

                    // Case 4: it might be a plain scalar boxed (e.g., string)
                    var s = first as string;
                    if (!string.IsNullOrEmpty(s))
                    {
                        return s;
                    }

                    // Case 5: fallback to ToString() if that seems useful
                    var fallback = first.ToString();
                    if (!string.IsNullOrWhiteSpace(fallback))
                    {
                        // sometimes ToString() returns something like: "{ Status = TRUE }" — try to extract TRUE/FALSE
                        if (fallback.IndexOf("TRUE", StringComparison.OrdinalIgnoreCase) >= 0) return "TRUE";
                        if (fallback.IndexOf("FALSE", StringComparison.OrdinalIgnoreCase) >= 0) return "FALSE";
                        // otherwise return whole fallback
                        return fallback;
                    }
                }
            }

            // default fallback when nothing found
            return "FALSE";
        }


        public async Task<IEnumerable<object>> GetFeeStructureRegularAsync(string regu, string sem, string course, string grp, string examMy, string regulation, string regNo)
        {
            var ps = new[] {
                new SqlParameter("@REGU", SqlDbType.VarChar) { Value = regu ?? string.Empty },
                new SqlParameter("@SEM", SqlDbType.VarChar) { Value = sem ?? string.Empty },
                new SqlParameter("@COURSE", SqlDbType.VarChar) { Value = course ?? string.Empty },
                new SqlParameter("@GRP", SqlDbType.VarChar) { Value = grp ?? string.Empty },
                new SqlParameter("@EXAMMY", SqlDbType.VarChar) { Value = examMy ?? string.Empty },
                new SqlParameter("@REGULATION", SqlDbType.VarChar) { Value = regulation ?? string.Empty },
                new SqlParameter("@Regno", SqlDbType.VarChar) { Value = regNo ?? string.Empty }
            };
            var sql = StoredProcSql.Exec(StoredProcedures.SPM_GET_FEESTRUCTURE_REG, "@REGU", "@SEM", "@COURSE", "@GRP", "@EXAMMY", "@REGULATION", "@Regno");
            var raw = await _repo.QueryFromStoredProcAsync(sql, ps);
            return raw ?? Enumerable.Empty<object>();
        }

        public async Task<IEnumerable<object>> GetFeeStructureSupplyAsync(int fCount, string course, string examMy, string regulation, string regNo, int sem)
        {
            var ps = new[] {
                new SqlParameter("@fCount", SqlDbType.Int) { Value = fCount },
                new SqlParameter("@COURSE", SqlDbType.VarChar) { Value = course ?? string.Empty },
                new SqlParameter("@EXAMMY", SqlDbType.VarChar) { Value = examMy ?? string.Empty },
                new SqlParameter("@REGULATION", SqlDbType.VarChar) { Value = regulation ?? string.Empty },
                new SqlParameter("@Regno", SqlDbType.VarChar) { Value = regNo ?? string.Empty },
                new SqlParameter("@sem", SqlDbType.Int) { Value = sem }
            };
            var sql = StoredProcSql.Exec(StoredProcedures.SPM_GET_FEESTRUCTURE_SUP, "@fCount", "@COURSE", "@EXAMMY", "@REGULATION", "@Regno", "@sem");
            var raw = await _repo.QueryFromStoredProcAsync(sql, ps);
            return raw ?? Enumerable.Empty<object>();
        }

        public async Task<int> RegisterAsync(ExamRegisterRequest req)
        {
            if (req == null || req.Papers == null || !req.Papers.Any()) return 0;

            int totalResult = 0;
            
            // IMPORTANT: Original code processes registration PER SEMESTER
            // Each semester can have different registration types (REG or SUP)
            // Group papers by semester and process each semester separately
            var papersBySem = req.Papers.GroupBy(p => p.Sem).ToList();
            
            foreach (var semGroup in papersBySem)
            {
                var sem = semGroup.Key;
                var papersInSem = semGroup.ToList();
                
                // Determine registration type for this semester
                // Check if all papers in this semester have the same RegSup
                // If mixed, use the first paper's RegSup (original code behavior)
                var firstPaper = papersInSem.First();
                var regSupForSem = firstPaper.RegSup ?? req.RegSup ?? (req.IsSupply ? "SUP" : "REG");
                bool isSupplyForSem = regSupForSem.Equals("SUP", StringComparison.OrdinalIgnoreCase);
                
                int semResult = 0;
                
                if (isSupplyForSem)
                {
                    // Supply registration: Use SP_SH_EXAMREG_SAVE
                    // Original code flow:
                    // 1. Save papers to temp table using sp_SH_ExamReg_Pap (for each paper)
                    // 2. Call SP_SH_EXAMREG_SAVE which reads from temp table
                    // Original: "SP_SH_EXAMREG_SAVE '" + ExReg.Regno + "', " + ExReg.Sem + ", '" + ExReg.ExamMY + "', '" +ExReg.UserID+"'"
                    
                    // Step 1: Save papers to temp table (original: saveOpetedPaperTemp for each paper)
                    // Extract paper codes from Papers field (comma-separated)
                    var paperCodes = new List<string>();
                    foreach (var paper in papersInSem)
                    {
                        if (!string.IsNullOrWhiteSpace(paper.Papers))
                        {
                            // Papers field contains comma-separated paper codes (e.g., "J211D_J211E_")
                            // Split by underscore and filter empty strings
                            var codes = paper.Papers.Split(new[] { '_', ',' }, StringSplitOptions.RemoveEmptyEntries);
                            paperCodes.AddRange(codes);
                        }
                    }
                    
                    // Create per-user staging table [PAP_REG_<USERID>] before inserting papers
                    // sp_SH_ExamReg_Pap does: INSERT INTO [PAP_REG_<USERID>] (PCODE) VALUES (...)
                    // SP_SH_EXAMREG_SAVE reads from it and drops it at the end
                    var userId = req.UserId ?? string.Empty;
                    var createStagingTable = $"IF OBJECT_ID('[PAP_REG_{userId}]') IS NOT NULL DROP TABLE [PAP_REG_{userId}]; CREATE TABLE [PAP_REG_{userId}] (PCODE VARCHAR(20))";
                    await _repo.ExecuteStoredProcAsync(createStagingTable);

                    // Save each paper to temp table (original: sp_SH_ExamReg_Pap)
                    foreach (var pcode in paperCodes.Distinct())
                    {
                        var tempPs = new[] {
                            new SqlParameter("@PCODE", SqlDbType.VarChar) { Value = pcode ?? string.Empty },
                            new SqlParameter("@USERID", SqlDbType.VarChar) { Value = req.UserId ?? string.Empty }
                        };
                        var tempSql = StoredProcSql.Exec(StoredProcedures.sp_SH_ExamReg_Pap, "@PCODE", "@USERID");
                        await _repo.ExecuteStoredProcAsync(tempSql, tempPs);
                    }
                    
                    // Step 2: Call SP_SH_EXAMREG_SAVE which processes papers from temp table
                    var ps = new[] {
                        new SqlParameter("@REGNO", SqlDbType.VarChar) { Value = req.RegNo ?? string.Empty },
                        new SqlParameter("@SEM", SqlDbType.VarChar) { Value = sem ?? string.Empty },
                        new SqlParameter("@EXAMMY", SqlDbType.VarChar) { Value = req.ExamMy ?? string.Empty },
                        new SqlParameter("@USERID", SqlDbType.VarChar) { Value = req.UserId ?? string.Empty }
                    };
                    var sql = StoredProcSql.Exec(StoredProcedures.SP_SH_EXAMREG_SAVE, "@REGNO", "@SEM", "@EXAMMY", "@USERID");
                    semResult = await _repo.ExecuteStoredProcAsync(sql, ps);
                }
                else
                {
                    // Regular registration: Use sp_SH_ExamReg_Update
                    // Original: "sp_SH_ExamReg_Update '" + ExReg.Regno + "', " + ExReg.Sem + ", '" + ExReg.ExamMY + "'"
                    // SEM is passed as integer (no quotes in original SQL string), so stored procedure expects INT
                    if (!int.TryParse(sem, out var semInt))
                    {
                        // If Sem cannot be parsed, skip this semester
                        continue;
                    }
                    
                    // Note: sp_SH_ExamReg_Update is an UPDATE stored procedure
                    // It updates TBL_SH.REGD = 'Y' for matching RegNo, Sem, ExamMY
                    // For regular registration, the student must exist in TBL_SH first
                    var ps = new[] {
                        new SqlParameter("@REGNO", SqlDbType.VarChar) { Value = req.RegNo ?? string.Empty },
                        new SqlParameter("@SEM", SqlDbType.Int) { Value = semInt },
                        new SqlParameter("@EXAMMY", SqlDbType.VarChar) { Value = req.ExamMy ?? string.Empty }
                    };
                    var sql = StoredProcSql.Exec(StoredProcedures.sp_SH_ExamReg_Update, "@REGNO", "@SEM", "@EXAMMY");
                    semResult = await _repo.ExecuteStoredProcAsync(sql, ps);
                }
                
                // Accumulate results (original code: res += ExamRegistration(i.ToString()))
                totalResult += semResult;
            }

            // If any registration was successful, save exam fee data to TBL_EXAMFEE
            if (totalResult > 0 && req.TotalAmount > 0)
            {
                // Build the fee payment query string (similar to original code)
                // The ExFeePay parameter should contain the fee data query
                // Based on reference code, it builds a query to insert into TBL_EXAMFEE
                var exFeePayQuery = BuildExamFeeQuery(req);
                
                var feePayReq = new ExamFeePayRequest
                {
                    ExFeePay = exFeePayQuery,
                    AppFee = req.AppFee,
                    Concession = req.Concession
                };
                
                // Call SPM_EXAMFEE_PAY to save fee data to TBL_EXAMFEE
                await PayExamFeeAsync(feePayReq);
            }

            return totalResult;
        }

        // Build exam fee query string for SPM_EXAMFEE_PAY
        // The SP prepends: 'INSERT INTO tbl_ExamFee(RECEIPTNO, PDATE, EXAMMY, COURSE, REGNO, SEM, PAPERS, REGSUP, PCOUNT, APPFEE, EXAMFEE, FINEFEE, CONCESSION, USERID) '
        // So @Q should only contain the SELECT part, not the full INSERT statement
        // Column order must match: RECEIPTNO, PDATE, EXAMMY, COURSE, REGNO, SEM, PAPERS, REGSUP, PCOUNT, APPFEE, EXAMFEE, FINEFEE, CONCESSION, USERID
        // IMPORTANT: RegSup value must match the registration type (REG for regular, SUP for supply)
        // This value comes from the papers data (SPM_REQ_DATA_FOR_EXAMREG returns REGSUP column)
        private string BuildExamFeeQuery(ExamRegisterRequest req)
        {
            // Escape single quotes to prevent SQL injection
            var examMyEscaped = (req.ExamMy ?? "").Replace("'", "''");
            var courseEscaped = (req.Course ?? "").Replace("'", "''");
            var regNoEscaped = (req.RegNo ?? "").Replace("'", "''");
            // Use RegSup from request, but ensure it's set correctly based on registration type
            // If RegSup is not provided, determine from IsSupply flag
            var regSupValue = req.RegSup;
            if (string.IsNullOrWhiteSpace(regSupValue))
            {
                regSupValue = req.IsSupply ? "SUP" : "REG";
            }
            var regSupEscaped = regSupValue.Replace("'", "''");
            var userIdEscaped = (req.UserId ?? "").Replace("'", "''");
            
            if (req.Papers != null && req.Papers.Any())
            {
                // Multiple rows - one per paper
                var selectQueries = new List<string>();
                foreach (var paper in req.Papers)
                {
                    // Escape single quotes in paper codes
                    var papersEscaped = (paper.Papers ?? "").Replace("'", "''");
                    var semEscaped = (paper.Sem ?? "").Replace("'", "''");
                    // Use RegSup from paper data (each paper can have different RegSup)
                    // If paper.RegSup is not provided, use the request-level RegSup
                    var paperRegSupValue = paper.RegSup;
                    if (string.IsNullOrWhiteSpace(paperRegSupValue))
                    {
                        paperRegSupValue = regSupValue; // Fallback to request-level RegSup
                    }
                    var paperRegSupEscaped = paperRegSupValue.Replace("'", "''");
                    
                    // Build SELECT statement (SP will prepend INSERT INTO)
                    // Column order: RECEIPTNO, PDATE, EXAMMY, COURSE, REGNO, SEM, PAPERS, REGSUP, PCOUNT, APPFEE, EXAMFEE, FINEFEE, CONCESSION, USERID
                    // Note: RECEIPTNO is generated by [dbo].[EXAMFEE_RECIEPT]() function, PDATE is GETDATE()
                    var selectQuery = $"SELECT [dbo].[EXAMFEE_RECIEPT](), GETDATE(), '{examMyEscaped}', '{courseEscaped}', '{regNoEscaped}', '{semEscaped}', '{papersEscaped}', '{paperRegSupEscaped}', {paper.PCount}, {req.AppFee}, {paper.ExamFee}, {paper.FineFee}, {req.Concession}, '{userIdEscaped}'";
                    selectQueries.Add(selectQuery);
                }
                return string.Join(" UNION ALL ", selectQueries);
            }
            else
            {
                // Single row insert
                // Build SELECT statement (SP will prepend INSERT INTO)
                // Column order: RECEIPTNO, PDATE, EXAMMY, COURSE, REGNO, SEM, PAPERS, REGSUP, PCOUNT, APPFEE, EXAMFEE, FINEFEE, CONCESSION, USERID
                var semEscaped = (req.Sem ?? "").Replace("'", "''");
                return $"SELECT [dbo].[EXAMFEE_RECIEPT](), GETDATE(), '{examMyEscaped}', '{courseEscaped}', '{regNoEscaped}', '{semEscaped}', '', '{regSupEscaped}', 0, {req.AppFee}, {req.ExamFee}, {req.FineFee}, {req.Concession}, '{userIdEscaped}'";
            }
        }

        public async Task<int> UnRegisterAsync(UnRegisterRequest req)
        {
            if (req == null) return 0;
            if (!string.IsNullOrWhiteSpace(req.RegNo))
            {
                var p1 = new SqlParameter("@RegNo", SqlDbType.VarChar) { Value = req.RegNo };
                var p2 = new SqlParameter("@ExamMY", SqlDbType.VarChar) { Value = req.ExamMy ?? string.Empty };
                var sql = StoredProcSql.Exec(StoredProcedures.SPM_ExamUnRegistration, "@RegNo", "@ExamMY");
                var raw = await _repo.QueryFromStoredProcAsync(sql, p1, p2);
                return raw != null ? 1 : 0;
            }
            return 0;
        }

        public async Task<IEnumerable<object>> PayExamFeeAsync(ExamFeePayRequest req)
        {
            if (req == null) return null;
            var p1 = new SqlParameter("@Q", SqlDbType.NVarChar) { Value = req.ExFeePay ?? string.Empty };
            var p2 = new SqlParameter("@APPFEE", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = req.AppFee };
            var p3 = new SqlParameter("@CONCESSION", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = req.Concession };
            var sql = StoredProcSql.Exec(StoredProcedures.SPM_EXAMFEE_PAY, "@Q", "@APPFEE", "@CONCESSION");
            var raw = await _repo.QueryFromStoredProcAsync(sql, p1, p2, p3);
            return raw ?? Enumerable.Empty<object>();
        }

        public async Task<IEnumerable<object>> ExamRegisteredListAsync(string examMy, string course, string regulation, string status)
        {
            var ps = new[] {
                new SqlParameter("@EXAMMY", SqlDbType.VarChar) { Value = examMy ?? string.Empty },
                new SqlParameter("@COURSE", SqlDbType.VarChar) { Value = course ?? string.Empty },
                new SqlParameter("@REGULATION", SqlDbType.VarChar) { Value = regulation ?? string.Empty },
                new SqlParameter("@STATUS", SqlDbType.VarChar) { Value = status ?? "REG" }
            };
            var sql = StoredProcSql.Exec(StoredProcedures.SPM_EXAM_REGISTER_LIST, "@EXAMMY", "@COURSE", "@REGULATION", "@STATUS");
            var raw = await _repo.QueryFromStoredProcAsync(sql, ps);
            return raw ?? Enumerable.Empty<object>();
        }

        public async Task<IEnumerable<object>> DuplicateReceiptAsync(string receiptNo)
        {
            var sql = "SELECT DISTINCT Regno FROM tbl_examfee WHERE receiptno = @RECEIPTNO";
            var p = new SqlParameter("@RECEIPTNO", SqlDbType.VarChar) { Value = (object)(receiptNo ?? string.Empty) };

            // reuse your existing repo call that accepted SQL (even though name suggests SP)
            var raw = await _repo.QueryFromStoredProcAsync(sql, p);
            return raw ?? Enumerable.Empty<object>();
        }

        public async Task<IEnumerable<object>> PrintReceiptAsync(string receiptNo)
        {
            var p = new SqlParameter("@RECEIPTNO", SqlDbType.VarChar) { Value = receiptNo ?? string.Empty };
            var sql = StoredProcSql.Exec(StoredProcedures.SPM_EXAMFEE_RECEIPT, "@RECEIPTNO");
            var raw = await _repo.QueryFromStoredProcAsync(sql, p);
            return raw ?? Enumerable.Empty<object>();
        }

        // Register All — bulk mark REGD='Y' for all students in batch-sem
        // Confirmed from DLL IL: DataAccessLayer.dll Regular_Exmas_Update
        // SQL: UPDATE TBL_SH SET REGD = 'Y' WHERE COURSE = ? AND REGU = ? AND SEM = ?
        public async Task<int> RegisterAllAsync(RegisterAllRequest req)
        {
            if (req == null) return 0;
            var sql = "UPDATE TBL_SH SET REGD = 'Y' WHERE COURSE = @Course AND REGU = @Regu AND SEM = @Sem";
            var ps = new[]
            {
                new SqlParameter("@Course", SqlDbType.VarChar, 30) { Value = req.Course ?? string.Empty },
                new SqlParameter("@Regu",   SqlDbType.VarChar, 5)  { Value = req.Regu   ?? string.Empty },
                new SqlParameter("@Sem",    SqlDbType.VarChar, 5)  { Value = req.Sem    ?? string.Empty }
            };
            return await _repo.ExecuteStoredProcAsync(sql, ps);
        }

        // Unregister All — bulk clear REGD (set to NULL) for all students in batch-sem
        // Confirmed from DLL IL: DataAccessLayer.dll Regular_Exmas_Update_UnRegester
        // SQL: UPDATE TBL_SH SET REGD = null WHERE COURSE = ? AND REGU = ? AND SEM = ?
        public async Task<int> UnRegisterAllAsync(RegisterAllRequest req)
        {
            if (req == null) return 0;
            var sql = "UPDATE TBL_SH SET REGD = null WHERE COURSE = @Course AND REGU = @Regu AND SEM = @Sem";
            var ps = new[]
            {
                new SqlParameter("@Course", SqlDbType.VarChar, 30) { Value = req.Course ?? string.Empty },
                new SqlParameter("@Regu",   SqlDbType.VarChar, 5)  { Value = req.Regu   ?? string.Empty },
                new SqlParameter("@Sem",    SqlDbType.VarChar, 5)  { Value = req.Sem    ?? string.Empty }
            };
            return await _repo.ExecuteStoredProcAsync(sql, ps);
        }

        // Get Batch-Semester data for modal dropdown
        // Original query: SELECT CAST(BATCH AS VARCHAR) + '- ' + DBO.TOROMAN(SEM) + ' SEM' AS 'BATCH-SEM', CAST(regu AS VARCHAR) + CAST(sem AS VARCHAR) AS regusem FROM TBL_REGSUP WHERE COURSE = @COURSE AND EXAMMY = @EXAMMY
        public async Task<IEnumerable<object>> GetBatchSemesterAsync(string course, string examMy)
        {
            var sql = @"SELECT 
                CAST(BATCH AS VARCHAR) + '- ' + DBO.TOROMAN(SEM) + ' SEM' AS 'BATCH-SEM',
                CAST(regu AS VARCHAR) + CAST(sem AS VARCHAR) AS regusem
            FROM TBL_REGSUP
            WHERE COURSE = @COURSE
              AND EXAMMY = @EXAMMY";

            var p1 = new SqlParameter("@COURSE", SqlDbType.VarChar) { Value = course ?? string.Empty };
            var p2 = new SqlParameter("@EXAMMY", SqlDbType.VarChar) { Value = examMy ?? string.Empty };

            var raw = await _repo.QueryFromStoredProcAsync(sql, p1, p2);
            return raw ?? Enumerable.Empty<object>();
        }
    }
}
    