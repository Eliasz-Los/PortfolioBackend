using BL.pathfinder;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace PortfolioBackend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FloorplanController : ControllerBase
{
    private readonly FloorplanManager _floorplanManager;

    public FloorplanController(FloorplanManager floorplanManager)
    {
        _floorplanManager = floorplanManager;
    }
    
    [HttpGet("floorplans")]
    public ActionResult<IEnumerable<Floorplan>> GetFloorplans()
    {
        var floorplans = _floorplanManager.GetAllFloorplans();
        return Ok(floorplans);
    }
}