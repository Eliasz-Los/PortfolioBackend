using System.Text.Json.Serialization;
using Domain.hospital;
using Domain.hospital.types;

namespace BL.hospital.dto;

public class AppointmentDto
{
    public Guid Id { get; set; }
    public DateTime AppointmentDate { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public AppointmentStatus Status { get; set; }
    public PatientDto? Patient { get; set; }
    public DoctorDto? Doctor { get; set; }
}