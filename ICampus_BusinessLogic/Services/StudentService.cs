using ICampus_BusinessLogic.Interfaces;
using ICampus_DataAccessLayer.Helpers;
using ICampus_DataAccessLayer.Interfaces;
using ICampus_Models.DTOs;
using ICampus_Models.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Services
{
    public class StudentService : IStudentService
    {
        private readonly IGenericRepository<StudentDto> _studentRepo;
        private readonly IGenericRepository<StudentListDto> _studentListRepo;

        public StudentService(IGenericRepository<StudentDto> studentRepo, IGenericRepository<StudentListDto> studentListRepo)
        {
            _studentRepo = studentRepo;
            _studentListRepo = studentListRepo;
        }

        // 1) Get all students (by batch + branch)
        public async Task<IEnumerable<StudentListDto>> GetStudentsAsync(string regu, string course, string branch)
        {
            var pRegu = new SqlParameter("@Regu", SqlDbType.VarChar, 50) { Value = (object?)regu ?? DBNull.Value };
            var pCourse = new SqlParameter("@Course", SqlDbType.VarChar, 50) { Value = (object?)course ?? DBNull.Value };
            var pBranch = new SqlParameter("@Branch", SqlDbType.VarChar, 100) { Value = (object?)branch ?? DBNull.Value };

            var sql = StoredProcSql.Exec(StoredProcedures.sp_stdDATA_Load_Details, "@Regu", "@Course", "@Branch");
            return await _studentListRepo.QueryFromStoredProcAsync(sql, pRegu, pCourse, pBranch);
        }


        // 2) Get single student by RegNo
        public async Task<StudentDto> GetStudentByRegNoAsync(string regNo)
        {
            var pRegNo = new SqlParameter("@REGNO", SqlDbType.VarChar, 20) { Value = regNo ?? string.Empty };

            var sql = StoredProcSql.Exec(StoredProcedures.sp_stdDATA_Load, "@REGNO");
            var rows = await _studentRepo.QueryFromStoredProcAsync(sql, pRegNo);
            return rows != null ? System.Linq.Enumerable.FirstOrDefault(rows) : null;
        }

        // 3) Save student (insert/update)
        public async Task<int> SaveStudentAsync(SaveStudentRequest request)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.sp_stdDATA_Save,
                "@REGU", "@COURSE", "@BRANCH", "@SecLan", "@REGNO", "@ROLLNUM",
                "@Medium", "@SECTION", "@SNAME", "@FNAME", "@MNAME", "@DOB", "@CASTE",
                "@GENDER", "@ADDRESS", "@DISTRICT", "@STATE", "@MOBILE", "@EMAIL",
                "@PINCODE", "@ISACTIVE", "@READMISSION", "@REGULATION", "@STREAM",
                "@REMARKS", "@AADHARNO", "@NATIONALITY", "@RELIGION", "@MOLE1",
                "@MOLE2", "@ADMISSIONNO", "@DATEOFADMITTED");

            var pRegu = new SqlParameter("@REGU", request.Regu ?? string.Empty);
            var pCourse = new SqlParameter("@COURSE", request.Course ?? string.Empty);
            var pBranch = new SqlParameter("@BRANCH", request.Branch ?? string.Empty);
            var pSecLan = new SqlParameter("@SecLan", SqlDbType.VarChar, 20) { Value = "null" }; // kept same as DAL
            var pRegNo = new SqlParameter("@REGNO", request.RegNo ?? string.Empty);
            var pRoll = new SqlParameter("@ROLLNUM", request.RollNo ?? string.Empty);
            var pMedium = new SqlParameter("@Medium", request.Medium ?? string.Empty);
            var pSec = new SqlParameter("@SECTION", request.Section ?? string.Empty);
            var pSName = new SqlParameter("@SNAME", request.Name ?? string.Empty);
            var pFName = new SqlParameter("@FNAME", request.FatherName ?? string.Empty);
            var pMName = new SqlParameter("@MNAME", request.MotherName ?? string.Empty);
            var pDob = new SqlParameter("@DOB", request.DOB ?? string.Empty);
            var pCaste = new SqlParameter("@CASTE", request.Caste ?? string.Empty);
            var pGender = new SqlParameter("@GENDER", request.Gender ?? string.Empty);
            var pAddr = new SqlParameter("@ADDRESS", request.Address ?? string.Empty);
            var pDist = new SqlParameter("@DISTRICT", request.District ?? string.Empty);
            var pState = new SqlParameter("@STATE", request.State ?? string.Empty);
            var pMob = new SqlParameter("@MOBILE", request.Mobile ?? string.Empty);
            var pEmail = new SqlParameter("@EMAIL", request.Email ?? string.Empty);
            var pPin = new SqlParameter("@PINCODE", request.Pincode ?? string.Empty);
            var pIsActive = new SqlParameter("@ISACTIVE", request.IsActive);
            var pReadmit = new SqlParameter("@READMISSION", request.IsReadmitted);
            var pRegulation = new SqlParameter("@REGULATION", request.Regulation ?? string.Empty);
            var pStream = new SqlParameter("@STREAM", request.Stream);
            var pRemarks = new SqlParameter("@REMARKS", request.Remarks ?? string.Empty);
            var pAadhar = new SqlParameter("@AADHARNO", request.AadhaarNo ?? string.Empty);
            var pNat = new SqlParameter("@NATIONALITY", request.Nationality ?? string.Empty);
            var pRel = new SqlParameter("@RELIGION", request.Religion ?? string.Empty);
            var pMole1 = new SqlParameter("@MOLE1", request.Mole1 ?? string.Empty);
            var pMole2 = new SqlParameter("@MOLE2", request.Mole2 ?? string.Empty);
            var pAdmNo = new SqlParameter("@ADMISSIONNO", request.AdmissionNo ?? string.Empty);
            var pAdmDate = new SqlParameter("@DATEOFADMITTED", request.AdmissionDate ?? string.Empty);

            return await _studentRepo.ExecuteStoredProcAsync(sql,
                pRegu, pCourse, pBranch, pSecLan, pRegNo, pRoll, pMedium, pSec,
                pSName, pFName, pMName, pDob, pCaste, pGender, pAddr, pDist,
                pState, pMob, pEmail, pPin, pIsActive, pReadmit, pRegulation,
                pStream, pRemarks, pAadhar, pNat, pRel, pMole1, pMole2,
                pAdmNo, pAdmDate);
        }

        // 4) Inactivate
        public async Task<int> InactivateStudentAsync(InactivateRequest request)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.sp_SH_Inactive, "@REGNO", "@SEM", "@regmarks");

            var pRegNo = new SqlParameter("@REGNO", request.RegNo ?? string.Empty);
            var pSem = new SqlParameter("@SEM", SqlDbType.Int) { Value = request.Semester };   
            var pRemarks = new SqlParameter("@regmarks", request.Remarks ?? string.Empty);     

            return await _studentRepo.ExecuteStoredProcAsync(sql, pRegNo, pSem, pRemarks);
        }

        // 5) Reactivate
        public async Task<int> ReactivateStudentAsync(ReactivateRequest request)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.sp_SH_Reactive, "@oldREGNO", "@newREGNO", "@REGU");

            var pOld = new SqlParameter("@oldREGNO", request.OldRegNo ?? string.Empty);
            var pNew = new SqlParameter("@newREGNO", request.NewRegNo ?? string.Empty);

            // Ensure Batch/Regu is passed as int
            var pRegu = new SqlParameter("@REGU", SqlDbType.Int) { Value = request.Batch };

            return await _studentRepo.ExecuteStoredProcAsync(sql, pOld, pNew, pRegu);
        }

        // 6) Re-Admission
        public async Task<int> ReAdmissionAsync(ReadmissionRequest request)
        {
            var sql = StoredProcSql.Exec(
                StoredProcedures.SP_READMISSION,
                "@OLDREGNO", "@NewRegno", "@REGU", "@sem"
            );

            var pOldReg = new SqlParameter("@OLDREGNO", request.RegNo ?? string.Empty);
            var pNewReg = new SqlParameter("@NewRegno", request.NewRegNo ?? string.Empty);
            var pRegu = new SqlParameter("@REGU", request.Batch ?? string.Empty);
            var pSem = new SqlParameter("@sem", SqlDbType.Int) { Value = request.Semester };

            return await _studentRepo.ExecuteStoredProcAsync(sql, pOldReg, pNewReg, pRegu, pSem);
        }

        // 7) Exports
        public async Task<IEnumerable<StudentDto>> ExportDetainedListAsync()
        {
            var sql = StoredProcSql.Exec(StoredProcedures.PROC_DETAINEDLIST);
            return await _studentRepo.QueryFromStoredProcAsync(sql);
        }

        public async Task<IEnumerable<StudentDto>> ExportPassedListAsync(string regulation, string course, string examMy)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.PROC_PASSED_ALL_SEMS, "@REGULATION", "@COURSE", "@EXAMMY");

            var pReg = new SqlParameter("@REGULATION", regulation ?? string.Empty);
            var pCourse = new SqlParameter("@COURSE", course ?? string.Empty);
            var pExamMy = new SqlParameter("@EXAMMY", examMy ?? string.Empty);

            return await _studentRepo.QueryFromStoredProcAsync(sql, pReg, pCourse, pExamMy);
        }

        public async Task<IEnumerable<StudentDto>> ExportBranchWiseAsync(string regulation, string course)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.PROC_BranchWiseCount, "@REGULATION", "@COURSE");

            var pReg = new SqlParameter("@REGULATION", regulation ?? string.Empty);
            var pCourse = new SqlParameter("@COURSE", course ?? string.Empty);

            return await _studentRepo.QueryFromStoredProcAsync(sql, pReg, pCourse);
        }

        public async Task<IEnumerable<StudentDto>> ExportReadmissionListAsync(string regulation, string course)
        {
            var sql = StoredProcSql.Exec(StoredProcedures.PROC_Readmissionlist, "@REGULATION", "@COURSE");

            var pReg = new SqlParameter("@REGULATION", regulation ?? string.Empty);
            var pCourse = new SqlParameter("@COURSE", course ?? string.Empty);

            return await _studentRepo.QueryFromStoredProcAsync(sql, pReg, pCourse);
        }

        public async Task<IEnumerable<StudentDto>> ExportLateralAsync()
        {
            var sql = StoredProcSql.Exec(StoredProcedures.SPR_STDDATA_FOR_ICAMPUS);
            return await _studentRepo.QueryFromStoredProcAsync(sql);
        }

        // 8) Uploads
        public async Task<bool> UploadPhotoAsync(IFormFile file, string regNo)
        {
            try
            {
                if (file == null || file.Length == 0) return false;

                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "Photos");
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                var filePath = Path.Combine(folderPath, $"{regNo}.jpg");
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return true;
            }
            catch { return false; }
        }

        public async Task<bool> UploadSignatureAsync(IFormFile file, string regNo)
        {
            try
            {
                if (file == null || file.Length == 0) return false;

                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "Signatures");
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                var filePath = Path.Combine(folderPath, $"{regNo}.jpg");
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return true;
            }
            catch { return false; }
        }
    }
}
