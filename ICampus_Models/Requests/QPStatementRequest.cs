namespace ICampus_Models.Requests
{
    public class QPStatementRequest
    {
        public string Course { get; set; } = string.Empty;      // Required
        public string ExamMY { get; set; } = string.Empty;       // Required
        public string Regulation { get; set; } = string.Empty;   // Required (for semester list)
        public string Sem { get; set; } = string.Empty;          // Optional (for filtering)
    }
}

