namespace ICampus_Models.DTOs
{
    /// <summary>
    /// Returned by PROC_LOADMARKS_MRKENTRY (TYPE='P')
    /// Grid columns from PracticalMarksEntry.aspx: aSHID (hidden), RegNo, grp, PCODE, PMARKS, pMax (hidden)
    /// </summary>
    public class PracticalMarksStudentDto
    {
        public long aSHID { get; set; }
        public string RegNo { get; set; } = string.Empty;
        public string GRP { get; set; } = string.Empty;
        public string PCODE { get; set; } = string.Empty;
        public string? PMARKS { get; set; }   // nullable — null means not yet entered
        public int pMax { get; set; }      // max practical marks (PMAX from tbl_Pap)
    }
}
