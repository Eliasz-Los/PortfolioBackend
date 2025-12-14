using Domain.hospital.types;

namespace Domain.hospital;

public class Doctor : BaseEntity
{
    public Name FullName { get; set; }
    public Specialisation Specialisation { get; set; }
    public Location WorkAddress { get; set; }
    
    public ICollection<Appointment> Appointments { get; set; }
    
    public Doctor(Name fullName, Specialisation specialisation, Location workAddress, Guid id)
    {
        FullName = fullName;
        Specialisation = specialisation;
        WorkAddress = workAddress;
        Id = id;
    }
}