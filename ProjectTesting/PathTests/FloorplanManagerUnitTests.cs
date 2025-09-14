using BL.pathfinder;
using DAL.EntityFramework;
using DAL.Repository;
using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace ProjectTesting.PathTests;

public class FloorplanManagerUnitTests
{
    private readonly PortfolioDbContext _dbContext;
    private readonly FloorplanRepository _repo;
    private readonly FloorplanManager _manager;

    public FloorplanManagerUnitTests()
    {
        var options = new DbContextOptionsBuilder<PortfolioDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        
        _dbContext = new PortfolioDbContext(options);
        _repo = new FloorplanRepository(_dbContext);
        _manager = new FloorplanManager(_repo, new Mock<ILogger<FloorplanManager>>().Object);
        
    }
    
    [Fact]
    public void GetAllFloorplans_ShouldReturnAllFloorplans()
    {
        // Arrange
        _dbContext.Floorplans.Add(new Floorplan("Hospital A", 2, "1/200", "hospital_a_floor2.png"));
        _dbContext.SaveChanges();
        
        // Act
        var floorplans = _manager.GetAllFloorplans();

        // Assert
        Assert.NotNull(floorplans);
        Assert.Equal(2, floorplans.Count());
    }

    [Fact]
    public void GetFloorplanByNameAndFloor_ValidInput_ShouldReturnFloorplan()
    {
        // Arrange
        string name = "Hospital A";
        int floorNumber = 1;

        // Act
        var floorplan = _manager.GetFloorplanByNameAndFloor(name, floorNumber);

        // Assert
        Assert.NotNull(floorplan);
        Assert.Equal(name, floorplan.Name);
        Assert.Equal(floorNumber, floorplan.FloorNumber);
    }
}