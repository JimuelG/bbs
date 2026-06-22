using System.Linq.Expressions;
using Core.Entities;

namespace Core.Specifications;

public class ConcernSpecification : BaseSpecification<Concern>
{
    public ConcernSpecification(ConcernParams specParams) 
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
        switch (specParams.Sort)
        {
            case "dateAsc":
                AddOrderBy(x => x.DateReported);
                break;
            
            case "priority":
                AddOrderBy(x => x.Priority);
                break;

            case "dateDesc":
            default:
                AddOrderByDescending(x => x.DateReported);
                break;
        }

        ApplyPaging(specParams.PageSize * (specParams.PageIndex -1), specParams.PageSize);
    }

    protected ConcernSpecification()
    {
    }
}