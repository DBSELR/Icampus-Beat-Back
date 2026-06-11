namespace ICampus_Models.Requests
{
    /// <summary>
    /// Request to load the student grid for ExamMY Update
    /// Maps to: sp_loadstdexammy_update (@Regulation, @Course, @Batch, @Branch, @Sem)
    /// Confirmed from DLL IL: BOL_StudentWiseMasterCreation — 5 positional params
    /// Regulation and Course come from session context in the old project (now passed explicitly)
    /// </summary>
    public class ExamMyLoadRequest
    {
        public string Regulation { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        public string Batch { get; set; } = string.Empty;
        public string Branch { get; set; } = string.Empty;
        public string Sem { get; set; } = string.Empty;
    }

    /// <summary>
    /// Single row update — all identifying fields + new ACT_EXAMMY value
    /// Maps to: sp_Exammy_update (@ACT_EXAMMY, @Regulation, @Batch, @Course, @Branch, @Sem, @REGNO, @EXAMMY)
    /// Confirmed from DLL IL: BOL_StudentWiseMasterCreation — 8 positional params
    /// The SP identifies the row by REGNO + full exam context (not by ASHID)
    /// </summary>
    public class ExamMyUpdateRowRequest
    {
        public string ACT_EXAMMY { get; set; } = string.Empty;  // new ExamMY value (editable in grid)
        public string Regulation { get; set; } = string.Empty;   // REGULATION column from grid
        public string Batch { get; set; } = string.Empty;        // REGU column from grid
        public string Course { get; set; } = string.Empty;       // COURSE column from grid
        public string Branch { get; set; } = string.Empty;       // GRP column from grid
        public string Sem { get; set; } = string.Empty;          // SEM column from grid
        public string REGNO { get; set; } = string.Empty;        // REGNO column from grid
        public string EXAMMY { get; set; } = string.Empty;       // original EXAMMY from grid
    }

    /// <summary>
    /// Bulk update request — all edited rows sent at once (btnUpdateExammy click)
    /// Maps to: sp_Exammy_update called once per row (8 params each)
    /// </summary>
    public class ExamMyUpdateRequest
    {
        public List<ExamMyUpdateRowRequest> Rows { get; set; } = new();
    }
}
