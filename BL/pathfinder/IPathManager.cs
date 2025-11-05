using BL.dto;

namespace BL.pathfinder;

public interface IPathManager
{
    Task<List<PathPointDto>> FindPath(PathRequestDto pathRequestDto, string folderPath);
}