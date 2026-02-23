using System;
using Core.Entities;

namespace Core.Specifications;

public class ActiveBarangayOfficialsSpecifications : BaseSpecification<BarangayOfficial>
{
    public ActiveBarangayOfficialsSpecifications() : base(o => o.IsActive)
    {
        AddOrderBy(o => o.Rank);
    }

}
