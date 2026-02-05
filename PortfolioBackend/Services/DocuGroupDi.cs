using BL.DocuGroup;
using DAL.Repository.DocuGroup;
using DAL.Repository.UoW;

namespace PortfolioBackend.Services;

public static class DocuGroupDi
{
    public static IServiceCollection AddDocuGroupDi(this IServiceCollection services)
    {
        
        // Repository
        services.AddScoped<UnitOfWork>();
        services.AddScoped<IDocumentRepository,DocumentRepository>();
        services.AddScoped<IComponentRepository, ComponentRepository>();

        // Manager
        services.AddScoped<IDocumentManager, DocumentManager>();

        return services;
    }
}