namespace ICampus_Models.Requests
{
    public class ExamFeeCollectionRequest
    {
        public string ExamMY { get; set; } = string.Empty;      // Required
        public string Course { get; set; } = string.Empty;       // Required
        public string Regulation { get; set; } = string.Empty;   // Required (for export)
        public string FDate { get; set; } = string.Empty;        // Required (format: dd-MM-yyyy or yyyy-MM-dd)
        public string TDate { get; set; } = string.Empty;        // Required (format: dd-MM-yyyy or yyyy-MM-dd)
        public string UserID { get; set; } = string.Empty;       // Optional (for filtering by user)
    }
}

