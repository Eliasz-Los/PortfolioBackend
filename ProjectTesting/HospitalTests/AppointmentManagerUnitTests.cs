using System.ComponentModel.DataAnnotations;
using AutoMapper;
using BL.hospital;
using BL.hospital.dto;
using BL.hospital.validation;
using DAL.Repository.hospital;
using Domain.hospital;
using Domain.hospital.types;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace ProjectTesting.HospitalTests;

public class AppointmentManagerUnitTests
{
    private readonly Mock<IAppointmentRepository> _appointmentRepository;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IValidation<Appointment>> _validationMock;
    private readonly AppointmentManager _appointmentManager;
    private readonly Mock<IBaseManager<
        Patient, PatientDto, AddPatientDto>> _patientManager;

    private readonly Mock<IBaseManager<
        Doctor, DoctorDto, AddDoctorDto>> _doctorManager;
    private readonly Mock<IInvoiceManager> _invoiceManagerMock;

    
    public AppointmentManagerUnitTests()
    {
        _patientManager = new Mock<IBaseManager<
            Patient, PatientDto, AddPatientDto>>();

        _doctorManager = new Mock<IBaseManager<
            Doctor, DoctorDto, AddDoctorDto>>();


        _appointmentRepository = new Mock<IAppointmentRepository>();
        _mapperMock = new Mock<IMapper>();
        _validationMock = new Mock<IValidation<Appointment>>();
        _invoiceManagerMock = new Mock<IInvoiceManager>();
        
        _appointmentManager = new AppointmentManager(
            _appointmentRepository.Object,
            _validationMock.Object,
            _mapperMock.Object,
            _patientManager.Object,
            _doctorManager.Object,
            _invoiceManagerMock.Object
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



        var apppointment1 = new Appointment(DateTime.Now, patient1, patient1.Id, doctor1, doctor1.Id, Guid.NewGuid());
        var apppointment2 = new Appointment(DateTime.Now, patient2,patient2.Id, doctor2, doctor2.Id, Guid.NewGuid());

        _appointmentRepository.Setup(repo => repo.ReadAppointmentsByPatientId(patientId))
            .ReturnsAsync(new List<Appointment> { apppointment1 });
        // Act
        var result = await _appointmentManager.GetAllAppointmentsFromPatientById(patientId);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(patientId, result.First().Patient.Id);
        
    }

    [Fact]
    public async Task AddAppointment_ReturnsAddedAppointment_WhenValidAppointmentProvided()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var patient1 = new Patient(
            new Name("PatientFirst", "PatientLast"),
            new DateOnly(2000, 1, 1),
            "patient@mail.com",
            "555-000-1234",
            new Location("Mortselhaar", 154, "Antwerp", "2640", "Belgium"),
            patientId
        );

        _patientManager.Setup( pm => pm.GetById(patientId))
            .ReturnsAsync(new PatientDto
            {
                Id = patient1.Id,
                FullName = patient1.FullName,
                DateOfBirth = patient1.DateOfBirth,
                Email = patient1.Email,
                PhoneNumber = patient1.PhoneNumber,
                Location = patient1.Location
            });
        
        var doctorId = Guid.NewGuid();
        var doctor1 = new Doctor(new Name("Dr.w John", "Doe"),
            Specialisation.Cardiology,
            new Location("Mortselhaar", 154, "Antwerp", "2640", "Belgium"),
            doctorId);
        
        _doctorManager.Setup( dm => dm.GetById(doctorId))
            .ReturnsAsync(new DoctorDto
            {
                Id = doctor1.Id,
                FullName = doctor1.FullName,
                Specialisation = doctor1.Specialisation,
                WorkAddress = doctor1.WorkAddress
            });
        
        var appointmentDto = new AddAppointmentDto
        {
            AppointmentDate = DateTime.Now,
            PatientId = patientId,
            DoctorId = doctorId
        };

        // Act
        _appointmentRepository
            .Setup(ar => ar.Create(It.IsAny<Appointment>()))
            .ReturnsAsync((Appointment a) => a);
        
        var createdAppointment = await _appointmentManager.Add(appointmentDto);
        
        // Assert
        Assert.NotNull(createdAppointment);
        Assert.Equal(appointmentDto.AppointmentDate.ToUniversalTime(), createdAppointment.AppointmentDate.ToUniversalTime());

    }
    

    [Fact]
    public void Appointment_Validate_ReturnsError_WhenDateIsInThePast()
    {
        // Arrange
        var appointment = new Appointment
        {
            Id = Guid.NewGuid(),
            AppointmentDate = DateTime.UtcNow.AddDays(-1), // past date
            DoctorId = Guid.NewGuid(),
            PatientId = Guid.NewGuid()
        };

        var validationContext = new ValidationContext(appointment);

        // Act
        var results = appointment.Validate(validationContext).ToList();

        // Assert
        Assert.Single(results);
        Assert.Equal(
            "Appointment date cannot be in the past.",
            results[0].ErrorMessage
        );
        Assert.Contains(
            nameof(Appointment.AppointmentDate),
            results[0].MemberNames
        );
    }

    [Fact]
    public async Task Add_ThrowsValidationException_WhenAppointmentDateIsInThePast()
    {
        // Arrange
        var dto = new AddAppointmentDto
        {
            AppointmentDate = DateTime.UtcNow.AddDays(-1),
            PatientId = Guid.NewGuid(),
            DoctorId = Guid.NewGuid()
        };

        _validationMock
            .Setup(v => v.Validate(It.IsAny<Appointment>()))
            .Returns(new[]
            {
                new ValidationResult("Appointment date cannot be in the past.")
            });

        // Act + Assert
        var ex = await Assert.ThrowsAsync<ValidationException>(() =>
            _appointmentManager.Add(dto)
        );

        Assert.Contains("Appointment date cannot be in the past", ex.Message);
    }



    [Fact]
    public async Task CompleteAppointment_SetsAppointmentStatusToCompleted_AndCreatesAnInvoice()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();
        var patientId = Guid.NewGuid();
        var doctorId = Guid.NewGuid();

        var patient = new Patient(
            new Name("PatientSecond", "Sloan"),
            new DateOnly(1988, 5, 12),
            "sec.sloan@mail.com",
            "578-150-1224",
            new Location("Mortselhaar", 154, "Antwerp", "2640", "Belgium"),
            patientId
        );

        var doctor = new Doctor(
            new Name("Dr.w John", "Doe"),
            Specialisation.Cardiology,
            new Location("Mortselhaar", 154, "Antwerp", "2640", "Belgium"),
            doctorId
        );

        var appointment = new Appointment(DateTime.Now, patient, patient.Id, doctor, doctor.Id, appointmentId);

        _appointmentRepository
            .Setup(repo => repo.ReadAppointmentWithRelationsById(appointmentId))
            .ReturnsAsync(appointment);

        // Act
        await _appointmentManager.CompleteAppointment(appointmentId);

        // Assert
        _appointmentRepository.Verify(
            repo => repo.Update(It.Is<Appointment>(
                a => a.Status == AppointmentStatus.Completed
            )),
            Times.Once
        );

        _invoiceManagerMock.Verify(
            im => im.Add(It.Is<Invoice>(invoice =>
                invoice.Patient.Id == patientId &&
                invoice.Amount == 100.00m &&
                invoice.IsPaid == false &&
                invoice.Title == "Medical Consultation"
            )),
            Times.Once
        );
    }


}