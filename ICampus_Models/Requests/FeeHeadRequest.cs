// ICampus_Models.Requests/FeeHeadRequest.cs
namespace ICampus_Models.Requests
{
    public class FeeHeadRequest
    {
        public int? ID { get; set; } // null or 0 for insert
        public string COURSE { get; set; } = "B.TECH";
        public string FEETYPE { get; set; } = string.Empty;
        public string FEENAME { get; set; } = string.Empty;
        public string SHORTNAME { get; set; } = string.Empty;
        public decimal JNTUAMOUNT { get; set; } = 0m;
        public decimal LBRAMOUNT { get; set; } = 0m;
    }
}
