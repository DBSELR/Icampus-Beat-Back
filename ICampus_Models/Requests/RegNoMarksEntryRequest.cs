namespace ICampus_Models.Requests
{
    /// <summary>
    /// Request to load all papers + marks for a student by RegNo
    /// Maps to: PROC_GETSUBJTS_REGNO_WISE (@REGNO, @EXAMMY, @REGU)
    /// ExamMY and Regu are optional filters
    /// </summary>
    public class RegNoMarksLoadRequest
    {
        public string RegNo { get; set; } = string.Empty;
        public string? ExamMY { get; set; }   // optional filter; null = all exams
        public string? Regu { get; set; }     // optional batch/regulation (e.g. "16","18","20")
        public string? Course { get; set; }   // course code (e.g. "B.Tech", "CSE") from session context
    }

    /// <summary>
    /// Single paper row to update in the bulk save
    /// Mirrors the editable grid row in MarksEntry_Regnowise.aspx
    /// </summary>
    public class RegNoMarksPaperUpdateRequest
    {
        public long ASHID { get; set; }
        public string PTYPE { get; set; } = string.Empty;
        public string? SMARKS { get; set; }
        public string? TMARKS { get; set; }
        public string? RVMARKS { get; set; }
        public string? V3 { get; set; }
        public string? MRK_FIN { get; set; }
        public string? PMARKS { get; set; }
    }

    /// <summary>
    /// Request to save marks for all grid rows at once (btnOmrNo "UPDATE MARKS")
    /// Maps to: PROC_marksupdate_regnowise — called once per paper row
    /// </summary>
    public class RegNoMarksSaveRequest
    {
        public string RegNo { get; set; } = string.Empty;
        public List<RegNoMarksPaperUpdateRequest> Papers { get; set; } = new();
    }
}
