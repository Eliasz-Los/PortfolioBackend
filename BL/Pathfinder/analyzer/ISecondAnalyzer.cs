using Domain.pathfinder;

namespace BL.Pathfinder.analyzer;

public interface ISecondAnalyzer
{
    (Point start, Point end, bool[,] walkableGrid) GetWalkableGrid(string imagePath, Point start, Point end, int threshold = 75);
}