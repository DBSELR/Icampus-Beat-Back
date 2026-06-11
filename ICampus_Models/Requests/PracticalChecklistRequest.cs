namespace ICampus_Models.Requests
{
    /// <summary>
    /// Request to generate Practical Check List report
    /// Confirmed from DLL IL (App_Web_gp3pforx.dll + DataAccessLayer.dll):
    ///   - Old project has NO PracticalCheckList_data DAL method
    ///   - Old project calls InternalCheckList_data (same as InternalChecklist)
    ///   - Same SP: SP_REP_INTERNAL_CHKLIST
    ///   - Same table: TBL_REPORT_PERIODICAL
    ///   - SP_REP_PRAC_CHKLIST does NOT appear in any DLL — not used by old project
    /// Parameters (same as InternalChecklistRequest):
    ///   param 1: Course     (varchar) — REQUIRED
    ///   param 2: ExamMY     (varchar) — REQUIRED
    ///   param 3: Regulation (varchar) — REQUIRED (full e.g. "R20")
    ///   param 4: REGU       (varchar,2) — REQUIRED (2-char batch year e.g. "20")
    ///   param 5: GRP        (varchar) — OPTIONAL, NULL = all branches
    ///   param 6: SEM        (varchar) — OPTIONAL, NULL = all semesters
    /// </summary>
    public class PracticalChecklistRequest
    {
        public string Course { get; set; } = string.Empty;
        public string ExamMY { get; set; } = string.Empty;
        public string Regulation { get; set; } = string.Empty;
        public string Regu { get; set; } = string.Empty;   // 2-char batch year e.g. "20", "18"
        public string? GRP { get; set; }    // optional — null = all branches
        public string? SEM { get; set; }    // optional — null = all semesters
    }
}
