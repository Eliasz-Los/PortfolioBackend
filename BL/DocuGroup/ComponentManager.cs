using BL.DocuGroup.Dto.Component;
using DAL.Repository.DocuGroup;
using DAL.Repository.UoW;
using Domain.DocuGroup;

namespace BL.DocuGroup;

public class ComponentManager : IComponentManager
{
    private readonly IComponentRepository _componentRepository;
    private readonly IUnitOfWork _uow;

    public ComponentManager(IComponentRepository componentRepository, IUnitOfWork uow)
    {
        _componentRepository = componentRepository;
        _uow = uow;
    }

    public async Task<DocumentComponent> GetComponentByDocumentIdAndComponentId(Guid documentId, Guid componentId)
    {
        return await _componentRepository.ReadComponentByDocumentIdAndComponentId(documentId, componentId);
    }

    public async Task AddComponentForDocumentByDocumentId( AddComponentDto componentDto)
    {
        await _uow.BeginTransaction();
        try
        {
            var maxOrder = await GetMaxOrderForDocumentById(componentDto.GroupDocumentId);
            var component = new DocumentComponent
            {
                Id = Guid.NewGuid(),
                GroupDocumentId = componentDto.GroupDocumentId,
                ComponentType = componentDto.ComponentType,
                LastPublishedContentJson = componentDto.LastPublishedContentJson,
                Order = maxOrder + 1
            };
            await _componentRepository.CreateComponentForDocumentByDocumentId(component);
            await _uow.Commit();
        }
        catch
        {
            await _uow.Rollback();
            throw;
        }
    }

    public async Task ChangeContent(ChangeContentComponentDto changeContentDto)
    {
        await _uow.BeginTransaction();
        try
        {
            await _componentRepository.UpdateContent(changeContentDto.GroupDocumentId,
                changeContentDto.Id, changeContentDto.LastPublishedContentJson);
            await _uow.Commit();
        }
        catch
        {
            await _uow.Rollback();
            throw;
        }
    }

    public async Task ReorderComponent(ReorderComponentDto reorderComponentDto)
    {
        await _uow.BeginTransaction();
        try
        {
            await _componentRepository.ReorderComponent(reorderComponentDto.GroupDocumentId,
                reorderComponentDto.Id, reorderComponentDto.NewOrder);
            await _uow.Commit();
        }
        catch
        {
            await _uow.Rollback();
            throw;
        }
    }

    public async Task ChangeType(ChangeTypeComponentDto changeTypeDto)
    {
        await _uow.BeginTransaction();
        try
        {
            await _componentRepository.UpdateType(changeTypeDto.GroupDocumentId,
                changeTypeDto.Id, changeTypeDto.Type, changeTypeDto.clearLastPublishedContent);
            await _uow.Commit();
        }
        catch
        {
            await _uow.Rollback();
            throw;
        }
    }

    public async Task RemoveComponent(Guid documentId, Guid componentId)
    {
        await _uow.BeginTransaction();
        try
        {
            await _componentRepository.DeleteComponent(documentId, componentId);
            await _uow.Commit();
        }
        catch
        {
            await _uow.Rollback();
            throw;
        }
    }
    
    private async Task<int> GetMaxOrderForDocumentById(Guid documentId)
    {
        var components = await _componentRepository.ReadAllComponentsByDocumentId(documentId);
        var documentComponents = components.ToList();
        return documentComponents.Any() ? documentComponents.Max(c => c.Order) : 0;
    }
}