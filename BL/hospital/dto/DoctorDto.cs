using Domain.hospital.types;

namespace BL.hospital.dto;

public class DoctorDto
{
    public Guid Id { get; set; }
    public Name FullName { get; set; }
    public Specialisation Specialisation { get; set; }
    public Location WorkAddress { get; set; }
}