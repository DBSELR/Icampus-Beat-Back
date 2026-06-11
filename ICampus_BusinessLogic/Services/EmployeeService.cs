using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using ICampus_DataAccessLayer.Helpers;
using ICampus_DataAccessLayer.Interfaces;
using ICampus_Models.DTOs;
using ICampus_Models.Requests;

public class EmployeeService : IEmployeeService
{
    private readonly IGenericRepository<EmployeeDto> _empRepo;        // for SELECT -> EmployeeDto
    private readonly IGenericRepository<object> _execRepo;            // for ExecuteStoredProcAsync -> returns int
    private readonly IGenericRepository<UserGroupDto> _userGroupRepo; // for usergroups query
    private readonly IGenericRepository<EmployeeDto> _facultyRepo;    // for SP_Facultydata -> map to EmployeeDto
    private readonly IGenericRepository<EmpCheckDto> _empCheckRepo;   // for count check (Check_UserId)

    public EmployeeService(
        IGenericRepository<EmployeeDto> empRepo,
        IGenericRepository<object> execRepo,
        IGenericRepository<UserGroupDto> userGroupRepo,
        IGenericRepository<EmployeeDto> facultyRepo,
        IGenericRepository<EmpCheckDto> empCheckRepo)
    {
        _empRepo = empRepo ?? throw new ArgumentNullException(nameof(empRepo));
        _execRepo = execRepo ?? throw new ArgumentNullException(nameof(execRepo));
        _userGroupRepo = userGroupRepo ?? throw new ArgumentNullException(nameof(userGroupRepo));
        _facultyRepo = facultyRepo ?? throw new ArgumentNullException(nameof(facultyRepo));
        _empCheckRepo = empCheckRepo ?? throw new ArgumentNullException(nameof(empCheckRepo));
    }

    // 1) Save (insert/update) using stored proc SPL_EMPREGISTRATION_SAVE
    public async Task<int> SaveEmployeeAsync(SaveEmployeeRequest request)
    {
        var sql = StoredProcSql.Exec(StoredProcedures.SPL_EMPREGISTRATION_SAVE,
            "@EMPLOYEEID", "@EMPLOYEENAME", "@USERNAME", "@PASSWORD", "@USERGROUP", "@COURSE", "@EXAMMY",
            "@GENDER", "@DOB", "@CATEGORY", "@MOBILE", "@DEPARTMENT", "@QUALIFICATION", "@DOJ",
            "@TEACHINGSUBJECT", "@EMAIL", "@DESIGNATION", "@AADHARNO", "@IsActive");

        var pEmployeeId = new SqlParameter("@EMPLOYEEID", request.EmployeeId ?? string.Empty);
        var pEmployeeName = new SqlParameter("@EMPLOYEENAME", request.EmployeeName ?? string.Empty);
        var pUsername = new SqlParameter("@USERNAME", request.UserName ?? string.Empty);
        var pPassword = new SqlParameter("@PASSWORD", request.Password ?? string.Empty);
        var pUserGroup = new SqlParameter("@USERGROUP", request.UserGroup ?? string.Empty);
        var pCourse = new SqlParameter("@COURSE", request.Course ?? string.Empty);
        var pExamMY = new SqlParameter("@EXAMMY", request.ExamMY ?? string.Empty);
        var pGender = new SqlParameter("@GENDER", request.Gender ?? string.Empty);
        var pDOB = new SqlParameter("@DOB", request.DOB ?? string.Empty);
        var pCategory = new SqlParameter("@CATEGORY", request.Category ?? string.Empty);
        var pMobile = new SqlParameter("@MOBILE", request.Mobile ?? string.Empty);
        var pDepartment = new SqlParameter("@DEPARTMENT", request.Department ?? string.Empty);
        var pQualification = new SqlParameter("@QUALIFICATION", request.Qualification ?? string.Empty);
        var pDOJ = new SqlParameter("@DOJ", request.DOJ ?? string.Empty);
        var pTeachingSub = new SqlParameter("@TEACHINGSUBJECT", request.TeachingSubject ?? string.Empty);
        var pEmail = new SqlParameter("@EMAIL", request.Email ?? string.Empty);
        var pDesignation = new SqlParameter("@DESIGNATION", request.Designation ?? string.Empty);
        var pAadhar = new SqlParameter("@AADHARNO", request.AadharNo ?? string.Empty);
        // keep legacy convention: "true"/"false" string for IsActive
        var pIsActive = new SqlParameter("@IsActive", request.IsActive ? (object)"true" : (object)"false");

        // ExecuteStoredProcAsync on the execRepo returns Task<int>
        return await _execRepo.ExecuteStoredProcAsync(sql,
            pEmployeeId, pEmployeeName, pUsername, pPassword, pUserGroup, pCourse, pExamMY,
            pGender, pDOB, pCategory, pMobile, pDepartment, pQualification, pDOJ,
            pTeachingSub, pEmail, pDesignation, pAadhar, pIsActive);
    }

