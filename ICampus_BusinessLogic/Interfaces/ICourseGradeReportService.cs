// ICampus_BusinessLogic.Interfaces/ICourseGradeReportService.cs
using ICampus_Models.Requests;

public interface ICourseGradeReportService
{
    Task<IEnumerable<object>> LoadSubjectListAsync(); // SPM_SUBJECTLIST
    Task<IEnumerable<object>> LoadBatchesAsync(string course, string regulation); // SubjectList_Batch logic
    Task<IEnumerable<object>> LoadBranchesAsync(string course, string regulation, string batch); // SubjectList_Branch
    Task<IEnumerable<object>> LoadSemsAsync(string course, string regulation, string batch); // SubjectList_Semester
    Task<IEnumerable<object>> LoadPapersListAsync(CourseGradePapersRequest request); // proc_Load_PapersList
    Task<int> RunIasReportAsync(); // PROC_IS_IASUPDATE
    Task<IEnumerable<object>> LoadExammyAsync(string course, string regulation); // sp_regno_exammy
    //Task<int> RunRegnoResultProcessAsync(ResultProcessRequest request); // REGNORESULTPROCESS -> proc_resultprocess_alldata
}
