namespace ICampus_Models.Requests
{
    /// <summary>
    /// Request to load papers dropdown for Mid Absentees Entry
    /// Maps to: PROC_LOADPAPERS_MRKENTRY (@Regulation, @ExamMy, @Sem[INT], @Course, @GRP, @TYPE='T')
    /// Confirmed from DLL IL: same SP and params as regular AbsenteesEntry papers
    /// ExamType is NOT passed to the papers SP — only to the students and save SPs
    /// </summary>
    public class MidAbsenteesPapersRequest
    {
        public string Regulation { get; set; } = string.Empty;
        public string ExamMY { get; set; } = string.Empty;
        public string Sem { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        public string GRP { get; set; } = string.Empty;
    }

    /// <summary>
    /// Request to load student list for the selected paper (Mid exam)
    /// Maps to: PROC_LOADMARKS_MRKENTRY_MID
    ///   (@Regulation, @ExamMy, @Sem, @Course, @Branch, @PaperCode, @TYPE='T', @ExamType)
    /// Confirmed from DLL IL: 8 positional params in this exact order
    /// ExamType: "1" = MID-I, "2" = MID-II (from ddlExamType in AbsenteesEntry_Mid.aspx)
    /// Returns: aSHID, RegNo, grp, PCODE, MIDCODE (current AB/MP code)
    /// </summary>
    public class MidAbsenteesStudentsRequest
    {
        public string Regulation { get; set; } = string.Empty;
        public string ExamMY { get; set; } = string.Empty;
        public string Sem { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        public string GRP { get; set; } = string.Empty;
        public string? PCode { get; set; }
        public string ExamType { get; set; } = string.Empty;   // "1" = MID-I, "2" = MID-II
    }

    /// <summary>
    /// Request to save a single student's Mid absentee code (AB or MP)
    /// Maps to: PROC_UPDATE_MID_MARKS_INT_S_T (@Marks, @AshId, @TYPE='T', @ExamType)
    /// Confirmed from DLL IL: 4 positional params in this exact order
    /// Code must be "AB" (Absent) or "MP" (Malpractice)
    /// ExamType: "1" = MID-I, "2" = MID-II
    /// </summary>
    public class MidAbsenteesSaveRequest
    {
        public long ASHID { get; set; }
        public string Code { get; set; } = string.Empty;       // "AB" or "MP"
        public string ExamType { get; set; } = string.Empty;   // "1" = MID-I, "2" = MID-II
    }
}
