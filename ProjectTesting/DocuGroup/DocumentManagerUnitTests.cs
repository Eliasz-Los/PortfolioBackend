using AutoMapper;
using BL.DocuGroup;
using BL.DocuGroup.Caching;
using BL.DocuGroup.Draft;
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
    private readonly Mock<IDocumentDraftCache> _draftStore;
    private readonly Mock<IComponentManager> _componentManager;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly Mock<IDocumentRepository> _documentRepository;
    private readonly DocumentManager _documentManager;
    private readonly Mock<IMembershipManager> _membershipManager;
    private readonly Mock<IMapper> _mapper;
    private readonly Mock<IDraftDocumentManager> _draftDocumentManager;
    
    public DocumentManagerUnitTests()
    {
        _membershipManager = new Mock<IMembershipManager>();
        _draftStore = new Mock<IDocumentDraftCache>();
        _componentManager = new Mock<IComponentManager>();
        _documentRepository = new Mock<IDocumentRepository>();
        _mapper = new Mock<IMapper>();
        _draftDocumentManager = new Mock<IDraftDocumentManager>();
        
        //Im not testing repository here, so I can just set up the UOW to do nothing on transaction methods, and not worry about the repository's internal behavior.
        //The repository will be verified for calls,
        //but its methods won't have any side effects or
        //return values unless explicitly set up in the test.
        _unitOfWork = new Mock<IUnitOfWork>();
        _unitOfWork.Setup(x => x.BeginTransaction()).Returns(Task.CompletedTask);
        _unitOfWork.Setup(x => x.Commit()).Returns(Task.CompletedTask);
        _unitOfWork.Setup(x => x.Rollback()).Returns(Task.CompletedTask);
        
        _documentManager = new DocumentManager(
            _documentRepository.Object,
            _unitOfWork.Object,
            _componentManager.Object, 
            _draftStore.Object,
            _membershipManager.Object, 
            _mapper.Object, 
            _draftDocumentManager.Object);
    }
    
    [Fact]
    public async Task AddDocument_CreatesDocument_AddsTwoComponents_AndCommits()
    {
        //Arrange
        var addDocumentDto = new AddDocumentDto { Title = "New Doc" };
        var userId = "user-1";
        Guid generatedId = Guid.NewGuid();
        
        _documentRepository
            .Setup(r => r.CreateDocument(It.IsAny<GroupDocument>()))
            .Callback<GroupDocument>(d => generatedId = d.Id)
            .Returns(Task.CompletedTask);

        _membershipManager
            .Setup(m => m.AddMembership(It.IsAny<Membership>()))
            .Returns(Task.CompletedTask);

        _componentManager
            .Setup(c => c.AddComponentForDocumentByDocumentId(It.IsAny<AddComponentDto>()))
            .Returns(Task.CompletedTask);
        //Act
        await _documentManager.AddDocument(addDocumentDto, userId);
        
        //assert
        _documentRepository.Verify(r => r.CreateDocument(It.Is<GroupDocument>(d =>
            d.Id == generatedId &&
            d.Title == addDocumentDto.Title &&
            d.CreatedByUserId == userId
        )), Times.Once);
        
        _membershipManager.Verify(m => m.AddMembership(It.Is<Membership>(m =>
            m.GroupDocumentId == generatedId &&
            m.UserId == userId &&
            m.Role == DocumentRole.Owner
        )), Times.Once);
        
        //I create 2 components as part of the initial draft setup

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
        var expectedDto = new DocumentDetailsDto { Id = id };

        _documentRepository
            .Setup(r => r.ReadDocumentWithComponentsById(id))
            .ReturnsAsync(doc);
        
        _mapper!
            .Setup(m => m.Map<DocumentDetailsDto>(doc))
            .Returns(expectedDto);
   
        var result = await _documentManager.GetDocumentWithComponentsById(id);

        Assert.Same(expectedDto, result);
        _documentRepository.Verify(r => r.ReadDocumentWithComponentsById(id), Times.Once);
        _mapper.Verify(m => m.Map<DocumentDetailsDto>(doc), Times.Once);
    }

    [Fact]
    public async Task GetAllDocumentsByUserId_ReturnsDocuments()
    {
        //Arrange
        var userId = "user-1";
        var docs = new[] { new GroupDocument { Id = Guid.NewGuid() } };

        _documentRepository.Setup(r => r.ReadAllDocumentsByUserId(userId)).ReturnsAsync(docs);
        _mapper
            .Setup(m => m.Map<IEnumerable<DocumentDto>>(It.Is<IEnumerable<GroupDocument>>(x => x == docs)))
            .Returns(new[] { new DocumentDto() });
        
        //Act
        var result = await _documentManager.GetAllDocumentsByUserId(userId);

        //Assert
        Assert.Single(result);
        _documentRepository.Verify(r => r.ReadAllDocumentsByUserId(userId), Times.Once);
    }
    

    [Fact]
    public async Task PublishDocument_UpdatesDocumentAndComponentsAndRemovesDraft()
    {
        //arrange
        
        var docId = Guid.NewGuid();
        
        var draft = new DraftDocument
        {
            Id = docId,
            Title = "Draft Title",
            Components = new List<DraftComponent>
            {
                new DraftComponent
                {
                    Id = Guid.NewGuid(),
                    Order = 2,
                    ComponentType = ComponentType.Paragraph,
                    LastPublishedContentJson = "B"
                },
                new DraftComponent
                {
                    Id = Guid.NewGuid(),
                    Order = 1,
                    ComponentType = ComponentType.Title,
                    LastPublishedContentJson = "A"
                }
            }
        };

        _draftDocumentManager
            .Setup(m => m.GetDraftDocumentWithComponentsById(docId))
            .ReturnsAsync(draft);

        _componentManager
            .Setup(m => m.SyncComponentsByDocumentId(docId, It.IsAny<IReadOnlyList<DocumentComponent>>()))
            .Returns(Task.CompletedTask);

        _draftStore
            .Setup(c => c.RemoveDraft(docId))
            .Returns(Task.CompletedTask);

        var dto = new PublishDto { Id = docId, Title = "New", publishedByUserId = "u" };

        //Act
        await _documentManager.PublishDocument(dto);

        //assert - used draft atleast
        _draftDocumentManager.Verify(m => m.GetDraftDocumentWithComponentsById(docId), Times.Once);
        //assert - synced components  and order
        _componentManager.Verify(m => m.SyncComponentsByDocumentId(docId, It.Is<IReadOnlyList<DocumentComponent>>(components =>
            components.Count == 2 &&
            components[0].Id == draft.Components[1].Id && 
            components[0].ComponentType == draft.Components[1].ComponentType &&
            components[0].LastPublishedContentJson == draft.Components[1].LastPublishedContentJson &&
            components[1].Id == draft.Components[0].Id &&
            components[1].ComponentType == draft.Components[0].ComponentType &&
            components[1].LastPublishedContentJson == draft.Components[0].LastPublishedContentJson
        )), Times.Once);
        
        
        //assert - removing draft from cache, since it will be published now and is not needed

        _unitOfWork.Verify(x => x.BeginTransaction(), Times.Once);
        _unitOfWork.Verify(x => x.Commit(), Times.Once);
        _draftStore.Verify(s => s.RemoveDraft(docId), Times.Once);
    }

    [Fact]
    public async Task PublishDocument_Throws_WhenNoDraftExists()
    {
        var id = Guid.NewGuid();
        _draftStore.Setup(s => s.GetDraftSnapshotJson(id)).ReturnsAsync((string?)null);

        var dto = new PublishDto { Id = id, Title = "T", publishedByUserId = "u" };

        //assert
        await Assert.ThrowsAsync<NullReferenceException>(() => _documentManager.PublishDocument(dto));

        _unitOfWork.Verify(x => x.Rollback(), Times.Once);
    }
    
}