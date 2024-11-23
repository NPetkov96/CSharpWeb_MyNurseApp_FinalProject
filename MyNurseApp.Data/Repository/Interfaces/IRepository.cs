namespace MyNurseApp.Data.Repository.Interfaces
{
    public interface IRepository<TType, TId>
    {
        Task AddAsync(TType item);

    }
}
