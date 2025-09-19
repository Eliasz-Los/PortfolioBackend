﻿using BL.dto;
using BL.pathfinder;
using Microsoft.AspNetCore.Mvc;

namespace PortfolioBackend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PathController : ControllerBase
{
    private readonly PathManager _pathManager;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public PathController(PathManager pathManager, IWebHostEnvironment webHostEnvironment)
    {
        _pathManager = pathManager;
        _webHostEnvironment = webHostEnvironment;
    }

    [HttpGet("route")]
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