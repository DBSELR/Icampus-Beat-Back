namespace ICampus_Models.Requests
{
    /// <summary>
    /// Request to load student data by OMR number
    /// Maps to: PROC_OMRNUM_UPDATE_Get (@Regno, @Course, @Regulation, @ExamMy, @RegSup)
    /// Confirmed from DLL IL: method Get_loadOmrNumUpdate, 5 positional params
    /// OmrNumber is passed as the @Regno arg (1st param — the OMR scan sheet text)
    /// RegSup = Regular/Supplementary indicator (e.g. "R" or "S") — optional
    /// </summary>
    public class OmrMarksLoadRequest
    {
        public string OmrNumber { get; set; } = string.Empty;   // passed as @Regno
        public string Course { get; set; } = string.Empty;
        public string Regulation { get; set; } = string.Empty;
        public string ExamMY { get; set; } = string.Empty;
        public string? RegSup { get; set; }                     // Regular/Supplementary indicator
    }

    /// <summary>
    /// Request to save marks for a given OMR number
    /// Maps to: SP_OMRSAVEMARKSENTRY (@Regulation, @ExamMy, @Course, @OmrNumber, @Marks, @Type, @Sem)
    /// Confirmed from DLL IL: method SaveMarksEntry — 7 positional params
    /// OmrNumber is INT (passed without quotes in DLL IL template) — use the OMRNUMBER value from load result
    ///
    /// Type determines which marks column is updated:
    ///   "RV" → RVMARKS (Revaluation — RbtnRv selected in reference project)
    ///   "V3" → V3MARKS (V3 marks — Rbtnv3 selected in reference project)
    /// </summary>
    public class OmrMarksSaveRequest
    {
        public string Regulation { get; set; } = string.Empty;
        public string ExamMY { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        public int OmrNumber { get; set; }                  // INT — numeric OMRNUMBER from load result
        public string Marks { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;   // "RV" or "V3"
        public string Sem { get; set; } = string.Empty;
    }
}
