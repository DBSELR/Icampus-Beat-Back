using Microsoft.EntityFrameworkCore;
using ICampus_DataAccessLayer.Interfaces;
using ICampus_DataAccessLayer.Data;

namespace ICampus_DataAccessLayer.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ICampusDbContext _context;
        public GenericRepository(ICampusDbContext context) => _context = context;

        public async Task<IEnumerable<T>> QueryFromStoredProcAsync(string sql, params object[] parameters)
        {
            return await _context.Set<T>().FromSqlRaw(sql, parameters).AsNoTracking().ToListAsync();
        }

        public async Task<int> ExecuteStoredProcAsync(string sql, params object[] parameters)
        {
            return await _context.Database.ExecuteSqlRawAsync(sql, parameters);
        }
    }
}
