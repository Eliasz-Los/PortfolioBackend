using BL.hospital.dto;
using Domain.hospital;

namespace BL.hospital;

public interface IDoctorManager : IBaseManager<Doctor, DoctorDto, AddDoctorDto>
{
    Task<IEnumerable<DoctorDto>> SearchByFullNameOrSpecialisation(string? term, CancellationToken cancellationToken = default);
}