public class ExamMasterDto
{
    public int AEXAMID { get; set; }
    public string COURSE { get; set; } = string.Empty;
    public string EXAMMY { get; set; } = string.Empty;
    public string ETYPE { get; set; } = string.Empty;
    public string? REMARKS { get; set; }
    public string? EXSEMS { get; set; }
    public string? REGSEM { get; set; }
    public string? SUPSEM { get; set; }
    public string? SEMS { get; set; }
    public string? REGULATION { get; set; }
}

public class ExistingExamDto
{
    public string ETYPE { get; set; } = string.Empty;
    public string? EXSEMS { get; set; }
    public string? SUPSEM { get; set; }
    public string? REGU { get; set; }
    public string? SEM { get; set; }
    public string? CNAME { get; set; }
    public string? REGULATION { get; set; }
}

public class ExamNotificationDto
{
    public int Enid { get; set; }            // was string
    public int Nnum { get; set; }            // was string
    public string Semesters { get; set; }    // if DB returns varchar
    public DateTime NDate { get; set; }     // match DB type
    public DateTime ExReg_Date { get; set; }
    public DateTime ExReg_End_Date { get; set; }
    public string Regulation { get; set; }
}
