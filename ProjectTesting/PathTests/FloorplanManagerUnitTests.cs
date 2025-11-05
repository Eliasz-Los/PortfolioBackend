using BL.pathfinder;
using DAL.EntityFramework;
using DAL.Repository;
using Domain;
using Domain.pathfinder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace ProjectTesting.PathTests;

public class FloorplanManagerUnitTests
{
    private readonly Mock<IFloorplanRepository> _repo;
    private readonly Mock<ILogger<FloorplanManager>> _loggerMock;
    private readonly FloorplanManager _manager;

    public FloorplanManagerUnitTests()
    {
        _repo = new Mock<IFloorplanRepository>(); 
        _loggerMock = new Mock<ILogger<FloorplanManager>>();
        _manager = new FloorplanManager(_repo.Object, _loggerMock.Object);
    }
    
    [Fact]
    public void GetAllFloorplans_ShouldReturnAllFloorplans()
    {
        // Arrange
        var list = new List<Floorplan>
        {
            new Floorplan("Hospital A", 2, "1/200", "hospital_a_floor2.png"),
            new Floorplan("teachers_floor", 1, "1/100", "teachers_floor.png"),
            new Floorplan("office", 1, "1/100", "office1.png")
        };
        _repo.Setup(r => r.ReadFloorplans()).Returns(list);
        
        // Act
        var floorplans = _manager.GetAllFloorplans();

        // Assert
        Assert.NotNull(floorplans);
        Assert.Equal(3, floorplans.Count());
    }

    [Fact]
    public void GetFloorplanByNameAndFloor_ValidInput_ShouldReturnFloorplan()
    {
        // Arrange
        string name = "teachers_floor";
        int floorNumber = 1;
        var expected = new Floorplan(name, floorNumber, "1/100", "teachers_floor.png");
        _repo.Setup(r => r.ReadFloorplanByNameAndFloor(name, floorNumber)).Returns(expected);


        // Act
        var floorplan = _manager.GetFloorplanByNameAndFloor(name, floorNumber);

        // Assert
        Assert.NotNull(floorplan);
        Assert.Equal(name, floorplan.Name);
        Assert.Equal(floorNumber, floorplan.FloorNumber);
    }
}