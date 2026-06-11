public class BranchSaveRequest
{
    public int? Priority { get; set; }          // INT → nullable, since SP auto-generates if null/0
    public int? Sem { get; set; }               // INT → optional semester number
    public string Branch { get; set; } = string.Empty;      // VARCHAR(10)
    public string DaySession { get; set; } = string.Empty;  // VARCHAR(50)
    public string Course { get; set; } = string.Empty;      // VARCHAR(50)
    public string ExamMy { get; set; } = string.Empty;      // VARCHAR(50)
}

public class BranchPriorityUpdateRequest
{
    // original code accepted raw SQL in Up_Q string. Keep same shape.
    public string UpdateQuery { get; set; } = string.Empty;
}

public class DeleteBranchRequest
{
    public string Priority { get; set; } = string.Empty;
    public string Sem { get; set; } = string.Empty;
    public string Branch { get; set; } = string.Empty;
    public string EDate { get; set; } = string.Empty;
    public string Session { get; set; } = string.Empty;
}

public class RoomMasterListRequest
{
    public int? Id { get; set; }           // maps to RM.ID (SPM_RoomMaster_List / SPM_BranchPriority_List)
    public string Session { get; set; } = string.Empty; // maps to RM.Session (DaySession)
}

// For raw update query behavior (legacy Up_Q)
public class RawUpdateRequest
{
    // e.g. "SETPRIORITY ... CONDITION ..." - we'll replace tokens to produce final UPDATE statement
    public string UpdateQuery { get; set; } = string.Empty;
}

// Delete branch priority request
public class DeleteBranchPriorityRequest
{
    public int Priority { get; set; }
    public string Branch { get; set; } = string.Empty;
    public string DaySession { get; set; } = string.Empty;
}

// For check priority
public class CheckPriorityRequest
{
    public int Priority { get; set; }
}
