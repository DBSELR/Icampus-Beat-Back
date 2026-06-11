namespace ICampus_Models.Requests
{
    public class RoomWiseNominalRollsRequest
    {
        public string Course { get; set; } = string.Empty;       // Required
        public string ExamMY { get; set; } = string.Empty;      // Required
        public string Regulation { get; set; } = string.Empty;  // Required
        public string ExamType { get; set; } = string.Empty;     // Required (1=External, 2=MID-I, 3=MID-II)
        public string Sem { get; set; } = string.Empty;         // Optional
        public string Edate { get; set; } = string.Empty;        // Optional (format: yyyy-MM-dd)
        public string Branch { get; set; } = string.Empty;       // Optional
    }
}

