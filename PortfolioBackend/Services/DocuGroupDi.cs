using BL.DocuGroup;
using BL.DocuGroup.Caching;
using BL.DocuGroup.Draft;
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
        services.AddScoped<IDocumentManager,DocumentManager>();
        services.AddScoped<IComponentManager, ComponentManager>();
        services.AddScoped<IMembershipManager, MembershipManager>();
        
        
        // Redis cache
        services.AddScoped<IDocumentDraftCache, DocumentDraftCache>();
        // Draft services working with the draft cache
        services.AddScoped<IDraftSnapshotService, DraftSnapshotService>();
        services.AddScoped<IDraftDocumentManager, DraftDocumentManager>();
        services.AddScoped<IDraftComponentManager, DraftComponentManager>();
        
        // Mappers
        services.AddAutoMapper(typeof(DocumentMappingProfile));
        services.AddAutoMapper(typeof(ComponentMappingProfile));
        return services;
    }
}