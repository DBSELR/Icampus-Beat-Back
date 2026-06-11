using ICampus_Models.DTOs;
using ICampus_Models.Requests;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IDropdownService
    {
        Task<IEnumerable<RegulationDto>> GetRegulationsAsync();
        Task<IEnumerable<CourseListDto>> GetCoursesAsync(string regulation);
        Task<IEnumerable<ExamMYDto>> GetExamMYAsync(string regulation, string course);
        Task<int> SaveUserSelectionAsync(SaveUserSelectionRequest request, string userId);
    }
}