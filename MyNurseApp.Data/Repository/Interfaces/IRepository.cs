namespace MyNurseApp.Data.Repository.Interfaces
{
    public interface IRepository<TType, TId>
    {
        void Add(TType item);

        Task AddAsync(TType item);
    }
}
