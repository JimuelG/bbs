
using Core.Entities;
using Core.Enums;
using Core.Interfaces;
using Infrastructure.PdfLayouts;
using Infrastructure.Services.Interface;
using Microsoft.Extensions.Configuration;
using QRCoder;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace Infrastructure.Services;

public class CertificatePdfService : ICertificatePdfService
{
    private readonly string _pdfFolder;
    private readonly string _logoFolder;
    private readonly string _baseUrl;

    private readonly Dictionary<CertificateType, ICertificateLayout> _layouts = new()
    {
        { CertificateType.Residency, new ResidencyLayout() },
    };
    public CertificatePdfService(IConfiguration config)
    {
        _pdfFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "certificates");
        _logoFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images/logo");

        if (!Directory.Exists(_pdfFolder)) Directory.CreateDirectory(_pdfFolder);
        
        _baseUrl = config["AppSettings:BaseUrl"] ?? "https://localhost:5001";

    }

    public async Task<string> GenerateCertificatePdfAsync(BarangayCertificate certificate)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        
        var fileName = $"{certificate.ReferenceNumber}.pdf";
        var filePath = Path.Combine(_pdfFolder, fileName);
        var signaturePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "signatures", "current.png");

        if (!_layouts.TryGetValue(certificate.CertificateType, out var layout))
        {
            throw new Exception($"Layout for {certificate.CertificateType} is not yet implemented.");
        }

        await Task.Run(() =>
        {
            Document.Create(container =>
            {
                layout.Compose(container, certificate, _logoFolder);
            }).GeneratePdf(filePath);
        });

        return $"/certificates/{fileName}";
    }
}
