using System.ComponentModel.DataAnnotations;
using AutoMapper;
using BL.hospital.dto;
using BL.hospital.validation;
using DAL.Repository.hospital;
using Domain.hospital;

namespace BL.hospital;

public class AppointmentManager : IBaseManager<Appointment, Appointment, AddAppointmentDto>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IBaseManager<Patient, PatientDto, AddPatientDto> _patientManager;
    private readonly IBaseManager<Doctor, DoctorDto, AddDoctorDto> _doctorManager;
    private readonly IValidation<Appointment> _appointmentValidation;
    private readonly IMapper _mapper;
    private readonly IInvoiceManager _invoiceManager;
    
    public AppointmentManager(IAppointmentRepository appointmentRepository,
        IValidation<Appointment> appointmentValidation, IMapper mapper, 
        IBaseManager<Patient, PatientDto, AddPatientDto> patientManager, 
        IBaseManager<Doctor, DoctorDto, AddDoctorDto> doctorManager, IInvoiceManager invoiceManager)
    {
        _appointmentRepository = appointmentRepository;
        _appointmentValidation = appointmentValidation;
        _mapper = mapper;
        _patientManager = patientManager;
        _doctorManager = doctorManager;
        _invoiceManager = invoiceManager;
    }

    public async Task<IEnumerable<Appointment>> GetAllAppointmentsFromPatientById(Guid patientId)
    {
        return await _appointmentRepository.ReadAppointmentsByPatientId(patientId);
    }

    public async Task<IEnumerable<Appointment>> GetAllAppointmentsFromDoctorById(Guid doctorId)
    {
        return await _appointmentRepository.ReadAppointmentsByDoctorId(doctorId);
    }

    public Task<Appointment?> GetById(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Appointment>> GetAll()
    {
        throw new NotImplementedException();
    }

    public async Task<Appointment> Add(AddAppointmentDto appointment)
    {
        PatientDto? patient = await _patientManager.GetById(appointment.PatientId);
        DoctorDto? doctor = await _doctorManager.GetById(appointment.DoctorId);
        Appointment newAppointment = new Appointment(appointment.AppointmentDate, 
            _mapper.Map<Patient>(patient),
            _mapper.Map<Doctor>(doctor),
            Guid.NewGuid());
        
        var validationResults = _appointmentValidation.Validate(newAppointment).ToList();
        if (validationResults.Any())
        {
            var errors = string.Join(", ", validationResults.Select(vr => vr.ErrorMessage));
            throw new ValidationException(errors);
        }   
        
        return await _appointmentRepository.Create(newAppointment);
    }

    public void Remove(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task CompleteAppointment(Guid appointmentId)
    {
        var appointment = await _appointmentRepository.ReadAppointmentWithRelationsById(appointmentId)
            ?? throw new KeyNotFoundException("Appointment not found");
        
        appointment.MarkAsCompleted();

        var invoice = new Invoice
        {
            Id = Guid.NewGuid(),
            Patient = appointment.Patient,
            Amount = 100.00m,
            InvoiceDate = DateOnly.FromDateTime(DateTime.UtcNow),
            DueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30)),
            Title = "Medical Consultation",
            Description =
                $"Invoice for appointment on {appointment.AppointmentDate:yyyy-MM-dd} with Dr. {appointment.Doctor.FullName}",
            InvoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMddHHmmss}-{appointment.Id.ToString().Substring(0, 8)}",
            IsPaid = false
        };
        
        await _appointmentRepository.Update(appointment);
        await _invoiceManager.Add(invoice);
    }
}