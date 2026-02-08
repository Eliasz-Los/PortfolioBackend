using DAL.EntityFramework;
using Domain.DocuGroup;

namespace DAL.Repository.DocuGroup;

public class MembershipRepository : IMembershipRepository
{
    private readonly PortfolioDbContext _context;

    public MembershipRepository(PortfolioDbContext context)
    {
        this._context = context;
    }

    public async Task CreateMembership(Membership membership)
    {
        await _context.AddAsync(membership);
    }
}