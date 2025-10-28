using AutoMapper;
using BL.dto;
using Domain;
using Domain.pathfinder;
using Microsoft.Extensions.Logging;

namespace BL.pathfinder;

public class PathManager
{
    private readonly ILogger<PathManager> _logger;
    private readonly FloorplanManager _floorplanManager;
    private readonly IMapper _mapper;
    

    public PathManager(ILogger<PathManager> logger, FloorplanManager floorplanManager, IMapper mapper)
    {
        _logger = logger;
        _floorplanManager = floorplanManager;
        _mapper = mapper;
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
        var (startP, endP, walkablePoints) = await Task.Run( () => FloorplanAnalyzer.GetWalkablePoints(imagePath, start ,end));
        
        // navigating the path
        var path = await Task.Run(() => AStarPathfinding.FindPath(startP, endP, walkablePoints));
        _logger.LogInformation($"Path found: {path} {path.Count}");
        return _mapper.Map<List<PathPointDto>>(path);
    }
}