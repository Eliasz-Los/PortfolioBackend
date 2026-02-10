using BL.DocuGroup;
using BL.DocuGroup.Draft;
using BL.DocuGroup.Dto.Component;
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
    public async Task<IActionResult> AddComponentToDocument(AddComponentDto componentDto)
    {
        await _componentManager.AddComponent(componentDto);
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
        await _componentManager.Reorder(reorderComponentDto);
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
        await _componentManager.Remove(documentId, componentId);
        return Ok();
    }
    
}