using AutoMapper;
using BL.dto;
using BL.pathfinder;
using BL.Pathfinder.algorithm;
using BL.Pathfinder.analyzer;
using BL.pathfinder.dto;
using Domain.pathfinder;
using Microsoft.Extensions.Logging;

namespace BL.Pathfinder;

public class SecondPathManager : IPathManager
{
    private readonly ISecondAnalyzer _secondAnalyzer;
    private readonly ISecondAStar _secondAStar;
    private readonly IFloorplanManager _floorplanManager;
    private readonly IMapper _mapper;
    private readonly ILogger<SecondPathManager> _logger;

    public SecondPathManager(ILogger<SecondPathManager> logger, IMapper mapper, IFloorplanManager floorplanManager, ISecondAStar secondAStar, ISecondAnalyzer secondAnalyzer)
    {
        _logger = logger;
        _mapper = mapper;
        _floorplanManager = floorplanManager;
        _secondAStar = secondAStar;
        _secondAnalyzer = secondAnalyzer;
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
        var (startP, endP, walkablePoints) = await Task.Run( () => _secondAnalyzer.GetWalkableGrid(imagePath, start ,end));
        
        // navigating the path
        var path = await Task.Run(() => _secondAStar.FindPath(startP, endP, walkablePoints));
        _logger.LogInformation($"Path found: {path} {path.Count}");
        return _mapper.Map<List<PathPointDto>>(path);
        
    }
}