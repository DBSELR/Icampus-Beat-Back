using ICampus_BusinessLogic.Interfaces;
using ICampus_DataAccessLayer.Helpers;
using ICampus_DataAccessLayer.Interfaces;
using ICampus_Models.DTOs;
using ICampus_Models.Requests;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data; 
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Services
{
    public class CgpService : ICgpService
    {
        private readonly IGenericRepository<CourseDto> _courseRepo;
        private readonly IGenericRepository<CourseDto> _genericRepoForDto;

        public CgpService(IGenericRepository<CourseDto> courseRepo)
        {
            _courseRepo = courseRepo;
            _genericRepoForDto = courseRepo;
        }

        // 1) Grid load
        public async Task<IEnumerable<CourseDto>> LoadGridAsync(string type, string searchString, string regulation)
        {
            var pType = new SqlParameter("@TYPE", SqlDbType.VarChar) { Value = type ?? string.Empty };
            var pString = new SqlParameter("@STRING", SqlDbType.VarChar) { Value = searchString ?? string.Empty };
            var pReg = new SqlParameter("@REGULATION", SqlDbType.VarChar) { Value = regulation ?? string.Empty };

            var sql = StoredProcSql.Exec(StoredProcedures.PROC_LOAD_REGU_BATCH_COURSE, "@TYPE", "@STRING", "@REGULATION");
            return await _courseRepo.QueryFromStoredProcAsync(sql, pType, pString, pReg);
        }

        public async Task<IEnumerable<CourseDto>> SearchCgAsync(string regu, string course, string grp)
        {
            var pRegu = new SqlParameter("@REGU", SqlDbType.VarChar) { Value = regu ?? string.Empty };
            var pCourse = new SqlParameter("@COURSE", SqlDbType.VarChar) { Value = course ?? string.Empty };
            var pGrp = new SqlParameter("@GRP", SqlDbType.VarChar) { Value = grp ?? string.Empty };

            var sql = StoredProcSql.Exec(StoredProcedures.PROC_SEARCH_COURSE, "@REGU", "@COURSE", "@GRP");
            return await _courseRepo.QueryFromStoredProcAsync(sql, pRegu, pCourse, pGrp);
        }

        // 2) Autocomplete lists
        private async Task<IEnumerable<string>> LoadSimpleListAsync(string type, string searchValue, string regulation = "")
        {
            var pType = new SqlParameter("@TYPE", SqlDbType.VarChar) { Value = type };
            var pString = new SqlParameter("@STRING", SqlDbType.VarChar) { Value = searchValue ?? string.Empty };
            var pReg = new SqlParameter("@REGULATION", SqlDbType.VarChar) { Value = regulation ?? string.Empty };

            var sql = StoredProcSql.Exec(StoredProcedures.PROC_LOAD_REGU_BATCH_COURSE, "@TYPE", "@STRING", "@REGULATION");
            var rows = await _courseRepo.QueryFromStoredProcAsync(sql, pType, pString, pReg);

            return type switch
            {
                "REGULATION" => rows.Select(r => r.REGULATION).Distinct(),
                "REGU" => rows.Select(r => r.BATCH).Distinct(),
                "COURSE" => rows.Select(r => r.COURSE).Distinct(),
                "DEGREE" => rows.Select(r => r.DEGREE).Distinct(),
                "grp" => rows.Select(r => r.GRP).Distinct(),
                "gsub" => rows.Select(r => r.GSUB).Distinct(),
                _ => Enumerable.Empty<string>()
            };
        }

        public Task<IEnumerable<string>> SearchReguAsync(string prefix) =>
            LoadSimpleListAsync("REGULATION", prefix);

        public Task<IEnumerable<string>> SearchBatchAsync(string prefix)
        {
            var input = prefix ?? string.Empty;
            string search = input.Length >= 4 && input.Contains("-") ? input.Substring(2, 2) : input;
            return LoadSimpleListAsync("REGU", search);
        }

        public Task<IEnumerable<string>> SearchCourseAsync(string prefix) =>
            LoadSimpleListAsync("COURSE", prefix);

        public Task<IEnumerable<string>> SearchCourseNameAsync(string prefix) =>
            LoadSimpleListAsync("DEGREE", prefix);

        public Task<IEnumerable<string>> SearchGrpAsync(string prefix) =>
            LoadSimpleListAsync("grp", prefix);

        public Task<IEnumerable<string>> SearchGrpNameAsync(string prefix) =>
            LoadSimpleListAsync("gsub", prefix);

        // 3) Save
        public async Task<int> SaveCourseAsync(SaveCourseRequest request)
        {
            var pReg = new SqlParameter("@REGULATION", SqlDbType.VarChar, 10) { Value = request.Regulation ?? string.Empty };
            var pBatch = new SqlParameter("@BATCH", SqlDbType.VarChar, 2)
            {
                Value = request.Batch?.Length == 2
                    ? request.Batch
                    : (request.Batch != null && request.Batch.Length >= 2
                        ? request.Batch.Substring(0, 2)
                        : request.Batch ?? string.Empty)
            };
            var pCourse = new SqlParameter("@COURSE", SqlDbType.VarChar, 25) { Value = request.Course ?? string.Empty };
            var pCourseName = new SqlParameter("@COURSENAME", SqlDbType.VarChar, 225) { Value = request.CourseName ?? string.Empty };
            var pGrp = new SqlParameter("@GRP", SqlDbType.VarChar, 25) { Value = request.Grp ?? string.Empty };
            var pGrpName = new SqlParameter("@GRPNAME", SqlDbType.VarChar, 225) { Value = request.GrpName ?? string.Empty };
            var pMaxSem = new SqlParameter("@MaxSEM", SqlDbType.Int) { Value = request.MaxSem };
            var pMaxStreams = new SqlParameter("@MaxSTREAMs", SqlDbType.VarChar, 2) { Value = request.MaxStreams.ToString() };
            var pGrpOrder = new SqlParameter("@GRP_ORDER", SqlDbType.TinyInt) { Value = request.GrpOrder };

            var sql = StoredProcSql.Exec(StoredProcedures.SP_GROUPMASTER_SAVE,
                                         "@REGULATION", "@BATCH", "@COURSE", "@COURSENAME",
                                         "@GRP", "@GRPNAME", "@MaxSEM", "@MaxSTREAMs", "@GRP_ORDER");

            return await _courseRepo.ExecuteStoredProcAsync(sql,
                pReg, pBatch, pCourse, pCourseName, pGrp, pGrpName, pMaxSem, pMaxStreams, pGrpOrder);
        }

        // 4) Delete
        public async Task<int> DeleteCourseAsync(DeleteCourseRequest request)
        {
            var pBatch = new SqlParameter("@BATCH", SqlDbType.VarChar, 20) { Value = request.Batch ?? string.Empty };
            var pCourse = new SqlParameter("@COURSE", SqlDbType.VarChar, 20) { Value = request.Course ?? string.Empty };
            var pGrp = new SqlParameter("@GRP", SqlDbType.VarChar, 20) { Value = request.Grp ?? string.Empty };
            var pCName = new SqlParameter("@CNAME", SqlDbType.VarChar, 120) { Value = request.CourseName ?? string.Empty };
            var pGrpName = new SqlParameter("@GRPNAME", SqlDbType.VarChar, 150) { Value = request.GrpName ?? string.Empty };
            var pGrpOrder = new SqlParameter("@GRP_ORDER", SqlDbType.TinyInt) { Value = request.GrpOrder };
            var pType = new SqlParameter("@TYPE", SqlDbType.VarChar, 20) { Value = "DELETE" };

            var sql = StoredProcSql.Exec(StoredProcedures.PROC_DELETE_COURSE_GRP,
                                         "@BATCH", "@COURSE", "@GRP", "@CNAME",
                                         "@GRPNAME", "@GRP_ORDER", "@TYPE");

            return await _courseRepo.ExecuteStoredProcAsync(sql,
                pBatch, pCourse, pGrp, pCName, pGrpName, pGrpOrder, pType);
        }

        // 5) Copy group
        public async Task<int> CopyGroupAsync(CopyGroupRequest request)
        {
            var pReg = new SqlParameter("@Regulation", SqlDbType.VarChar, 20) { Value = request.Regulation ?? string.Empty };
            var pRegu = new SqlParameter("@REGU", SqlDbType.Char, 2) { Value = request.ToBatch ?? string.Empty };
            var pPregu = new SqlParameter("@PREGU", SqlDbType.Char, 2) { Value = request.FromBatch ?? string.Empty };
            var pCourse = new SqlParameter("@COURSE", SqlDbType.VarChar, 20) { Value = request.Course ?? string.Empty };

            var sql = StoredProcSql.Exec(StoredProcedures.PROC_COPY_GRP_DATA,
                                         "@Regulation", "@REGU", "@PREGU", "@COURSE");

            return await _courseRepo.ExecuteStoredProcAsync(sql, pReg, pRegu, pPregu, pCourse);
        }

        // 6) Check & copy
        public async Task<bool> CheckAndCopyAsync(CopyGroupRequest request)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.PROC_LOAD_REGU_BATCH_COURSE,
                                         "@TYPE", "@STRING", "@REGULATION");

            var pType = new SqlParameter("@TYPE", "COURSE");
            var pString = new SqlParameter("@STRING", request.FromBatch ?? string.Empty);
            var pReg = new SqlParameter("@REGULATION", $"'{request.Regulation}'");

            var rows = await _courseRepo.QueryFromStoredProcAsync(sql, pType, pString, pReg);
            var exists = rows != null && rows.Any();

            if (exists) return true;

            var copyReq = new CopyGroupRequest
            {
                Course = request.Course,
                FromBatch = request.FromBatch,
                ToBatch = request.ToBatch,
                Regulation = request.Regulation
            };

            var res = await CopyGroupAsync(copyReq);
            return res > 0;
        }
    }
}
