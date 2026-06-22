using System.Linq.Expressions;
using Core.Entities;

namespace Core.Specifications
{
    public class ResidentSpecification : BaseSpecification<Resident>
    {
        public ResidentSpecification(ResidentSpecParams specParams) 
            : base(x =>
                string.IsNullOrEmpty(specParams.Search) ||
                x.FirstName.ToLower().Contains(specParams.Search) ||
                x.LastName.ToLower().Contains(specParams.Search) ||
                x.Purok.ToLower().Contains(specParams.Search)
            )
        {
            ApplyPaging(specParams.PageSize * (specParams.PageIndex - 1), specParams.PageSize);
        }

        protected ResidentSpecification()
        {
        }
    }
}