using Domain.DocuGroup;

namespace DAL.Repository.DocuGroup;

public interface IMembershipRepository
{
    Task CreateMembership(Membership membership);
}