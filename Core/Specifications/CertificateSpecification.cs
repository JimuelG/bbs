using System.Linq.Expressions;
using Core.Entities;

namespace Core.Specifications
{
    public class CertificateSpecification : BaseSpecification<BarangayCertificate>
    {
        public CertificateSpecification(CertificateSpecParams specParams) 
            : base(x => 
                (
                string.IsNullOrEmpty(specParams.Search) ||
                x.ReferenceNumber.ToLower().Contains(specParams.Search) ||
                x.FullName.ToLower().Contains(specParams.Search) ||
                x.Purpose.ToLower().Contains(specParams.Search)
                )
                &&
                (
                    string.IsNullOrEmpty(specParams.Status) ||
                    specParams.Status == "All" ||
                    x.Status == specParams.Status
                )
            )
        {
            switch (specParams.Sort)
            {
                case "dateAsc":
                    AddOrderBy(x => x.RequestDate);
                    break;
                
                case "nameAsc":
                    AddOrderBy(x => x.FullName);
                    break;

                case "nameDesc":
                    AddOrderByDescending(x => x.FullName);
                    break;

                case "dateDesc":
                default:
                    AddOrderByDescending(x => x.RequestDate);
                    break;
            }

            ApplyPaging(specParams.PageSize * (specParams.PageIndex -1), specParams.PageSize);
        }

        public CertificateSpecification()
        {
        }
    }
}