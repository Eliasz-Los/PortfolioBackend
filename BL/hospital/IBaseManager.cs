using Domain.hospital;

namespace BL.hospital;

public interface IBaseManager<T, ReadDto, CreateDto> where T : BaseEntity
{
    Task<ReadDto?> GetById(Guid id);
    Task<IEnumerable<ReadDto>> GetAll();
    Task<T> Add(CreateDto entity);
    void Remove(Guid id);
    
}