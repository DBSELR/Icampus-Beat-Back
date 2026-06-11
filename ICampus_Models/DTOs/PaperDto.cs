using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICampus_Models.DTOs
{
   // public class RegulationDto { public string REGULATION { get; set; } = null!; }

    //public class CourseDto { public string COURSE { get; set; } = null!; }

    public class BatchDto
    {
        public string REGU { get; set; } = null!;
        public string BATCH { get; set; } = null!;
    }

    public class BranchDto
    {
        public string GRP { get; set; } = null!;
        public string BRANCH { get; set; } = null!;
    }

    public class SemDto { public string Sem { get; set; } = null!; }

    public class StreamDto { public string Stream { get; set; } = null!; }

    public class PaperListDto
    {
        public string PCODE { get; set; } = null!;
        public int? PNO { get; set; }
    }

    public class PaperDetailDto
    {
        public int PID { get; set; }
        public string? REGULATION { get; set; } = null!;
        public string? REGU { get; set; } = null!;
        public string? COURSE { get; set; } = null!;
        public string? GRP { get; set; }

        // 🔄 Fix: SEM is varchar in DB, so string
        public string SEM { get; set; } = null!;

        // 🔄 Fix: STREAM is int in DB, so int?
        public int? STREAM { get; set; }

        public string? PCODE { get; set; } = null!;
        public string? ELEC { get; set; }
        public string? PNAME { get; set; } = null!;
        public string? TempCode { get; set; } = null!;
        public string? PTYPE { get; set; } = null!;
        public int? MAXMRK { get; set; }
        public int? SMAX { get; set; }
        public int? TMAX { get; set; }
        public int? PMAX { get; set; }
        public int? TPASS { get; set; }
        public int? PASS { get; set; }
        public int? TIPASS { get; set; }
        public decimal? CREDITS { get; set; }
        public string? REMARKS { get; set; }
        public int? PPass { get; set; }
        public int? SPass { get; set; }

        // 🔄 Fix: Part is int in DB
        public int? Part { get; set; }

        public string? SUBCODE { get; set; }
        public string? PTitle { get; set; }
        public int? P1MAX { get; set; }
        public int? P2MAX { get; set; }
        public int? ASGMAX { get; set; }
        public int? ATTMAX { get; set; }
        public int? PAPID { get; set; }
        public int? TPPASS { get; set; }
        public int? PIPASS { get; set; }
        public string? EntryType { get; set; }
        public decimal? sub_Cr { get; set; }
        public string? Readmission_np { get; set; }
        public int? Old_TPASS { get; set; }
        public int? Old_PPASS { get; set; }
        public string? GGRP { get; set; }
        public string? PNAME_BRANCHWISE { get; set; }
        public string? ELEC_BRANCH { get; set; }
        public string? ELEC_ALL { get; set; }
    }

    public class ExamDto
    {
        public string EXAMMY { get; set; }
    }

  
    public class RegCheckDto
    {
        public int Count { get; set; }
    }


}

