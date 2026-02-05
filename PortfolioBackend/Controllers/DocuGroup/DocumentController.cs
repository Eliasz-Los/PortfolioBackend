using BL.DocuGroup;
using Domain.DocuGroup;
using Microsoft.AspNetCore.Mvc;

namespace PortfolioBackend.Controllers.DocuGroup;

[ApiController]
[Route("api/documents/")]
public class DocumentController : ControllerBase
{
    private readonly IDocumentManager _documentManager;

    public DocumentController(IDocumentManager documentManager)
    {
        _documentManager = documentManager;
    }

    [HttpGet("{documentId}")]
    public async Task<IActionResult> GetDocumentById(Guid documentId)
    {
        var document = await _documentManager.GetDocumentWithComponentsById(documentId);

        return Ok(document);
    }

    [HttpPost]
    public async Task<IActionResult> CreateDocument(GroupDocument document)
    {
        await _documentManager.AddDocument(document);
        return Ok();
    }

}