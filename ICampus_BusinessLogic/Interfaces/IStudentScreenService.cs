using System.Collections.Generic;
using System.Threading.Tasks;

public interface IStudentScreenService
{
    Task<IEnumerable<object>> GetStudentScreenStudentDataAsync(string regno);
    Task<IEnumerable<object>> GetMaxSemestersAsync(string regno);
    Task<IEnumerable<object>> GetStudentGradesAsync(string regno, string examMy);
}
