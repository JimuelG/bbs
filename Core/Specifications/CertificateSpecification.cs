using System.Linq.Expressions;
using Core.Entities;

namespace Core.Specifications
{
    public class CertificateSpecification : BaseSpecification<BarangayCertificate>
    {
        public CertificateSpecification(CertificateSpecParams specParams) : base()
        {
            ApplyPaging(specParams.PageSize * (specParams.PageIndex -1), specParams.PageSize);
        }

        public CertificateSpecification()
        {
        }
    }
}