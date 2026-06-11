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
    public class ScIssueService : IScIssueService
    {
        private readonly IGenericRepository<object> _repo;

        public ScIssueService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Auto-fill student info on RegNo entry — txtRegNo_TextChanged / loadingSC_Issue
        /// SQL: SELECT SNAME, COURSE, GRP, PHOTO FROM TBL_STDDATA WHERE REGNO=@RegNo
        /// Confirmed: DataAccessLayer.dll UTF-16LE US heap 0x28fd0:
        ///   "sp_getdetails_student '老厃 SELECT SNAME, COURSE, GRP, PHOTO FROM TBL_STDDATA WHERE REGNO = '@"
        /// </summary>
        public async Task<IEnumerable<object>> GetStudentInfoAsync(string regNo)
        {
            var sql = "SELECT * FROM TBL_STDDATA WHERE REGNO=@RegNo";

            var parameters = new[]
            {
                new SqlParameter("@RegNo", SqlDbType.VarChar, 20) { Value = regNo ?? string.Empty }
            };

            var result = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Issue Study Certificate (btnSCIssue_Click → SC_issue)
        /// SP: Proc_SC_Issue (CommandType.StoredProcedure)
        /// Params (14): @RegNo, @Regulation, @Section, @SName, @FName, @MName,
        ///              @Conduct, @DOB, @Gender, @Caste, @Email, @Mobile, @AadhaarNo, @Religion
        /// Crystal: Studycertificate_JBIET.rpt
        /// Confirmed: DataAccessLayer.dll UTF-16LE US heap 0x28f8c: "Proc_SC_Issue  '℁"
        ///   adjacent to "Proc_TC_Issue" and "sp_getdetails_student" in sequence
        /// </summary>
        public async Task<int> IssueScAsync(
            string regNo, string regulation, string section,
            string sName, string fName, string mName,
            string conduct, string dob, string gender, string caste,
            string email, string mobile, string aadhaarNo, string religion)
        {
            // SP Proc_SC_Issue accepts exactly 2 params: @Regno, @Conduct
            var sql = StoredProcSql.Exec(StoredProcedures.Proc_SC_Issue,
                "@Regno", "@Conduct");

            var parameters = new object[]
            {
                new SqlParameter("@Regno",   SqlDbType.VarChar, 50)  { Value = regNo   ?? string.Empty },
                new SqlParameter("@Conduct", SqlDbType.VarChar, 250) { Value = conduct ?? string.Empty }
            };

            return await _repo.ExecuteStoredProcAsync(sql, parameters);
        }
    }
}
