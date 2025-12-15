using Domain.hospital.types;

namespace BL.hospital.dto;

public class AddPatientDto
{
    public Name FullName { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public Location Location { get; set; }
}