using BL.pathfinder;
using BL.pathfinder.dto;
using Microsoft.AspNetCore.Mvc;

namespace PortfolioBackend.Controllers.pathfinder;

[Route("api/[controller]")]
[ApiController]
public class PathController : ControllerBase
{
    private readonly IPathManager _pathManager;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public PathController(IPathManager pathManager, IWebHostEnvironment webHostEnvironment)
    {
        _pathManager = pathManager;
        _webHostEnvironment = webHostEnvironment;
    }

    [HttpPost("route")]
    public async Task<IActionResult> FindPath(PathRequestDto pathRequestDto)
    {
        string folderPath = Path.Combine(_webHostEnvironment.ContentRootPath, "floor_images");
        var path = await _pathManager.FindPath(pathRequestDto, folderPath);
        
        if (!path.Any())
        {
            return NotFound("No path found.");
        }

        return Ok(path);
    }
}