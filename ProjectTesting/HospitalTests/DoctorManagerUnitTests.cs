using AutoMapper;
using BL.hospital;
using BL.hospital.dto;
using BL.hospital.validation;
using DAL.Repository.hospital;
using Domain.hospital;
using Domain.hospital.types;
using Moq;

namespace ProjectTesting.HospitalTests;

public class DoctorManagerUnitTests
{
    private readonly Mock<IBaseRepository<Doctor>> _repository;
    private readonly DoctorManager _doctorManager;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IValidation<Doctor>> _validationMock;

    public DoctorManagerUnitTests()
    {
        _repository = new Mock<IBaseRepository<Doctor>>();
        _mapperMock = new Mock<IMapper>();
        _validationMock = new Mock<IValidation<Doctor>>();
        
        _doctorManager = new DoctorManager(
            _repository.Object,
            _mapperMock.Object,
            _validationMock.Object
        );
    }

    
    [Fact]
    public async Task GetDoctorById_ReturnsDoctorDto_WhenDoctorExists()
    {
        var doctorId = Guid.NewGuid();
        var doctor = new Doctor(new Name("Dr.w John", "Doe"),
            Specialisation.Cardiology,
            new Location("Mortselhaar", 154, "Antwerp", "2640", "Belgium"),
            doctorId);
        
        var doctorDto = new DoctorDto
        {
            Id = doctorId,
            FullName = new Name("Dr.w John", "Doe"),
            Specialisation = Specialisation.Cardiology,
            WorkAddress = new Location("Mortselhaar", 154, "Antwerp", "2640", "Belgium")
        };
        
        _repository.Setup(r =>r.ReadById(doctorId)).ReturnsAsync(doctor);
        
        _mapperMock.Setup(m => m.Map<DoctorDto>(doctor))
            .Returns(doctorDto);

        var result = await _doctorManager.GetById(doctorId);
        Assert.NotNull(result);
        Assert.Equal(doctorId, result.Id);
        Assert.Equal("Dr.w John", result.FullName.FirstName);
        Assert.Equal("Doe", result.FullName.LastName);
    }
    
    [Fact]
    public async Task GetAllDoctors_ReturnListOfDoctorDtos()
    {
        var doctor1 = new Doctor(new Name("Dr.w John", "Doe"),
            Specialisation.Cardiology,
            new Location("Mortselhaar", 154, "Antwerp", "2640", "Belgium"),
            Guid.NewGuid());
        
        var doctor2 = new Doctor(new Name("Dr.w Jane", "Smith"),
            Specialisation.Neurology,
            new Location("Baker Street", 221, "London", "NW1", "UK"),
            Guid.NewGuid());
        
        var doctors = new List<Doctor> { doctor1, doctor2 };
        
        _repository.Setup(r => r.ReadAll()).ReturnsAsync(doctors);
        
        var doctorDtos = new List<DoctorDto>
        {
            new DoctorDto
            {
                Id = doctor1.Id,
                FullName = doctor1.FullName,
                Specialisation = doctor1.Specialisation,
                WorkAddress = doctor1.WorkAddress
            },
            new DoctorDto
            {
                Id = doctor2.Id,
                FullName = doctor2.FullName,
                Specialisation = doctor2.Specialisation,
                WorkAddress = doctor2.WorkAddress
            }
        };
        
        _mapperMock.Setup(m => m.Map<IEnumerable<DoctorDto>>(doctors))
            .Returns(doctorDtos);
        
        var result = await _doctorManager.GetAll();
        
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }
    
    
    [Fact]
    public async Task AddDoctor_ValidDoctor_ReturnsCreatedDoctor()
    {
        var addDoctorDto = new AddDoctorDto
        {
            FullName = new Name("Dr.w Emily", "Clark"),
            Specialisation = Specialisation.Pediatrics,
            WorkAddress = new Location("Elm Street", 13, "Springwood", "1234", "USA")
        };
        
        var doctor = new Doctor(
            addDoctorDto.FullName,
            addDoctorDto.Specialisation,
            addDoctorDto.WorkAddress,
            Guid.NewGuid()
        );
        
        _mapperMock.Setup(m => m.Map<Doctor>(addDoctorDto))
            .Returns(doctor);
        
        _validationMock.Setup(v => v.Validate(doctor))
            .Returns(new List<System.ComponentModel.DataAnnotations.ValidationResult>());
        
        _repository.Setup(r => r.Create(doctor))
            .ReturnsAsync(doctor);
        
        var result = await _doctorManager.Add(addDoctorDto);
        
        Assert.NotNull(result);
        Assert.Equal("Dr.w Emily", result.FullName.FirstName);
        Assert.Equal("Clark", result.FullName.LastName);
    }

    [Fact]
    public void RemoveDoctor_ValidId_CallsRepositoryDelete()
    {
        var doctorId = Guid.NewGuid();

        _repository.Setup(r => r.Delete(doctorId));
        
        _doctorManager.Remove(doctorId);
        
        _repository.Verify(r => r.Delete(doctorId), Times.Once);
    }
    
}