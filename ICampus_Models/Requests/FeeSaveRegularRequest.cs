using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICampus_Models.Requests
{
    public class FeeSaveRegularRequest
    {
        public string Batch { get; set; } = string.Empty;
        public string Regu { get; set; } = string.Empty;      // REGU
        public string Sem { get; set; } = string.Empty;       // SEM (string because DAL mixes types)
        public string Course { get; set; } = string.Empty;
        public string Grp { get; set; } = string.Empty;       // GRP
        public int FromPap { get; set; } = 1;
        public int ToPap { get; set; } = 1;
        public decimal Amount { get; set; } = 0m;
        public string Stat { get; set; } = "R";
        public string ExamMy { get; set; } = string.Empty;
        public string Regulation { get; set; } = string.Empty;
        public bool AllGrp { get; set; } = false; // maps to @ALLGRP char Y/N
    }

    public class SupplyFeeSaveRequest
    {
        public string Grp { get; set; } = string.Empty;       // branch or null
        public string PType { get; set; } = string.Empty;     // 'T'|'P'|'I'
        public string ExamMy { get; set; } = string.Empty;
        public int FCount { get; set; } = 0;
        public decimal Amount { get; set; } = 0m;
        public string SType { get; set; } = "SAVE_FEE";       // 'CHK_FEE'|'SAVE_FEE'|'SAVE_PRAC_FEE'
        public string Course { get; set; } = string.Empty;
        public string Regulation { get; set; } = string.Empty;
    }

    public class FineSaveRequest
    {
        public string Course { get; set; } = string.Empty;
        public string ExamMy { get; set; } = string.Empty;
        public int Sem { get; set; } = 0;
        public decimal FineAmt { get; set; } = 0m;
        public DateTime FromDate { get; set; } = DateTime.MinValue;
        public DateTime ToDate { get; set; } = DateTime.MinValue;
        public int? Fid { get; set; } = null;
        public string SType { get; set; } = "SAVE_FEE"; // 'CHK_FEE'|'SAVE_FEE'|'DEL_FEE'
        public string Regulation { get; set; } = string.Empty;
    }

    public class CondinationDateSaveRequest
    {
        public string Course { get; set; } = string.Empty;
        public string Regulation { get; set; } = string.Empty;
        public string ExamMy { get; set; } = string.Empty;
        public int Sem { get; set; } = 0;
        public DateTime FromDate { get; set; } = DateTime.MinValue;
        public DateTime ToDate { get; set; } = DateTime.MinValue;
    }

}
