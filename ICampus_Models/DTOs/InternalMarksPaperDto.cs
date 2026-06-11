namespace ICampus_Models.DTOs
{
    /// <summary>
    /// Returned by PROC_LOADPAPERS_MRKENTRY (TYPE='I')
    /// Columns: PCODE, PName (PCODE + '-' + PNAME)
    /// </summary>
    public class InternalMarksPaperDto
    {
        public string PCODE { get; set; } = string.Empty;
        public string PName { get; set; } = string.Empty;
    }
}
