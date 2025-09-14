using Domain;
using Microsoft.Extensions.Logging;

namespace BL.pathfinder;

public class PathManager
{
    private readonly ILogger<PathManager> _logger;
    private readonly FloorplanManager _floorplanManager;
    private const string FolderPath = "wwwroot/images/floorplans";
    

    public PathManager(ILogger<PathManager> logger, FloorplanManager floorplanManager)
    {
        _logger = logger;
        _floorplanManager = floorplanManager;
    }
    
    public async Task<List<Point>> FindPath(Point start, Point end, String hospitalName, int floorNumber)
    {
        //Getting the image path
        var floorplan = _floorplanManager.GetFloorplanByNameAndFloor(hospitalName, floorNumber);
        var imagePath = Path.Combine(FolderPath, floorplan.Image);
        
        //Analyzing the walkable points
        var (startP, endP, walkablePoints) = await Task.Run( () => FloorplanAnalyzer.GetWalkablePoints(imagePath, start ,end));
        if (walkablePoints.Count == 0)
        {
            _logger.LogError("No walkable points found.");
            return new List<Point>();
        }
        
        // navigating the path
        var path = await Task.Run(() => AStarPathfinding.FindPath(startP, endP, walkablePoints));
        _logger.LogInformation($"Path found: {path} {path.Count}");
        return path;
    }
}