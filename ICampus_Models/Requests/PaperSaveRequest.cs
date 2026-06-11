using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICampus_Models.Requests
{
    public class PaperSaveRequest
    {
        public string? Regulation { get; set; }          // varchar(15)
        public int? ReguInt { get; set; }                // int (REGU)
        public string? PName { get; set; }               // varchar(255)
        public string? PCode { get; set; }               // varchar(20)
        public string? PType { get; set; }               // varchar(20)
        public int? MaxMrk { get; set; }                 // int
        public int? SMax { get; set; }                   // int
        public int? TMax { get; set; }                   // int
        public int? PMax { get; set; }                   // int
        public int? TPass { get; set; }                  // int
        public int? Pass { get; set; }                   // int
        public decimal? Credits { get; set; }            // decimal(10,2)
        public int? SemInt { get; set; }                 // int
        public string? Grp { get; set; }                 // varchar(20)
        public int? SPass { get; set; }                  // int
        public int? PPass { get; set; }                  // int
        public int? Part { get; set; }                   // int
        public string? SubCode { get; set; }             // varchar(10)
        public string? PTitle { get; set; }              // varchar(255)
        public int? P1MAX { get; set; }                  // int
        public int? P2MAX { get; set; }                  // int
        public int? ASGMAX { get; set; }                 // int
        public int? ATTMAX { get; set; }                 // int

        // ✅ Added: SUBCR (subject credits, matches @SUBCR in SP)
        public decimal? Sub_Cr { get; set; }             // decimal(10,2)

        public int? TIPASS { get; set; }                 // int
        public int? TPPASS { get; set; }                 // int
        public int? PIPASS { get; set; }                 // int
        public int? Stream { get; set; }                 // int
        public string? EntryType { get; set; }           // varchar(3)
        public string? Course { get; set; }              // varchar(15)
        public string? Elec_All { get; set; }            // varchar(4) -> @IsElective
        public string? IsElecBranch { get; set; }        // varchar(20)
        public string? PNameBranchwise { get; set; }     // varchar(500)

        // ✅ Added: Remarks (for @REMARKS in SP)
        public string? Remarks { get; set; }             // nvarchar(510)
    }

    public class PaperDeleteRequest
    {
        public string Regu { get; set; } = null!;
        public int Sem { get; set; }
        public string Branch { get; set; } = null!;
        public string PCode { get; set; } = null!;
    }

    public class PaperReorderRequest
    {
        public string Regu { get; set; } = null!;
        public int Sem { get; set; }
        public string Branch { get; set; } = null!;
        public string Course { get; set; } = null!;
        public string Stream { get; set; } = null!;
        // Ordered list of paper codes in desired order
        public List<string> OrderedPaperCodes { get; set; } = new();
    }

    public class PaperCopyRequest
    {
        public string Course { get; set; } = null!;
        public string ToBatch { get; set; } = null!;   // new batch (target)
        public string FromBatch { get; set; } = null!; // source batch
        public string Sem { get; set; } = null!;
        public string UserId { get; set; } = string.Empty; // set from token in controller
    }

    public class IsRegularRequest
    {
        public string Course { get; set; } = null!;
        public int Regu { get; set; }
        public int Sem { get; set; }
        public string Branch { get; set; } = null!;
    }
}
