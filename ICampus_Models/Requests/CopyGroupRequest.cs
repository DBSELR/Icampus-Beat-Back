namespace ICampus_Models.Requests
{
    public class CopyGroupRequest
    {
        public string Regulation { get; set; } = null!;
        public string ToBatch { get; set; } = null!;   // @REGU
        public string FromBatch { get; set; } = null!; // @PREGU
        public string Course { get; set; } = null!;
    }
}
