using BL.hospital.dto;
using Domain.hospital;

namespace BL.hospital;

public interface IPatientManager : IBaseManager<Patient, PatientDto, AddPatientDto>
{
    Task<IEnumerable<PatientDto>> Search(string? term, CancellationToken cancellationToken = default);

}