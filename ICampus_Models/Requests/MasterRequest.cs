public class UpdatePapRequest
{
    public string PName { get; set; }
    public string TMax { get; set; }
    public string TPass { get; set; }
    public string PMax { get; set; }
    public string PPass { get; set; }
    public string SMax { get; set; }
    public string SPass { get; set; }
    public string MaxMrk { get; set; }
    public string Pass { get; set; }
    public string Credits { get; set; }
    public string PID { get; set; }
}

public class CreateMasterRequest
{
    public string Course { get; set; }
    public string ExamMy { get; set; }
    public string Regu { get; set; } // batch
    public string Sem { get; set; }
}

