using AutoMapper;
using BL.hospital;
using BL.hospital.dto;
using BL.hospital.validation;
using DAL.Repository.hospital;
using Domain.hospital;
using Domain.hospital.types;
using Moq;

namespace ProjectTesting.HospitalTests;

public class PatientUnitTests
{
 
    private readonly Mock<IBaseRepository<Patient>> _patientRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IValidation<Patient>> _validationMock;

    private readonly PatientManager _patientManager;

    public PatientUnitTests()
    {
        _patientRepositoryMock = new Mock<IBaseRepository<Patient>>();
        _mapperMock = new Mock<IMapper>();
        _validationMock = new Mock<IValidation<Patient>>();

        _patientManager = new PatientManager(
            _patientRepositoryMock.Object,
            _mapperMock.Object,
            _validationMock.Object
        );
    }

    [Fact]
    public async Task GetPatientById_ReturnsPatientDto_WhenPatientExists()
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

        var patientDto = new PatientDto
        {
            Id = patientId,
            FullName =  new Name("PatientFirst", "PatientLast"),
            DateOfBirth = new DateOnly(2000, 1, 1),
            Email = "patient@mail.com",
            PhoneNumber = "555-000-1234",
            Location = new Location("Mortselhaar", 154, "Antwerp", "2640", "Belgium")
        };

        _patientRepositoryMock
            .Setup(r => r.ReadById(patientId))
            .ReturnsAsync(patient);

        _mapperMock
            .Setup(m => m.Map<PatientDto>(patient))
            .Returns(patientDto);

        // Act
        var result = await _patientManager.GetPatientById(patientId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(patientId, result.Id);
        Assert.Equal("PatientFirst", result.FullName.FirstName);
    }
    
    [Fact]
    public async Task GetAllPatients_ReturnsMappedPatientDtos()
    {
        // Arrange
        var patient1Id = Guid.NewGuid();
        var patient2Id = Guid.NewGuid();
        var patients = new List<Patient>
        {
            new Patient(
                new Name("John", "Doe"),
                new DateOnly(1990, 1, 1),
                "john@mail.com",
                "555-111-1234",
                new Location("Street", 1, "City", "1000", "Belgium"),
                patient1Id
            ),
            new Patient(
                new Name("Jane", "Smith"),
                new DateOnly(1985, 5, 5),
                "jane@mail.com",
                "555-222-1598",
                new Location("Road", 2, "Town", "2000", "Belgium"),
                patient2Id
            )
        };

        var dtos = new List<PatientDto>
        {
            new PatientDto {
                Id = patient1Id,
                FullName =  new Name("John", "Doe"),
                DateOfBirth = new DateOnly(1990, 1, 1),
                Email = "john@mail.com",
                PhoneNumber = "555-111-1234",
                Location = new Location("Street", 1, "City", "1000", "Belgium")
            },
            new PatientDto { 
                Id = patient2Id,
                FullName =  new Name("Jane", "Smith"),
                DateOfBirth = new DateOnly(1985, 5, 5),
                Email = "jane@mail.com",
                PhoneNumber = "555-222-1598",
                Location = new Location("Road", 2, "Town", "2000", "Belgium")
            }
        };

        _patientRepositoryMock
            .Setup(r => r.ReadAll())
            .ReturnsAsync(patients);

        _mapperMock
            .Setup(m => m.Map<IEnumerable<PatientDto>>(patients))
            .Returns(dtos);

        // Act
        var result = await _patientManager.GetAllPatients();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Equal("John", result.First().FullName.FirstName);
    }

    [Fact]
    public async Task AddPatient_ValidPatient_AddsAndReturnsPatient()
    {
        // Arrange
        var addDto = new AddPatientDto
        {
           FullName = new Name("Alice", "Brown"),
            DateOfBirth = new DateOnly(1995, 3, 3),
            Email = "alice@mail.com",
            PhoneNumber = "555-333-1784",
            Location = new Location("Lane", 3, "Village", "3000", "Belgium")
        };

        var patient = new Patient(
            new Name("Alice", "Brown"),
            new DateOnly(1995, 3, 3),
            "alice@mail.com",
            "555-333-1784",
            new Location("Lane", 3, "Village", "3000", "Belgium"),
            Guid.NewGuid()
        );

        _mapperMock
            .Setup(m => m.Map<Patient>(addDto))
            .Returns(patient);

        _validationMock
            .Setup(v => v.Validate(patient))
            .Returns(Enumerable.Empty<System.ComponentModel.DataAnnotations.ValidationResult>());

        _patientRepositoryMock
            .Setup(r => r.Create(patient))
            .ReturnsAsync(patient);

        // Act
        var result = await _patientManager.AddPatient(addDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(patient, result);

        _mapperMock.Verify(m => m.Map<Patient>(addDto), Times.Once);
        _validationMock.Verify(v => v.Validate(patient), Times.Once);
        _patientRepositoryMock.Verify(r => r.Create(patient), Times.Once);
    }

    
    [Fact]
    public void DeletePatient_ExistingId_RemovesPatient()
    {
        // Arrange
        var patientId = Guid.NewGuid();

        _patientRepositoryMock
            .Setup(r => r.Delete(patientId));

        // Act
         _patientManager.RemovePatient(patientId);

        // Assert
        _patientRepositoryMock.Verify(r => r.Delete(patientId), Times.Once);
    }

    
}