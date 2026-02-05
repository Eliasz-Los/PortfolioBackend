using DAL.EntityFramework;
using Domain.DocuGroup;
using Domain.DocuGroup.types;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository.DocuGroup;

public class ComponentRepository : IComponentRepository
{
    private readonly PortfolioDbContext _context;

    public ComponentRepository(PortfolioDbContext context)
    {
        _context = context;
    }

    public async Task<DocumentComponent?> ReadComponentByDocumentIdAndComponentId(Guid documentId, Guid componentId)
    {
        return await _context.Components
            .FirstOrDefaultAsync(dc => dc.GroupDocumentId == documentId && dc.Id == componentId);
    }

    public async Task CreateComponentForDocumentByDocumentId(Guid documentId, DocumentComponent component)
    {
        component.GroupDocumentId = documentId;
        await _context.Components.AddAsync(component);
    }

    public async Task ReorderComponent(Guid documentId, Guid componentId, int newOrder)
    {
       
        var component =  await ReadComponentByDocumentIdAndComponentId(documentId, componentId);
        if (component == null)
        {
            throw new KeyNotFoundException("Component not found");
        }

        component.Reorder(newOrder);
    }

    public async Task ChangeType(Guid documentId, Guid componentId, ComponentType newType, bool clearLastPublishedContent = true)
    {
        var component = await ReadComponentByDocumentIdAndComponentId(documentId, componentId);
        if (component == null)
        {
            throw new KeyNotFoundException("Component not found");
        }
        component.ComponentType = newType;
        if (clearLastPublishedContent)
        {
            component.LastPublishedContentJson = null;
        }
    }
}