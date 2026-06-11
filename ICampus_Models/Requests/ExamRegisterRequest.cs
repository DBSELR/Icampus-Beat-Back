namespace ICampus_Models.Requests
{
    public class ExamRegisterRequest
    {
        public string RegNo { get; set; }
        public string Sem { get; set; }
        public string ExamMy { get; set; }
        public string UserId { get; set; }
        public bool IsSupply { get; set; } = false;
        public string Course { get; set; }
        public string Regulation { get; set; }
        public string RegSup { get; set; } // "REG" or "SUP"
        // Fee payment data
        public decimal ExamFee { get; set; }
        public decimal FineFee { get; set; }
        public decimal AppFee { get; set; }
        public decimal Concession { get; set; }
        public decimal TotalAmount { get; set; }
        public List<PaperFeeInfo> Papers { get; set; } = new List<PaperFeeInfo>();
    }

    public class PaperFeeInfo
    {
        public string Sem { get; set; }
        public string Papers { get; set; } // Comma-separated paper codes
        public int PCount { get; set; }
        public string RegSup { get; set; }
        public decimal ExamFee { get; set; }
        public decimal FineFee { get; set; }
    }

    public class UnRegisterRequest
    {
        public string RegNo { get; set; }
        public string ExamMy { get; set; }
    }

    public class ExamFeePayRequest
    {
        public string ExFeePay { get; set; }
        public decimal AppFee { get; set; }
        public decimal Concession { get; set; }
    }

    /// <summary>
    /// Request for Register All / Unregister All bulk operation (btnRegAll_Click modal)
    /// Confirmed from DLL IL (DataAccessLayer.dll Regular_Exmas_Update / Regular_Exmas_Update_UnRegester):
    ///   Register All:   UPDATE TBL_SH SET REGD = 'Y'   WHERE COURSE = ? AND REGU = ? AND SEM = ?
    ///   Unregister All: UPDATE TBL_SH SET REGD = null   WHERE COURSE = ? AND REGU = ? AND SEM = ?
    /// Regu comes from ddlBatch_Sem regusem value (e.g. "20" for batch 2020)
    /// Sem  comes from ddlBatch_Sem regusem value (e.g. "2"  for semester 2)
    /// </summary>
    public class RegisterAllRequest
    {
        public string Course { get; set; }
        public string Regu { get; set; }   // batch year e.g. "20"
        public string Sem { get; set; }    // semester e.g. "2"
    }
}