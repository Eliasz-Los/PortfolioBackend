using Domain.hospital;

namespace BL.hospital;

public interface IBaseManager<T, TReadDto, TCreateDto> where T : BaseEntity
{
    Task<TReadDto?> GetById(Guid id);
    Task<IEnumerable<TReadDto>> GetAll();
    Task<T> Add(TCreateDto entity);
    void Remove(Guid id);
    
}