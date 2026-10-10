using SchoolManager.Data.Entities;
using System.Linq.Expressions;

namespace SchoolManager.Data.Repositories
{
    public interface IGenericRepository<T> where T : class, IEntity
    {
        IQueryable<T> GetAll();

        Task<T?> GetByIdAsync(int id);

        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);

        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);

        Task CreateAsync(T entity);

        Task UpdateAsync(T entity);

        Task DeleteAsync(T entity);

        Task<bool> SaveAllAsync();
    }
}
