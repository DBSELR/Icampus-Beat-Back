public class ExamsSaveRequest
{
    public string ExamMY { get; set; } = string.Empty;      // @EXAMMY
    public string EType { get; set; } = string.Empty;       // @ETYPE
    public string Remarks { get; set; } = string.Empty;     // @REMARKS
    public string Sems { get; set; } = string.Empty;        // @SEMS
    public string ExSems { get; set; } = string.Empty;      // @EXSEMS
    public string Course { get; set; } = string.Empty;      // @COURSE
    public string RegSem { get; set; } = string.Empty;      // @REGSEM
    public string SupSem { get; set; } = string.Empty;      // @SUPSEM
    public string Regulation { get; set; } = string.Empty;  // @REGULATION
}

public class ExamNotificationSaveRequest
{
    public string ENID { get; set; } = "0";      // pass "0" for insert
    public string NNumber { get; set; } = string.Empty;
    public string ExamMY { get; set; } = string.Empty;
    public string Course { get; set; } = string.Empty;
    public string Sem { get; set; } = string.Empty;
    public string NDate { get; set; } = string.Empty;          // "yyyy-MM-dd"
    public string ExRegDate { get; set; } = string.Empty;     // "yyyy-MM-dd"
    public string ExRegEndDate { get; set; } = string.Empty;  // "yyyy-MM-dd"
    public string Regulation { get; set; } = string.Empty;
}

public class ExamWithNotificationSaveRequest
{
    // Exam-related fields
    public string ExamMY { get; set; } = string.Empty;
    public string EType { get; set; } = string.Empty;
    public string Remarks { get; set; } = string.Empty;
    public string Sems { get; set; } = string.Empty;
    public string ExSems { get; set; } = string.Empty;
    public string Course { get; set; } = string.Empty;
    public string RegSem { get; set; } = string.Empty;
    public string SupSem { get; set; } = string.Empty;
    public string Regulation { get; set; } = string.Empty;

    // Notification-related fields
    public string ENID { get; set; } = "0";
    public string NNumber { get; set; } = string.Empty;
    public string Sem { get; set; } = string.Empty;
    public string NDate { get; set; } = string.Empty;
    public string ExRegDate { get; set; } = string.Empty;
    public string ExRegEndDate { get; set; } = string.Empty;
}

public class RegsUpSaveRequest
{
    public string Regu { get; set; } = string.Empty;      // @Regu (regulation/batch)
    public int Sem { get; set; } = 0;                     // @Sem
    public string Mode { get; set; } = string.Empty;      // @Mode
    public string ExamMy { get; set; } = string.Empty;    // @ExamMY
    public string Batch { get; set; } = string.Empty;     // @Batch
    public string Course { get; set; } = string.Empty;    // @Course
    public string Cname { get; set; } = string.Empty;    // @cname
}