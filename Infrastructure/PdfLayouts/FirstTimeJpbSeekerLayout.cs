using Core.Entities;
using Infrastructure.Services.Interface;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Infrastructure.PdfLayouts;
public class FirstTimeJpbSeekerLayout : ICertificateLayout
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

                col.Item().PaddingTop(18).AlignCenter().Text("C E R T I F I C A T I O N")
                    .FontSize(16)
                    .FontFamily(Fonts.Arial);

                col.Item().PaddingTop(30).Text("To whom it may concern:");

                col.Item().PaddingTop(18).Text(text =>
                {
                    text.Justify();

                    text.Span("\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0");
                    text.Span("This is to certify that person whose name and signature appear hereunder is a ");
                    text.Span("bonafide resident of our Barangay, to wit:");
                });

                col.Item().PaddingTop(18).Row(row =>
                {
                    row.RelativeItem().Column(left =>
                    {
                        left.Item().Text(text =>
                        {
                            text.Span("Name: ");
                            text.Span(certificate.FullName.ToUpper()).Bold();
                        });

                        left.Item().Text(text =>
                        {
                            text.Span("Date of Birth: ");
                            text.Span($"{certificate.BirthDate:MMMM dd, yyyy}".ToUpper()).Bold();
                        });

                        left.Item().Text(text =>
                        {
                            text.Span("Address: ");
                            text.Span($"{certificate.Purok} Brgy. Guevara, La Paz, Tarlac").Bold();
                        });
                    });

                    row.RelativeItem().Column(right =>
                    {
                        right.Item().Text(text =>
                        {
                            text.Span("Gender: ");
                            text.Span(GetValue(certificate.Gender, "FEMALE").ToUpper()).Bold();
                        });

                        right.Item().Text(text =>
                        {
                            text.Span("Place of Birth: ");
                            text.Span(GetValue(certificate.PlaceOfBirth, "LA PAZ, TARLAC").ToUpper()).Bold();
                        });

                        right.Item().Text(text =>
                        {
                            text.Span("Civil Status: ");
                            text.Span(GetValue(certificate.CivilStatus, "SINGLE").ToUpper()).Bold();
                        });
                    });
                });

                col.Item().PaddingTop(18).Text(text =>
                {
                    text.Justify();

                    text.Span("\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0");
                    text.Span("Based on record of this office he/ she has been resident in our Barangay for ");
                    text.Span(GetValue(certificate.StayDuration, "6 years")).Bold();
                    text.Span(" (January 2020) qualified for ");
                    text.Span("Republic Act No. 11261 otherwise known as First Time Job Seekers Assistance Act.")
                        .Italic();
                });

                var issuedDate = certificate.IssuedAt
                    ?? throw new Exception("Issued date is required before generating the certificate PDF.");

                col.Item().PaddingTop(18).Text(text =>
                {
                    text.Justify();

                    text.Span("\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0");
                    text.Span("ISSUED this ");
                    text.Span(GetDayWithSuffix(issuedDate.Day) + " day").Bold();
                    text.Span(" of ");
                    text.Span($"{issuedDate:MMMM yyyy}").Bold();
                    text.Span(", here at Barangay Guevara, La Paz, Tarlac, as requested for whatever legal intent it may serve.");
                });

                col.Item().PaddingTop(52).AlignCenter().Column(sig =>
                {
                    sig.Item().AlignCenter().Text(certificate.FullName.ToUpper());

                    sig.Item().AlignCenter().Text("Grantee");
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