namespace ICampus_Models.Requests
{
    public class CancelReceiptListRequest
    {
        public string Regulation { get; set; } = string.Empty;  // Required (for dropdowns)
        public string Course { get; set; } = string.Empty;       // Required (for data/export)
        public string ExamMY { get; set; } = string.Empty;      // Required (for data/export)
    }
}

