namespace ICampus_Models.Requests
{
    public class MidHallTicketRequest
    {
        public string ExamMY { get; set; } = string.Empty;      // Required
        public string Course { get; set; } = string.Empty;       // Required
        public string Regulation { get; set; } = string.Empty;  // Required
        public string Sem { get; set; } = string.Empty;         // Optional (can be empty)
        public string Batch { get; set; } = string.Empty;       // Optional (can be empty)
        public string Branch { get; set; } = string.Empty;       // Optional (can be empty)
        public string Regno { get; set; } = string.Empty;        // Optional (can be empty)
        public string ExamType { get; set; } = string.Empty;     // Required (MID-I or MID-II)
    }
}

