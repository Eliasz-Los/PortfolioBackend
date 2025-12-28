namespace BL.hospital.dto;

public class AddAppointmentDto
{
    public DateTime AppointmentDate { get; set; }
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
}