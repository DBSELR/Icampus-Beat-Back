public class DupCertificateSaveRequest
{
    public string ReceiptNo { get; set; } = string.Empty;   // maps to @RECEIPTNO
    public string RegNo { get; set; } = string.Empty;       // maps to @REGNO
    public int Sem { get; set; }                             // maps to @SEM
    public string ExamMy { get; set; } = string.Empty;      // maps to @EXAMMY
    public string CertificateName { get; set; } = string.Empty; // maps to @CERTIFICATE_NAME
    public string Remarks { get; set; } = string.Empty;     // maps to @REMARKS
    public string CrId { get; set; } = string.Empty;        // maps to @CR_ID (created by)
}

public class MarksMemoRequest
{
    public string Regulation { get; set; } = string.Empty;   // @REGULATION
    public string ExamMy { get; set; } = string.Empty;      // @EXAMMY
    public string Course { get; set; } = string.Empty;      // @Course
    public string Semester { get; set; } = string.Empty;    // @SEMESTER (use "1","2",... or "08" per SP)
    public string RV { get; set; } = "N";                   // @RV ('Y' or 'N')
    public string Branch { get; set; } = string.Empty;      // @BRANCH (grp)
    public string RegNo { get; set; } = string.Empty;       // @REGNO (optional; pass empty string for NULL)
    public string Date { get; set; } = string.Empty;        // @Date (pass '' or 'yyyy-MM-dd' if you want to override)
}


public class DupCertificateCheckRequest
{
    public string RegNo { get; set; } = string.Empty;
    public int Sem { get; set; }
    public string ExamMy { get; set; } = string.Empty;
    public string CertificateName { get; set; } = string.Empty;
    public string ReceiptNo { get; set; } = string.Empty; // optional for receipt-wise check
}
