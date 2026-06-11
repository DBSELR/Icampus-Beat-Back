public class RegisterRvPaperRequest
{
    public string Regno { get; set; } = string.Empty;
    public string ExamMy { get; set; } = string.Empty;
    public string Sem { get; set; } = string.Empty;         // DDLSemester.Text
    public string PCode { get; set; } = string.Empty;       // single paper pcode
    public string RegistrationType { get; set; } = "RV";    // RV | RC | CHRV
}

public class ResetRvPaperRequest
{
    public string Regno { get; set; } = string.Empty;
    public string ExamMy { get; set; } = string.Empty;
    public string Sem { get; set; } = string.Empty;
}

public class RvFeePayRequest
{
    public string ExFeePay { get; set; } = string.Empty; // this is the full dynamic Q used in SPM_EXAMFEE_PAY (as in aspx)
    public decimal? AppFee { get; set; } = 0m;
    public decimal? Concession { get; set; } = 0m;
}
