namespace ICampus_Models.Requests
{
    /// <summary>
    /// Request to generate Subject Wise Present List report (Studentwise_presentlist.aspx)
    /// Maps to: SP_Pcode_Presentlist
    ///
    /// Parameters confirmed from DLL IL (DataAccessLayer.dll DAL_StudentHistory::Pcode_PresentList,
    ///   US heap US[0xc0c1], String.Concat IL with 11 elements):
    ///   "SP_Pcode_Presentlist '"  → Course  → "','"  → Regulation → "','"
    ///   → ExamMY → "', '" → Sem → "', '" → Regsup → "'"
    ///
    ///   param 1: Course     (varchar, quoted) — from Session["Course"],     REQUIRED
    ///   param 2: Regulation (varchar, quoted) — from Session["Regulation"], REQUIRED
    ///   param 3: ExamMY     (varchar, quoted) — from Session["ExamMY"],     REQUIRED
    ///   param 4: Sem        (varchar, quoted) — from ddlSemester,           REQUIRED
    ///   param 5: Regsup     (varchar, quoted) — from ddlregsup: "REG" or "Sup", REQUIRED
    ///
    /// Validation: Both Sem and Regsup must be selected (SelectedIndex != 0 in original)
    /// Excel output: worksheet "REGNO_VS_OMR", file "{Course}_{Regulation}_{ExamMY}.xlsx"
    /// </summary>
    public class StudentWisePresentListRequest
    {
        public string Course { get; set; } = string.Empty;
        public string Regulation { get; set; } = string.Empty;
        public string ExamMY { get; set; } = string.Empty;
        public string Sem { get; set; } = string.Empty;
        public string Regsup { get; set; } = string.Empty;   // "REG" or "Sup"
    }
}
