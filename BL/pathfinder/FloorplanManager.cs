using DAL.Repository;
using Domain;

namespace BL.pathfinder;

public class FloorplanManager
{
    private readonly FloorplanRepository _floorplanRepository;

    public FloorplanManager(FloorplanRepository floorplanRepository)
    {
        _floorplanRepository = floorplanRepository;
    }
    
    
    public IEnumerable<Floorplan> GetAllFloorplans()
    {
        return _floorplanRepository.ReadFloorplans();
    }
}