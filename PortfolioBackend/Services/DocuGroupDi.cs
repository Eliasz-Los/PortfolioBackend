using BL.DocuGroup;
using BL.DocuGroup.Caching;
using BL.DocuGroup.Mapper;
using DAL.Repository.DocuGroup;
using DAL.Repository.UoW;

namespace PortfolioBackend.Services;

public static class DocuGroupDi
{
    public static IServiceCollection AddDocuGroupDi(this IServiceCollection services)
    {
        
        // Repositories & UoW
        services.AddScoped<IUnitOfWork,UnitOfWork>();
        services.AddScoped<IDocumentRepository,DocumentRepository>();
        services.AddScoped<IComponentRepository, ComponentRepository>();
        services.AddScoped<IMembershipRepository, MembershipRepository>();

        // Managers
        services.AddScoped<DocumentManager>();
        services.AddScoped<IDocumentManager>(sp => sp.GetRequiredService<DocumentManager>());
        services.AddScoped<IDraftDocumentManager>(sp => sp.GetRequiredService<DocumentManager>());
        services.AddScoped<IComponentManager, ComponentManager>();
        services.AddScoped<IMembershipManager, MembershipManager>();
        
        // Mappers
        services.AddAutoMapper(typeof(DocumentMappingProfile));
        // Redis cache
        services.AddScoped<IDocumentDraftStore, DocumentDraftCache>();
        
        return services;
    }
}