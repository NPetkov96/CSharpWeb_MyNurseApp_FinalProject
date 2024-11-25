using System.Linq.Expressions;

namespace MyNurseApp.Data.Repository.Interfaces
{
    public interface IRepository<TType, TId>
    {
        Task<IEnumerable<TType>> GetAllAsync();

        Task<TType> GetByIdAsync(TId id);

        Task AddAsync(TType item);

        Task<TType> FirstOrDefaultAsync(Expression<Func<TType, bool>> predicate);

        Task<bool> DeleteAsync(TType entity);

        Task<bool> UpdateAsync(TType item);

        IQueryable<TType> GetAllAttached();
    }
}
