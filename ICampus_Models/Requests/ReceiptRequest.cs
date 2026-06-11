public class ReceiptCollectionRequest
{
    // date format: your SP expects dd-MM-yyyy (as string converted by SP), keep same as UI
    public string ExamMy { get; set; } = string.Empty;
    public string Course { get; set; } = string.Empty;
    public string FDate { get; set; } = string.Empty; // "dd-MM-yyyy"
    public string TDate { get; set; } = string.Empty; // "dd-MM-yyyy"
    public string UserId { get; set; } = string.Empty; // optional - maps to @USERID
}

public class ReceiptDetailRequest
{
    public string Course { get; set; } = string.Empty;
    public string ExamMy { get; set; } = string.Empty;
    public string ReceiptNo { get; set; } = string.Empty;
}

public class SearchRegnoRequest
{
    public string Regno { get; set; } = string.Empty;
}
