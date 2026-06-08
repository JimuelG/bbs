using System;
using System.Linq.Expressions;
using Core.Entities;

namespace Core.Specifications;

public class ConcernWithDetailsSpecification : BaseSpecification<Concern>
{
    public ConcernWithDetailsSpecification(int id) : base(x => x.Id == id)
    {
        AddInclude(x => x.Resident);
        AddInclude(x => x.AssignedOfficial!);
    }

    public ConcernWithDetailsSpecification()
    {
        AddInclude(x => x.Resident);
        AddInclude(x => x.AssignedOfficial!);
        AddOrderByDescending(x => x.DateReported);
    }

    public ConcernWithDetailsSpecification(int residentId, bool isResidentQuery) : base(x => x.ResidentId == residentId)
    {
        AddInclude(x => x.Resident);
        AddInclude(x => x.AssignedOfficial!);
        AddOrderByDescending(x => x.DateReported);
    }
}
