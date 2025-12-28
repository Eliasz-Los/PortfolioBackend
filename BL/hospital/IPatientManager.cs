using BL.hospital.dto;
using Domain.hospital;

namespace BL.hospital;

public interface IPatientManager
{
    Task<PatientDto> GetPatientById(Guid id);
    Task<IEnumerable<PatientDto>> GetAllPatients();
    Task<Patient> AddPatient(AddPatientDto patient);
    void RemovePatient(Guid id);
}