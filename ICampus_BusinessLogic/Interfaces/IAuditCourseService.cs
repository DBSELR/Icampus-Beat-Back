using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface IAuditCourseService
    {
        Task<IEnumerable<object>> LoadBatchAsync(string course);
        Task<IEnumerable<object>> LoadSemAsync(string course, string regu);
        Task<IEnumerable<object>> LoadAcademicYearAsync(string course);
        Task<IEnumerable<object>> GetDataAsync(string course, string regu, string sem, string academicYear);
    }
}
