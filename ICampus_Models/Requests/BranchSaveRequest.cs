namespace ICampus_Models.Requests
{
    public class BranchSaveRequest
    {
        public string Priority { get; set; } = string.Empty;
        public string Sem { get; set; } = string.Empty;      // legacy sometimes uses null/strings
        public string Branch { get; set; } = string.Empty;
        public string EDate { get; set; } = string.Empty;    // yyyy-MM-dd expected
        public string Session { get; set; } = string.Empty;  // DaySession / ESESS
        public string Course { get; set; } = string.Empty;
        public string ExamMy { get; set; } = string.Empty;
    }

    public class CheckPriorityRequest
    {
        public string Priority { get; set; } = string.Empty;
    }

    public class BranchPriorityQueryRequest
    {
        public string ID { get; set; } = string.Empty;    // group id in legacy code
        public string SSID { get; set; } = string.Empty;  // session id
    }

    public class UpdateBranchPriorityRequest
    {
        // The legacy code passes a single string (Up_Q) representing the order/SQL.
        public string Up_Q { get; set; } = string.Empty;
    }

    public class DeleteBranchRequest
    {
        public string Priority { get; set; } = string.Empty;
        public string Sem { get; set; } = string.Empty;
        public string Branch { get; set; } = string.Empty;
        public string EDate { get; set; } = string.Empty;   // yyyy-MM-dd
        public string Session { get; set; } = string.Empty;
    }
}
