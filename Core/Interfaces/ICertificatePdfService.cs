using Core.Entities;

namespace Core.Interfaces;

public interface ICertificatePdfService
{
    Task<string> GenerateCertificatePdfAsync(BarangayCertificate certificate);
    Task<byte[]> GenerateCertificatePdfBytesAsync(BarangayCertificate certificate);
}