    // 2) Load grid / single employee (uses raw SELECT)
    public async Task<IEnumerable<EmployeeDto>> LoadEmployeesAsync(string empId = "NULL")
    {
        if (string.IsNullOrWhiteSpace(empId) || empId.ToUpper() == "NULL")
        {
            var sql = "SELECT E.*, R.PWD, R.USERGROUP, R.USERID RUSERID, '' AS PhotoUrl, '' AS SignatureUrl FROM TBL_EMPREG E INNER JOIN TBL_REGISTRATIONS R ON E.USERNAME = R.USERID";
            return await _empRepo.QueryFromStoredProcAsync(sql);
        }
        else
        {
            var sql = "SELECT E.*, R.PWD, R.USERGROUP, R.USERID RUSERID, '' AS PhotoUrl, '' AS SignatureUrl FROM TBL_EMPREG E INNER JOIN TBL_REGISTRATIONS R ON E.USERNAME = R.USERID WHERE E.EMPLOYEEID = @EMPID";
            var p = new SqlParameter("@EMPID", empId ?? string.Empty);
            return await _empRepo.QueryFromStoredProcAsync(sql, p);
        }
    }

    // 3) Delete employee (same effect as WebForms DeleteEmp)
    public async Task<int> DeleteEmployeeAsync(string empId, string userName)
    {
        var sql = "DELETE FROM tbl_Registrations WHERE userid = @UserName; DELETE FROM tbl_EmpReg WHERE employeeID = @EmpId;";
        var pUser = new SqlParameter("@UserName", userName ?? string.Empty);
        var pEmp = new SqlParameter("@EmpId", empId ?? string.Empty);
        return await _execRepo.ExecuteStoredProcAsync(sql, pUser, pEmp);
    }

    // 4) Load user groups (SELECT DISTINCT USERID AS USERGROUP FROM TBL_USERS_MENU)
    public async Task<IEnumerable<string>> LoadUserGroupsAsync()
    {
        var sql = "SELECT DISTINCT USERID AS USERGROUP FROM TBL_USERS_MENU";
        var rows = await _userGroupRepo.QueryFromStoredProcAsync(sql);
        var list = new List<string>();
        if (rows != null)
        {
            foreach (var r in rows)
            {
                // UserGroupDto has property USERGROUP (see DTO below)
                if (!string.IsNullOrEmpty(r.USERGROUP))
                    list.Add(r.USERGROUP);
            }
        }
        return list;
    }

    // 5) Get faculty data via stored proc SP_Facultydata
    public async Task<IEnumerable<EmployeeDto>> GetFacultyDataAsync()
    {
        var sql = StoredProcSql.Exec(StoredProcedures.SP_Facultydata);
        return await _facultyRepo.QueryFromStoredProcAsync(sql);
    }

    // 6) Check user id (replicates Check_UserId used in WebForms)
    public async Task<bool> CheckUserIdAsync(string userId)
    {
        var sql = "SELECT COUNT(1) CNT FROM TBL_REGISTRATIONS WHERE USERID = @UID";
        var p = new SqlParameter("@UID", userId ?? string.Empty);
        var rows = await _empCheckRepo.QueryFromStoredProcAsync(sql, p);
        var first = rows?.FirstOrDefault();
        if (first == null) return false;
        int cnt = 0;
        try { cnt = Convert.ToInt32(first.Cnt); } catch { cnt = 0; }
        return cnt > 0;
    }
}
