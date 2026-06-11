using System.Collections.Generic;
using System.Threading.Tasks;
using ICampus_Models.DTOs;
using ICampus_Models.Requests;

public interface IEmployeeService
{
    Task<int> SaveEmployeeAsync(SaveEmployeeRequest request);        // calls SPL_EMPREGISTRATION_SAVE
    Task<IEnumerable<EmployeeDto>> LoadEmployeesAsync(string empId = "NULL"); // LoadEmpData (grid)
    Task<int> DeleteEmployeeAsync(string empId, string userName);   // DeleteEmp
    Task<IEnumerable<string>> LoadUserGroupsAsync();                 // LoadUserGroups
    Task<IEnumerable<EmployeeDto>> GetFacultyDataAsync();           // SP_Facultydata
    Task<bool> CheckUserIdAsync(string userId);                     // Check_UserId
}
