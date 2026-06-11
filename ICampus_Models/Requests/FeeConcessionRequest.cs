public class FeeConcessionSaveRequest
{
    public string Regno { get; set; } = string.Empty;      // @REGNO
    public string Course { get; set; } = string.Empty;     // optional for grid but present on page
    public string Branch { get; set; } = string.Empty;     // Branch / GRP
    public int Sem { get; set; } = 0;                      // @SEM
    public string ExamMy { get; set; } = string.Empty;     // @Exammy
    public string Regu { get; set; } = string.Empty;       // @Regu (REGU/Regu)
    public decimal TotalAmount { get; set; } = 0m;         // @TOTALAMOUNT
    public decimal FineAmount { get; set; } = 0m;          // @FINEAMOUNT
    public decimal Concession { get; set; } = 0m;          // @CONCESSION
    public decimal TobePaid { get; set; } = 0m;            // @TOBEPAID
    public string Remarks { get; set; } = string.Empty;    // (optional)
    public string Regulation { get; set; } = string.Empty; // @REGULATION
}

public class FeeConcessionDeleteRequest
{
    public int Id { get; set; }
}
