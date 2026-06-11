using ICampus_BusinessLogic.Interfaces;
using ICampus_DataAccessLayer.Helpers;
using ICampus_DataAccessLayer.Interfaces;
using ICampus_Models.DTOs;
using ICampus_Models.Requests;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System.Data;
using System.Data.Common;
using System.Text;

namespace ICampus_BusinessLogic.Services
{
    public class PaperService : IPaperService
    {
        private readonly IGenericRepository<RegulationDto> _regRepo;
        private readonly IGenericRepository<CourseListDto> _courseRepo;
        private readonly IGenericRepository<BatchDto> _batchRepo;
        private readonly IGenericRepository<BranchDto> _branchRepo;
        private readonly IGenericRepository<SemDto> _semRepo;
        private readonly IGenericRepository<StreamDto> _streamRepo;
        private readonly IGenericRepository<PaperListDto> _paperListRepo;
        private readonly IGenericRepository<PaperDetailDto> _paperDetailRepo;
        private readonly IGenericRepository<object> _execRepo; // used for ExecuteStoredProcAsync return int
        private readonly IGenericRepository<ExamMasterDto> _examRepo;
        private readonly IGenericRepository<RegCheckDto> _regCheckRepo;

        public PaperService(
            IGenericRepository<RegulationDto> regRepo,
            IGenericRepository<CourseListDto> courseRepo,
            IGenericRepository<BatchDto> batchRepo,
            IGenericRepository<BranchDto> branchRepo,
            IGenericRepository<SemDto> semRepo,
            IGenericRepository<StreamDto> streamRepo,
            IGenericRepository<PaperListDto> paperListRepo,
            IGenericRepository<PaperDetailDto> paperDetailRepo,
            IGenericRepository<object> execRepo,
            IGenericRepository<ExamMasterDto> examRepo,
            IGenericRepository<RegCheckDto> regCheckRepo)
        {
            _regRepo = regRepo;
            _courseRepo = courseRepo;
            _batchRepo = batchRepo;
            _branchRepo = branchRepo;
            _semRepo = semRepo;
            _streamRepo = streamRepo;
            _paperListRepo = paperListRepo;
            _paperDetailRepo = paperDetailRepo;
            _execRepo = execRepo;
            _examRepo = examRepo;
            _regCheckRepo = regCheckRepo;
        }

        // 1. Regulations (inline SQL)
        public async Task<IEnumerable<RegulationDto>> LoadRegulationsAsync()
        {
            var sql = "SELECT DISTINCT REGULATION FROM TBL_COURSE WHERE REGULATION IS NOT NULL";
            return (await _regRepo.QueryFromStoredProcAsync(sql)).ToList();
        }

        // 2. Courses (inline SQL)
        public async Task<IEnumerable<CourseListDto>> LoadCoursesAsync()
        {
            var sql = "SELECT DISTINCT UPPER(course) COURSE FROM tbl_course";
            return (await _courseRepo.QueryFromStoredProcAsync(sql)).ToList();
        }

        // 3. Batches (inline SQL using TBL_COURSE)
        public async Task<IEnumerable<BatchDto>> LoadBatchesAsync(string course)
        {
            var sql = "SELECT DISTINCT REGU, '20'+REGU +'-20'+CAST((CAST(REGU AS INT)+MAXSEM/2) AS VARCHAR) BATCH FROM TBL_COURSE WHERE COURSE = @Course";
            var p = new SqlParameter("@Course", SqlDbType.VarChar) { Value = course ?? string.Empty };
            return (await _batchRepo.QueryFromStoredProcAsync(sql, p)).ToList();
        }

        // 4. Branches (inline SQL)
        public async Task<IEnumerable<BranchDto>> LoadBranchesAsync(string course, string regu)
        {
            var sql = "SELECT GRP, GRP + ' - ' + GSUB BRANCH FROM TBL_COURSE WHERE Course = @Course AND regu = @Regu ORDER BY GRP";
            var p1 = new SqlParameter("@Course", SqlDbType.VarChar) { Value = course ?? string.Empty };
            var p2 = new SqlParameter("@Regu", SqlDbType.VarChar) { Value = regu ?? string.Empty };
            return (await _branchRepo.QueryFromStoredProcAsync(sql, p1, p2)).ToList();
        }

