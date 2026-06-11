using ICampus_Models.DTOs;
using ICampus_Models.Requests;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IPaperService
    {
        Task<IEnumerable<RegulationDto>> LoadRegulationsAsync();
        Task<IEnumerable<CourseListDto>> LoadCoursesAsync();
        Task<IEnumerable<BatchDto>> LoadBatchesAsync(string course);
        Task<IEnumerable<BranchDto>> LoadBranchesAsync(string course, string regu);
        Task<IEnumerable<SemDto>> LoadSemsAsync(string course, string batch, string branch);
        Task<IEnumerable<StreamDto>> LoadStreamsAsync(string course, string batch, string branch, int sem);
        Task<IEnumerable<PaperListDto>> LoadPaperListAsync(string course, string regu, string branch, int sem, string stream);
        Task<IEnumerable<PaperDetailDto>> GetPaperDetailsAsync(string course, string regu, int sem, string pcode, string branch);
        Task<int> SavePaperAsync(PaperSaveRequest request);
        Task<int> DeletePaperAsync(PaperDeleteRequest request);
        Task<bool> ReorderPapersAsync(PaperReorderRequest request);
        Task<int> CopyPapersAsync(PaperCopyRequest request);
        Task<bool> IsRegularBatchAsync(IsRegularRequest request);
    }
}
