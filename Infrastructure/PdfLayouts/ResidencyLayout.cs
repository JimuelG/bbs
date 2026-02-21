using Core.Entities;
using Infrastructure.Services.Interface;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Infrastructure.PdfLayouts;

public class ResidencyLayout : ICertificateLayout
{
    public void Compose(IDocumentContainer container, BarangayCertificate certificate, string logoFolder)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(50);
            page.PageColor(Colors.White);
            page.DefaultTextStyle(x => x.FontSize(11).FontFamily(Fonts.TimesNewRoman));

            page.Header().Row(row =>
            {
                var lguLogo = Path.Combine(logoFolder, "Lapaz_Logo.png");
                var brgyLogo = Path.Combine(logoFolder, "Barangay_Guevara_logo.png");
                var bpLogo = Path.Combine(logoFolder, "Bagong_Pilipinas_Logo.png");

                // Use ConstantColumn instead of ConstantItem
                if (File.Exists(brgyLogo)) row.ConstantItem(70).Image(brgyLogo);

                row.RelativeItem().Column(col =>
                {
                    col.Item().AlignCenter().Text("Republic of the Philippines").FontSize(10);
                    col.Item().AlignCenter().Text("Province of Tarlac").FontSize(10);
                    col.Item().AlignCenter().Text("Municipality of La Paz").FontSize(10);
                    col.Item().AlignCenter().Text("BARANGAY GUEVARA").FontSize(12).Bold();
                    col.Item().AlignCenter().PaddingTop(2).Text("OFFICE OF THE PUNONG BARANGAY").FontSize(11).Bold().FontColor(Colors.Red.Medium);
                });

                if (File.Exists(lguLogo)) row.ConstantItem(70).PaddingRight(5).Image(lguLogo);
                if (File.Exists(bpLogo)) row.ConstantItem(70).Image(bpLogo);
            });

            page.Content().PaddingTop(30).Column(col =>
            {
               col.Item().AlignCenter().PaddingBottom(30).Text("CERTIFICATE OF RESIDENCY")
                .FontSize(18).ExtraBold().Underline();
                col.Item().PaddingBottom(10).Text("To Whom It May concern:").Italic();
                col.Item().PaddingTop(10).Text(text =>
                {
                   text.Span("This is to certify that ");
                   text.Span(certificate.FullName.ToUpper()).Bold();
                   text.Span($", of legal age, born on {certificate.BirthDate:MMMM dd, yyyy}, {certificate.CivilStatus},");
                   text.Span("is a bona fide resident of ");
                   text.Span($"Purok {certificate.Purok} Barangay Guevara, Lapaz, Tarlac").Bold(); 
                });

                col.Item().PaddingTop(15).Text(text =>
                {
                   text.Span("Further certifies that the above-named persion has been staying in this barangay since");
                   text.Span(certificate.StayDuration ?? "birth").Bold();
                   text.Span("."); 
                });

                col.Item().PaddingTop(15).Text(text =>
                {
                   text.Span("This certification is issued upon request of above-named for ");
                   text.Span(certificate.Purpose).Bold();
                   text.Span(" and for whatever legal purpose it may serve.");
                });
                col.Item().PaddingTop(60).Row(row =>
                {
                   row.RelativeItem().Column(sigCol =>
                   {
                       sigCol.Item().Text("Prepared/Verified by:").FontSize(10);
                       sigCol.Item().PaddingTop(25).Text("ABEGAIL N. VALDOZ").Bold().Underline();
                       sigCol.Item().Text("Barangay Secretary").FontSize(10);
                   });

                   row.RelativeItem().AlignRight().Column(sigCol =>
                   {
                       sigCol.Item().PaddingTop(32).AlignCenter().Text("HON. CARLITO R.MARIANO").Bold().Underline();
                       sigCol.Item().AlignCenter().PaddingRight(15).Text("Punong Barangay").FontSize(10);
                   });
                });
            });

            page.Footer().PaddingBottom(10).Row(row =>
            {
                var nationalLogoPath = Path.Combine(logoFolder, "Bagong_Pilipinas.png");
                if (File.Exists(nationalLogoPath)) row.ConstantItem(50).AlignLeft().Image(nationalLogoPath);

                row.RelativeItem().AlignRight().AlignBottom().Text("Note: Not valid without the official dry seal").FontSize(8);
            });
        });
    }
}
