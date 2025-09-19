using System.Diagnostics;
using Domain;
using Domain.pathfinder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DAL.EntityFramework;

public class PortfolioDbContext : DbContext
{
    public DbSet<Floorplan> Floorplans { get; set; }
    public DbSet<Point> Points { get; set; }

    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=54326;Database=portfolio_db;Username=user;Password=postgres");
        }
        optionsBuilder.LogTo(message => Debug.WriteLine(message), LogLevel.Information);
    }

    public PortfolioDbContext(DbContextOptions options) : base(options)
    {
        PortfolioDbInitializer.Initialize(this, false);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Floorplan>()
            .HasMany(f => f.Points)
            .WithOne(p => p.Floorplan)
            .OnDelete(DeleteBehavior.Cascade);
    }
}