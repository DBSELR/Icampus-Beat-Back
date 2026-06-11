namespace ICampus_Models.Requests
{
    public class SaveCourseRequest
    {
        public string Regulation { get; set; } = null!;
        public string Batch { get; set; } = null!;        // expected like "20XX-20YY" or "XX" depending on UI
        public string Course { get; set; } = null!;
        public string CourseName { get; set; } = null!;
        public string Grp { get; set; } = null!;
        public string GrpName { get; set; } = null!;
        public int MaxSem { get; set; }
        public int MaxStreams { get; set; } = 1;
        public byte GrpOrder { get; set; } = 1;
        public bool IsUpdate { get; set; } = false; // if true then update flow
    }
}