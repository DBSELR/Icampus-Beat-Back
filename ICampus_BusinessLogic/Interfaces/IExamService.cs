using System.Collections.Generic;
using System.Threading.Tasks;
using ICampus_Models.DTOs;
using ICampus_Models.Requests;

public interface IExamService
{
    Task<IEnumerable<string>> LoadRegulationsAsync();
    Task<IEnumerable<string>> LoadCoursesByRegulationAsync(string regulation);
    Task<IEnumerable<ExistingExamDto>> LoadExistingExamsAsync(string regulation, string examMy, string course);
    Task<int> SaveExamsAsync(ExamsSaveRequest request);
    Task<IEnumerable<object>> LoadNotificationsAsync(string examMy, string course, string regulation);
    Task<int> SaveNotificationAsync(ExamNotificationSaveRequest request);
    Task<IEnumerable<ExamMasterDto>> LoadExamMasterListAsync(string regulation, string course);
    Task<IEnumerable<object>> DeleteExamMasterAsync(long aExamId);
    Task<int> ResetRegsupExamsAsync(string examMy, string course);
    Task<int> RegsUp_SaveAsync(RegsUpSaveRequest request);
    Task<IEnumerable<ExamMasterDto>> ExistingExamMasterListAsync(string regulation, string course);
    Task<IEnumerable<object>> GetBatchByCourseAsync(int regu, string course);
    // add more helper methods if you want (Regu_CHk, Std_CHk etc) - implemented as needed below
}
