
namespace BL.dto;

public class PathRequestDto
{
    public PathPointDto Start { get; set; }
    public PathPointDto End { get; set; }
    public String Name { get; set; }
    public int FloorNumber { get; set; }
}