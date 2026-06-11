namespace ICampus_Models.Requests
{
    public class SubjectGradeSaveRequest
    {
        public string Id { get; set; } = string.Empty;         // maps to ID (GR_ID)
        public string Regu { get; set; } = string.Empty;       // REGU
        public string MrkFrom { get; set; } = string.Empty;    // MRK_FROM (varchar in SP)
        public string MrkTo { get; set; } = string.Empty;      // MRK_TO
        public string Grade { get; set; } = string.Empty;      // GR
        public string GradePoint { get; set; } = string.Empty; // GRPTS
        public string Course { get; set; } = string.Empty;     // COURSE
    }

    public class CopyGradeRequest
    {
        public string FromBatch { get; set; } = string.Empty;  // @PREGU in proc
        public string ToBatch { get; set; } = string.Empty;    // @REGU in proc
        public string Course { get; set; } = string.Empty;
        public string ProcType { get; set; } = "TBL_GRADE";    // 'TBL_GRADE' or 'TBL_SEMGRADE' (used by PROC_COPY_GRADE_DATA)
    }

    public class DeleteGradeRequest
    {
        public string Id { get; set; } = string.Empty;
    }
}
