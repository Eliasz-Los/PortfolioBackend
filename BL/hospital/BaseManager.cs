using DAL.Repository.hospital;
using Domain.hospital;

namespace BL.hospital;

//It's better to have a specific manager per entity as every entity has different values, validation and structure
public class BaseManager<T> : IBaseManager<T> where T : BaseEntity
{
    protected readonly IBaseRepository<T> Repository;

    public BaseManager(IBaseRepository<T> repository)
    {
        Repository = repository;
    }

    public async Task<T?> GetById(Guid id)
    {
        return await Repository.ReadById(id);
    }

    public async Task<IEnumerable<T>> GetAll()
    {
        return await Repository.ReadAll();
    }

    public void Add(T entity)
    {
        Repository.Create(entity);
    }

    public void Remove(Guid id)
    {
        Repository.Delete(id);
    }
    
}