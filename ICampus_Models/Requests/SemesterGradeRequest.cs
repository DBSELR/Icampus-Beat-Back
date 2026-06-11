using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICampus_Models.Requests
{
    public class SemGradeSaveRequest
    {
        public string Id { get; set; } = string.Empty;         // maps to @ID
        public string Regu { get; set; } = string.Empty;       // @REGU
        public string SgpaFrom { get; set; } = string.Empty;   // @MRKFROM
        public string SgpaTo { get; set; } = string.Empty;     // @MRKTO
        public string Grade { get; set; } = string.Empty;      // @GR
        public string Course { get; set; } = string.Empty;     // @course
    }

    public class CopySemesterGradeRequest
    {
        public string FromBatch { get; set; } = string.Empty;  // @PREGU
        public string ToBatch { get; set; } = string.Empty;    // @REGU
        public string Course { get; set; } = string.Empty;     // @COURSE
        public string Type { get; set; } = "TBL_SEMGRADE";      // @TYPE (TBL_SEMGRADE | TBL_GRADE)
    }

    public class DeleteRequest
    {
        public int Id { get; set; }
    }

}
