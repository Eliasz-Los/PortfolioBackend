using Domain.hospital;

namespace DAL.Repository.hospital;

public interface IPatientRepository
{
    Task<IEnumerable<Patient>> SearchPatientsByFullNameOrDateOfBirth(string? term,CancellationToken cancellationToken);
}