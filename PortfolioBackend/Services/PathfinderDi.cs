using BL.pathfinder;
using BL.Pathfinder;
using BL.Pathfinder.algorithm;
using BL.Pathfinder.analyzer;
using BL.pathfinder.mapper;
using DAL.Repository;

namespace PortfolioBackend.Services;

public static class PathfinderDi
{
    public static IServiceCollection AddPathfinderDi(this IServiceCollection services)
    {
        
        //services.AddScoped<IPathManager, PathManager>();
       services.AddScoped<IPathfinding, AStarPathfinding>();
       services.AddScoped<IFloorplanAnalyzer, FloorplanAnalyzer>();
       services.AddScoped<IFloorplanRepository,FloorplanRepository>();
       services.AddScoped<IFloorplanManager,FloorplanManager>();
       services.AddScoped<ISecondAnalyzer, SecondFpAnalyzer>();
       services.AddScoped<ISecondAStar, AStarGridPathfinding>();
       services.AddScoped<IPathManager, SecondPathManager>();
       
       // mappers
      services.AddAutoMapper(typeof(PointMappingProfile));
        
        return services;
    }
}