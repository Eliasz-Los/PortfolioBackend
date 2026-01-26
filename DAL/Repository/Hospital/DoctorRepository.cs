using DAL.EntityFramework;
using Domain.hospital;
using Domain.hospital.types;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository.hospital;

public class DoctorRepository : BaseRepository<Doctor>, IDoctorRepository
{
    public DoctorRepository(PortfolioDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Doctor>> SearchDoctorsByFullNameOrSpecialisation(string? term, CancellationToken cancellationToken)
    {

        IQueryable<Doctor> query = _dbSet.AsNoTracking();
        
        term = term?.Trim().ToLower();
        if (string.IsNullOrWhiteSpace(term))
        {
            return await query.ToListAsync(cancellationToken);
        }

        if (Enum.TryParse(term , true, out Specialisation spec))
        {
            query = query.Where(s => s.Specialisation == spec);
            return await query.ToListAsync(cancellationToken);
        }
        
        return await query
            .Where(d =>
                ((d.FullName.FirstName + " " + d.FullName.LastName).ToLower().Contains(term)) ||
                ((d.FullName.LastName + " " + d.FullName.FirstName).ToLower().Contains(term)) ||
                d.FullName.FirstName.ToLower().Contains(term) ||
                d.FullName.LastName.ToLower().Contains(term))
            .ToListAsync(cancellationToken);

    }
}