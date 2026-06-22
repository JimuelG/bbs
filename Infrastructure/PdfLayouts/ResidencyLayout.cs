using System.Drawing;
using Core.Entities;
using Infrastructure.Services.Interface;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Infrastructure.PdfLayouts;

public class ResidencyLayout : ICertificateLayout
{
    public void Compose(IDocumentContainer container, BarangayCertificate certificate, string logoFolder, IReadOnlyList<BarangayOfficial> officials)
    {
        container.Page(page =>
        {
            SharedLayout.ApplyBaseSetting(page);

            page.Header().Element(c => SharedLayout.ComposeHeader(c, logoFolder));

            page.Content().PaddingTop(30).Column(col =>
            {
                col.Item().AlignCenter().PaddingBottom(30).Text("CERTIFICATE OF RESIDENCY")
                    .FontSize(16).Bold().FontFamily(Fonts.TimesNewRoman);
                col.Item().PaddingBottom(10).Text("To Whom It May concern:");
                col.Item().PaddingTop(10).Text(text =>
                {
                    text.Justify();

                    text.Span("\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0");
                    text.Span("This is to certify that ");
                    text.Span(certificate.FullName.ToUpper()).Bold();
                    text.Span($", of legal age, born on ");
                    text.Span($"{certificate.BirthDate:MMMM dd, yyyy}").Bold();
                    text.Span($", ");
                    text.Span($"{certificate.CivilStatus.ToLower()}").Bold();
                    text.Span(", is a bona fide resident of ");
                    text.Span($"{certificate.Purok} Barangay Guevara, Lapaz, Tarlac.").Bold();

                });

                col.Item().PaddingTop(15).Text(text =>
                {
                    text.Justify();

                    text.Span("\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0");
                    text.Span("Further certifies that the above-named persion has been staying in this barangay since ");
                    text.Span(certificate.StayDuration ?? " birth").Bold();
                    text.Span("."); 
                });

                col.Item().PaddingTop(15).Text(text =>
                {
                    text.Justify();
                    
                    text.Span("\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0");
                    text.Span("This certification is issued upon request of above-named for ");
                    text.Span(certificate.Purpose).Bold();
                    text.Span(" and for whatever legal purpose it may serve.");
                });

                var issuedDate = certificate.IssuedAt 
                    ?? throw new Exception("Issued date is required before generating the certificate PDF.");

                col.Item().PaddingTop(15).Text(text =>
                {
                    text.Justify();

                    text.Span("\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0");
                    text.Span("ISSUED this ");
                    text.Span(GetDayWithSuffix(issuedDate.Day) + " day").Bold();
                    text.Span(" of ");
                    text.Span($"{certificate.IssuedAt:MMMM yyyy}").Bold();
                    text.Span(", here at Barangay Guevara, La Paz, Tarlac.");
                });

                col.Item().Element(c => SharedLayout.ComposeSignatures(c, officials));
            });

            page.Footer().Element(c => SharedLayout.ComposeFooter(c));
        });
    }

    private static string GetDayWithSuffix(int day)
    {
        if (day <= 0) return day.ToString();

        return (day % 100) switch
        {
           11 or 12 or 13 => $"{day}th",
           _ =>  (day % 10) switch
           {
               1 => $"{day}st",
               2 => $"{day}nd",
               3 => $"{day}rd",
               _ => $"{day}th"
           }
        };
    }
}
