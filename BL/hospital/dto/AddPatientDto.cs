using System.ComponentModel.DataAnnotations;
using BL.generalDto;
using Domain.hospital.types;

namespace BL.hospital.dto;

public class AddPatientDto : AddDto, IValidatableObject
{
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