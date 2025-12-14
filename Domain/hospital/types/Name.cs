using System.ComponentModel.DataAnnotations;

namespace Domain.hospital.types;

public class Name
{
    [Required]
    [MinLength(3, ErrorMessage = "First name must be at least 3 characters long.")]
    [MaxLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
    public string FirstName { get; set; }
    [Required]
    [MinLength(3, ErrorMessage = "First name must be at least 3 characters long.")]
    [MaxLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
    public string LastName { get; set; }
    
    public Name(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }
}