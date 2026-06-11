namespace ICampus_Models.Requests
{
    public class OMRSheetRequest
    {
        public string Regulation { get; set; } = string.Empty;  // Required
        public string Course { get; set; } = string.Empty;        // Required
        public string ExamMY { get; set; } = string.Empty;        // Required
        public string Sem { get; set; } = string.Empty;           // Optional (for filtering)
        public string Edate { get; set; } = string.Empty;          // Optional (for filtering)
        public string Room { get; set; } = string.Empty;           // Optional (for filtering)
    }
}

