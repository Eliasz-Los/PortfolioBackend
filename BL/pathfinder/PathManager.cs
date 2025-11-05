using AutoMapper;
using BL.dto;
using Domain;
using Domain.pathfinder;
using Microsoft.Extensions.Logging;

namespace BL.pathfinder;

public class PathManager : IPathManager
{
    private readonly ILogger<PathManager> _logger;
    private readonly IFloorplanManager _floorplanManager;
    private readonly IMapper _mapper;
    private readonly AStarPathfinding _pathfinding;
    private readonly  FloorplanAnalyzer _floorplanAnalyzer;
    

    public PathManager(ILogger<PathManager> logger, IFloorplanManager floorplanManager, IMapper mapper, AStarPathfinding pathfinding, FloorplanAnalyzer floorplanAnalyzer)
    {
        _logger = logger;
        _floorplanManager = floorplanManager;
        _mapper = mapper;
        _pathfinding = pathfinding;
        _floorplanAnalyzer = floorplanAnalyzer;
    }
    
    public async Task<List<PathPointDto>> FindPath(PathRequestDto pathRequestDto, string folderPath)
    {
        // Extracting data from DTO
        Point start = _mapper.Map<Point>(pathRequestDto.Start);
        Point end = _mapper.Map<Point>(pathRequestDto.End);
        
        //Getting the image path
        var floorplan = _floorplanManager.GetFloorplanByNameAndFloor(pathRequestDto.FloorplanName, pathRequestDto.FloorNumber);
        var imagePath = Path.Combine(folderPath, floorplan.Image);
        
        //Analyzing the walkable points
        var (startP, endP, walkablePoints) = await Task.Run( () => _floorplanAnalyzer.GetWalkablePoints(imagePath, start ,end));
        
        // navigating the path
        var path = await Task.Run(() => _pathfinding.FindPath(startP, endP, walkablePoints));
        _logger.LogInformation($"Path found: {path} {path.Count}");
        return _mapper.Map<List<PathPointDto>>(path);
    }
}