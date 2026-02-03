using Domain.pathfinder;
using Microsoft.Extensions.Logging;

namespace BL.Pathfinder.algorithm;

public class AStarGridPathfinding : ISecondAStar
{
    private readonly ILogger<AStarGridPathfinding> _logger;

    public AStarGridPathfinding(ILogger<AStarGridPathfinding> logger)
    {
        _logger = logger;
    }

    public List<Point> FindPath(Point start, Point end, bool[,] walkablePoints)
    {
        int width = walkablePoints.GetLength(0);
        int height = walkablePoints.GetLength(1);
        
        if(start.XWidth >= width || start.YHeight >= height
           || end.XWidth >= width || end.YHeight >= height)
        {
            _logger.LogWarning("Start or end point is out of bounds.");
            return new List<Point>();
        }
        
        if(!walkablePoints[start.XWidth, start.YHeight] 
           || !walkablePoints[end.XWidth, end.YHeight])
        {
            _logger.LogWarning("Start or end point is not walkable.");
            return new List<Point>();
        }
        
        var open = new PriorityQueue<int, int>(); // nodeId, priority
        var closed = new bool[width, height];
        var gCost = new int[width, height];
        var parent = new int[width, height];

        const int noParent = -1;
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                gCost[x, y] = int.MaxValue;
                parent[x, y] = noParent;
            }
        }
        
        int startX = start.XWidth;
        int startY = start.YHeight;
        int endX = end.XWidth;
        int endY = end.YHeight;
        
        int startId = ToId(startX, startY, width);
        int endId = ToId(endX, endY, width);
        
        gCost[startX, startY] = 0;
        int startF = Heuristic(startX, startY, endX, endY);
        open.Enqueue(startId, startF);

        const int maxIterations = 7_000_000;
        int iterations = 0;

        while (open.Count > 0)
        {
            iterations++;
            if (iterations > maxIterations)
            {
                _logger.LogWarning("Pathfinding exceeded maximum iterations.");
                break;
            }
            
            int currentId = open.Dequeue();
            FromId(currentId, width, out int cx, out int cy);
            
            if(closed[cx, cy])
                continue;
            
            if (currentId == endId)
                return ReconstructPath(parent, end.XWidth, end.YHeight, start.XWidth, start.YHeight, width);
            
            closed[cx, cy] = true;
            
            int baseGCost = gCost[cx, cy];
            
            if (baseGCost == int.MaxValue)
                continue;
            
            // 4-way movement
            TryRelax(cx + 1, cy);
            TryRelax(cx - 1, cy);
            TryRelax(cx, cy + 1);
            TryRelax(cx, cy - 1);

            void TryRelax(int nx, int ny)
            {
                if ((uint)nx >= (uint)width || (uint)ny >= (uint)height)
                    return;
                if (!walkablePoints[nx, ny] || closed[nx, ny])
                    return;

                int tentativeG = baseGCost + 1;
                if (tentativeG >= gCost[nx, ny])
                    return;

                gCost[nx, ny] = tentativeG;
                parent[nx, ny] = currentId;

                int f = tentativeG + Heuristic(nx, ny, end.XWidth, end.YHeight);
                open.Enqueue(ToId(nx, ny, width), f);
            }
        }
        
        _logger.LogInformation("No path found. iterationCount: {iterationCount}", iterations);
        return new List<Point>();
    }
    
    
    private static int Heuristic(int x, int y, int ex, int ey)
        => Math.Abs(x - ex) + Math.Abs(y - ey);

    private static int ToId(int x, int y, int width) => y * width + x;

    private static void FromId(int id, int width, out int x, out int y)
    {
        y = id / width;
        x = id - (y * width);
    }
    
    private static List<Point> ReconstructPath(int[,] parent, int endX, int endY, int startX, int startY, int width)
    {
        var path = new List<Point>();
        int cx = endX;
        int cy = endY;

        while (!(cx == startX && cy == startY))
        {
            path.Add(new Point(cx, cy));
            int p = parent[cx, cy];
            if (p == -1)
                break;

            FromId(p, width, out cx, out cy);
        }

        path.Reverse();
        return path;
    }
    
}