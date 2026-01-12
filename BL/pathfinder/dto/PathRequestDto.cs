
using BL.dto;

namespace BL.pathfinder.dto;

public class PathRequestDto
{
    public PathPointDto Start { get; set; }
    public PathPointDto End { get; set; }
    public String FloorplanName { get; set; }
    public int FloorNumber { get; set; }
}