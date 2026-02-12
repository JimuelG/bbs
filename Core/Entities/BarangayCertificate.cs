using Core.Enums;

namespace Core.Entities;

public class BarangayCertificate : BaseEntity
{
    public string ReferenceNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public CertificateType CertificateType { get; set; }
    public string Purpose { get; set; } = string.Empty;
    public DateTime IssuedAt { get; set; }
    public string IssuedBy { get; set; } = string.Empty;
    public decimal? Fee { get; set; }
    public bool IsPaid { get; set; }
}
