using System;
using Microsoft.EntityFrameworkCore;

namespace ICampus_Models.DTOs
{
    // ===============================
    // DTO: Fee Structure (SPM_FEESTUCTURE_LOAD)
    // ===============================
    [Keyless]
    public class FeeStructureRowDto
    {
        public string Regu { get; set; } = string.Empty;          // F.REGU
        public string Batch { get; set; } = string.Empty;         // F.BATCH
        public string Sem { get; set; } = string.Empty;           // dbo.ToRoman(F.SEM)
        public string Grp { get; set; } = string.Empty;           // F.GRP
        public decimal Amount { get; set; }                       // F.AMOUNT
        public string Branch { get; set; } = string.Empty;        // F.GRP + '-' + C.GSUB
    }

    // ===============================
    // DTO: Supply Fee (for another SP)
    // ===============================
    [Keyless]
    public class SupplyFeeRowDto
    {
        public int Id { get; set; }                               // Record ID
        public int FCount { get; set; }                           // Number of papers
        public decimal Amount { get; set; }                       // Fee Amount
        public string PType { get; set; } = string.Empty;         // Paper Type (Regular / Supply)
    }

    // ===============================
    // DTO: Fine Fee (used in PROC_LOAD_SEMS_FOR_FEE, TYPE = 'FINE')
    // ===============================
    [Keyless]
    public class FineRowDto
    {
        public int Fid { get; set; }                              // F.FID
        public int Sem { get; set; }                              // Semester (optional)
        public DateTime FromDate { get; set; }                    // F.FROMDATE
        public DateTime ToDate { get; set; }                      // F.TODATE
        public decimal FineAmount { get; set; }                   // F.FINEAMOUNT
        public string Course { get; set; } = string.Empty;        // F.COURSE
    }

    // ===============================
    // DTO: Semester List (PROC_LOAD_SEMS_FOR_FEE, TYPE = 'SEMS')
    // ===============================
    [Keyless]
    public class FeeSemResultDto
    {
        public string Regu { get; set; } = string.Empty;          // R.REGU
        public string Sem { get; set; } = string.Empty;           // R.SEM
        public string Batch { get; set; } = string.Empty;         // R.BATCH
    }

    // ===============================
    // DTO: Branch Group (PROC_LOAD_SEMS_FOR_FEE, TYPE = 'GRP')
    // ===============================
    [Keyless]
    public class FeeBranchResultDto
    {
        public string Grp { get; set; } = string.Empty;           // C.GRP
        public string Branch { get; set; } = string.Empty;        // C.GRP + '-' + C.GSUB
    }

    // ===============================
    // DTO: Re-Supply Semesters (PROC_LOAD_SEMS_FOR_FEE, TYPE = 'RSUPSEMS')
    // ===============================
    [Keyless]
    public class FeeResupplySemDto
    {
        public string Sem { get; set; } = string.Empty;           // ITEMS from SPLIT(@SEM, ' ')
    }
}
