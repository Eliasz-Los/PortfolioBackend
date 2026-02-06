using BL.DocuGroup;
using BL.DocuGroup.Caching;
using BL.DocuGroup.Dto.Component;
using BL.DocuGroup.Dto.Document;
using BL.DocuGroup.Dto.Draft;
using DAL.Repository.DocuGroup;
using DAL.Repository.UoW;
using Domain.DocuGroup;
using Domain.DocuGroup.types;
using Moq;

namespace ProjectTesting.DocuGroup;

public class DocumentManagerUnitTests
{
    private readonly Mock<IDocumentDraftStore> _draftStore;
    private readonly Mock<IComponentManager> _componentManager;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly Mock<IDocumentRepository> _documentRepository;
    private readonly DocumentManager _documentManager;
    
    public DocumentManagerUnitTests()
    {
        _draftStore = new Mock<IDocumentDraftStore>();
        _componentManager = new Mock<IComponentManager>();
        _documentRepository = new Mock<IDocumentRepository>();
        
        _unitOfWork = new Mock<IUnitOfWork>();
        _unitOfWork.Setup(x => x.BeginTransaction()).Returns(Task.CompletedTask);
        _unitOfWork.Setup(x => x.Commit()).Returns(Task.CompletedTask);
        _unitOfWork.Setup(x => x.Rollback()).Returns(Task.CompletedTask);
        
        _documentManager = new DocumentManager(
            _documentRepository.Object,
            _unitOfWork.Object,
            _componentManager.Object, 
            _draftStore.Object);
    }
    
    [Fact]
    public async Task AddDocument_CreatesDocument_AddsTwoComponents_AndCommits()
    {
        var doc = new GroupDocument { Id = Guid.NewGuid(), Title = "T" };

        _unitOfWork.Setup(x => x.BeginTransaction()).Returns(Task.CompletedTask);
        _documentRepository.Setup(x => x.CreateDocument(doc)).Returns(Task.CompletedTask);

        _componentManager
            .Setup(x => x.AddComponentForDocumentByDocumentId(It.IsAny<AddComponentDto>()))
            .Returns(Task.CompletedTask);

        _unitOfWork.Setup(x => x.Commit()).Returns(Task.CompletedTask);

        await _documentManager.AddDocument(doc);

        _documentRepository.Verify(r => r.CreateDocument(doc), Times.Once);

        _componentManager.Verify(x => x.AddComponentForDocumentByDocumentId(It.Is<AddComponentDto>(d =>
            d.GroupDocumentId == doc.Id &&
            d.ComponentType == ComponentType.Title &&
            d.LastPublishedContentJson == "Welcome to your new document!")), Times.Once);

        _componentManager.Verify(x => x.AddComponentForDocumentByDocumentId(It.Is<AddComponentDto>(d =>
            d.GroupDocumentId == doc.Id &&
            d.ComponentType == ComponentType.Paragraph &&
            d.LastPublishedContentJson == "Here is a sample paragraph. You can edit this content.")), Times.Once);

        _unitOfWork.Verify(x => x.BeginTransaction(), Times.Once);
        _unitOfWork.Verify(x => x.Commit(), Times.Once);
        _unitOfWork.Verify(x => x.Rollback(), Times.Never);
        
        _draftStore.VerifyNoOtherCalls();
        _documentRepository.VerifyNoOtherCalls();
        _componentManager.VerifyNoOtherCalls();
        _unitOfWork.VerifyNoOtherCalls();
    }
    
    
    [Fact]
    public async Task GetDocumentWithComponentsById_ReturnsDocument()
    {
        var id = Guid.NewGuid();
        var doc = new GroupDocument { Id = id };

        _documentRepository.Setup(r => r.ReadDocumentWithComponentsById(id)).ReturnsAsync(doc);

        var result = await _documentManager.GetDocumentWithComponentsById(id);

        Assert.Same(doc, result);
        _documentRepository.Verify(r => r.ReadDocumentWithComponentsById(id), Times.Once);
    }

    [Fact]
    public async Task GetAllDocumentsByUserId_ReturnsDocuments()
    {
        var userId = "user-1";
        var docs = new[] { new GroupDocument { Id = Guid.NewGuid() } };

        _documentRepository.Setup(r => r.ReadAllDocumentsByUserId(userId)).ReturnsAsync(docs);

        var result = await _documentManager.GetAllDocumentsByUserId(userId);

        Assert.Single(result);
        _documentRepository.Verify(r => r.ReadAllDocumentsByUserId(userId), Times.Once);
    }

    [Fact]
    public async Task SaveDraftSnapshot_StoresDraftInCache()
    {
        var dto = new DraftDto
        {
            Id = Guid.NewGuid(),
            SnapshotJson = "{ \"x\": 1 }"
        };

        _draftStore
            .Setup(s => s.SetDraftSnapshotJson(dto.Id, dto.SnapshotJson, It.IsAny<TimeSpan>()))
            .Returns(Task.CompletedTask);

        await _documentManager.SaveDraftSnapshot(dto);

        _draftStore.Verify(
            s => s.SetDraftSnapshotJson(dto.Id, dto.SnapshotJson, It.Is<TimeSpan>(t => t.TotalDays == 30)),
            Times.Once);
    }

    [Fact]
    public async Task PublishDocument_UpdatesDocumentAndRemovesDraft()
    {
        var id = Guid.NewGuid();
        var draftJson = "{ \"snapshot\": true }";
        var doc = new GroupDocument { Id = id, Title = "Old" };

        _draftStore.Setup(s => s.GetDraftSnapshotJson(id)).ReturnsAsync(draftJson);
        _documentRepository.Setup(r => r.ReadDocumentById(id)).ReturnsAsync(doc);
        _draftStore.Setup(s => s.RemoveDraft(id)).Returns(Task.CompletedTask);

        var dto = new PublishDto { Id = id, Title = "New", publishedByUserId = "u" };

        await _documentManager.PublishDocument(dto);

        Assert.Equal("New", doc.Title);
        Assert.Equal(draftJson, doc.SnapshotJson);

        _unitOfWork.Verify(x => x.BeginTransaction(), Times.Once);
        _unitOfWork.Verify(x => x.Commit(), Times.Once);
        _draftStore.Verify(s => s.RemoveDraft(id), Times.Once);
    }

    [Fact]
    public async Task PublishDocument_Throws_WhenNoDraftExists()
    {
        var id = Guid.NewGuid();
        _draftStore.Setup(s => s.GetDraftSnapshotJson(id)).ReturnsAsync((string?)null);

        var dto = new PublishDto { Id = id, Title = "T", publishedByUserId = "u" };

        await Assert.ThrowsAsync<InvalidOperationException>(() => _documentManager.PublishDocument(dto));

        _unitOfWork.Verify(x => x.Rollback(), Times.Once);
    }

    [Fact]
    public async Task GetDraftDocumentWithComponentsById_UsesDraftIfPresent()
    {
        var id = Guid.NewGuid();
        var doc = new GroupDocument { Id = id, Title = "PublishedTitle" };
        var draftJson = "{ \"draft\": 1 }";

        _documentRepository.Setup(r => r.ReadDocumentWithComponentsById(id)).ReturnsAsync(doc);
        _draftStore.Setup(s => s.GetDraftSnapshotJson(id)).ReturnsAsync(draftJson);

        var result = await _documentManager.GetDraftDocumentWithComponentsById(id);

        Assert.Same(doc, result);
        Assert.Equal(draftJson, doc.SnapshotJson);
    }

    
    
}