namespace ICampus_Models.Requests
{
    /// <summary>
    /// Request to load papers dropdown for Absentees Entry
    /// Maps to: PROC_LOADPAPERS_MRKENTRY (@Regulation, @ExamMy, @Sem[INT], @Course, @GRP, @TYPE='T')
    /// Confirmed from DLL IL: 5 positional params + TYPE='T' hardcoded
    /// @Sem is INT (unquoted in DLL IL template)
    /// </summary>
    public class AbsenteesPapersRequest
    {
        public string Regulation { get; set; } = string.Empty;
        public string ExamMY { get; set; } = string.Empty;
        public string Sem { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        public string GRP { get; set; } = string.Empty;
    }

    /// <summary>
    /// Request to load student list for the selected paper
    /// Maps to: PROC_LOADMARKS_MRKENTRY (@Regulation, @ExamMy, @Sem, @Course, @Branch, @PaperCode, @TYPE='T')
    /// Confirmed from DLL IL: 6 positional params in this order + TYPE='T' hardcoded
    /// Returns: aSHID, RegNo, grp, PCODE, CODE (current AB/MP code)
    /// </summary>
    public class AbsenteesStudentsRequest
    {
        public string Regulation { get; set; } = string.Empty;
        public string ExamMY { get; set; } = string.Empty;
        public string Sem { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        public string GRP { get; set; } = string.Empty;
        public string? PCode { get; set; }
    }

    /// <summary>
    /// Request to save a single student's absentee code
    /// Maps to: PROC_UPDATE_MARKS_INT_S_T (@Marks, @AshId, @TYPE='T')
    /// Confirmed from DLL IL: 2 positional params + TYPE='T' hardcoded
    /// Code must be "AB" (Absent) or "MP" (Malpractice) — validated client-side in reference project
    /// </summary>
    public class AbsenteesSaveRequest
    {
        public long ASHID { get; set; }
        public string Code { get; set; } = string.Empty;   // "AB" or "MP"
    }
}
