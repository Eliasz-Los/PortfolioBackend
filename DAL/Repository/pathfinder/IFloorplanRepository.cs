using Domain.pathfinder;

namespace DAL.Repository;

public interface IFloorplanRepository
{
    IEnumerable<Floorplan> ReadFloorplans();

    Floorplan ReadFloorplanByNameAndFloor(string name, int floorNumber);
}