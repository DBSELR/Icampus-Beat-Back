// ICampus_Models.Requests/SubjectRequests.cs
namespace ICampus_Models.Requests
{
    public class PapersListRequest
    {
        public string Course { get; set; } = string.Empty;
        public string Regulation { get; set; } = string.Empty;
        public string Batch { get; set; } = string.Empty;   // may be null/empty
        public string Branch { get; set; } = string.Empty;  // GRP
        public string Sem { get; set; } = string.Empty;     // may be null/empty
    }

    // Generic request for dropdown calls (batch/branch/sem)
    public class SubjectDropdownRequest
    {
        public string Course { get; set; } = string.Empty;
        public string Regulation { get; set; } = string.Empty;
        public string Batch { get; set; } = string.Empty;   // optional
    }

    // request to run result process or IAS if needed (maps to DAL methods)
    public class SubjectProcessRequest
    {
        public string Regno { get; set; } = string.Empty;
        public string ExamMy { get; set; } = string.Empty;
        public string PrevExamy { get; set; } = string.Empty;
        public string ResultType { get; set; } = string.Empty;
    }
}
