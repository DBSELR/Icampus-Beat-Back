public class CondonationSaveRequest
{
    public string Regno { get; set; } = string.Empty;
    public string Sem { get; set; } = string.Empty;
    public string ExamMy { get; set; } = string.Empty;
    public decimal CondonationAmount { get; set; } = 0m;
    public string Regulation { get; set; } = string.Empty;
    public string Regu { get; set; } = "REG";
}

public class CondonationDeleteRequest
{
    public int Id { get; set; }              // primary key in condonation table
    public string Regno { get; set; } = ""; // optional
}
