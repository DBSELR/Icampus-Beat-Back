namespace ICampus_Models.Requests
{
    /// <summary>
    /// Request to generate D Form report (DFORM.aspx)
    /// Maps to: sp_rep_deform (normal) or sp_rep_deform_Readmit (readmit)
    ///
    /// Parameters confirmed from DLL IL (App_Web_gp3pforx.dll, method loadingdform, IL offset 0x0000c3be):
    ///   param 1: Regulation (varchar) — from Session["Regulation"], REQUIRED
    ///   param 2: Course     (varchar) — from Session["Course"],     REQUIRED
    ///   param 3: ExamMY     (varchar) — from Session["ExamMY"],     REQUIRED
    ///   param 4: SEM        (varchar) — from ddlSemester.SelectedValue, OPTIONAL
    ///   param 5: EDate      (varchar) — from ddlEdate.SelectedValue, formatted yyyy-MM-dd, OPTIONAL
    ///
    /// IsReadmit: when true calls sp_rep_deform_Readmit, when false calls sp_rep_deform
    /// (mirrors ChkIsreadmitresult checkbox in DFORM.aspx)
    /// </summary>
    public class DFormRequest
    {
        public string Regulation { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        public string ExamMY { get; set; } = string.Empty;
        public string? SEM { get; set; }        // optional — null or empty = all semesters
        public string? EDate { get; set; }      // optional — format: yyyy-MM-dd
        public bool IsReadmit { get; set; } = false;   // false = regular, true = readmission
    }
}
