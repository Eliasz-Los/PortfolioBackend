
using Domain.pathfinder;

namespace BL.Pathfinder.algorithm;

public interface ISecondAStar
{
    List<Point> FindPath(Point start, Point end, bool[,] walkablePoints);

}