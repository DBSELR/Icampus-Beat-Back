public class CancelReceiptRequest
{
    public string ReceiptNo { get; set; } = string.Empty;
    public string RegNo { get; set; } = string.Empty;    // for reference only (SP only needs ReceiptNo + UserId)
    public string UserId { get; set; } = string.Empty;
}

public class ReceiptSubjectsRequest
{
    public string RegNo { get; set; } = string.Empty;
    public string ExamMy { get; set; } = string.Empty;
}
