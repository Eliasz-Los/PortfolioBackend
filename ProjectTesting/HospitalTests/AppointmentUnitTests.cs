using AutoMapper;
using BL.hospital;
using BL.hospital.dto;
using BL.hospital.validation;
using DAL.Repository.hospital;
using Domain.hospital;
using Domain.hospital.types;
using Moq;

namespace ProjectTesting.HospitalTests;

public class AppointmentUnitTests
{
    private readonly Mock<IAppointmentRepository> _appointmentRepository;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IValidation<Appointment>> _validationMock;
    private readonly AppointmentManager _appointmentManager;
    private readonly Mock<IBaseManager<
        Patient, PatientDto, AddPatientDto>> _patientManager;

    private readonly Mock<IBaseManager<
        Doctor, DoctorDto, AddDoctorDto>> _doctorManager;

    
    public AppointmentUnitTests()
    {
        _patientManager = new Mock<IBaseManager<
            Patient, PatientDto, AddPatientDto>>();

        _doctorManager = new Mock<IBaseManager<
            Doctor, DoctorDto, AddDoctorDto>>();


        _appointmentRepository = new Mock<IAppointmentRepository>();
        _mapperMock = new Mock<IMapper>();
        _validationMock = new Mock<IValidation<Appointment>>();
        
        _appointmentManager = new AppointmentManager(
            _appointmentRepository.Object,
            _validationMock.Object,
            _mapperMock.Object,
            _patientManager.Object,
            _doctorManager.Object
        );
    }
    
    [Fact]
    public async Task GetAppointmentsOfPatientById_ReturnsAppointments_WhenAppointmentsExist()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var patient2Id = Guid.NewGuid();
        var patient1 = new Patient(
            new Name("PatientFirst", "PatientLast"),
            new DateOnly(2000, 1, 1),
            "patient@mail.com",
            "555-000-1234",
            new Location("Mortselhaar", 154, "Antwerp", "2640", "Belgium"),
            patientId
        );
        
        var patient2 = new Patient(
            new Name("PatientSecond", "Sloan"),
            new DateOnly(1988, 5, 12),
            "sec.sloan@mail.com",
            "578-150-1224",
            new Location("Mortselhaar", 154, "Antwerp", "2640", "Belgium"),
            patient2Id
        );
        
        var doctor1 = new Doctor(new Name("Dr.w John", "Doe"),
            Specialisation.Cardiology,
            new Location("Mortselhaar", 154, "Antwerp", "2640", "Belgium"),
            Guid.NewGuid());
        
        var doctor2 = new Doctor(new Name("Dr.w Jane", "Smith"),
            Specialisation.Neurology,
            new Location("Baker Street", 221, "London", "NW1", "UK"),
            Guid.NewGuid());



        var apppointment1 = new Appointment(DateTime.Now, patient1, doctor1, Guid.NewGuid());
        var apppointment2 = new Appointment(DateTime.Now, patient2, doctor2, Guid.NewGuid());

        _appointmentRepository.Setup(repo => repo.ReadAppointmentsByPatientId(patientId))
            .ReturnsAsync(new List<Appointment> { apppointment1 });
        // Act
        var result = await _appointmentManager.GetAllAppointmentsFromPatientById(patientId);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(patientId, result.First().Patient.Id);
        
    }
    
}