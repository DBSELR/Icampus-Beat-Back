using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IUniversityCdDataService
    {
        Task<IEnumerable<object>> LoadBatchAsync(string course, string regulation);
        Task<IEnumerable<object>> LoadSemAsync(string course, string examMY, string regulation);
        Task<IEnumerable<object>> GetSubjectListAsync(string course, string regulation, string examMY, string sem);
        Task<IEnumerable<object>> GetStudentsDataAsync(string course, string regulation, string examMY, string sem, string regSup);
    }
}
