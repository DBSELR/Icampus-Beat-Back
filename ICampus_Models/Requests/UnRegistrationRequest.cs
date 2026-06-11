namespace ICampus_Models.Requests
{
    public class UnRegistrationRequest
    {
        public string Regulation { get; set; } = string.Empty;  // Required
        public string Course { get; set; } = string.Empty;       // Required
        public string ExamMY { get; set; } = string.Empty;       // Required
        public string Regno { get; set; } = string.Empty;        // Required, min length: 10
    }
}

