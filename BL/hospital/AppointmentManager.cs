using System.ComponentModel.DataAnnotations;
using AutoMapper;
using BL.hospital.dto;
using BL.hospital.validation;
using DAL.Repository.hospital;
using Domain.hospital;

namespace BL.hospital;

public class AppointmentManager : IBaseManager<Appointment, Appointment, AddAppointmentDto>, IAppointmentManager
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
        Appointment newAppointment = new Appointment
        {
            Id = Guid.NewGuid(),
            AppointmentDate = appointment.AppointmentDate.ToUniversalTime(),
            PatientId = appointment.PatientId,
            DoctorId = appointment.DoctorId
        };
        
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

    // Perhaps split this method into more reusable parts in the future
    public async Task<IEnumerable<DoctorAvailabilityDto>> GetDoctorAvailability(
        Guid doctorId,
        DateOnly from,
        DateOnly to)
    {
        var fromUtc = new DateTime(from.Year, from.Month, from.Day, 0, 0, 0, DateTimeKind.Utc);
        var toUtc = new DateTime(to.Year, to.Month, to.Day, 23, 59, 59, DateTimeKind.Utc);
        
        var appointments = await _appointmentRepository
            .ReadAppointmentsForDoctorInDateRange(
                doctorId,
                fromUtc,
                toUtc
            );

        Console.WriteLine($"Appointments count: {appointments.Count()}");
        foreach (var a in appointments)
        {
            Console.WriteLine($"Appointment: {a.AppointmentDate:O} Doctor: {a.Doctor.Id}");
        }

        var normalized = appointments.Select(a =>
        {
            var local = DateTime.SpecifyKind(a.AppointmentDate, DateTimeKind.Utc).ToLocalTime();
            var roundedHour = (local.Minute > 0) ? local.Hour + 1 : local.Hour;
            roundedHour = Math.Min(roundedHour, HospitalConstants.WorkingHoursEnd - 1);

            return new
            {
                Date = DateOnly.FromDateTime(local),
                Hour = roundedHour
            };
        });
        
        var grouped = normalized.GroupBy(a => a.Date);

        var result = new List<DoctorAvailabilityDto>();

        for (var date = from; date <= to; date = date.AddDays(1))
        {
            if (date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
                continue;

            var allHours = Enumerable
                .Range(HospitalConstants.WorkingHoursStart, HospitalConstants.WorkingHoursEnd)
                .ToList();

            var takenHours = grouped
                .FirstOrDefault(g => g.Key == date)?
                .Select(a => a.Hour)
                .Distinct()
                .ToList() ?? new List<int>();
            Console.WriteLine($"Date: {date}, TakenHours: {string.Join(",", takenHours)}");
            foreach (var g in grouped)
            {
                Console.WriteLine($"Group Date: {g.Key}, Hours: {string.Join(",", g.Select(a => a.Hour))}");
            }

            var availableHours = allHours
                .Except(takenHours)
                .ToList();
            result.Add(new DoctorAvailabilityDto
            {
                Date = date,
                AvailableHours = availableHours,
                TakenHours = takenHours
            });
        }

        return result;
    }
}