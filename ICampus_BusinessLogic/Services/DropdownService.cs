using ICampus_BusinessLogic.Interfaces;
using ICampus_DataAccessLayer.Interfaces;
using ICampus_DataAccessLayer.Helpers;
using ICampus_Models.DTOs;
using ICampus_Models.Requests;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ICampus_BusinessLogic.Services
{
    public class DropdownService : IDropdownService
    {
        private readonly IGenericRepository<RegulationDto> _regRepo;
        private readonly IGenericRepository<CourseListDto> _courseRepo;
        private readonly IGenericRepository<ExamMYDto> _examRepo;

        public DropdownService(
            IGenericRepository<RegulationDto> regRepo,
            IGenericRepository<CourseListDto> courseRepo,
            IGenericRepository<ExamMYDto> examRepo)
        {
            _regRepo = regRepo;
            _courseRepo = courseRepo;
            _examRepo = examRepo;
        }

        public async Task<IEnumerable<RegulationDto>> GetRegulationsAsync()
        {
            var sql = "SELECT DISTINCT REGULATION FROM TBL_COURSE WHERE REGULATION IS NOT NULL";
            return await _regRepo.QueryFromStoredProcAsync(sql);
        }

        public async Task<IEnumerable<CourseListDto>> GetCoursesAsync(string regulation)
        {
            var pReg = new SqlParameter("@REGULATION", SqlDbType.VarChar) { Value = regulation ?? string.Empty };
            var sql = StoredProcSql.Exec(StoredProcedures.SPM_COURSE_LIST, "@REGULATION");
            return await _courseRepo.QueryFromStoredProcAsync(sql, pReg);
        }

        public async Task<IEnumerable<ExamMYDto>> GetExamMYAsync(string regulation, string course)
        {
            var pReg = new SqlParameter("@Regulation", SqlDbType.VarChar) { Value = regulation ?? string.Empty };
            var pCourse = new SqlParameter("@Course", SqlDbType.VarChar) { Value = course ?? string.Empty };
            var sql = StoredProcSql.Exec(StoredProcedures.SPM_EXAMS_ExamMY_Load, "@Regulation", "@Course");
            return await _examRepo.QueryFromStoredProcAsync(sql, pReg, pCourse);
        }

        public async Task<int> SaveUserSelectionAsync(SaveUserSelectionRequest request, string userId)
        {
            var pReg = new SqlParameter("@REGULATION", SqlDbType.VarChar) { Value = request.Regulation };
            var pCourse = new SqlParameter("@COURSE", SqlDbType.VarChar) { Value = request.Course };
            var pExamMY = new SqlParameter("@EXAMMY", SqlDbType.VarChar) { Value = request.ExamMY };
            var pUserId = new SqlParameter("@USERID", SqlDbType.VarChar) { Value = userId };

            var sql = StoredProcSql.Exec(StoredProcedures.SPM_Course_ExamMY_to_User,
                                         "@REGULATION", "@COURSE", "@EXAMMY", "@USERID");

            return await _regRepo.ExecuteStoredProcAsync(sql, pReg, pCourse, pExamMY, pUserId);
        }
    }
}
