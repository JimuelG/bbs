using Core.Entities;
using Infrastructure.Services.Interface;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Infrastructure.PdfLayouts;

public class SmallBusinessLayout : ICertificateLayout
{
    public void Compose(
        IDocumentContainer container,
        BarangayCertificate certificate,
        string logoFolder,
        IReadOnlyList<BarangayOfficial> officials)
    {
        container.Page(page =>
        {
            SharedLayout.ApplyBaseSetting(page);

            page.Header().Element(c => SharedLayout.ComposeHeader(c, logoFolder));


            page.Content().Column(col =>
            {

                col.Item().PaddingTop(15).AlignCenter().Text("Business Certificate")
                    .FontSize(16);

                col.Item().PaddingTop(32).Text("To whom it may concern:");

                col.Item().PaddingTop(16).Text(text =>
                {
                    text.Justify();

                    text.Span("\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0");
                    text.Span("This is to certify that ");
                    text.Span(certificate.FullName.ToUpper()).Bold();
                    text.Span(", of legal age, ");
                    text.Span(GetValue(certificate.CivilStatus, "single").ToLower()).Bold();
                    text.Span(", married, widow is a bonafide resident of ");
                    text.Span($"{GetValue(certificate.Purok, "Purok")} Barangay Guevara, La Paz, Tarlac.");
                });

                col.Item().PaddingTop(14).Text(text =>
                {
                    text.Justify();

                    text.Span("\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0");
                    text.Span("Further certifies that in addition to ");
                    text.Span("his/her").Bold();
                    text.Span(" livelihood the above-mentioned has ");
                    text.Span(GetValue(certificate.Purpose, "a small business or livelihood"));
                    text.Span(".");
                });

                col.Item().PaddingTop(14).Text(text =>
                {
                    text.Justify();

                    text.Span("\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0");
                    text.Span("This certification is being issued upon request of the above-named person for whatever legal intent it may serve.");
                });

                var issuedDate = certificate.IssuedAt
                    ?? throw new Exception("Issued date is required before generating the certificate PDF.");

                col.Item().Element(c => SharedLayout.ComposeSignatures(c, officials));
            });
            page.Footer().Element(c => SharedLayout.ComposeFooter(c));
        });
    }

    private static string GetValue(string? value, string fallback)
    {
        return string.IsNullOrWhiteSpace(value) ? fallback : value;
    }

    private static string GetDayWithSuffix(int day)
    {
        if (day <= 0) return day.ToString();

        return (day % 100) switch
        {
            11 or 12 or 13 => $"{day}th",
            _ => (day % 10) switch
            {
                1 => $"{day}st",
                2 => $"{day}nd",
                3 => $"{day}rd",
                _ => $"{day}th"
            }
        };
    }
    
}