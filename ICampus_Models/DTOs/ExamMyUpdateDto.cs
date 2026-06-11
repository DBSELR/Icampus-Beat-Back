namespace ICampus_Models.DTOs
{
    /// <summary>
    /// Grid row returned by sp_loadstdexammy_update
    /// Columns from ExamMyUpdate.aspx:
    ///   ASHID (hidden), REGULATION (hidden), REGU, COURSE, GRP,
    ///   SEM, STREAM, REGNO, EXAMMY (current), ACT_EXAMMY (editable)
    /// </summary>
    public class ExamMyUpdateDto
    {
        public long ASHID { get; set; }
        public string REGULATION { get; set; } = string.Empty;     // hidden
        public string REGU { get; set; } = string.Empty;
        public string COURSE { get; set; } = string.Empty;
        public string GRP { get; set; } = string.Empty;
        public string SEM { get; set; } = string.Empty;
        public string STREAM { get; set; } = string.Empty;
        public string REGNO { get; set; } = string.Empty;
        public string EXAMMY { get; set; } = string.Empty;         // current ExamMY (read-only)
        public string? ACT_EXAMMY { get; set; }                    // actual ExamMY to update to (editable)
    }
}
