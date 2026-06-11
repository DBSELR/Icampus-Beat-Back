// ICampus_Models.Requests/SemesterGradeRequests.cs
namespace ICampus_Models.Requests
{
    public class SemesterGradePapersRequest
    {
        public string Course { get; set; } = string.Empty;
        public string Regulation { get; set; } = string.Empty;
        public string Batch { get; set; } = string.Empty;   // REGU
        public string Branch { get; set; } = string.Empty;  // GRP
        public string Sem { get; set; } = string.Empty;
    }

    public class SemesterGradeDropdownRequest
    {
        public string Course { get; set; } = string.Empty;
        public string Regulation { get; set; } = string.Empty;
        public string Batch { get; set; } = string.Empty;
    }

    public class SemesterResultProcessRequest
    {
        public string Regno { get; set; } = string.Empty;
        public string ExamMy { get; set; } = string.Empty;
        public string PrevExamy { get; set; } = string.Empty; // 'Y' or 'N'
        public string ResultType { get; set; } = string.Empty; // 'NORMAL','SM','GR','RV'
    }
}
