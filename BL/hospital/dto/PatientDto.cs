using System.ComponentModel.DataAnnotations;
using Domain.hospital.types;

namespace BL.hospital.dto;

public class PatientDto
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
}