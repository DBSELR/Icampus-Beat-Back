namespace ICampus_Models.Requests
{
    /// <summary>
    /// Request to load papers dropdown
    /// Maps to: PROC_LOADPAPERS_MRKENTRY (@Regulation, @EXAMMY, @Sem, @Course, @GRP, @TYPE='I')
    /// </summary>
    public class InternalMarksPapersRequest
    {
        public string Regulation { get; set; } = string.Empty;
        public string ExamMY { get; set; } = string.Empty;
        public string Sem { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        public string GRP { get; set; } = string.Empty;
    }

    /// <summary>
    /// Request to load student marks grid
    /// Maps to: PROC_LOADMARKS_MRKENTRY (@Regulation, @EXAMMY, @Sem, @Course, @GRP, @PCode, @TYPE='I')
    /// PCode is optional - pass null/empty to load all papers in that branch/sem
    /// </summary>
    public class InternalMarksStudentsRequest
    {
        public string Regulation { get; set; } = string.Empty;
        public string ExamMY { get; set; } = string.Empty;
        public string Sem { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        public string GRP { get; set; } = string.Empty;
        public string? PCode { get; set; }
    }

    /// <summary>
    /// Request to save a single student's internal marks
    /// Maps to: PROC_UPDATE_MARKS_INT_S_T (@MARKS, @ASHID, @TYPE='S')
    /// Marks can be a numeric string or "AB" (Absent)
    /// </summary>
    public class InternalMarksSaveRequest
    {
        public long ASHID { get; set; }
        public string Marks { get; set; } = string.Empty;
    }
}
