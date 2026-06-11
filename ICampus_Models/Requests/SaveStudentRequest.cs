namespace ICampus_Models.Requests
{
    public class SaveStudentRequest
    {
        public string RegNo { get; set; }
        public string RollNo { get; set; }
        public string Section { get; set; }
        public string Sem { get; set; }
        public string Medium { get; set; }
        public string Name { get; set; }
        public string FatherName { get; set; }
        public string MotherName { get; set; }
        public string DOB { get; set; }
        public string Gender { get; set; }
        public string District { get; set; }
        public string Pincode { get; set; }
        public string Email { get; set; }
        public string Caste { get; set; }
        public string Address { get; set; }
        public string State { get; set; }
        public string Mobile { get; set; }
        public string Regu { get; set; }   // Batch alias
        public string Batch { get; set; }
        public string Course { get; set; }
        public string Branch { get; set; }
        public int Stream { get; set; }
        public bool IsReadmitted { get; set; }
        public string NewRegNo { get; set; }
        public bool IsActive { get; set; }
        public string Regulation { get; set; }
        public string Remarks { get; set; }
        public string UserID { get; set; }
        public string EXAMMY { get; set; }
        public string AadhaarNo { get; set; }
        public string Nationality { get; set; }
        public string Religion { get; set; }
        public string Mole1 { get; set; }
        public string Mole2 { get; set; }
        public string AdmissionNo { get; set; }
        public string AdmissionDate { get; set; }
    }

    public class InactivateRequest
    {
        public string RegNo { get; set; }
        public int Semester { get; set; }  
        public string Remarks { get; set; }
    }

    public class ReactivateRequest
    {
        public string OldRegNo { get; set; }
        public string NewRegNo { get; set; }
        public int Batch { get; set; }
    }

    public class ReadmissionRequest
    {
        public string RegNo { get; set; }       // maps to @OLDREGNO
        public string NewRegNo { get; set; }    // maps to @NewRegno
        public string Batch { get; set; }       // maps to @REGU (nvarchar(3))
        public int Semester { get; set; }       // maps to @sem (int)
    }

    public class UploadFileRequest
    {
        public string RegNo { get; set; }
        // No IFormFile here, keep it clean
    }

}
