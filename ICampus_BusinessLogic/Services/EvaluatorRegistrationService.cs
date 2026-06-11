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
    public class EvaluatorRegistrationService : IEvaluatorRegistrationService
    {
        private readonly IGenericRepository<object> _repo;

        public EvaluatorRegistrationService(IGenericRepository<object> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Loads evaluator/scrutinizer grid (gvUsers in ASPX).
        /// SP: SP_EVALUATOR_LOAD @UserGroup
        /// Returns: EID, USERID, NAME, DESIGNATION, DEPTARTMENT, USERGROUP, MOBILE, EMAIL, COLLEGE, ISACTIVE
        /// Confirmed from App_Web_xplim0cm.dll ASPX grid column bindings.
        /// </summary>
        public async Task<IEnumerable<object>> GetEvaluatorListAsync(string userGroup)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.SP_EVALUATOR_LOAD, "@UserGroup");
            var parameters = new[]
            {
                new SqlParameter("@UserGroup", SqlDbType.VarChar, 50) { Value = userGroup ?? string.Empty }
            };
            var raw = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return raw ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Returns active evaluators dropdown.
        /// SP: SP_OE_EVALUATOR_LIST (no params)
        /// Confirmed inline SQL: Select USERID,USERID+'_'+NAME as NAME from tbl_Eval_Registrations
        ///   where ISACTIVE='1' and UserGroup='Evaluator' order by NAME
        /// </summary>
        public async Task<IEnumerable<object>> GetEvaluatorDropdownAsync()
        {
            var sql = StoredProcSql.Exec(StoredProcedures.SP_OE_EVALUATOR_LIST);
            var raw = await _repo.QueryFromStoredProcAsync(sql);
            return raw ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Loads full details for one evaluator (for edit form).
        /// SP: SP_EVALUATOR_LOAD_USER_DETAILS @UserID
        /// Confirmed from DataAccessLayer.dll UTF-16LE string analysis.
        /// </summary>
        public async Task<IEnumerable<object>> GetUserDetailsAsync(string userId)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.SP_EVALUATOR_LOAD_USER_DETAILS, "@UserID");
            var parameters = new[]
            {
                new SqlParameter("@UserID", SqlDbType.VarChar, 100) { Value = userId ?? string.Empty }
            };
            var raw = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return raw ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Auto-generates next UserID for a group.
        /// Inline SQL: select isnull(max(right(UserID,3)),0)+1 as UserID
        ///             from tbl_Eval_Registrations where USERGROUP='...'
        /// Confirmed from DataAccessLayer.dll.
        /// </summary>
        public async Task<IEnumerable<object>> GetNextUserIdAsync(string userGroup)
        {
            var sql = "SELECT ISNULL(MAX(RIGHT(UserID,3)),0)+1 AS NextUserID FROM tbl_Eval_Registrations WHERE USERGROUP = @UserGroup";
            var parameters = new[]
            {
                new SqlParameter("@UserGroup", SqlDbType.VarChar, 50) { Value = userGroup ?? string.Empty }
            };
            var raw = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return raw ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Loads papers already assigned to an evaluator (populates lstPap pre-selection).
        /// Inline SQL: Select PCode, PCode+'_'+PNAME as PNAME from tbl_Eval_UserPapers where UserId='...'
        /// Confirmed from DataAccessLayer.dll UTF-16LE analysis.
        /// </summary>
        public async Task<IEnumerable<object>> GetUserPapersAsync(string userId)
        {
            var sql = "SELECT PCode, PCode+'_'+PNAME AS PNAME FROM tbl_Eval_UserPapers WHERE UserId = @UserId";
            var parameters = new[]
            {
                new SqlParameter("@UserId", SqlDbType.VarChar, 100) { Value = userId ?? string.Empty }
            };
            var raw = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return raw ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Loads department/branch dropdown (ddlDepartment in ASPX).
        /// Inline SQL: select distinct GRP from tbl_sh
        ///   where Regulation='...' and Course='...' and EXAMMY='...' order by GRP
        /// Confirmed from DataAccessLayer.dll UTF-16LE analysis (near SP_EVALUATOR_LOAD).
        /// </summary>
        public async Task<IEnumerable<object>> GetDepartmentsAsync(string regulation, string course, string examMY)
        {
            var sql = @"SELECT DISTINCT GRP FROM tbl_sh
                        WHERE Regulation = @Regulation AND Course = @Course AND EXAMMY = @ExamMY
                        ORDER BY GRP";
            var parameters = new[]
            {
                new SqlParameter("@Regulation", SqlDbType.VarChar, 100) { Value = regulation ?? string.Empty },
                new SqlParameter("@Course",     SqlDbType.VarChar, 100) { Value = course     ?? string.Empty },
                new SqlParameter("@ExamMY",     SqlDbType.VarChar, 50)  { Value = examMY     ?? string.Empty }
            };
            var raw = await _repo.QueryFromStoredProcAsync(sql, (object[])parameters);
            return raw ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Saves evaluator profile + paper assignments.
        /// Step 1: SP_EVALUATOR_REGISTRATIONS — saves profile to tbl_Eval_Registrations
        ///   params: @UserID, @Name, @Designation, @Department, @UserGroup, @Mobile, @Email, @College, @IsActive, @Sem
        /// Step 2: Sp_Eval_Save_UserPapers @UserId, @PapCode — per paper loop (to tbl_Eval_UserPapers)
        /// Confirmed from App_Web_xplim0cm.dll btnSaveCourse_Click flow + DataAccessLayer.dll.
        /// </summary>
        public async Task<int> SaveEvaluatorAsync(SaveEvaluatorRequest req)
        {
            // Step 1: Save evaluator profile.
            // SP params: @USERID, @NAME, @DESIGNATION, @DEPTARTMENT, @USERGROUP,
            //            @MOBILE, @EMAIL, @COLLEGE, @PWD, @ISACTIVE
            var saveSql = StoredProcSql.ExecNamed(
                StoredProcedures.SP_EVALUATOR_REGISTRATIONS,
                "@USERID", "@NAME", "@DESIGNATION", "@DEPTARTMENT",
                "@USERGROUP", "@MOBILE", "@EMAIL", "@COLLEGE", "@PWD", "@ISACTIVE");

            var saveParams = new[]
            {
                new SqlParameter("@USERID",      SqlDbType.VarChar, 100) { Value = req.UserID      ?? string.Empty },
                new SqlParameter("@NAME",        SqlDbType.VarChar, 200) { Value = req.Name        ?? string.Empty },
                new SqlParameter("@DESIGNATION", SqlDbType.VarChar, 100) { Value = req.Designation ?? string.Empty },
                new SqlParameter("@DEPTARTMENT", SqlDbType.VarChar, 100) { Value = req.Department  ?? string.Empty },
                new SqlParameter("@USERGROUP",   SqlDbType.VarChar, 50)  { Value = req.UserGroup   ?? string.Empty },
                new SqlParameter("@MOBILE",      SqlDbType.VarChar, 20)  { Value = req.Mobile      ?? string.Empty },
                new SqlParameter("@EMAIL",       SqlDbType.VarChar, 200) { Value = req.Email       ?? string.Empty },
                new SqlParameter("@COLLEGE",     SqlDbType.VarChar, 200) { Value = req.College     ?? string.Empty },
                new SqlParameter("@PWD",         SqlDbType.VarChar, 50)  { Value = string.Empty },
                new SqlParameter("@ISACTIVE",    SqlDbType.Char,    1)   { Value = req.IsActive    ?? "1"          }
            };

            var rows = await _repo.ExecuteStoredProcAsync(saveSql, (object[])saveParams);

            // Step 2: Assign papers — Sp_Eval_Save_UserPapers
            // SP params: @Regulation, @Course, @ExamMY, @PCode, @PName, @TEMPCODE, @UserID, @SEM
            // papCodes are composite: "PCode@TempCode@PName@Sem"
            if (req.PapCodes != null)
            {
                foreach (var composite in req.PapCodes)
                {
                    var parts    = (composite ?? string.Empty).Split('@');
                    var pCode    = parts.Length > 0 ? parts[0] : string.Empty;
                    var tempCode = parts.Length > 1 ? parts[1] : pCode;
                    var pName    = parts.Length > 2 ? parts[2] : string.Empty;

                    var paperSql = StoredProcSql.ExecNamed(
                        StoredProcedures.Sp_Eval_Save_UserPapers,
                        "@Regulation", "@Course", "@ExamMY", "@PCode", "@PName", "@TEMPCODE", "@UserID", "@SEM");

                    var paperParams = new[]
                    {
                        new SqlParameter("@Regulation", SqlDbType.VarChar, 100) { Value = req.Regulation ?? string.Empty },
                        new SqlParameter("@Course",     SqlDbType.VarChar, 100) { Value = req.Course     ?? string.Empty },
                        new SqlParameter("@ExamMY",     SqlDbType.VarChar, 50)  { Value = req.ExamMY     ?? string.Empty },
                        new SqlParameter("@PCode",      SqlDbType.VarChar, 100) { Value = pCode                         },
                        new SqlParameter("@PName",      SqlDbType.VarChar, 200) { Value = pName                         },
                        new SqlParameter("@TEMPCODE",   SqlDbType.VarChar, 100) { Value = tempCode                      },
                        new SqlParameter("@UserID",     SqlDbType.VarChar, 100) { Value = req.UserID     ?? string.Empty },
                        new SqlParameter("@SEM",        SqlDbType.VarChar, 50)  { Value = req.Sem        ?? string.Empty }
                    };

                    await _repo.ExecuteStoredProcAsync(paperSql, (object[])paperParams);
                }
            }

            return rows;
        }
    }
}
