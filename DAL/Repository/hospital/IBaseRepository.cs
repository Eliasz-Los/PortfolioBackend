using Domain.hospital;

namespace DAL.Repository.hospital;

public interface IBaseRepository<T> where T : BaseEntity
{
    Task<T?> ReadById(Guid id);
    Task<IEnumerable<T>> ReadAll();
    Task<T> Create(T entity);
    void Delete(Guid id);
}