using System.Linq.Expressions;
using Core.Entities;

namespace Core.Specifications;

public class ConcernCountSpecification : BaseSpecification<Concern>
{
    public ConcernCountSpecification(ConcernParams specParams) 
        : base(x =>
            (
            string.IsNullOrEmpty(specParams.Search) ||
            x.Title.ToLower().Contains(specParams.Search) ||
            x.Description.ToLower().Contains(specParams.Search)
            )
            &&
            (
                !specParams.Priority.HasValue ||
                x.Priority == specParams.Priority.Value
            )
        )
    {
    }

    protected ConcernCountSpecification()
    {
    }
}