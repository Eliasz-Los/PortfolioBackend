using BL.DocuGroup;
using BL.DocuGroup.Dto.Component;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PortfolioBackend.Controllers.DocuGroup;

[ApiController]
[Route("api/docugroup/components")]
[Authorize]
public class ComponentController : ControllerBase
{
    private readonly IComponentManager _componentManager;

    public ComponentController(IComponentManager componentManager)
    {
        _componentManager = componentManager;
    }
    
    [HttpPost]
    public async Task<IActionResult> AddComponentToDocument(AddComponentDto componentDto)
    {
        await _componentManager.AddComponentForDocumentByDocumentId(componentDto);
        return Ok();
    }

    [HttpPut("content")]
    public async Task<IActionResult> ChangeContent(ChangeContentComponentDto changeContentDto)
    {
        await _componentManager.ChangeContent(changeContentDto);
        return Ok();
    }

    [HttpPut("reorder")]
    public async Task<IActionResult> ReorderComponent(ReorderComponentDto reorderComponentDto)
    {
        await _componentManager.ReorderComponent(reorderComponentDto);
        return Ok();
    }

    [HttpPut("type")]
    public async Task<IActionResult> ChangeType(ChangeTypeComponentDto changeTypeComponentDto)
    {
        await _componentManager.ChangeType(changeTypeComponentDto);
        return Ok();
    }

    [HttpDelete("{documentId:guid}/{componentId:guid}")]
    public async Task<IActionResult> RemoveComponent(Guid documentId, Guid componentId)
    {
        await _componentManager.RemoveComponent(documentId, componentId);
        return Ok();
    }
    
}