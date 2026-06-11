namespace ICampus_Models.Requests
{
    public class RoomAbstractRequest
    {
        public string Course { get; set; } = string.Empty;       // Required
        public string ExamMY { get; set; } = string.Empty;      // Required
        public string Sem { get; set; } = string.Empty;         // Required
        public int Session { get; set; }                        // Required (integer)
        public string EDate { get; set; } = string.Empty;      // Required (format: yyyy-MM-dd)
        public string ExamType { get; set; } = string.Empty;    // Required (1=External, 2=MID-I, 3=MID-II)
        public int Regsup { get; set; }                         // Required (0=Select/Reg, 1=Reg, 2=Sup)
    }
}

