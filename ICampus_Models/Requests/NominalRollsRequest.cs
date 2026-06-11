namespace ICampus_Models.Requests
{
    public class NominalRollsRequest
    {
        public string Course { get; set; } = string.Empty;       // Required
        public string ExamMY { get; set; } = string.Empty;      // Required
        public string Regulation { get; set; } = string.Empty;  // Required
        public string Sem { get; set; } = string.Empty;         // Optional
        public string Edate { get; set; } = string.Empty;        // Optional (format: yyyy-MM-dd)
        public string Room { get; set; } = string.Empty;         // Optional
        public bool IsReadmit { get; set; } = false;            // Optional (default: false)
    }
}

