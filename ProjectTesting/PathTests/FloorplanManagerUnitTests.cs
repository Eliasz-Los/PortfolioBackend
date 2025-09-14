using BL.pathfinder;
using DAL.EntityFramework;
using DAL.Repository;
using Domain;
using Microsoft.EntityFrameworkCore;
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
        _manager = new FloorplanManager(_repo);
        
    }
    
    [Fact]
    public void GetAllFloorplans_ShouldReturnAllFloorplans()
    {
        // Arrange
        /*_dbContext.Floorplans.Add(new Floorplan("Hospital A", 1, "1/200", "hospital_a_floor1.png"));
        _dbContext.SaveChanges();*/
        
        // Act
        var floorplans = _manager.GetAllFloorplans();

        // Assert
        Assert.NotNull(floorplans);
        Assert.Single(floorplans);
    }
}