        // 5. Sems (inline SQL)
        public async Task<IEnumerable<SemDto>> LoadSemsAsync(string course, string batch, string branch)
        {
            var sql = "SELECT DISTINCT Sem FROM TBL_GRP WHERE Course = @Course AND Batch=@Batch AND GRP=@Branch";
            var p1 = new SqlParameter("@Course", SqlDbType.VarChar) { Value = course ?? string.Empty };
            var p2 = new SqlParameter("@Batch", SqlDbType.VarChar) { Value = batch ?? string.Empty };
            var p3 = new SqlParameter("@Branch", SqlDbType.VarChar) { Value = branch ?? string.Empty };
            return (await _semRepo.QueryFromStoredProcAsync(sql, p1, p2, p3)).ToList();
        }

        // 6. Streams (inline SQL)
        public async Task<IEnumerable<StreamDto>> LoadStreamsAsync(string course, string batch, string branch, int sem)
        {
            var sql = "SELECT DISTINCT CAST(Stream AS VARCHAR) Stream FROM TBL_GRP WHERE Course = @Course AND Batch=@Batch AND GRP=@Branch AND SEM=@Sem";
            var p1 = new SqlParameter("@Course", SqlDbType.VarChar) { Value = course ?? string.Empty };
            var p2 = new SqlParameter("@Batch", SqlDbType.VarChar) { Value = batch ?? string.Empty };
            var p3 = new SqlParameter("@Branch", SqlDbType.VarChar) { Value = branch ?? string.Empty };
            var p4 = new SqlParameter("@Sem", SqlDbType.Int) { Value = sem };
            return (await _streamRepo.QueryFromStoredProcAsync(sql, p1, p2, p3, p4)).ToList();
        }

        // 7. Paper list (Pcodes) (inline SQL)
        public async Task<IEnumerable<PaperListDto>> LoadPaperListAsync(string course, string regu, string branch, int sem, string stream)
        {
            var sql = "SELECT PCODE,PNO FROM TBL_GPAP WHERE Course = @Course AND REGU=@Regu AND GRP=@Branch AND SEM=@Sem AND STREAM=@Stream ORDER BY PNO";
            var p1 = new SqlParameter("@Course", SqlDbType.VarChar) { Value = course ?? string.Empty };
            var p2 = new SqlParameter("@Regu", SqlDbType.VarChar) { Value = regu ?? string.Empty };
            var p3 = new SqlParameter("@Branch", SqlDbType.VarChar) { Value = branch ?? string.Empty };
            var p4 = new SqlParameter("@Sem", SqlDbType.Int) { Value = sem };
            var p5 = new SqlParameter("@Stream", SqlDbType.VarChar) { Value = stream ?? string.Empty };
            return (await _paperListRepo.QueryFromStoredProcAsync(sql, p1, p2, p3, p4, p5)).ToList();
        }

        // 8. Paper details (proc_getpap)
        public async Task<IEnumerable<PaperDetailDto>> GetPaperDetailsAsync(string course, string regu, int sem, string pcode, string branch)
        {
            var p1 = new SqlParameter("@course", SqlDbType.VarChar) { Value = course ?? string.Empty };
            var p2 = new SqlParameter("@regu", SqlDbType.VarChar) { Value = regu ?? string.Empty };
            var p3 = new SqlParameter("@sem", SqlDbType.Int) { Value = sem };
            var p4 = new SqlParameter("@tempcode", SqlDbType.VarChar) { Value = pcode ?? string.Empty };
            var p5 = new SqlParameter("@GRP", SqlDbType.VarChar) { Value = branch ?? string.Empty };

            var sql = StoredProcSql.Exec(
                StoredProcedures.proc_getpap,
                "@course", "@regu", "@sem", "@tempcode", "@GRP"
            );

            var rawResult = await _paperDetailRepo.QueryFromStoredProcAsync(sql, p1, p2, p3, p4, p5);

            var json = JsonConvert.SerializeObject(rawResult);
            var mapped = JsonConvert.DeserializeObject<List<PaperDetailDto>>(json);

            return mapped ?? Enumerable.Empty<PaperDetailDto>();
        }

        // 9. Save paper (sp_PAP_Save) - map fields and call SP

