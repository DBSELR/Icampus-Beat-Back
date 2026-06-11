namespace ICampus_Models.Requests
{
    /// <summary>
    /// Request to generate Internal Check List report
    /// Maps to: SP_REP_INTERNAL_CHKLIST
    /// Parameters confirmed from DLL IL (App_Web_oxqewfcs.dll, method loadingInternalCheckList):
    ///   param 1: Course     (varchar) — from Session['Course'], REQUIRED
    ///   param 2: ExamMY     (varchar) — from Session['ExamMY'], REQUIRED
    ///   param 3: Regulation (varchar) — from ddlBatch, REQUIRED
    ///   param 4: GRP        (varchar) — from ddlBranch, OPTIONAL (pass empty string for all)
    ///   param 5: SEM        (varchar) — from ddlSemester, OPTIONAL (pass empty string for all)
    /// </summary>
    public class InternalChecklistRequest
    {
        public string Course { get; set; } = string.Empty;
        public string ExamMY { get; set; } = string.Empty;
        public string Regulation { get; set; } = string.Empty;
        public string Regu { get; set; } = string.Empty;        // 2-char batch year e.g. "20", "18"
        public string? GRP { get; set; }    // optional — null or empty = all branches
        public string? SEM { get; set; }    // optional — null or empty = all semesters
    }
}
