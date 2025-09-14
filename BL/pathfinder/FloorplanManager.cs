using DAL.Repository;
using Domain;
using Microsoft.Extensions.Logging;

namespace BL.pathfinder;

public class FloorplanManager
{
    private readonly FloorplanRepository _floorplanRepository;
    private readonly ILogger<FloorplanManager> _logger;

    public FloorplanManager(FloorplanRepository floorplanRepository, ILogger<FloorplanManager> logger)
    {
        _floorplanRepository = floorplanRepository;
        _logger = logger;
    }
    
    
    public IEnumerable<Floorplan> GetAllFloorplans()
    {
        return _floorplanRepository.ReadFloorplans();
    }

    public Floorplan GetFloorplanByNameAndFloor(string name, int floorNumber)
    {
        if (string.IsNullOrEmpty(name) || floorNumber <= 0)
        {
            _logger.LogWarning("Invalid input: name is null/empty or floorNumber is <= 0. Name: {name} - FloorNumber: {floorNumber}",name, floorNumber);
            throw new ArgumentException("Name must not be null/empty and floorNumber must be greater than zero.");
        }

        var floorplan = _floorplanRepository.ReadFloorplanByNameAndFloor(name, floorNumber);
        if (floorplan == null)
        {
            _logger.LogWarning("No floorplan found for name '{Name}' and floor '{FloorNumber}'.", name, floorNumber);
            throw new InvalidOperationException("No floorplan found for name '{name}' and floor '{floorNumber}'.");
        }

        return floorplan;
        
    }
}