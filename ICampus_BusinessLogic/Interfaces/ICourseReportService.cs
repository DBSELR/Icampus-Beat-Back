// ICampus_BusinessLogic.Interfaces/ISubjectService.cs
using ICampus_Models.Requests;

public interface ICourseReportService
{
    Task<IEnumerable<object>> LoadSubjectListAsync(); // SPM_SUBJECTLIST
    Task<IEnumerable<object>> LoadBatchesAsync(string course, string regulation); // inline SQL from Dal.SubjectList_Batch
    Task<IEnumerable<object>> LoadBranchesAsync(string course, string regulation, string batch); // inline SQL
    Task<IEnumerable<object>> LoadSemsAsync(string course, string regulation, string batch); // inline SQL
    Task<IEnumerable<object>> LoadPapersListAsync(PapersListRequest request); // proc_Load_PapersList
    Task<int> RunIasReportAsync(); // IAS_Report
    Task<int> RunRegnoResultProcessAsync(SubjectProcessRequest request); // REGNORESULTPROCESS
}
