using Domain.DocuGroup;

namespace BL.DocuGroup;

public interface IMembershipManager
{
    Task AddMembership(Membership membership);
}