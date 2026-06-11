// ICampus_Models.DTOs.EmployeeDto.cs
using Microsoft.AspNetCore.Http;
using System;

public class EmployeeDto
{
    public string EmployeeID { get; set; }
    public string EmployeeName { get; set; }
    public string Gender { get; set; }
    public DateTime? DOB { get; set; }
    public string Category { get; set; }
    public string Mobile { get; set; }
    public string Department { get; set; }
    public string Designation { get; set; }
    public string Qualification { get; set; }
    public DateTime? DOJ { get; set; }
    public string TeachingSubject { get; set; }
    public string Email { get; set; }
    public string IsActive { get; set; }
    public string UserName { get; set; }
    public string AadharNo { get; set; }

    // From TBL_REGISTRATIONS join (in LoadEmpData)
    public string Pwd { get; set; }
    public string USERGROUP { get; set; }
    public string RUSERID { get; set; } // alias for R.USERID in legacy query

    public string? PhotoUrl { get; set; }
    public string? SignatureUrl { get; set; }

    // RegCheckDto.cs
    public class RegCheckDto
    {
        public int CNT { get; set; }
    }

   


}

public class EmpCheckDto
{
    public int Cnt { get; set; }
}

public class SaveEmployeeFormModel
{
    // basic fields (string names must match form keys)
    public string? EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public string? UserGroup { get; set; }
    public string? Course { get; set; }
    public string? ExamMY { get; set; }
    public string? Gender { get; set; }
    public string? DOB { get; set; }
    public string? Category { get; set; }
    public string? Mobile { get; set; }
    public string? Department { get; set; }
    public string? Qualification { get; set; }
    public string? DOJ { get; set; }
    public string? TeachingSubject { get; set; }
    public string? Email { get; set; }
    public string? Designation { get; set; }
    public string? AadharNo { get; set; }
    public bool IsActive { get; set; } = true;

    // files
    public IFormFile? Photo { get; set; }       // binds field name "Photo"
    public IFormFile? Signature { get; set; }   // binds field name "Signature"
}

