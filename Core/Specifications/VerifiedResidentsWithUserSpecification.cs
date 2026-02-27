using System.Linq.Expressions;
using Core.Entities;

namespace Core.Specifications;

public class VerifiedResidentsWithUserSpecification : BaseSpecification<Resident>
{
    public VerifiedResidentsWithUserSpecification() : base(r => r.AppUser != null && r.AppUser.IsIdVerified)
    {
        AddInclude(r => r.AppUser!);
        AddOrderBy(r => r.LastName);
    }

}
