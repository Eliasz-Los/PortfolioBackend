using BL.dto;
using BL.pathfinder.dto;

namespace BL.pathfinder;

public interface IPathManager
{
    Task<List<PathPointDto>> FindPath(PathRequestDto pathRequestDto, string folderPath);
}