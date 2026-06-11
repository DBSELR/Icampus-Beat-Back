using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IReAdmissionService
    {
        /// <summary>Load student personal details by RegNo (SPM_STUDENT_DETAILS)</summary>
        Task<IEnumerable<object>> LoadStudentDetailsAsync(string regNo);

        /// <summary>Load full subject-wise history grid for a student (SPM_STUDENTHISTORY)</summary>
        Task<IEnumerable<object>> LoadHistoryAsync(string regNo);

        /// <summary>Load Entry Type dropdown — all distinct PTYPE values from TBL_PAP (raw SQL, no params)</summary>
        Task<IEnumerable<object>> LoadPaperTypesAsync();

        /// <summary>Load marks for one TBL_SH record into the edit modal (SPM_ReAdmissionsStudentMarks)</summary>
        Task<IEnumerable<object>> GetMarksByAshIdAsync(string ashId);

        /// <summary>Load paper structure details when paper code is typed (SPM_PaperDetails_ReAdmission)</summary>
        Task<IEnumerable<object>> GetPaperDetailsAsync(string pCode, string regulation, string sem);

        /// <summary>Update marks for a readmit TBL_SH record (SetReAdmissionsStudentMarks)</summary>
        Task<int> UpdateMarksAsync(UpdateReAdmissionMarks model);

        /// <summary>Delete a TBL_SH record by ASHID (PROC_DEL_ASHID)</summary>
        Task<int> DeleteByAshIdAsync(string ashId);
    }

    public class UpdateReAdmissionMarks
    {
        public string AshId      { get; set; }
        public string PCode      { get; set; }
        public string PName      { get; set; }
        public string Regulation { get; set; }
        public string Sem        { get; set; }
        public string TMarks     { get; set; }
        public string MMarks     { get; set; }
        public string RVMarks    { get; set; }
        public string SMarks     { get; set; }
        public string PMarks     { get; set; }
        public string EntryType  { get; set; }
        public string Credits    { get; set; }
        public string SgpaCr     { get; set; }
        public string TMax       { get; set; }
        public string TPass      { get; set; }
        public string SMax       { get; set; }
        public string SPass      { get; set; }
        public string PMax       { get; set; }
        public string PPass      { get; set; }
        public string MaxMrk     { get; set; }
        public string Pass       { get; set; }
    }
}
