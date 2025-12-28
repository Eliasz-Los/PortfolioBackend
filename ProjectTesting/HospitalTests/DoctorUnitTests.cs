using AutoMapper;
using BL.hospital;
using BL.hospital.dto;
using BL.hospital.validation;
using DAL.Repository.hospital;
using Domain.hospital;
using Domain.hospital.types;
using Moq;

namespace ProjectTesting.HospitalTests;

public class DoctorUnitTests
{
    private readonly Mock<IBaseRepository<Doctor>> _repository;
    private readonly DoctorManager _doctorManager;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IValidation<Doctor> > _validationMock;

    public DoctorUnitTests()
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

        var result = await _doctorManager.GetDoctorById(doctorId);
        Assert.NotNull(result);
        Assert.Equal(doctorId, result.Id);
        Assert.Equal("Dr.w John", result.FullName.FirstName);
        Assert.Equal("Doe", result.FullName.LastName);
    }
    
}