// ICampus_Models.Requests/CourseGradeRequests.cs
namespace ICampus_Models.Requests
{
    public class CourseGradePapersRequest
    {
        public string Course { get; set; } = string.Empty;
        public string Regulation { get; set; } = string.Empty;
        public string Batch { get; set; } = string.Empty;   // may be null/empty
        public string Branch { get; set; } = string.Empty;  // GRP
        public string Sem { get; set; } = string.Empty;     // may be null/empty
    }

    public class CourseGradeDropdownRequest
    {
        public string Course { get; set; } = string.Empty;
        public string Regulation { get; set; } = string.Empty;
        public string Batch { get; set; } = string.Empty;
    }

    public class ResultProcessRequest
    {
        public string Regno { get; set; } = string.Empty;
        public string ExamMy { get; set; } = string.Empty;
        public string PrevExamy { get; set; } = string.Empty; // 'Y' or other
        public string ResultType { get; set; } = string.Empty; // 'NORMAL','SM','GR','RV'
    }
}
