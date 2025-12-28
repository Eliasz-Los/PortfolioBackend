using System.ComponentModel.DataAnnotations;
using Domain.hospital.types;

namespace Domain.hospital;

public class Patient : BaseEntity, IValidatableObject
{
    public Guid Id { get; set; }
    public Name FullName { get; set; }
    public DateOnly DateOfBirth { get; set; }
    [Required]
    [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", 
        ErrorMessage = "Email must contain '@' and a '.' after it. ex: jhon@gmail.com")]
    public string Email { get; set; }
    [Required]
    [Phone(ErrorMessage = "Invalid phone number format.")]
    public string PhoneNumber { get; set; }
    public Location Location { get; set; }
    
    public ICollection<Appointment> Appointments { get; set; }
    public ICollection<Invoice> Invoices { get; set; }
    
    public Patient(Name fullName, DateOnly dateOfBirth, string email, string phoneNumber, Location location, Guid id)
    {
        FullName = fullName;
        DateOfBirth = dateOfBirth;
        Email = email;
        PhoneNumber = phoneNumber;
        Location = location;
        Id = id;
    }

    public Patient() {}

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        List<ValidationResult> validationResults = new List<ValidationResult>();
        if (DateOfBirth > DateOnly.FromDateTime(DateTime.Now))
        {
            validationResults.Add(new ValidationResult("Date of birth cannot be in the future.", new[] { nameof(DateOfBirth) }));
        }else if (DateOfBirth == default)
        {
            validationResults.Add(new ValidationResult("Date of birth cannot be empty.", new[] { nameof(DateOfBirth) }));
        }

        return validationResults;
    }
}