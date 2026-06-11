namespace ICampus_Models.Requests
{
    public class HallTicketRequest
    {
        public string ExamMY { get; set; } = string.Empty;      // Required
        public string Course { get; set; } = string.Empty;       // Required
        public string Regulation { get; set; } = string.Empty;   // Required
        public string Batch { get; set; } = string.Empty;        // Optional (REGU value)
        public string Branch { get; set; } = string.Empty;       // Optional (GRP value)
        public string Sem { get; set; } = string.Empty;          // Optional
        public string Regno { get; set; } = string.Empty;        // Optional (Hall Ticket Number)
        public string SelectionFormula { get; set; } = string.Empty; // Optional (built from filters)
    }
}

