namespace BL.hospital.dto;

public class DoctorAvailabilityDto
{
    public DateOnly Date { get; set; }
    public List<int> AvailableHours { get; set; }
    public List<int> TakenHours { get; set; }
}