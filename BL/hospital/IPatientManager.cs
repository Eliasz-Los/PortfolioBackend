using BL.hospital.dto;

namespace BL.hospital;

public interface IPatientManager
{
    Task<AddPatientDto>  AddPatient(AddPatientDto patient);
    Task DeletePatient(Guid patientId);
}