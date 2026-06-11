namespace ICampus_Models.DTOs
{
    /// <summary>
    /// Returned by PROC_LOADMARKS_MRKENTRY (TYPE='E')
    /// Grid columns from AbsenteesEntry.aspx: aSHID (hidden), RegNo, grp, PCODE, CODE
    /// CODE holds the current absentee status: "AB" (Absent), "MP" (Malpractice), or null/empty
    /// </summary>
    public class AbsenteesStudentDto
    {
        public long aSHID { get; set; }
        public string RegNo { get; set; } = string.Empty;
        public string GRP { get; set; } = string.Empty;
        public string PCODE { get; set; } = string.Empty;
        public string? CODE { get; set; }   // "AB", "MP", or null if not yet marked
    }
}
