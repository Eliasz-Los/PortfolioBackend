using BL.DocuGroup.Dto.Component;
using BL.DocuGroup.Dto.Draft;

namespace BL.DocuGroup.Draft;

public class DraftComponentManager : IDraftComponentManager
{
    private readonly IDraftSnapshotService _snapshotService;

    public DraftComponentManager(IDraftSnapshotService draftSnapshotService)
    {
        _snapshotService = draftSnapshotService;
    }

    public async Task AddComponent(AddComponentDto dto)
    {
        var snapshot = await _snapshotService.GetOrCreate(dto.GroupDocumentId);

        var nextOrder = snapshot.Components.Count == 0 ? 1 : snapshot.Components.Max(c => c.Order) + 1;

        snapshot.Components.Add(new DraftComponent
        {
            Id = Guid.NewGuid(),
            Order = nextOrder,
            ComponentType = dto.ComponentType,
            LastPublishedContentJson = dto.LastPublishedContentJson
        });

        NormalizeOrder(snapshot);
        await _snapshotService.Save(dto.GroupDocumentId, snapshot);
        
    }

   

    public async Task ChangeContent(ChangeContentComponentDto dto)
    {
        var snapshot = await _snapshotService.GetOrCreate(dto.GroupDocumentId);

        var component = snapshot.Components.FirstOrDefault(c => c.Id == dto.Id);
        if (component == null) throw new KeyNotFoundException($"Component not found: {dto.Id}");

        component.LastPublishedContentJson = dto.LastPublishedContentJson;

        await _snapshotService.Save(dto.GroupDocumentId, snapshot);
    }

    public async Task Reorder(ReorderComponentDto dto)
    {
        var snapshot = await _snapshotService.GetOrCreate(dto.GroupDocumentId);

        var component = snapshot.Components.FirstOrDefault(c => c.Id == dto.Id);
        if (component == null) throw new KeyNotFoundException($"Component not found: {dto.Id}");

        component.Order = dto.NewOrder;

        NormalizeOrder(snapshot);
        await _snapshotService.Save(dto.GroupDocumentId, snapshot);
        
    }

    public async Task ChangeType(ChangeTypeComponentDto dto)
    {
        var snapshot = await _snapshotService.GetOrCreate(dto.GroupDocumentId);

        var component = snapshot.Components.FirstOrDefault(c => c.Id == dto.Id);
        if (component == null) throw new KeyNotFoundException($"Component not found: {dto.Id}");

        component.ComponentType = dto.Type;

        if (dto.ClearLastPublishedContent)
            component.LastPublishedContentJson = null;

        await _snapshotService.Save(dto.GroupDocumentId, snapshot);
        
    }

    public async Task Remove(Guid documentId, Guid componentId)
    {
        await _snapshotService.RemoveComponentFromDraftDocument(documentId, componentId);
    }
    
    private void NormalizeOrder(DraftDocument document)
    {
        document.Components = document.Components.OrderBy(c => c.Order).ToList();
    }
}