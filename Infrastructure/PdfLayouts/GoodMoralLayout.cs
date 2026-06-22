using Core.Entities;
using Infrastructure.Services.Interface;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Infrastructure.PdfLayouts;
public class GoodMoralLayout : ICertificateLayout
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

                col.Item().PaddingTop(45).AlignCenter().Text("Certificate of Good Moral")
                    .FontSize(16);

                col.Item().PaddingTop(35).Text("To whom it may concern,");

                col.Item().PaddingTop(18).Text(text =>
                {
                    text.Justify();

                    text.Span("This is to certify that ");
                    text.Span(certificate.FullName.ToUpper()).Bold();
                    text.Span(", of legal age, ");
                    text.Span(GetValue(certificate.CivilStatus, "single").ToLower());
                    text.Span(", is a bonafide residence of ");
                    text.Span($"{GetValue(certificate.Purok, "Purok")} Barangay Guevara, La Paz, Tarlac.");
                });

                col.Item().PaddingTop(18).Text(text =>
                {
                    text.Justify();

                    text.Span("\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0");
                    text.Span("Further certifies that the above-named person is law abiding citizen and has a good moral character. ");
                    text.Span("Records of this barangay shows that he/she ");
                    text.Span("has not committed nor been involved in any kind of unlawful activities in this barangay.");
                });

                col.Item().PaddingTop(18).Text(text =>
                {
                    text.Justify();

                    text.Span("\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0");
                    text.Span("This certification is being issued upon the request of the above name for ");
                    text.Span(GetValue(certificate.Purpose, "whatever legal purpose"));
                    text.Span(" purpose/s.");
                });

                var issuedDate = certificate.IssuedAt
                    ?? throw new Exception("Issued date is required before generating the certificate PDF.");

                col.Item().PaddingTop(18).Text(text =>
                {
                    text.Justify();

                    text.Span("\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0");
                    text.Span("ISSUED this ");
                    text.Span(GetDayWithSuffix(issuedDate.Day)).Bold();
                    text.Span(" day of ");
                    text.Span($"{issuedDate:MMMM yyyy}").Bold();
                    text.Span(", here at Barangay Guevara, La Paz, Tarlac.");
                });

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