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
    public class TcIssueService : ITcIssueService
    {
        private readonly IGenericRepository<object> _repo;

        public TcIssueService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Auto-fill student info on RegNo entry — txtRegNo_TextChanged / loadingTC_Issue
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
        /// Diagnostic: find sample REGNOs from TBL_STDDATA for testing
        /// </summary>
        public async Task<IEnumerable<object>> GetSampleRegnosAsync()
        {
            var sql = "SELECT TOP 10 REGNO, SNAME, COURSE FROM TBL_STDDATA ORDER BY REGNO";
            var result = await _repo.QueryFromStoredProcAsync(sql, new object[0]);
            return result ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Issue Transfer Certificate (ddlTCissue_Click — TC_issue / TC_issue_Noresult)
        /// SP: Proc_TC_Issue (CommandType.StoredProcedure)
        /// Params (20): @RegNo, @Regulation, @Section, @SName, @FName, @MName,
        ///              @DOB, @Gender, @Caste, @Email, @Mobile, @AadhaarNo,
        ///              @MOLE1, @MOLE2, @Religion, @DateofAdmitted,
        ///              @Scholar, @CourseComplete, @HigherEdu, @DateofLeave
        /// Crystal: Transfercertificate_JBIET.rpt
        /// Confirmed: DataAccessLayer.dll UTF-16LE US heap 0x28fae: "Proc_TC_Issue  '⼁"
        /// </summary>
        public async Task<int> IssueTcAsync(
            string regNo, string regulation, string section,
            string sName, string fName, string mName,
            string dob, string gender, string caste,
            string email, string mobile, string aadhaarNo,
            string mole1, string mole2, string religion,
            string dateofAdmitted, string scholar,
            string courseComplete, string higherEdu, string dateofLeave)
        {
            // SP Proc_TC_Issue accepts exactly 5 params: @Regno, @Is_Scholar, @Course_Complete, @Higher_Edu, @dateofleave
            var sql = StoredProcSql.Exec(StoredProcedures.Proc_TC_Issue,
                "@Regno", "@Is_Scholar", "@Course_Complete", "@Higher_Edu", "@dateofleave");

            var parameters = new object[]
            {
                new SqlParameter("@Regno",           SqlDbType.VarChar, 50)  { Value = regNo          ?? string.Empty },
                new SqlParameter("@Is_Scholar",      SqlDbType.VarChar, 50)  { Value = scholar        ?? string.Empty },
                new SqlParameter("@Course_Complete", SqlDbType.VarChar, 100) { Value = courseComplete  ?? string.Empty },
                new SqlParameter("@Higher_Edu",      SqlDbType.VarChar, 100) { Value = higherEdu      ?? string.Empty },
                new SqlParameter("@dateofleave",     SqlDbType.VarChar, 100) { Value = dateofLeave    ?? string.Empty }
            };

            return await _repo.ExecuteStoredProcAsync(sql, parameters);
        }
    }
}
