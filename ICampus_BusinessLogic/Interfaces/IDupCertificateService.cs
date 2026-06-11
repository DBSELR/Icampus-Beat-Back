using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IDupCertificateService
    {
        Task<IEnumerable<object>> LoadReceiptStudentDataAsync(string receiptNo);
        Task<IEnumerable<object>> LoadHallTicketAsync(string examMy, string course, string regno, string regulation);
        Task<IEnumerable<object>> LoadMarksMemoAsync(MarksMemoRequest req);
        Task<int> SaveDupCertificateAsync(DupCertificateSaveRequest req);
        Task<int> CheckRegWiseDupCountAsync(string regNo, int sem, string examMy, string certificateName);
        Task<int> CheckReceiptWiseDupCountAsync(string receiptNo, string regNo, int sem, string examMy, string certificateName);
    }

}
