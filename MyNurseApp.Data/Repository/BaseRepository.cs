using Microsoft.EntityFrameworkCore;
using MyNurseApp.Data.Repository.Interfaces;

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
    }
}