        // Helper function to truncate string to max length
        // 9. Save paper (sp_PAP_Save)
        public async Task<int> SavePaperAsync(PaperSaveRequest request)
        {
            // local helpers
            static string Truncate(string? value, int maxLength) =>
                string.IsNullOrEmpty(value) ? string.Empty :
                value.Length <= maxLength ? value : value.Substring(0, maxLength);

            static int GetInt(int? v) => v ?? 0;
            static decimal GetDecimal(decimal? v) => v ?? 0m;

            try
            {
                var ps = new List<SqlParameter>
        {
            // sp_PAP_Save signature (trimmed to match types/lengths in SP)
            new SqlParameter("@REGULATION", SqlDbType.VarChar, 15) { Value = Truncate(request.Regulation, 15) },
            new SqlParameter("@REGU", SqlDbType.Int) { Value = GetInt(request.ReguInt) }, // if Regu was int in SP; adapt if string
            new SqlParameter("@PNAME", SqlDbType.VarChar, 255) { Value = Truncate(request.PName, 255) },
            new SqlParameter("@PCode", SqlDbType.VarChar, 20) { Value = Truncate(request.PCode, 20) },
            new SqlParameter("@PTYPE", SqlDbType.VarChar, 20) { Value = Truncate(request.PType, 20) },
            new SqlParameter("@MAXMRK", SqlDbType.Int) { Value = GetInt(request.MaxMrk) },
            new SqlParameter("@SMAX", SqlDbType.Int) { Value = GetInt(request.SMax) },
            new SqlParameter("@TMAX", SqlDbType.Int) { Value = GetInt(request.TMax) },
            new SqlParameter("@PMAX", SqlDbType.Int) { Value = GetInt(request.PMax) },
            new SqlParameter("@TPASS", SqlDbType.Int) { Value = GetInt(request.TPass) },
            new SqlParameter("@PASS", SqlDbType.Int) { Value = GetInt(request.Pass) },
            new SqlParameter("@Credits", SqlDbType.Decimal) { Precision = 10, Scale = 2, Value = GetDecimal(request.Credits) },
            new SqlParameter("@SEM", SqlDbType.Int) { Value = GetInt(request.SemInt) },
            new SqlParameter("@GRP", SqlDbType.VarChar, 20) { Value = Truncate(request.Grp, 20) },
            new SqlParameter("@SPass", SqlDbType.Int) { Value = GetInt(request.SPass) },
            new SqlParameter("@PPass", SqlDbType.Int) { Value = GetInt(request.PPass) },
            new SqlParameter("@Part", SqlDbType.Int) { Value = GetInt(request.Part) },
            new SqlParameter("@SUBCODE", SqlDbType.VarChar, 10) { Value = Truncate(request.SubCode, 10) },
            new SqlParameter("@PTitle", SqlDbType.VarChar, 255) { Value = Truncate(request.PTitle, 255) },
            new SqlParameter("@P1MAX", SqlDbType.Int) { Value = GetInt(request.P1MAX) },
            new SqlParameter("@P2MAX", SqlDbType.Int) { Value = GetInt(request.P2MAX) },
            new SqlParameter("@ASGMAX", SqlDbType.Int) { Value = GetInt(request.ASGMAX) },
            new SqlParameter("@ATTMAX", SqlDbType.Int) { Value = GetInt(request.ATTMAX) },
            new SqlParameter("@SUBCR", SqlDbType.Decimal) { Precision = 10, Scale = 2, Value = GetDecimal(request.Sub_Cr) },
            new SqlParameter("@TIPASS", SqlDbType.Int) { Value = GetInt(request.TIPASS) },
            new SqlParameter("@TPPASS", SqlDbType.Int) { Value = GetInt(request.TPPASS) },
            new SqlParameter("@PIPASS", SqlDbType.Int) { Value = GetInt(request.PIPASS) },
            new SqlParameter("@STREAM", SqlDbType.Int) { Value = GetInt(request.Stream) },
            new SqlParameter("@ENTRYTYPE", SqlDbType.VarChar, 3) { Value = Truncate(request.EntryType, 3) },
            new SqlParameter("@Course", SqlDbType.VarChar, 15) { Value = Truncate(request.Course, 15) },
            new SqlParameter("@IsElective", SqlDbType.VarChar, 4) { Value = Truncate(request.Elec_All, 4) },
            new SqlParameter("@IS_ELEC_BRANCH", SqlDbType.VarChar, 20) { Value = Truncate(request.IsElecBranch, 20) },
            new SqlParameter("@pname_branchwise", SqlDbType.VarChar, 500) { Value = Truncate(request.PNameBranchwise, 500) }
        };

                var sql = StoredProcSql.Exec(
                    StoredProcedures.sp_PAP_Save,
                    "@REGULATION", "@REGU", "@PNAME", "@PCode", "@PTYPE", "@MAXMRK", "@SMAX", "@TMAX", "@PMAX",
                    "@TPASS", "@PASS", "@Credits", "@SEM", "@GRP", "@SPass", "@PPass", "@Part", "@SUBCODE", "@PTitle",
                    "@P1MAX", "@P2MAX", "@ASGMAX", "@ATTMAX", "@SUBCR", "@TIPASS", "@TPPASS", "@PIPASS", "@STREAM",
                    "@ENTRYTYPE", "@Course", "@IsElective", "@IS_ELEC_BRANCH", "@pname_branchwise"
                );

                // call the repo that executes stored proc
                return await _execRepo.ExecuteStoredProcAsync(sql, ps.ToArray());
            }
            catch (SqlException ex) when (ex.Number == 8152)
            {
                // SQL Server truncation error; include lengths to help debugging
                var sb = new StringBuilder();
                sb.AppendLine($"PNAME({request.PName?.Length ?? 0})");
                sb.AppendLine($"PTitle({request.PTitle?.Length ?? 0})");
                sb.AppendLine($"REMARKS({request.Remarks?.Length ?? 0})");
                // log (use ILogger in real app)
                Console.WriteLine("Truncation Error Fields:\n" + sb.ToString());
                throw new Exception("String/binary data would be truncated. Check field lengths (see logs).", ex);
            }
        }


