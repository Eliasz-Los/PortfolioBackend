using BL.hospital.dto;
using Domain.hospital;

namespace BL.hospital;

public interface IDoctorManager
{
    Task<DoctorDto> GetDoctorById(Guid id);
    Task<IEnumerable<DoctorDto>> GetAllDoctors();
    Task<Doctor> AddDoctor(AddDoctorDto patient);
    void RemoveDoctor(Guid id);
}