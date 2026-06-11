// ICampus_BusinessLogic.Interfaces/IRoomMasterReportService.cs
using ICampus_Models.Requests;

public interface IRoomMasterReportService
{
    Task<IEnumerable<object>> LoadSubjectListAsync();
    Task<IEnumerable<object>> LoadBatchesAsync(string course, string regulation);
    Task<IEnumerable<object>> LoadBranchesAsync(string course, string regulation, string batch);
    Task<IEnumerable<object>> LoadSemsAsync(string course, string regulation, string batch);
    Task<IEnumerable<object>> LoadExammyAsync(string course, string regulation);
    Task<IEnumerable<object>> LoadPapersListAsync(RoomMasterPapersRequest request);
    Task<int> RunIasReportAsync();
    //Task<int> RunRegnoResultProcessAsync(RoomResultProcessRequest request);
}
