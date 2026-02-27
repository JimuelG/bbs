using System;
using Core.Entities;
using Infrastructure.Services.Interface;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Infrastructure.PdfLayouts;

public class IndegencyLayout : ICertificateLayout
{
    public void Compose(IDocumentContainer container, BarangayCertificate certificate, string logoFolder, IReadOnlyList<BarangayOfficial> officials)
    {
        container.Page(page =>
        {
            SharedLayout.ApplyBaseSetting(page);

            page.Header().Element(c => SharedLayout.ComposeHeader(c, logoFolder));

            page.Content().PaddingTop(30).Column(col =>
            {
                col.Item().AlignCenter().PaddingBottom(30).Text("BARANGAY INDIGENCY")
                    .FontSize(16).Bold().FontFamily(Fonts.TimesNewRoman);
                col.Item().PaddingBottom(10).Text("To Whom It May concern:");

                col.Item().PaddingBottom(10).Text(text =>
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
                    text.Span($"Purok {certificate.Purok} Barangay Guevara, Lapaz, Tarlac.").Bold();
                });

                col.Item().PaddingBottom(10).Text(text =>
                {
                    text.Justify();
                    
                    text.Span("\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0");
                    text.Span("Further ");
                    text.Span("CERTIFY ").Bold();
                    text.Span("that he/ she known to me of good moral character and is a law abiding citizen. He/ She has no pending case nor derogatory record in this Barangay.");
                });

                col.Item().PaddingBottom(10).Text(text =>
                {
                    text.Justify();

                    text.Span("\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0");
                    text.Span("This certification is being issued upon request of the above- mentioned name for ");
                    text.Span($"{certificate.Purpose.ToUpper()}.").Bold();
                });

                col.Item().PaddingBottom(10).Text(text =>
                {
                    text.Justify();

                    text.Span("\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0");
                    text.Span("We are very grateful and thankful for the help you extend to her/his family.");
                });

                col.Item().PaddingTop(15).Text(text =>
                {
                    text.Justify();

                    text.Span("\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0");
                    text.Span("ISSUED this ");
                    text.Span(GetDayWithSuffix(certificate.IssuedAt.Day) + " day").Bold();
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
