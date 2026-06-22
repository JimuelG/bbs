using System.Linq.Expressions;
using Core.Entities;

namespace Core.Specifications;

public class CertificateCOuntSpecification : BaseSpecification<BarangayCertificate>
{
    public CertificateCOuntSpecification(CertificateSpecParams specParams) 
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
                x.Status == specParams.Status
            )
        )
    {
    }

    protected CertificateCOuntSpecification()
    {
    }
}