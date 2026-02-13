using System.Linq.Expressions;
using Core.Entities;

namespace Core.Specifications;

public class CertificateByReferenceSpecification : BaseSpecification<BarangayCertificate>
{
    public CertificateByReferenceSpecification(string referenceNumber) 
        : base(x => x.ReferenceNumber == referenceNumber)
    {
    }

    protected CertificateByReferenceSpecification()
    {
    }
}
