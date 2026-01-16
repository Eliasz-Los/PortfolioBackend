using Domain.pathfinder;

namespace BL.pathfinder;

public interface IFloorplanManager
{
    IEnumerable<Floorplan> GetAllFloorplans();
    Floorplan GetFloorplanByNameAndFloor(string name, int floorNumber);
}