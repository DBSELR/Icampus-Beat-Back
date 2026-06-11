namespace ICampus_Models.Requests
{
    /// <summary>
    /// Request to load subject rows by RegNo for the Regno Exammywise Subjects screen
    /// Maps to: PROC_OMRNUM_UPDATE (@Regno, @Course, @Regulation, @ExamMy)
    /// Confirmed from DLL IL: method loadOmrNumUpdate, 4 positional params, all varchar quoted
    /// Triggered by txtregno AutoPostBack in Regno_Exammywise_Subjets.aspx
    /// </summary>
    public class RegnoExammywiseLoadRequest
    {
        public string RegNo { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        public string Regulation { get; set; } = string.Empty;
        public string ExamMY { get; set; } = string.Empty;
    }

    /// <summary>
    /// Request to load subject rows by RegNo with Reg/Sup type filter
    /// Maps to: PROC_OMRNUM_UPDATE_Get (@Regno, @Course, @Regulation, @ExamMy, @RegSup)
    /// Confirmed from DLL IL: method Get_loadOmrNumUpdate, 5 positional params, all varchar quoted
    /// Triggered by btnGetData_Click (ddlSemType = Reg/Sup) in Regno_Exammywise_Subjets.aspx
    /// RegSup: "Reg" = Regular, "Sup" = Supplementary
    /// </summary>
    public class RegnoExammywiseGetLoadRequest
    {
        public string RegNo { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        public string Regulation { get; set; } = string.Empty;
        public string ExamMY { get; set; } = string.Empty;
        public string RegSup { get; set; } = string.Empty;     // "Reg" or "Sup"
    }

    /// <summary>
    /// Single paper row to update in the marks save
    /// Mirrors the editable grid row in Regno_Exammywise_Subjets.aspx
    /// </summary>
    public class RegnoExammywisePaperUpdateRequest
    {
        public long ASHID { get; set; }
        public string? SMARKS { get; set; }
        public string? PMARKS { get; set; }
        public string? TMARKS { get; set; }
        public string? RVMARKS { get; set; }
        public string? V3 { get; set; }
        public string? MRK_FIN { get; set; }
    }

    /// <summary>
    /// Request to save marks for all grid rows (btnOmrNo "UPDATE MARKS")
    /// Maps to: PROC_marksupdate_regnowise — called once per paper row
    /// Confirmed from DLL IL: 7 positional params: AshId, Smarks, Pmarks, Tmarks, Rvmarks, v3marks, Tmrk_final
    /// </summary>
    public class RegnoExammywiseSaveRequest
    {
        public string RegNo { get; set; } = string.Empty;
        public List<RegnoExammywisePaperUpdateRequest> Papers { get; set; } = new();
    }
}