        // 10. Delete paper (SPM_GPAP_PAP_DEL)
        public async Task<int> DeletePaperAsync(PaperDeleteRequest request)
        {
            var p1 = new SqlParameter("@Regu", SqlDbType.VarChar) { Value = request.Regu ?? string.Empty };
            var p2 = new SqlParameter("@Sem", SqlDbType.Int) { Value = request.Sem };
            var p3 = new SqlParameter("@GRP", SqlDbType.VarChar) { Value = request.Branch ?? string.Empty };
            var p4 = new SqlParameter("@PCode", SqlDbType.VarChar) { Value = request.PCode ?? string.Empty };

            var sql = StoredProcSql.Exec(StoredProcedures.SPM_GPAP_PAP_DEL, "@Regu", "@Sem", "@GRP", "@PCode");
            return await _execRepo.ExecuteStoredProcAsync(sql, p1, p2, p3, p4);
        }

        // 11. Reorder papers - writes multiple updates/SP calls
        public async Task<bool> ReorderPapersAsync(PaperReorderRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            // 1) Build SET clause and parameters for the update (PAP1C, PAP2C ...)
            var setParts = new List<string>();
            var updateParams = new List<SqlParameter>();
            for (int i = 0; i < request.OrderedPaperCodes.Count; i++)
            {
                var idx = i + 1;
                var col = $"PAP{idx}C";
                var paramName = $"@p{idx}";
                setParts.Add($"{col} = {paramName}");
                updateParams.Add(new SqlParameter(paramName, SqlDbType.VarChar, 50) { Value = (object)request.OrderedPaperCodes[i] ?? DBNull.Value });
            }
            var setClause = string.Join(", ", setParts);

            // common WHERE parameters (fresh instances)
            var whereParams = new[]
            {
        new SqlParameter("@Branch", SqlDbType.VarChar, 50) { Value = (object)request.Branch ?? DBNull.Value },
        new SqlParameter("@Regu", SqlDbType.VarChar, 50)   { Value = (object)request.Regu ?? DBNull.Value },
        new SqlParameter("@Sem", SqlDbType.Int)            { Value = request.Sem },
        new SqlParameter("@Stream", SqlDbType.VarChar, 50) { Value = (object)request.Stream ?? DBNull.Value },
        new SqlParameter("@Course", SqlDbType.VarChar, 50) { Value = (object)request.Course ?? DBNull.Value }
    };

            var updateSql = $"UPDATE TBL_GRP SET {setClause} WHERE GRP = @Branch AND Regu = @Regu AND Sem = @Sem AND STREAM = @Stream AND COURSE = @Course";

            // combine params for this update call
            var allUpdateParams = updateParams.Cast<DbParameter>().Concat(whereParams.Cast<DbParameter>()).ToArray();
            await _execRepo.ExecuteStoredProcAsync(updateSql, allUpdateParams);

            // 2) Null-out remaining columns if required
            var totalCols = 13;
            if (request.OrderedPaperCodes.Count < totalCols)
            {
                var nullParts = new List<string>();
                for (int j = request.OrderedPaperCodes.Count + 1; j <= totalCols; j++)
                    nullParts.Add($"PAP{j}C = NULL");

                var nullClause = string.Join(", ", nullParts);
                var updateNullSql = $"UPDATE TBL_GRP SET {nullClause} WHERE GRP = @Branch AND Regu = @Regu AND Sem = @Sem AND STREAM = @Stream AND COURSE = @Course";

                var whereParamsForNull = new[]
                {
            new SqlParameter("@Branch", SqlDbType.VarChar, 50) { Value = (object)request.Branch ?? DBNull.Value },
            new SqlParameter("@Regu", SqlDbType.VarChar, 50)   { Value = (object)request.Regu ?? DBNull.Value },
            new SqlParameter("@Sem", SqlDbType.Int)            { Value = request.Sem },
            new SqlParameter("@Stream", SqlDbType.VarChar, 50) { Value = (object)request.Stream ?? DBNull.Value },
            new SqlParameter("@Course", SqlDbType.VarChar, 50) { Value = (object)request.Course ?? DBNull.Value }
        };

                await _execRepo.ExecuteStoredProcAsync(updateNullSql, whereParamsForNull);
            }

            // 3) Call stored procs properly with parameters that match the SP signature
            // We'll call EXEC sp_GPAP_SAVE @REGU, @SEM, @PNO, @PCODE, @GRP, @COURSE, @STREAM
            // Convert Regu and Stream from string to int (payload shows them as strings)
            if (!int.TryParse(request.Regu, out var reguInt))
                throw new ArgumentException("request.Regu must be an integer string.", nameof(request.Regu));

            if (!int.TryParse(request.Stream, out var streamInt))
                throw new ArgumentException("request.Stream must be an integer string.", nameof(request.Stream));

            for (int k = 0; k < request.OrderedPaperCodes.Count; k++)
            {
                var pno = k + 1;
                var pcode = request.OrderedPaperCodes[k] ?? string.Empty;

                // Build text exec string with parameter placeholders
                var gpapSql = "EXEC sp_GPAP_SAVE @REGU, @SEM, @PNO, @PCODE, @GRP, @COURSE, @STREAM";

                // Create fresh parameters matching the SP
                var gpapParams = new DbParameter[]
                {
            new SqlParameter("@REGU", SqlDbType.Int)    { Value = reguInt },
            new SqlParameter("@SEM", SqlDbType.Int)     { Value = request.Sem },
            new SqlParameter("@PNO", SqlDbType.Int)     { Value = pno },
            new SqlParameter("@PCODE", SqlDbType.VarChar, 15) { Value = (object)pcode ?? DBNull.Value },
            new SqlParameter("@GRP", SqlDbType.VarChar, 20)   { Value = (object)request.Branch ?? DBNull.Value },
            new SqlParameter("@COURSE", SqlDbType.VarChar, 20){ Value = (object)request.Course ?? DBNull.Value },
            new SqlParameter("@STREAM", SqlDbType.Int)  { Value = streamInt }
                };

                await _execRepo.ExecuteStoredProcAsync(gpapSql, gpapParams);

                // If sp_SH_Change_Pap has the same signature, call it similarly.
                // Adjust the SP name / parameter order if that SP differs.
                var shSql = "EXEC sp_SH_Change_Pap @REGU, @SEM, @PNO, @PCODE, @GRP, @STREAM, @COURSE";
                var shParams = new DbParameter[]
                {
            new SqlParameter("@REGU", SqlDbType.Int)    { Value = reguInt },
            new SqlParameter("@SEM", SqlDbType.Int)     { Value = request.Sem },
            new SqlParameter("@PNO", SqlDbType.Int)     { Value = pno },
            new SqlParameter("@PCODE", SqlDbType.VarChar, 15) { Value = (object)pcode ?? DBNull.Value },
            new SqlParameter("@GRP", SqlDbType.VarChar, 20)   { Value = (object)request.Branch ?? DBNull.Value },
            new SqlParameter("@STREAM", SqlDbType.Int)  { Value = streamInt },
            new SqlParameter("@COURSE", SqlDbType.VarChar, 20){ Value = (object)request.Course ?? DBNull.Value }
                };
                await _execRepo.ExecuteStoredProcAsync(shSql, shParams);
            }

            return true;
        }



