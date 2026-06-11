public class ClassGradeSaveRequest
{
    public string Id { get; set; } = string.Empty;       // maps to @ID
    public string Regu { get; set; } = string.Empty;     // @REGU
    public decimal SgpaFrom { get; set; } = 0m;          // @MRKFROM (SP expects varchar, we convert to string)
    public decimal SgpaTo { get; set; } = 0m;            // @MRKTO
    public string ClassName { get; set; } = string.Empty; // @Class -> CLASS column
    public string Course { get; set; } = string.Empty;   // @Course
}

public class CopyClassGradeRequest
{
    public string FromRegu { get; set; } = string.Empty; // previous regu (PREGU)
    public string ToRegu { get; set; } = string.Empty;   // new regu (REGU)
    public string Course { get; set; } = string.Empty;
    public string ProcType { get; set; } = string.Empty; // optional for callers (kept for parity)
}

public class IdDeleteRequest
{
    public int Id { get; set; } = 0;
}
