using Domain.pathfinder;

namespace BL.pathfinder;

public interface IFloorplanAnalyzer
{
    (Point start, Point end, HashSet<Point> walkablePoints) GetWalkablePoints(string imagePath, Point start, Point end);
}