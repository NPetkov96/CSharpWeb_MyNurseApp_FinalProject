using Microsoft.EntityFrameworkCore;
using MyNurseApp.Data.Repository.Interfaces;
using System.Linq.Expressions;

namespace MyNurseApp.Data.Repository
{
    public class BaseRepository<TType, TId> : IRepository<TType, TId>
        where TType : class
    {
        private readonly ApplicationDbContext dbContext;
        private readonly DbSet<TType> dbSet;

        public BaseRepository(ApplicationDbContext context)
        {
            dbContext = context;
            this.dbSet = this.dbContext.Set<TType>();
        }


        public async Task AddAsync(TType item)
        {
            await this.dbSet.AddAsync(item);
            await this.dbContext.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(TType entity)
        {
            this.dbSet.Remove(entity);
            await this.dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<TType> FirstOrDefaultAsync(Expression<Func<TType, bool>> predicate)
        {
            TType? entity = await this.dbSet.FirstOrDefaultAsync(predicate);

            return entity!;
        }

        public async Task<IEnumerable<TType>> GetAllAsync()
        {
            return await this.dbSet.ToListAsync();

        }

        public async Task<TType> GetByIdAsync(TId id)
        {

            TType? entity = await this.dbSet.FindAsync(id);

            return entity!;
        }
    }
}
 