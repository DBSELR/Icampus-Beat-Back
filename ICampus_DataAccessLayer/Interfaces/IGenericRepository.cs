using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICampus_DataAccessLayer.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> QueryFromStoredProcAsync(string sql, params object[] parameters);
        Task<int> ExecuteStoredProcAsync(string sql, params object[] parameters);
    }
}
