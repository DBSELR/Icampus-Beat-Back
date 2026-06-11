using System.Collections.Generic;
using System.Threading.Tasks;

public interface IExamFeeConcessionService
{
    Task<IEnumerable<object>> LoadSemsAsync(string course, string regulation);
    Task<IEnumerable<object>> GetDetailsByRegnoAsync(string regno);
    Task<IEnumerable<object>> LoadFeeConcessionGridAsync(string course, string examMy, string regno, int sem);
    
    // Check if semester is Regular or Supply
    Task<string> CheckRegSupAsync(string regu, int sem, string course, string examMy);
    
    // Get fee for Regular semester
    // Parameters: @REGU VARCHAR(2), @SEM VARCHAR(2), @COURSE VARCHAR(20), @GRP VARCHAR(15), @EXAMMY VARCHAR(12), @REGULATION VARCHAR(10), @REGNO varchar(10)
    Task<IEnumerable<object>> GetFeeConcessionRegAsync(string regu, string sem, string course, string grp, string examMy, string regulation, string regno);
    
    // Get fee for Supply semester
    // Parameters: @COURSE VARCHAR(20), @EXAMMY VARCHAR(12), @REGULATION VARCHAR(10), @Regno varchar(20), @sem int
    Task<IEnumerable<object>> GetFeeConcessionSupAsync(string course, string examMy, string regulation, string regno, int sem);
    
    Task<int> SaveFeeConcessionAsync(FeeConcessionSaveRequest req);
    Task<int> DeleteFeeConcessionAsync(int id);
}
