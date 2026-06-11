namespace ICampus_Models.Requests
{
    /// <summary>
    /// Request to generate D Form Mid report (DFORM_MID.aspx)
    /// Maps to: sp_rep_deform_MID (normal) or sp_rep_deform_Readmit_MID (readmit)
    ///
    /// Parameters confirmed from DLL IL (App_Web_gp3pforx.dll iCampus_Reports_DFORM_MID,
    ///   ldstr at file offset 0xc4f1 / 0xc3be, 6-phase String.Concat analysis):
    ///   param 1: Regulation (varchar, quoted)   — from Session["Regulation"], REQUIRED
    ///   param 2: Course     (varchar, quoted)   — from Session["Course"],     REQUIRED
    ///   param 3: ExamMY     (varchar, quoted)   — from Session["ExamMY"],     REQUIRED
    ///   param 4: SEM        (INT, NO quotes)    — from ddlSemester.SelectedValue, OPTIONAL (skipped when DDL empty)
    ///   param 5: EDate      (varchar, quoted)   — DateTime.Parse + .ToString("yyyy-MM-dd"), OPTIONAL
    ///   param 6: ExamType   (varchar, quoted)   — "1"=MID-I, "2"=MID-II, OPTIONAL (skipped when DDL empty)
    ///
    /// IsReadmit: when true calls sp_rep_deform_Readmit_MID, when false calls sp_rep_deform_MID
    /// (mirrors ChkIsreadmitresult checkbox in DFORM_MID.aspx)
    ///
    /// IMPORTANT: SEM is INT (unquoted in original concat) — use SqlDbType.Int with DBNull when null
    /// EDate format: yyyy-MM-dd
    /// </summary>
    public class DFormMidRequest
    {
        public string Regulation { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        public string ExamMY { get; set; } = string.Empty;
        public int? SEM { get; set; }           // optional INT — null = all semesters
        public string? EDate { get; set; }      // optional — format: yyyy-MM-dd
        public string? ExamType { get; set; }   // optional — "1"=MID-I, "2"=MID-II
        public bool IsReadmit { get; set; } = false;
    }
}
