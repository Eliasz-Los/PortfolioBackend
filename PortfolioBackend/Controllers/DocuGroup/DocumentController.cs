using System.Security.Claims;
using BL.DocuGroup;
using BL.DocuGroup.Dto.Document;
using Domain.DocuGroup;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PortfolioBackend.Controllers.DocuGroup;

[ApiController]
[Route("api/docugroup/documents/")]
[Authorize]
public class DocumentController : ControllerBase
{
    private readonly IDocumentManager _documentManager;
    private readonly IDraftDocumentManager _draftDocumentManager;

    public DocumentController(IDocumentManager documentManager, IDraftDocumentManager draftDocumentManager)
    {
        _documentManager = documentManager;
        _draftDocumentManager = draftDocumentManager;
    }

    [HttpGet("{documentId}")]
    public async Task<IActionResult> GetDocumentById(Guid documentId)
    {
        var document = await _draftDocumentManager.GetDraftDocumentWithComponentsById(documentId);
        return Ok(document);
    }

    [HttpGet]
    public async Task<IActionResult> GetUsersDocuments()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                     User.FindFirstValue("sub");

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();
        
        var documents = await _documentManager.GetAllDocumentsByUserId(userId);
        return Ok(documents);
    }

    [HttpPost]
    public async Task<IActionResult> CreateDocument(GroupDocument document)
    {
        var userId =
            User.FindFirstValue(ClaimTypes.NameIdentifier) ??
            User.FindFirstValue("sub");

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();
        
        await _documentManager.AddDocument(document);
        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteDocument(Guid documentId)
    {
        await _documentManager.DeleteDocument(documentId);
        return Ok();
    }

    [HttpPost("publish")]
    public async Task<IActionResult> PublishDocument(PublishDto publishDto)
    {
        var userId =
            User.FindFirstValue(ClaimTypes.NameIdentifier) ??
            User.FindFirstValue("sub");
        
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();
        
        publishDto.publishedByUserId = userId;
        await _documentManager.PublishDocument(publishDto);
        return Ok();
    }
    
    
}