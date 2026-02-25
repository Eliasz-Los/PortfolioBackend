using BL.DocuGroup;
using BL.DocuGroup.Draft;
using BL.DocuGroup.Dto;
using BL.DocuGroup.Dto.Component;
using BL.DocuGroup.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PortfolioBackend.Controllers.DocuGroup;

[ApiController]
[Route("api/docugroup/components")]
[Authorize]
// [Authorize(Roles = "user")]
public class ComponentController : ControllerBase
{
    private readonly IDraftComponentManager _componentManager;

    public ComponentController(IDraftComponentManager componentManager)
    {
        _componentManager = componentManager;
    }
    
    [HttpPost]
    public async Task<IActionResult> AddComponentToDocument(AddComponentDto componentDto,
        [FromServices] IDocumentEventBroker broker)
    {
        await _componentManager.AddComponent(componentDto);
        broker.Publish(new DocEvent
        {
            Type = "ComponentAdded",
            DocumentId = componentDto.GroupDocumentId,
            Payload = componentDto,
            Version = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
        return Ok();
    }

    [HttpPut("content")]
    public async Task<IActionResult> ChangeContent(ChangeContentComponentDto changeContentDto,
        [FromServices] IDocumentEventBroker broker)
    {
        await _componentManager.ChangeContent(changeContentDto);
        
        broker.Publish(new DocEvent
        {
            Type = "ChangeComponentContent",
            DocumentId = changeContentDto.GroupDocumentId,
            Payload = changeContentDto,
            Version = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
        
        return Ok();
    }

    [HttpPut("reorder")]
    public async Task<IActionResult> ReorderComponent(ReorderComponentDto reorderComponentDto,
        [FromServices] IDocumentEventBroker broker)
    {
        await _componentManager.Reorder(reorderComponentDto);
        
        broker.Publish(new DocEvent
        {
            Type = "ReorderComponent",
            DocumentId = reorderComponentDto.GroupDocumentId,
            Payload = reorderComponentDto,
            Version = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
        
        return Ok();
    }

    [HttpPut("type")]
    public async Task<IActionResult> ChangeType(ChangeTypeComponentDto changeTypeComponentDto,
        [FromServices] IDocumentEventBroker broker)
    {
        await _componentManager.ChangeType(changeTypeComponentDto);
       
        broker.Publish(new DocEvent
        {
            Type = "ChangeComponentType",
            DocumentId = changeTypeComponentDto.GroupDocumentId,
            Payload = changeTypeComponentDto,
            Version = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
        
        return Ok();
    }

    [HttpDelete("{documentId:guid}/{componentId:guid}")]
    public async Task<IActionResult> RemoveComponent(Guid documentId, Guid componentId,
        [FromServices] IDocumentEventBroker broker)
    {
        await _componentManager.Remove(documentId, componentId);
        broker.Publish(new DocEvent
        {
            Type = "RemoveComponent",
            DocumentId = documentId,
            Payload = new {DocumentId = documentId, ComponentId = componentId },
            Version = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
        return Ok();
    }
    
}