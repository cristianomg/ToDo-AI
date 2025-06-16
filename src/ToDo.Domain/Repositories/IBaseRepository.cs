using System.Linq.Expressions;

namespace ToDo.Domain.Repositories
{
    public interface IBaseRepository<T>
    {
        Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes);
        Task<T?> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes);
        Task<IEnumerable<T>> GetByFilterAsync(
            Expression<Func<T, bool>> filter,
            params Expression<Func<T, object>>[] includes);

        Task<T> AddAsync(T entity);
        T Update(T entity);
        void Delete(T entity);
        Task DeleteByIdAsync(int id);
        Task<int> SaveChangesAsync();
        IQueryable<T> GetQueryable(params Expression<Func<T, object>>[] includes);
    }
}
