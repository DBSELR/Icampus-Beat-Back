namespace ICampus_Models.Requests
{
    /// <summary>
    /// Request to generate Theory Absentees List report (AbsenteesList.aspx)
    /// Maps to: SP_REP_ABLIST
    /// Parameters confirmed from DLL IL (App_Web_gp3pforx.dll, offset 0x000128e7, String.Concat IL):
    ///   param 1: Regulation (varchar, quoted)  — from Session["Regulation"], REQUIRED
    ///   param 2: Course     (varchar, quoted)  — from Session["Course"],     REQUIRED
    ///   param 3: ExamMY     (varchar, quoted)  — from Session["ExamMY"],     REQUIRED
    ///   param 4: SEM        (INT, NO quotes)   — from ddlSemester, OPTIONAL (only appended when ddlSemester.SelectedIndex > 0)
    ///   param 5: EDate      (varchar, quoted)  — from ddlEdate, yyyy-MM-dd format, OPTIONAL
    /// </summary>
    public class AbsenteesListRequest
    {
        public string Regulation { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        public string ExamMY { get; set; } = string.Empty;
        public int? SEM { get; set; }       // optional INT (unquoted in original SQL concat)
        public string? EDate { get; set; }  // optional, format: yyyy-MM-dd
    }

    /// <summary>
    /// Request to generate Lab Absentees List report (LabAbsenteesList.aspx)
    /// No stored procedure — uses inline SQL on tbl_SH confirmed from DLL IL:
    ///   SELECT * FROM tbl_SH
    ///   WHERE Course=@Course AND ExamMY=@ExamMY AND PMARKS IN ('ab','sm')
    ///     AND PCode=@PCode AND grp=@GRP
    /// Crystal Reports selection formula fields: tbl_SH.Course, tbl_SH.ExamMY,
    ///   tbl_SH.PMARKS in ['ab','sm'], tbl_Sh.PCode, tbl_Sh.grp
    /// </summary>
    public class LabAbsenteesListRequest
    {
        public string Course { get; set; } = string.Empty;
        public string ExamMY { get; set; } = string.Empty;
        public string? PCode { get; set; }  // optional — subject code filter
        public string? GRP { get; set; }    // optional — branch filter
    }
}
