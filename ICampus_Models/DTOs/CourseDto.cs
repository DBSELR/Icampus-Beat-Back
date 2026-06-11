namespace ICampus_Models.DTOs
{
    public class CourseDto
    {
        // From TBL_COURSE schema (nvarchar)
        public string COURSE { get; set; }

        // From TBL_COURSE schema (nvarchar)
        public string GRP { get; set; }

        // From TBL_COURSE schema (nvarchar)
        public string GSUB { get; set; }

        // Computed column (GRP + '-' + GSUB)
        public string BRANCH { get; set; }

        // From TBL_COURSE schema (nvarchar)
        public string DEGREE { get; set; }

        // From TBL_COURSE schema (nvarchar)
        public string REGU { get; set; }

        // From TBL_COURSE schema (int)
        public int MAXSEM { get; set; }

        // From TBL_COURSE schema (varchar)
        public string REGULATION { get; set; }

        // From TBL_COURSE schema (tinyint → maps to byte)
        public byte MAX_STREAMS { get; set; }

        // From TBL_COURSE schema (tinyint → maps to byte)
        public byte GRP_ORDER { get; set; }

        // Computed in SP → '20' + CAST(REGU AS VARCHAR) + '-' + (REGU + MAXSEM/2)
        public string? BATCH { get; set; }
    }
}
