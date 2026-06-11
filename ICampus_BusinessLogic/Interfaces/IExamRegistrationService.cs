using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using ICampus_Models.Requests;


namespace ICampus_BusinessLogic.Interfaces
{
    public interface IExamRegistrationService
    {
        Task<IEnumerable<object>> GetSemsForExamRegAsync(string course, string examMy, string regulation);
        Task<IEnumerable<object>> GetStudentDetailsAsync(string regNo);
        Task<IEnumerable<object>> GetDataForExamRegAsync(string regNo, string examMy, string course, string regulation, string sem = "");
        Task<string> CheckRegsUpAsync(int regu, int sem, string examMy, string course);
        Task<IEnumerable<object>> GetFeeStructureRegularAsync(string regu, string sem, string course, string grp, string examMy, string regulation, string regNo);
        Task<IEnumerable<object>> GetFeeStructureSupplyAsync(int fCount, string course, string examMy, string regulation, string regNo, int sem);
        Task<string> CheckExamNotificationAsync(string examMy, string course, string regulation, string sem);
        Task<bool> CheckAlreadyRegisteredAsync(string regNo, string sem, string examMy);
        Task<int> RegisterAsync(ExamRegisterRequest req);
        Task<int> UnRegisterAsync(UnRegisterRequest req);
        Task<IEnumerable<object>> PayExamFeeAsync(ExamFeePayRequest req);
        Task<IEnumerable<object>> ExamRegisteredListAsync(string examMy, string course, string regulation, string status);
        Task<IEnumerable<object>> DuplicateReceiptAsync(string receiptNo);
        Task<IEnumerable<object>> PrintReceiptAsync(string receiptNo);
        Task<IEnumerable<object>> GetBatchSemesterAsync(string course, string examMy);

        /// <summary>
        /// Register All: UPDATE TBL_SH SET REGD = 'Y' WHERE COURSE=? AND REGU=? AND SEM=?
        /// Confirmed from DLL IL: DataAccessLayer.dll Regular_Exmas_Update
        /// </summary>
        Task<int> RegisterAllAsync(RegisterAllRequest req);

        /// <summary>
        /// Unregister All: UPDATE TBL_SH SET REGD = null WHERE COURSE=? AND REGU=? AND SEM=?
        /// Confirmed from DLL IL: DataAccessLayer.dll Regular_Exmas_Update_UnRegester
        /// </summary>
        Task<int> UnRegisterAllAsync(RegisterAllRequest req);
    }
}