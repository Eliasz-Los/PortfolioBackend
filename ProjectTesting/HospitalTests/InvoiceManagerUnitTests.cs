using AutoMapper;
using BL.hospital;
using BL.hospital.dto;
using DAL.Repository.hospital;
using Domain.hospital;
using Domain.hospital.types;
using Moq;
using QuestPDF.Infrastructure;

namespace ProjectTesting.HospitalTests;

public class InvoiceManagerUnitTests
{
    
    private readonly Mock<IInvoiceRepository> _invoiceRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly InvoiceManager _invoiceManager;

    public InvoiceManagerUnitTests()
    {
        _invoiceRepositoryMock = new Mock<IInvoiceRepository>();
        _mapperMock = new Mock<IMapper>();

        _invoiceManager = new InvoiceManager(
            _invoiceRepositoryMock.Object,
            _mapperMock.Object
        );
    }
    
    
    [Fact]
    public void GenerateInvoicePdf_ReturnsByteArray()
    {
        // Arrange
        
        var patientId = Guid.NewGuid();

        var patient = new Patient(
            new Name("PatientFirst", "PatientLast"),
            new DateOnly(2000, 1, 1),
            "patient@mail.com",
            "555-000-1234",
            new Location("Mortselhaar", 154, "Antwerp", "2640", "Belgium"),
            patientId
        );
        
        var invoice = new Invoice
        {
            Id = Guid.NewGuid(),
            Title = "Medical Consultation",
            Amount = 100m,
            DueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(30)),
            IsPaid = false,
            Patient = patient,
            Description = "Consultation for general health check-up.",
            InvoiceDate = DateOnly.FromDateTime(DateTime.Now),
            InvoiceNumber = "INV-20231010-0001"
        };

        // Act
        var result = _invoiceManager.GenerateInvoicePdf(invoice);

        // Assert in this case we just check that a byte array is returned
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }
    
    [Fact]
    public async Task GetInvoiceWithPatientByInvoiceId_ReturnsInvoice_WhenFound()
    {
        // Arrange
        var invoiceId = Guid.NewGuid();
        var invoice = new Invoice
        {
            Id = invoiceId,
            Title = "Medical Consultation",
            Amount = 100m,
            DueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(30)),
            IsPaid = false,
            Description = "Consultation for general health check-up.",
            InvoiceDate = DateOnly.FromDateTime(DateTime.Now),
            InvoiceNumber = "INV-20231010-0001"
        };

        _invoiceRepositoryMock
            .Setup(repo => repo.ReadInvoiceWithPatientByInvoiceId(invoiceId))
            .ReturnsAsync(invoice);

        // Act
        var result = await _invoiceManager.GetInvoiceWithPatientByInvoiceId(invoiceId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(invoiceId, result!.Id);
        Assert.Equal("Medical Consultation", result.Title);
    }

    
    [Fact]
    public async Task GetInvoiceWithPatientByInvoiceId_ReturnsNull_WhenNotFound()
    {
        // Arrange
        var invoiceId = Guid.NewGuid();

        _invoiceRepositoryMock
            .Setup(repo => repo.ReadInvoiceWithPatientByInvoiceId(invoiceId))
            .ReturnsAsync((Invoice?)null);

        // Act
        var result = await _invoiceManager.GetInvoiceWithPatientByInvoiceId(invoiceId);

        // Assert
        Assert.Null(result);
    }

    
    
    [Fact]
    public async Task GetAllByPatientId_ReturnsInvoiceDtos()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var dueDate = DateOnly.FromDateTime(DateTime.Now).AddDays(30);
        var invoices = new List<Invoice>
        {
            new Invoice { Id = Guid.NewGuid(), Title = "Medical invoice", DueDate = dueDate, IsPaid = false },
            new Invoice { Id = Guid.NewGuid(), Title = "General check up", DueDate = dueDate, IsPaid = false },
            new Invoice { Id = Guid.NewGuid(), Title = "Medical invoice for an operation", DueDate = dueDate, IsPaid = false },
        };

        var invoiceDtos = new List<InvoiceDto>
        {
            new InvoiceDto { Id = invoices[0].Id, Title = "Medical invoice", DueDate = dueDate, IsPaid = false },
            new InvoiceDto { Id = Guid.NewGuid(), Title = "General check up", DueDate = dueDate, IsPaid = false },
            new InvoiceDto { Id = Guid.NewGuid(), Title = "Medical invoice for an operation", DueDate = dueDate, IsPaid = false }
        };

        _invoiceRepositoryMock
            .Setup(repo => repo.ReadAllByPatientId(patientId))
            .ReturnsAsync(invoices);

        _mapperMock
            .Setup(mapper => mapper.Map<IEnumerable<InvoiceDto>>(invoices))
            .Returns(invoiceDtos);

        // Act
        var result = await _invoiceManager.GetAllByPatientId(patientId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count());
    }

    
    [Fact]
    public async Task Add_ReturnsCreatedInvoice()
    {
        // Arrange
        var invoice = new Invoice
        {
            Id = Guid.NewGuid(),
            Amount = 100m
        };

        _invoiceRepositoryMock
            .Setup(repo => repo.Create(invoice))
            .ReturnsAsync(invoice);

        // Act
        var result = await _invoiceManager.Add(invoice);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(invoice.Id, result!.Id);
    }

    
    
    [Fact]
    public async Task Add_ReturnsNull_WhenInvoiceIsNull()
    {
        // Arrange
        _invoiceRepositoryMock
            .Setup(repo => repo.Create(null))
            .ReturnsAsync((Invoice?)null);

        // Act
        var result = await _invoiceManager.Add(null);

        // Assert
        Assert.Null(result);
    }

    

}