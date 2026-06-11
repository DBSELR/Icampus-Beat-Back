namespace ICampus_Models.DTOs
{
    /// <summary>
    /// Returned by PROC_LOADMARKS_MRKENTRY (TYPE='I')
    /// Columns: aSHID, RegNo, PCODE, SMARKS, SMAX, GRP
    /// Maps to tbl_SH joined with tbl_Pap
    /// </summary>
    public class InternalMarksStudentDto
    {
        public long aSHID { get; set; }
        public string RegNo { get; set; } = string.Empty;
        public string PCODE { get; set; } = string.Empty;
        public int? SMARKS { get; set; }
        public int SMAX { get; set; }
        public string GRP { get; set; } = string.Empty;
    }
}
