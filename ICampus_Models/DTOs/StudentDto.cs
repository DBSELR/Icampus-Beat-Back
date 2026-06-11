using System;
using System.ComponentModel.DataAnnotations.Schema;

public class StudentDto
{
    public int? aStdID { get; set; }
    public string? Regulation { get; set; }
    public string? Regu { get; set; }
    public string? Course { get; set; }
    public string? GRP { get; set; }
    public int? Stream { get; set; }
    public string? RegNo { get; set; }
    public string? SName { get; set; }
    public string? FName { get; set; }
    public string? MName { get; set; }
    public string? Flag { get; set; }
    public string? SecLan { get; set; }
    public string? RollNum { get; set; }
    public string? Medium { get; set; }
    public string? Section { get; set; }
    public string? DOB { get; set; }
    public string? Caste { get; set; }
    public string? Gender { get; set; }

    [Column("Addres")]
    public string? Address { get; set; }

    public string? District { get; set; }

    [Column("sState")]
    public string? State { get; set; }

    public string? Mobile { get; set; }
    public string? Email { get; set; }
    public string? Pincode { get; set; }

    [Column("Isactive")]
    public string? IsActive { get; set; }

    public string? ReAdmission { get; set; }
    public byte[]? Photo { get; set; }
    public byte[]? Sign { get; set; }
    public string? CLASS { get; set; }
    public string? CCRV { get; set; }
    public string? PCNO { get; set; }

    // Semester Results
    public string? Sem1Res { get; set; }
    public DateTime? Sem1My { get; set; }
    public int? Sem1TMrk { get; set; }
    public int? Sem1GTMrk { get; set; }
    public int? Sem1TCR { get; set; }
    public int? Sem1TGRP { get; set; }

    public string? Sem2Res { get; set; }
    public DateTime? Sem2My { get; set; }
    public int? Sem2TMrk { get; set; }
    public int? Sem2GTMrk { get; set; }
    public int? Sem2TCR { get; set; }
    public int? Sem2TGRP { get; set; }

    public string? Sem3Res { get; set; }
    public DateTime? Sem3My { get; set; }
    public int? Sem3TMrk { get; set; }
    public int? Sem3GTMrk { get; set; }
    public int? Sem3TCR { get; set; }
    public int? Sem3TGRP { get; set; }

    public string? Sem4Res { get; set; }
    public DateTime? Sem4My { get; set; }
    public int? Sem4TMrk { get; set; }
    public int? Sem4GTMrk { get; set; }
    public int? Sem4TCR { get; set; }
    public int? Sem4TGRP { get; set; }

    public string? Sem5Res { get; set; }
    public DateTime? Sem5My { get; set; }
    public int? Sem5TMrk { get; set; }
    public int? Sem5GTMrk { get; set; }
    public int? Sem5TCR { get; set; }
    public int? Sem5TGRP { get; set; }

    public string? Sem6Res { get; set; }
    public DateTime? Sem6My { get; set; }
    public int? Sem6TMrk { get; set; }
    public int? Sem6GTMrk { get; set; }
    public int? Sem6TCR { get; set; }
    public int? Sem6TGRP { get; set; }

    public string? Sem7Res { get; set; }
    public DateTime? Sem7My { get; set; }
    public int? Sem7TMrk { get; set; }
    public int? Sem7GTMrk { get; set; }
    public int? Sem7TCR { get; set; }
    public int? Sem7TGRP { get; set; }

    public string? Sem8Res { get; set; }
    public DateTime? Sem8My { get; set; }
    public int? Sem8TMrk { get; set; }
    public int? Sem8GTMrk { get; set; }
    public int? Sem8TCR { get; set; }
    public int? Sem8TGRP { get; set; }

    public string? Sem9Res { get; set; }
    public DateTime? Sem9My { get; set; }
    public int? Sem9TMrk { get; set; }
    public int? Sem9GTMrk { get; set; }
    public int? Sem9TCR { get; set; }
    public int? Sem9TGRP { get; set; }

    public string? Sem10Res { get; set; }
    public DateTime? Sem10My { get; set; }
    public int? Sem10TMrk { get; set; }
    public int? Sem10GTMrk { get; set; }
    public int? Sem10TCR { get; set; }
    public int? Sem10TGRP { get; set; }

    // Totals
    public int? TMRK { get; set; }
    public int? GTMRK { get; set; }
    public int? TCR { get; set; }
    public int? TGRP { get; set; }
    public DateTime? CCMY { get; set; }
    public decimal? CGPA { get; set; }
    public string? Result { get; set; }
    public string? IsIAS { get; set; }

    // Part-wise
    public string? PART1MARKS { get; set; }
    public string? PART2MARKS { get; set; }
    public string? PART1CLASS { get; set; }
    public string? PART2CLASS { get; set; }
    public string? PART1EMY { get; set; }
    public string? PART2EMY { get; set; }
    public string? PART1CR { get; set; }
    public string? PART1TGRP { get; set; }
    public string? PART1CGPA { get; set; }
    public string? PART2CR { get; set; }
    public string? PART2TGRP { get; set; }
    public string? PART2CGPA { get; set; }

    // Misc
    public string? ATT { get; set; }
    public string? NCC { get; set; }
    public string? SPORTS { get; set; }
    public string? OTHERS { get; set; }
    public int? SNO { get; set; }
    public string? isEnroll { get; set; }

    [Column("AADHARNO")]
    public string? AadhaarNo { get; set; }

    public string? MOLE1 { get; set; }
    public string? MOLE2 { get; set; }
    public string? PARENTMOBILE { get; set; }
    public string? PWD { get; set; }

    [Column("REMARKS")]
    public string? Remarks { get; set; }

    public string? OutOFReg { get; set; }
    public string? CGCNO { get; set; }
    public string? Readmit_Batch { get; set; }
    public string? READMIT_REGULATION { get; set; }
    public string? PC_Number { get; set; }

    [Column("4CrGracing_Status")]
    public string? FourCrGracing_Status { get; set; }

    public string? Nationality { get; set; }
    public string? Religion { get; set; }
    public string? AdmissionNo { get; set; }
    public string? TC_Sno { get; set; }
    public DateTime? DateOfAdmitted { get; set; }

    [NotMapped]
    public string? PhotoUrl { get; set; }

    [NotMapped]
    public string? SignatureUrl { get; set; }
}

public class StudentListDto
{
    public string? Regu { get; set; }
    public string? RegNo { get; set; }
    public string? SName { get; set; }
    public string? FName { get; set; }
    public string? Gender { get; set; }
    public string? Caste { get; set; }
    public string? Course { get; set; }
    public string? GRP { get; set; }
    public string? SecLan { get; set; }
    public string? Medium { get; set; }
    public string? Isactive { get; set; }
}

