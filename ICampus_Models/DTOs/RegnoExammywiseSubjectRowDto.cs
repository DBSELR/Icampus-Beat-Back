namespace ICampus_Models.DTOs
{
    /// <summary>
    /// Single subject row returned by PROC_OMRNUM_UPDATE
    /// Grid columns from Regno_Exammywise_Subjets.aspx (GrdOmrNum):
    ///   aSHID (hidden), PTYPE (hidden), Exammy, sem, PCode, TempCode, PName,
    ///   smarks (editable), TMARKS (editable), RVMARKS (editable),
    ///   V3 (editable), MRK_FIN (editable), pmarks (editable)
    /// </summary>
    public class RegnoExammywiseSubjectRowDto
    {
        public long aSHID { get; set; }
        public string PTYPE { get; set; } = string.Empty;      // hidden — paper type
        public string Exammy { get; set; } = string.Empty;
        public string sem { get; set; } = string.Empty;
        public string PCode { get; set; } = string.Empty;
        public string TempCode { get; set; } = string.Empty;
        public string PName { get; set; } = string.Empty;
        public string? smarks { get; set; }     // Sessional marks
        public string? TMARKS { get; set; }     // Theory marks
        public string? RVMARKS { get; set; }    // Revaluation marks
        public string? V3 { get; set; }         // V3 marks
        public string? MRK_FIN { get; set; }    // Final theory marks
        public string? pmarks { get; set; }     // Practical marks
    }
}
