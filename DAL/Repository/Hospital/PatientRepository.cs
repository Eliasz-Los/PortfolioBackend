using DAL.EntityFramework;
using Domain.hospital;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository.hospital;

public class PatientRepository : BaseRepository<Patient>, IPatientRepository
{
    public PatientRepository(PortfolioDbContext context) : base(context)
    {
    }
    
    public async Task<IEnumerable<Patient>> SearchPatientsByFullNameOrDateOfBirth(
        string? term,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Patient> query = _dbSet.AsNoTracking();

        term = term?.Trim().ToLower();
        if (string.IsNullOrWhiteSpace(term))
        {
            return await query.ToListAsync(cancellationToken);
        }
        
        if (DateOnly.TryParse(term, out var dateOfBirth))
        {
            query = query.Where(p => p.DateOfBirth == dateOfBirth);
            return await query.ToListAsync(cancellationToken);
        }
        
        query = query.Where(p => 
            p.FullName.FirstName.ToLower().Contains(term) || 
            p.FullName.LastName.ToLower().Contains(term));

        return await query.ToListAsync(cancellationToken); 
    }
}