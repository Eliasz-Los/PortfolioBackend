using Domain.hospital.types;

namespace BL.hospital.dto;

public class AddDoctorDto
{
    public Name FullName { get; set; }
    public Specialisation Specialisation { get; set; }
    public Location WorkAddress { get; set; }
}