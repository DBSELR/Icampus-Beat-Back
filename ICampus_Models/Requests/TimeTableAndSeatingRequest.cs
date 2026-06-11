public class UpdateExamSessionRequest
{
    public string ExamMy { get; set; } = string.Empty;
    public string Course { get; set; } = string.Empty;
    public int Sem { get; set; } = 0;
    public string Session { get; set; } = string.Empty;
    public string ExamTime { get; set; } = string.Empty; // ETIME
    public string Regulation { get; set; } = string.Empty;
}

public class UpdateExamDateRequest
{
    public string ExamMy { get; set; } = string.Empty;
    public string Course { get; set; } = string.Empty;
    public int Sem { get; set; } = 0;
    public string PCode { get; set; } = string.Empty;
    public string EDate { get; set; } = string.Empty;     // in 'yyyy-MM-dd' expected by SP
    public string Session { get; set; } = string.Empty;   // ESESS
    public string ExamTime { get; set; } = string.Empty;  // ETIME
    public string Regulation { get; set; } = string.Empty;
    public string Branch { get; set; } = "ALL BRANCHES";   // GRP / branch
    public string ExamType { get; set; } = string.Empty;   // External|MID-I|MID-II
    public string Remarks { get; set; } = string.Empty;
}

public class UpdateRoomNumbersRequest
{
    public string ExamMy { get; set; } = string.Empty;
    public string Course { get; set; } = string.Empty;
    public int Sem { get; set; } = 0;
    public string PCode { get; set; } = string.Empty;
    public string FromRegNo { get; set; } = string.Empty;
    public string ToRegNo { get; set; } = string.Empty;
    public string Room { get; set; } = string.Empty;
    public string Regulation { get; set; } = string.Empty;
}
