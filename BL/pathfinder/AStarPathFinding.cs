using Domain.pathfinder;
using Microsoft.Extensions.Logging;

namespace BL.pathfinder;
/*
 *  A* pathfinding algorithm is synchronous but because it is executed in a separate Task.Run(),
 *  it will not cause bottlenecks.
 *  This keeps the main thread free for other tasks.
 */

//TODO: somehow with the test_floor image I cannot get a path to the top left corner, like to far in big_room1 and it cannot find a way
public class AStarPathfinding
{
    private readonly ILogger<AStarPathfinding> _logger;

    public AStarPathfinding(ILogger<AStarPathfinding> logger)
    {
        _logger = logger;
    }

    public List<Point> FindPath( Point start, Point end, HashSet<Point> walkablePoints)
    {
        if (!walkablePoints.Contains(start) || !walkablePoints.Contains(end))
        {
            _logger.LogError("Start or end point is not walkable.");
            return new List<Point>();
        }

        
        // Bounded Search Space
        double margin = 350; 
        double minX = Math.Min(start.XWidth, end.XWidth) - margin;
        double maxX = Math.Max(start.XWidth, end.XWidth) + margin;
        double minY = Math.Min(start.YHeight, end.YHeight) - margin;
        double maxY = Math.Max(start.YHeight, end.YHeight) + margin;
        
        var openSet = new PriorityQueue<Node, double>();
        var openSetLookup = new HashSet<Point>(); // New HashSet for quick lookup, O(1) to check if the node is in the queue
        var nodeDictionary = new Dictionary<Point, Node>();
        var closedSet = new HashSet<Point>();
      
        Node GetOrCreateNode(Point point)
        {
            if (!nodeDictionary.TryGetValue(point, out var node))
            {
                node = new Node(point);
                nodeDictionary.Add(point, node);
            }
            return node;
        }

        var startNode = GetOrCreateNode(start);
        startNode.GCost = 0;
        startNode.HCost = GetDistance(start, end);
        openSet.Enqueue(startNode, startNode.FCost);
        openSetLookup.Add(start); 
        
        int iterationCount = 0;
        const int maxIterations = 7000000;//1 mil but it can go up to 6,988,086 because those are all walkable points in the test image

        _logger.LogInformation("Start pathfinding...");
        while (openSet.Count > 0)
        {
            iterationCount++;
            if (iterationCount > maxIterations)
            {
                _logger.LogInformation("Exceeded maximum iterations, breaking out of loop. iterationCount: {iterationCount}", iterationCount);
                break;
            }

            var currentNode = openSet.Dequeue();
            openSetLookup.Remove(currentNode.Point); // Remove from lookup set when processing

            if (currentNode.Point.Equals(end))
            {
                _logger.LogInformation("Path found. iterationCount: {iterationCount}", iterationCount);
                return RetracePath(nodeDictionary[start], currentNode);
            }
            
            closedSet.Add(currentNode.Point);

            //ipv dure functie GetNeighbors te callen, pakken we onmiddelijk de kardinale richtingen in array samen met de breedte en hoogte van de huidige node
            foreach (var (dx, dy) in new[] { (-1, 0), (1, 0), (0, -1), (0, 1)  }) //, (-1, -1), (1, -1), (-1, 1), (1, 1)
            {
                
                var newWidth = currentNode.Point.XWidth + dx;
                var newHeight = currentNode.Point.YHeight + dy;

                // we skip points outside the bounded search space, this saves a lot of unnecessary checks
                if (newWidth < minX || newWidth > maxX || newHeight < minY || newHeight > maxY)
                    continue;
                
                var neighborPoint = new Point(newWidth, newHeight);
                
                if (!walkablePoints.Contains(neighborPoint) || closedSet.Contains(neighborPoint)) continue; // Skip already processed nodes
                
                var neighbor = GetOrCreateNode(neighborPoint);
                var tentativeGCost = currentNode.GCost + 1.0;

                if (tentativeGCost < neighbor.GCost)
                {
                    neighbor.GCost = tentativeGCost;
                    neighbor.HCost = GetDistance(neighbor.Point, end); 
                    neighbor.Parent = currentNode;
                    if (!openSetLookup.Contains(neighbor.Point))
                    {
                      
                        openSet.Enqueue(neighbor, neighbor.GCost + 5.0 * neighbor.HCost); //neighbor.FCost
                        openSetLookup.Add(neighbor.Point); 
                    }
                }
            }
        }

        _logger.LogInformation("No path found. iterationCount: {iterationCount}", iterationCount);
        return new List<Point>();
    }

    private static double GetDistance(Point a, Point b)
    {
        return Math.Abs(a.XWidth - b.XWidth) + Math.Abs(a.YHeight - b.YHeight);
    }

    private static List<Point> RetracePath(Node startNode, Node endNode)
    {
        var path = new List<Point>();
        var currentNode = endNode;

        while (currentNode != null && !currentNode.Equals(startNode))
        {
            path.Add(currentNode.Point);
            currentNode = currentNode.Parent;
        }

        path.Reverse();
        return path;
    }
}