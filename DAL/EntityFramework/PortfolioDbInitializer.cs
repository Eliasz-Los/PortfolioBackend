using Domain;
using Domain.pathfinder;

namespace DAL.EntityFramework;

public class PortfolioDbInitializer
{
    public static void Initialize(PortfolioDbContext context, bool dropDatabase = false)
    {
        if (dropDatabase)
            context.Database.EnsureDeleted();
        if (context.Database.EnsureCreated())
            Seed(context);
    }

    private static void Seed(PortfolioDbContext context)
    {
        Floorplan floorplan1 = new Floorplan("teachers_floor", 1,"1/200", "teachers_floor1.png");
        Floorplan testFloorplan = new Floorplan("game_floor", 2,"1/200", "test_floorplan.png");
        Floorplan maze = new Floorplan("maze_floor", 3,"1/200", "maze.png");
        context.Floorplans.AddRange(floorplan1, testFloorplan, maze);
        
        context.SaveChanges();
        
    }
}