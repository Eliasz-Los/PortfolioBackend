using System.ComponentModel.DataAnnotations;
using Domain.hospital.types;

namespace Domain.hospital;

public class Appointment : BaseEntity, IValidatableObject
{
    public Guid Id { get; set; }
    public DateTime AppointmentDate { get; set; }
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;
    public Patient Patient { get; set; }
    public Doctor Doctor { get; set; }
    
    public Appointment(DateTime appointmentDate, Patient patient, Doctor doctor, Guid id)
    {
        AppointmentDate = appointmentDate;
        Patient = patient;
        Doctor = doctor;
        Id = id;
    }
    
    public Appointment(){}

    public void MarkAsCompleted()
    {
        if (Status != AppointmentStatus.Scheduled)
            throw new InvalidOperationException("Only scheduled appointments can be completed.");
        
        Status = AppointmentStatus.Completed;
    }
    
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        List<ValidationResult> validationResults = new List<ValidationResult>();
        if (AppointmentDate < DateTime.UtcNow)
        {
            validationResults.Add(new ValidationResult("Appointment date cannot be in the past.", new[] { nameof(AppointmentDate) }));
        }else if (AppointmentDate == default)
        {
            validationResults.Add(new ValidationResult("Appointment date cannot be empty.", new[] { nameof(AppointmentDate) }));
        }
        return validationResults;
    }
}