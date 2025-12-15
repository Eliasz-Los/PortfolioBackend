using Domain.hospital;

namespace BL.hospital;

public interface IBaseManager<T> where T : BaseEntity
{
    Task<T?> GetById(Guid id);
    Task<IEnumerable<T>> GetAll();
    void Add(T entity);
    void Remove(Guid id);
    
}