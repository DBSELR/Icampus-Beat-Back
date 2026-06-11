namespace ICampus_Models.Requests
{
    /// <summary>
    /// Request to load papers dropdown for Practical Marks Entry
    /// Maps to: PROC_LOADPAPERS_MRKENTRY (@Regulation, @EXAMMY, @Sem, @Course, @GRP, @TYPE='P')
    /// Only returns papers where PMAX > 0 (practical marks applicable)
    /// </summary>
    public class PracticalMarksPapersRequest
    {
        public string Regulation { get; set; } = string.Empty;
        public string ExamMY { get; set; } = string.Empty;
        public string Sem { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        public string GRP { get; set; } = string.Empty;
    }

    /// <summary>
    /// Request to load student marks grid for Practical Marks Entry
    /// Maps to: PROC_LOADMARKS_MRKENTRY (@Regulation, @EXAMMY, @Sem, @Course, @GRP, @PCode, @TYPE='P')
    /// PCode is optional - pass null/empty to load all papers in that branch/sem
    /// </summary>
    public class PracticalMarksStudentsRequest
    {
        public string Regulation { get; set; } = string.Empty;
        public string ExamMY { get; set; } = string.Empty;
        public string Sem { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        public string GRP { get; set; } = string.Empty;
        public string? PCode { get; set; }
    }

    /// <summary>
    /// Request to save a single student's practical mark
    /// Maps to: PROC_UPDATE_MARKS_INT_S_T (@MARKS, @ASHID, @TYPE='P')
    /// Marks can be a numeric string (0 to PMAX) or "AB" (Absent)
    /// </summary>
    public class PracticalMarksSaveRequest
    {
        public long ASHID { get; set; }
        public string Marks { get; set; } = string.Empty;
    }
}
