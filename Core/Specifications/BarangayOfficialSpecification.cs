using System.Linq.Expressions;
using Core.Entities;

namespace Core.Specifications;

public class BarangayOfficialSpecification : BaseSpecification<BarangayOfficial>
{
    public BarangayOfficialSpecification(BarangayOfficialSpecParams specParams) 
        : base(x =>
            string.IsNullOrEmpty(specParams.Search) ||
            x.FirstName.ToLower().Contains(specParams.Search) ||
            x.LastName.ToLower().Contains(specParams.Search) ||
            x.Position.ToLower().Contains(specParams.Search)
        )
    {
        ApplyPaging(specParams.PageSize * (specParams.PageIndex - 1), specParams.PageSize);
    }

    protected BarangayOfficialSpecification()
    {
    }
}