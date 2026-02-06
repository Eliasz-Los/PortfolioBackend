using DAL.EntityFramework;
using Domain.DocuGroup;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository.DocuGroup;

public class DocumentRepository : IDocumentRepository
{
    private readonly PortfolioDbContext _context;

    public DocumentRepository(PortfolioDbContext context)
    {
        _context = context;
    }

    public async Task<GroupDocument> ReadDocumentById(Guid documentId)
    {
        var document = await _context.GroupDocuments.FindAsync(documentId);
        if (document == null)
            throw new KeyNotFoundException($"Document with ID {documentId} not found.");
        return document;
    }

    public async Task<GroupDocument> ReadDocumentWithComponentsById(Guid documentId)
    {
        var documentWithComponents = await _context.GroupDocuments
            .Include(d => d.Components)
            .FirstOrDefaultAsync(d => d.Id == documentId);
        if (documentWithComponents == null)
            throw new KeyNotFoundException($"Document with ID {documentId} not found.");
        return documentWithComponents;
    }

    public async Task<IEnumerable<GroupDocument>> ReadAllDocumentsByUserId(string userId)
    {
        return await _context.GroupDocuments
            .Include(d => d.Memberships)
            .Where(d => d.Memberships.Any(m => m.UserId == userId))
            .ToListAsync();
    }

    public async Task CreateDocument(GroupDocument document)
    {
          await _context.GroupDocuments.AddAsync(document);
    }

    public async Task DeleteDocument(Guid documentId)
    {
        var document = await  ReadDocumentById(documentId);
        if (document is null)
        {
            throw new KeyNotFoundException($"Document with ID {documentId} not found.");
        }

        _context.GroupDocuments.Remove(document);
    }
}