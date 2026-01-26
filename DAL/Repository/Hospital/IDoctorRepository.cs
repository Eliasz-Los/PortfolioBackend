using Domain.hospital;

namespace DAL.Repository.hospital;

public interface IDoctorRepository
{
    Task<IEnumerable<Doctor>> SearchDoctorsByFullNameOrSpecialisation(string? term,CancellationToken cancellationToken);

    
}