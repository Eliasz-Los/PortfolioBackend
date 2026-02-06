using DAL.EntityFramework;
using Microsoft.EntityFrameworkCore.Storage;

namespace DAL.Repository.UoW;

public class UnitOfWork : IAsyncDisposable, IUnitOfWork
{
    private readonly PortfolioDbContext _context;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(PortfolioDbContext context)
    {
        _context = context;
    }


    public async Task BeginTransaction()
    {
        if (_transaction is not null)
        {
            return; // already in a transaction
        }
        
        _transaction = await _context.Database.BeginTransactionAsync();
    }
    
    public async Task Commit()
    {
        await _context.SaveChangesAsync();

        if (_transaction is null)
        {
            return;
        }
        
        await _transaction.CommitAsync();
        await _transaction.DisposeAsync();
        _transaction = null; // because it's been committed and is thus gone
    }
    
    public async Task Rollback()
    {
        if (_transaction is null)
        {
            return;
        }

        await _transaction.RollbackAsync();
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public async ValueTask DisposeAsync()
    {
        if (_transaction is not null)
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }
}