namespace ICampus_Models.DTOs
{
    /// <summary>
    /// Returned by PROC_LOADPAPERS_MRKENTRY (TYPE='P')
    /// Only papers with PMAX > 0 (practical marks applicable)
    /// </summary>
    public class PracticalMarksPaperDto
    {
        public string PCODE { get; set; } = string.Empty;
        public string PName { get; set; } = string.Empty;
    }
}