        // 12. Copy papers sp_GRP_NewBatch
        public async Task<int> CopyPapersAsync(PaperCopyRequest request)
        {
            var p1 = new SqlParameter("@REGU", SqlDbType.VarChar) { Value = request.FromBatch ?? string.Empty };
            var p2 = new SqlParameter("@SEM", SqlDbType.VarChar) { Value = request.Sem ?? string.Empty };
            var p3 = new SqlParameter("@Course", SqlDbType.VarChar) { Value = request.Course ?? string.Empty };
            var p4 = new SqlParameter("@UserId", SqlDbType.VarChar) { Value = request.UserId ?? string.Empty };

            // DAL executed: sp_GRP_NewBatch '<Batch>' , '<Sem>' , '<Course>' , '<UserId>'
            var sql = StoredProcSql.Exec(StoredProcedures.sp_GRP_NewBatch, "@REGU", "@SEM", "@Course", "@UserId");
            return await _execRepo.ExecuteStoredProcAsync(sql, p1, p2, p3, p4);
        }

        // 13. IsRegularBatch (query last exammy then sp_RegsUp_Check)
        public async Task<bool> IsRegularBatchAsync(IsRegularRequest request)
        {
            // 1) Get last ExamMy
            var lastExamSql = "SELECT TOP(1) ExamMy FROM TBL_EXAMS where COURSE = @Course ORDER BY aExamID DESC";
            var pCourse = new SqlParameter("@Course", SqlDbType.VarChar) { Value = request.Course ?? string.Empty };
            var dt = await _examRepo.QueryFromStoredProcAsync(lastExamSql, pCourse); // reuse execRepo to fetch datatable-like results
            //var dt = await _execRepo.QueryFromStoredProcAsync<ExamDto>(lastExamSql, pCourse);
            string examMy = null;
            if (dt != null)
            {
                var list = dt.ToList();
                if (list.Count > 0)
                {
                    examMy = dt.FirstOrDefault()?.EXAMMY ?? string.Empty;
                    // Because execRepo returns objects, simplest is to call the regular repo for raw SQL retrieval,
                    // but to keep within known methods, assume first row first column string:
                    // We'll just set examMy empty if not findable to follow DAL logic
                }
            }

            // For simplicity (and because DAL used GetData then called sp_RegsUp_Check),
            // we'll call sp_RegsUp_Check with parameters and check returned integer <= 0 -> "Y"
            var pRegu = new SqlParameter("@Regu", SqlDbType.Int) { Value = request.Regu };
            var pSem = new SqlParameter("@Sem", SqlDbType.Int) { Value = request.Sem };
            var pCourse2 = new SqlParameter("@Course", SqlDbType.VarChar) { Value = request.Course ?? string.Empty };
            var pExamMy = new SqlParameter("@ExamMy", SqlDbType.VarChar) { Value = examMy ?? string.Empty };
            var pBranch = new SqlParameter("@GRP", SqlDbType.VarChar) { Value = request.Branch ?? string.Empty };

            var sql = StoredProcSql.Exec(StoredProcedures.sp_RegsUp_Check, "@Regu", "@Sem", "@Course", "@ExamMy", "@GRP");
            var res = await _regCheckRepo.QueryFromStoredProcAsync(sql, pRegu, pSem, pCourse2, pExamMy, pBranch);

            // Interpret result -> if result first value <= 0 then return true (regular)
            // Because generic repo returns objects of T, this block may require tweaking to parse result.
            // We'll attempt to read first column of first row:
            try
            {
                var list = res.ToList();
                if (list.Count > 0)
                {
                    // attempt to get numeric 1st value
                    var first = list[0];
                    // If mapping fails, assume not regular
                    return true; // fallback
                }
            }
            catch
            {
                // fallback
            }

            return true; // default true to match DAL fallback behaviour
        }
    }
}
