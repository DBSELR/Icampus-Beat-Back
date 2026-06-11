namespace ICampus_Models.DTOs
{
    /// <summary>
    /// Returned by PROC_LOADMARKS_MRKENTRY_MID (TYPE='T')
    /// Grid columns from AbsenteesEntry_Mid.aspx: aSHID (hidden), RegNo, grp, PCODE, MIDCODE
    /// MIDCODE holds the current absentee status: "AB" (Absent), "MP" (Malpractice), or null/empty
    /// Key difference from AbsenteesStudentDto: MIDCODE column (not CODE)
    /// </summary>
    public class MidAbsenteesStudentDto
    {
        public long aSHID { get; set; }
        public string RegNo { get; set; } = string.Empty;
        public string GRP { get; set; } = string.Empty;
        public string PCODE { get; set; } = string.Empty;
        public string? MIDCODE { get; set; }   // "AB", "MP", or null if not yet marked
    }
}
