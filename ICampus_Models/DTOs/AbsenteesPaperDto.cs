namespace ICampus_Models.DTOs
{
    /// <summary>
    /// Returned by PROC_LOADPAPERS_MRKENTRY (TYPE='E')
    /// Papers for the external exam absentees entry dropdown
    /// </summary>
    public class AbsenteesPaperDto
    {
        public string PCODE { get; set; } = string.Empty;
        public string PName { get; set; } = string.Empty;
    }
}
