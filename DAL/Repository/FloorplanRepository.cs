using DAL.EntityFramework;
using Domain;

namespace DAL.Repository;

public class FloorplanRepository
{
    private readonly PortfolioDbContext _context;

    public FloorplanRepository(PortfolioDbContext context)
    {
        _context = context;
    }
    
    public IEnumerable<Floorplan> ReadFloorplans()
    {
        return _context.Floorplans;
    }
}