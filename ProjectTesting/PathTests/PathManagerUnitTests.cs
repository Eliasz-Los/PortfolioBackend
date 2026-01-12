using AutoMapper;
using BL.dto;
using BL.pathfinder;
using BL.pathfinder.dto;
using Domain.pathfinder;
using Microsoft.Extensions.Logging;
using Moq;

namespace ProjectTesting.PathTests;

public class PathManagerUnitTests
{

    public PathManagerUnitTests()
    {
        
    }
    [Fact]
    public async Task FindPath_ReturnsMappedPathPoints()
    {
        // Arrange
        var logger = Mock.Of<ILogger<PathManager>>();
        var floorplanManager = new Mock<IFloorplanManager>();
        var mapper = new Mock<IMapper>();
        var pathfinding = new Mock<IPathfinding>();
        var analyzer = new Mock<IFloorplanAnalyzer>();

        var floorplan = new Floorplan { Name = "game_floor", FloorNumber = 3, Scale = "1/200", Image = "image.png" };
        
        var manager = new PathManager(
            logger,
            floorplanManager.Object,
            mapper.Object,
            pathfinding.Object,
            analyzer.Object
        );

        var dto = new PathRequestDto
        {
            Start = new PathPointDto { XWidth = 0, YHeight = 0 },
            End = new PathPointDto { XWidth = 10, YHeight = 10 },
            FloorplanName = "game_floor",
            FloorNumber = 2
        };

        var start = new Point(0, 0);
        var end = new Point(10, 10);
        var walkablePoints = new HashSet<Point> { start, end };
        
        mapper.Setup(m => m.Map<Point>(dto.Start)).Returns(start);
        mapper.Setup(m => m.Map<Point>(dto.End)).Returns(end);

        floorplanManager
            .Setup(f => f.GetFloorplanByNameAndFloor("game_floor", 2))
            .Returns(floorplan);

        analyzer
            .Setup(a => a.GetWalkablePoints(It.IsAny<string>(), start, end))
            .Returns((start, end, walkablePoints));

        pathfinding
            .Setup(p => p.FindPath(start, end, It.IsAny<HashSet<Point>>()))
            .Returns(new List<Point> { start, end });

        mapper
            .Setup(m => m.Map<List<PathPointDto>>(It.IsAny<List<Point>>()))
            .Returns(new List<PathPointDto> { dto.Start, dto.End });

        // Act
        var result = await manager.FindPath(dto, "C:\\images");

        // Assert
        Assert.Equal(2, result.Count);
    }

}