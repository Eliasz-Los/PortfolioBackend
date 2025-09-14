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

    public Floorplan ReadFloorplanByNameAndFloor(string name, int floorNumber)
    {
        Floorplan floorplan = _context.Floorplans.FirstOrDefault(fp => fp.Name == name && fp.FloorNumber == floorNumber);

        return floorplan;
    }
}