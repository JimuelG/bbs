
using Core.Entities;
using Core.Interfaces;
using Microsoft.Extensions.Configuration;
using QRCoder;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace Infrastructure.Services;

public class CertificatePdfService : ICertificatePdfService
{
    private readonly string _pdfFolder;
    private readonly string _baseUrl;
    public CertificatePdfService(IConfiguration config)
    {
        _pdfFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "certificates");

        if (!Directory.Exists(_pdfFolder)) Directory.CreateDirectory(_pdfFolder);
        
        _baseUrl = config["AppSettings:BaseUrl"] ?? "https://localhost:5001";

    }

    public async Task<string> GenerateCertificatePdfAsync(BarangayCertificate certificate)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        
        var fileName = $"{certificate.ReferenceNumber}.pdf";
        var filePath = Path.Combine(_pdfFolder, fileName);
        var signaturePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "signatures", "current.png");

        var verificationUrl = $"{_baseUrl}/api/certificates/verify/{certificate.ReferenceNumber}";

        using var qrGeneratior = new QRCodeGenerator();
        using var qrData = qrGeneratior.CreateQrCode(verificationUrl, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new PngByteQRCode(qrData);
        var qrBytes = qrCode.GetGraphic(20);

        var qrImagePath = Path.Combine(_pdfFolder, $"{certificate.ReferenceNumber}_qr.png");

        await Task.Run(() =>
        {
            Document.Create(container =>
            {
              container.Page(page =>
              {
                  page.Margin(40);
                  page.Content().Column(col =>
                  {
                    col.Spacing(10);

                    col.Item().AlignCenter().Text("Republic of the Philippines").FontSize(12);
                    col.Item().AlignCenter().Text("Province of Tarlac").FontSize(12);
                    col.Item().AlignCenter().Text("Municipality of Tarlac City").FontSize(12);
                    col.Item().AlignCenter().Text("Barangay Guevara, Lapaz").FontSize(12);

                    col.Item().PaddingTop(20).AlignCenter()
                        .Text($"BARANGAY {certificate.CertificateType.ToString().ToUpper()}")
                        .FontSize(10).Bold();

                    col.Item().PaddingTop(30).Text(text =>
                    {
                        text.Span("This is to certify that ");
                        text.Span(certificate.FullName).Bold();
                        text.Span(", of legal age, and a resident of ");
                        text.Span(certificate.Address).Bold();
                        text.Span(", is known to be of good moral character and law-abiding citizen in this barangay.");
                    });

                    col.Item().PaddingTop(15).Text($"Purpose: {certificate.Purpose}");
                    col.Item().PaddingTop(40).Text($"Issued this {certificate.IssuedAt:MMM dd, yyyy} at Barangay Guevara");

                    col.Item().PaddingTop(40).AlignRight().Column(signatureCol =>
                    {
                        if (File.Exists(signaturePath))
                        {
                            signatureCol.Item().Height(60).Image(signaturePath);
                        }

                        signatureCol.Item().LineHorizontal(1);
                        signatureCol.Item().Text("Barangay Captain").Bold();
                    });
                    col.Item().PaddingTop(30).AlignLeft().Column(qrCol =>
                    {
                        qrCol.Item().Text("Scan to verify").FontSize(10);

                        qrCol.Item().Height(80).Width(80).Image(qrBytes);

                    });
                  });

                  
              });
            }).GeneratePdf(filePath);
        });

        return $"/certificates/{fileName}";
    }
}
