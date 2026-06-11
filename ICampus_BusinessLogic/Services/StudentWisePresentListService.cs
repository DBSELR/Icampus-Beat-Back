using ICampus_BusinessLogic.Interfaces;
using ICampus_DataAccessLayer.Helpers;
using ICampus_DataAccessLayer.Interfaces;
using ICampus_Models.Requests;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Services
{
    public class StudentWisePresentListService : IStudentWisePresentListService
    {
        private readonly IGenericRepository<object> _repo;

        public StudentWisePresentListService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Load semester list for ddlSemester on Page_Load
        /// Inline SQL confirmed from DLL IL: BusinessAccessLayer.dll BAL_StudentWiseMasterCreation::Load_Sems_Data
        /// 3 params: ExamMY, Course, Regulation — all varchar (all quoted in original)
        /// Note: uses all 3 session values (ExamMY + Course + Regulation) unlike most other modules
        /// </summary>
        public async Task<IEnumerable<object>> GetSemestersAsync(string course, string regulation, string examMY)
        {
            var sql = @"SELECT DISTINCT SH.SEM
                        FROM TBL_SH SH
                        WHERE SH.EXAMMY     = @ExamMY
                          AND SH.course     = @Course
                          AND SH.Regulation = @Regulation
                        ORDER BY SEM";

            var parameters = new[]
            {
                new SqlParameter("@ExamMY",     SqlDbType.VarChar, 20) { Value = examMY     ?? string.Empty },
                new SqlParameter("@Course",     SqlDbType.VarChar, 30) { Value = course     ?? string.Empty },
                new SqlParameter("@Regulation", SqlDbType.VarChar, 10) { Value = regulation ?? string.Empty }
            };

            var raw = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return raw ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Generate Subject Wise Present List report
        /// SP: SP_Pcode_Presentlist
        /// Parameters confirmed from DLL IL (DataAccessLayer.dll, US heap US[0xc0c1]):
        ///   Exec: "SP_Pcode_Presentlist 'Course','Regulation','ExamMY', 'Sem', 'Regsup'"
        ///   All 5 parameters are varchar (all quoted in string concat)
        ///   MR[522]=get_Course, MR[521]=get_Regulation, MR[523]=get_ExamMY,
        ///   MR[512]=get_Sem, MR[524]=get_Regsup
        /// </summary>
        public async Task<IEnumerable<object>> GetReportDataAsync(StudentWisePresentListRequest request)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.SP_Pcode_Presentlist,
                "@Course", "@Regulation", "@ExamMY", "@Sem", "@Regsup");

            var parameters = new[]
            {
                new SqlParameter("@Course",     SqlDbType.VarChar, 30) { Value = request.Course     ?? string.Empty },
                new SqlParameter("@Regulation", SqlDbType.VarChar, 10) { Value = request.Regulation ?? string.Empty },
                new SqlParameter("@ExamMY",     SqlDbType.VarChar, 20) { Value = request.ExamMY     ?? string.Empty },
                new SqlParameter("@Sem",        SqlDbType.VarChar, 5)  { Value = request.Sem        ?? string.Empty },
                new SqlParameter("@Regsup",     SqlDbType.VarChar, 10) { Value = request.Regsup     ?? string.Empty }  // "REG" or "Sup"
            };

            var raw = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return raw ?? Enumerable.Empty<object>();
        }
    }
}
