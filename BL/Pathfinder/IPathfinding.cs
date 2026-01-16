
using Domain.pathfinder;

namespace BL.pathfinder;

public interface IPathfinding
{
    List<Point> FindPath(Point start, Point end, HashSet<Point> walkablePoints);
    
}