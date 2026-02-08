using DAL.Repository.DocuGroup;
using Domain.DocuGroup;

namespace BL.DocuGroup;

public class MembershipManager : IMembershipManager
{
    private readonly IMembershipRepository _membershipRepository;

    public MembershipManager(IMembershipRepository membershipRepository)
    {
        _membershipRepository = membershipRepository;
    }

    public async Task AddMembership(Membership membership)
    {
        await _membershipRepository.CreateMembership(membership);
    }
}