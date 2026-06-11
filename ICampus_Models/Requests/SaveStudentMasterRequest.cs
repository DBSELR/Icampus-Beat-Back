public class SaveStudentMasterRequest
{
    public string Course { get; set; } = string.Empty;
    public string ExamMy { get; set; } = string.Empty;
    public string Batch { get; set; } = string.Empty;  // maps to REGU
    public string Sem { get; set; } = string.Empty;
    public string Regno { get; set; } = string.Empty;
}

public class UpdateOmrRequest
{
    public string Course { get; set; } = string.Empty;
    public string Regulation { get; set; } = string.Empty;
    public string ExamMy { get; set; } = string.Empty;
    public int Sem { get; set; } = 0;
    public int OmrNo { get; set; } = 0;
    public int AshId { get; set; } = 0;
}

public class StdUpdateRequest
{
    public string ActExamMy { get; set; } = string.Empty;
    public string Regulation { get; set; } = string.Empty;
    public string Regu { get; set; } = string.Empty;
    public string Course { get; set; } = string.Empty;
    public string Grp { get; set; } = string.Empty;
    public string Sem { get; set; } = string.Empty;
    public string Regno { get; set; } = string.Empty;
    public string ExamMy { get; set; } = string.Empty;
}

public class MarksUpdateRequest
{
    public int AshId { get; set; }
    public string SMarks { get; set; } = string.Empty;
    public string PMarks { get; set; } = string.Empty;
    public string TMarks { get; set; } = string.Empty;
    public string RvMarks { get; set; } = string.Empty;
    public string V3Marks { get; set; } = string.Empty;
    public string MrkFinal { get; set; } = string.Empty;
}

public class DeleteAshIdRequest
{
    public int AshId { get; set; }
}
