namespace ICampus_Models.Requests
{
    public class DeleteCourseRequest
    {
        public string Batch { get; set; } = null!;
        public string Course { get; set; } = null!;
        public string Grp { get; set; } = null!;
        public string CourseName { get; set; } = null!;
        public string GrpName { get; set; } = null!;
        public byte GrpOrder { get; set; }
    }
}
