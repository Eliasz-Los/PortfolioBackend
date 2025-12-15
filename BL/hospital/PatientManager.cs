using BL.hospital.dto;
using DAL.Repository.hospital;
using Domain.hospital;

namespace BL.hospital;

public class PatientManager : BaseManager<Patient>, IPatientManager
{
    public PatientManager(IBaseRepository<Patient> repository) : base(repository)
    {
    }

    public Task<AddPatientDto> AddPatient(AddPatientDto patient)
    {
        throw new NotImplementedException();
    }

    public Task DeletePatient(Guid patientId)
    {
        throw new NotImplementedException();
    }
}