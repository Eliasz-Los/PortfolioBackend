using Domain;
using Microsoft.Extensions.Logging;

namespace BL.pathfinder;

public class PathManager
{
    private readonly ILogger<PathManager> _logger;

    public PathManager(ILogger<PathManager> logger)
    {
        _logger = logger;
    }
    
    //TODO implement floorplan repository
    public async Task<List<Point>> FindPath(Point start, Point end, String folderPath, String hospitalName, int floorNumber)
    {
        //Getting the image path
        //TODO
        // var floorplan = _floorplanRepository.ReadFloorplanByHospitalNameAndFloorNumber(hospitalName, floorNumber);
        // var imagePath = Path.Combine(folderPath, floorplan?.Image);
        //Analyzing the walkable points
        // var (startP, endP, walkablePoints) = await Task.Run( () => FloorplanAnalyzer.GetWalkablePoints(imagePath, start ,end));
        // if (walkablePoints.Count == 0)
        // {
        //     _logger.LogError("No walkable points found.");
        //     return new List<Point>();
        // }
        // //Navigating the path
        // var path = await Task.Run(() => AStarPathfinding.FindPath(startP, endP, walkablePoints));
        // //var path = await Task.Run(() => AStarBidirectional.FindPathBidirectional(startP, endP, walkablePoints));
        // _logger.LogInformation($"Path found: {path} {path.Count}");
        return new List<Point>();
    }
}