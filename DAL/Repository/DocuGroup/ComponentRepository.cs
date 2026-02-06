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

    public async Task<DocumentComponent> ReadComponentByDocumentIdAndComponentId(Guid documentId, Guid componentId)
    {
        var component = await _context.Components.FirstOrDefaultAsync(c => c.Id == componentId);
        
        if (component == null)
            throw new KeyNotFoundException($"Component not found by documentId= {documentId} and componentId= {componentId}");

        return component;
    }

    public async Task<IEnumerable<DocumentComponent>> ReadAllComponentsByDocumentId(Guid documentId)
    {
        var components = await _context.Components
            .Where(c => c.GroupDocumentId == documentId)
            .ToListAsync();
        return components;
    }

    public async Task CreateComponentForDocumentByDocumentId(DocumentComponent component)
    {
        component.GroupDocumentId = component.GroupDocumentId;
        await _context.Components.AddAsync(component);
    }

    public async Task UpdateContent(Guid documentId, Guid componentId, string? lastPublishedContentJson)
    {
        var component = await ReadComponentByDocumentIdAndComponentId(documentId, componentId);
        component.LastPublishedContentJson = lastPublishedContentJson;
        
    }

    public async Task ReorderComponent(Guid documentId, Guid componentId, int newOrder)
    {
       
        var component =  await ReadComponentByDocumentIdAndComponentId(documentId, componentId);
        component.Reorder(newOrder);
    }

    public async Task UpdateType(Guid documentId, Guid componentId, ComponentType newType, bool clearLastPublishedContent = true)
    {
        var component = await ReadComponentByDocumentIdAndComponentId(documentId, componentId);
        component.ComponentType = newType;
        
        if (clearLastPublishedContent)
        {
            component.LastPublishedContentJson = null;
        }
    }

    public async Task DeleteComponent(Guid documentId, Guid componentId)
    {
        var component = await ReadComponentByDocumentIdAndComponentId(documentId, componentId); 
        _context.Components.Remove(component);
    }
}