namespace ICampus_Models.DTOs
{
    /// <summary>
    /// Grid row returned by PROC_OMRNUM_UPDATE_Get
    /// SP actual columns: ASHID, REGNO, SEM, SNAME, BATCH, TEMPCODE, PCODE, PNAME,
    ///                    PNO, MRK_FIN, OMRNUMBER, smarks, pmarks, TMARKS, RVMARKS, V3, PTYPE, exammy
    /// </summary>
    public class OmrMarksEntryDto
    {
        public long   ASHID      { get; set; }
        public string REGNO      { get; set; } = string.Empty;
        public string SNAME      { get; set; } = string.Empty;   // student name
        public string BATCH      { get; set; } = string.Empty;   // REGU e.g. "20"
        public string SEM        { get; set; } = string.Empty;
        public string TEMPCODE   { get; set; } = string.Empty;
        public string PCODE      { get; set; } = string.Empty;
        public string PNAME      { get; set; } = string.Empty;
        public string? OMRNUMBER { get; set; }
        public string? exammy    { get; set; }
        public int    PTYPE      { get; set; }   // 1 = Theory Internal (TI), 0 = other
        public string? smarks    { get; set; }
        public string? TMARKS    { get; set; }
        public string? pmarks    { get; set; }
        public string? RVMARKS   { get; set; }
        public string? V3        { get; set; }   // SP returns "V3" not "V3MARKS"
        public string? MRK_FIN   { get; set; }
    }
}
