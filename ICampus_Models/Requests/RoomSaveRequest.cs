public class RoomSaveRequest
{
    public string RoomNo { get; set; } = string.Empty;
    public byte? NoOfColumns { get; set; } = null;
    public byte? NoOfRows { get; set; } = null;
    public int Priority { get; set; } = 0;
    public int Capacity { get; set; } = 0;
    public int? Sem { get; set; } = null;
    public int TotalBranches { get; set; } = 0;
    public string DaySession { get; set; } = string.Empty; // maps to DaySession
    public string RoomType { get; set; } = string.Empty;
    public string Course { get; set; } = string.Empty;
    public string ExamMy { get; set; } = string.Empty;
}

public class UpdatePriorityRequest
{
    // Legacy DAL expected full raw query stored in Up_Q; to remain compatible we accept UpdateQuery.
    public string UpdateQuery { get; set; } = string.Empty;
}

public class DeleteRoomRequest
{
    public string RoomNo { get; set; } = string.Empty;
}

public class BranchPrioritySaveRequest
{
    public int Priority { get; set; } = 0;
    public int? Sem { get; set; } = null;
    public string Branch { get; set; } = string.Empty;
    public string Session { get; set; } = string.Empty;
    public string Course { get; set; } = string.Empty;
    public string ExamMy { get; set; } = string.Empty;
}

public class BranchPriorityDeleteRequest
{
    public string Priority { get; set; } = string.Empty;
    public string Branch { get; set; } = string.Empty;
    public string Session { get; set; } = string.Empty;
}
