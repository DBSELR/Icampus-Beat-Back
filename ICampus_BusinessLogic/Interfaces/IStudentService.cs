using ICampus_Models.DTOs;
using ICampus_Models.Requests;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IStudentService
    {
        Task<IEnumerable<StudentListDto>> GetStudentsAsync(string regu, string course, string branch);
        Task<StudentDto> GetStudentByRegNoAsync(string regNo);

        Task<int> SaveStudentAsync(SaveStudentRequest request);
        Task<int> InactivateStudentAsync(InactivateRequest request);
        Task<int> ReactivateStudentAsync(ReactivateRequest request);
        Task<int> ReAdmissionAsync(ReadmissionRequest request);

        // Exports
        Task<IEnumerable<StudentDto>> ExportDetainedListAsync();
        Task<IEnumerable<StudentDto>> ExportPassedListAsync(string regulation, string course, string examMy);
        Task<IEnumerable<StudentDto>> ExportBranchWiseAsync(string regulation, string course);
        Task<IEnumerable<StudentDto>> ExportReadmissionListAsync(string regulation, string course);
        Task<IEnumerable<StudentDto>> ExportLateralAsync();

        // Uploads
        Task<bool> UploadPhotoAsync(IFormFile file, string regNo);
        Task<bool> UploadSignatureAsync(IFormFile file, string regNo);
    }
}
