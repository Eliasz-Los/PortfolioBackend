using DAL.EntityFramework;
using Domain.hospital;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository.hospital;

public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
{
    protected readonly PortfolioDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public BaseRepository(PortfolioDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> ReadById(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<IEnumerable<T>> ReadAll()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<T> Create(T entity)
    {
         _dbSet.Add(entity);
         await _context.SaveChangesAsync();
         return entity;
    }

    public void Delete(Guid id)
    {
        var entity = _dbSet.Find(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            _context.SaveChanges();
        }
    }
}