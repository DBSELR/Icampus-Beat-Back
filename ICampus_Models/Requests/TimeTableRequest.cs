namespace ICampus_Models.Requests
{
    public class TimeTableRequest
    {
        public string Course { get; set; } = string.Empty;    // Required
        public string ExamMY { get; set; } = string.Empty;   // Required
        public string Sem { get; set; } = string.Empty;       // Optional (for filtering)
    }
}

