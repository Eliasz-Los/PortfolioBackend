using Domain;

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
        Floorplan floorplan1 = new Floorplan("Hospital A", 1,"1/200", "hospital_a_floor1.png");
        
        context.Floorplans.Add(floorplan1);
        
        context.SaveChanges();
        
    }
}