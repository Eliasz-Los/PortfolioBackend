using BL.DocuGroup;
using BL.DocuGroup.Caching;
using DAL.Repository.DocuGroup;
using DAL.Repository.UoW;

namespace PortfolioBackend.Services;

public static class DocuGroupDi
{
    public static IServiceCollection AddDocuGroupDi(this IServiceCollection services)
    {
        
        // Repositories & UoW
        services.AddScoped<UnitOfWork>();
        services.AddScoped<IDocumentRepository,DocumentRepository>();
        services.AddScoped<IComponentRepository, ComponentRepository>();

        // Managers
        services.AddScoped<DocumentManager>();
        services.AddScoped<IDocumentManager>(sp => sp.GetRequiredService<DocumentManager>());
        services.AddScoped<IDraftDocumentManager>(sp => sp.GetRequiredService<DocumentManager>());
        services.AddScoped<IComponentManager, ComponentManager>();
        
        // Mappers
        
        // Redis cache
        services.AddScoped<IDocumentDraftStore, DocumentDraftCache>();
        
        return services;
    }
}