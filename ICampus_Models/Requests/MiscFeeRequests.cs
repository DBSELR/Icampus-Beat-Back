namespace ICampus_Models.Requests
{
    public class MiscFeeItem
    {
        public string FeeType { get; set; } = string.Empty;  // FEETYPE
        public string FeeName { get; set; } = string.Empty;  // FEENAME
        public decimal JntukFee { get; set; } = 0m;          // JNTUK_FEE
        public decimal LbrceFee { get; set; } = 0m;          // LBRCE_FEE
        public int Count { get; set; } = 1;                  // Count
    }

    public class MiscFeeSaveRequest
    {
        public string ReceiptNo { get; set; } = string.Empty; // RECPTNO
        public string Date { get; set; } = string.Empty;      // "yyyy-MM-dd"
        public string Regno { get; set; } = string.Empty;     // REGNO
        public string Sem { get; set; } = string.Empty;       // SEM (string in old code)
        public decimal Concession { get; set; } = 0m;         // CONCESSION
        public string Remark { get; set; } = string.Empty;    // REMARK
        public string UserId { get; set; } = string.Empty;    // CREATEDID
        public string Course { get; set; } = "B.TECH";        // optional
        public List<MiscFeeItem> Items { get; set; } = new List<MiscFeeItem>();
    }

    public class MiscFeeDeleteRequest
    {
        public string ReceiptNo { get; set; } = string.Empty;
    }
}
