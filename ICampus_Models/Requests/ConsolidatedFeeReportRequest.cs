namespace ICampus_Models.Requests
{
    public class ConsolidatedFeeReportRequest
    {
        public string Regulation { get; set; } = string.Empty;  // Required
        public string Course { get; set; } = string.Empty;       // Required
        public string ExamMy { get; set; } = string.Empty;      // Required
        public string FDate { get; set; } = string.Empty;        // Required (format: yyyy-MM-dd or dd-MM-yyyy)
        public string TDate { get; set; } = string.Empty;        // Required (format: yyyy-MM-dd or dd-MM-yyyy)
    }
}

