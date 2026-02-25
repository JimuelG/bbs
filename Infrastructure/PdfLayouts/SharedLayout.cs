using Core.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace Infrastructure.PdfLayouts;

public class SharedLayout
{
    public static void ApplyBaseSetting(PageDescriptor page)
    {
        page.Size(PageSizes.Letter);
        page.Margin(50);
        page.PageColor(Colors.White);
        page.DefaultTextStyle(x => x.FontSize(12).FontFamily(Fonts.Arial));
    }

    public static void ComposeHeader(QuestPDF.Infrastructure.IContainer container, string logoFolder)
    {
        container.Column(headerCol =>
        {
           headerCol.Item().Row(row =>
           {
                var lguLogo = Path.Combine(logoFolder, "Lapaz_Logo.png");
                var brgyLogo = Path.Combine(logoFolder, "Barangay_Guevara_logo.png");
                var bpLogo = Path.Combine(logoFolder, "Bagong_Pilipinas_Logo.png");

                if (File.Exists(brgyLogo)) row.ConstantItem(70).Image(brgyLogo);

                row.RelativeItem().Column(col =>
                {
                    col.Item().AlignCenter().Text("Republic of the Philippines").FontSize(11);
                    col.Item().AlignCenter().Text("Province of Tarlac").FontSize(11);
                    col.Item().AlignCenter().Text("Municipality of La Paz").FontSize(11).Bold();
                    col.Item().AlignCenter().Text("BARANGAY GUEVARA").FontSize(11).Bold();
                });

                if (File.Exists(lguLogo)) row.ConstantItem(70).PaddingRight(5).Image(lguLogo);
                if (File.Exists(bpLogo)) row.ConstantItem(70).Image(bpLogo);
            });

            headerCol.Item().PaddingTop(10).Column(titleCol =>
            {
                titleCol.Item().LineHorizontal(1f).LineColor(Colors.Black);
                titleCol.Item().AlignCenter().Text("OFFICE OF THE PUNONG BARANGAY").FontSize(16).Bold().FontFamily(Fonts.TimesNewRoman);
                titleCol.Item().LineHorizontal(1f).LineColor(Colors.Black);
            });
        });
    }

    public static void ComposeFooter(QuestPDF.Infrastructure.IContainer container)
    {
        container.PaddingBottom(10).Row(row =>
        {
           row.RelativeItem().AlignRight().AlignBottom().Text("Note: Not valid without the official dry seal")
                .FontSize(10).FontColor(Colors.Red.Medium).FontFamily(Fonts.TimesNewRoman);
        });
    }

    public static void ComposeSignatures(QuestPDF.Infrastructure.IContainer container, IReadOnlyList<BarangayOfficial> officials)
    {
        container.PaddingTop(60).Row(row =>
        {
            row.RelativeItem().Column(sigCol =>
            {
                var secretary = officials.FirstOrDefault(x => x.Position == "Barangay Secretary");
                sigCol.Item().Text("Prepared/Verified by:").FontSize(10);

                if (!string.IsNullOrEmpty(secretary?.SignatureImage))
                {
                    var sigPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", secretary.SignatureImage.TrimStart('/'));
                    if (File.Exists(sigPath))
                    {
                        sigCol.Item().Height(40).Image(sigPath).FitHeight();
                    }
                    else
                    {
                        sigCol.Item().PaddingTop(25);
                    }
                }
                else
                {
                    sigCol.Item().PaddingTop(25);
                }
                
                sigCol.Item().PaddingTop(25).Text($"{secretary?.FirstName} {secretary?.MiddleName} {secretary?.LastName}").Bold().Underline();
                sigCol.Item().Text("Barangay Secretary").FontSize(10); 
            });
            
            row.RelativeItem().AlignRight().Column(sigCol =>
            {
                var captain = officials.FirstOrDefault(x => x.Position == "Barangay Captain");

                if (!string.IsNullOrEmpty(captain?.SignatureImage))
                {
                    var sigPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", captain.SignatureImage.TrimStart('/'));
                    if (File.Exists(sigPath))
                    {
                        sigCol.Item().AlignCenter().Height(50).Image(sigPath).FitHeight();
                    }
                    else
                    {
                        sigCol.Item().Padding(32);
                    }
                }
                else
                {
                    sigCol.Item().PaddingTop(32);
                }

                sigCol.Item().PaddingTop(32).AlignCenter().Text($"HON. {captain?.FirstName} {captain?.MiddleName} {captain?.LastName}").Bold().Underline();
                sigCol.Item().AlignCenter().PaddingRight(15).Text("Barangay Captain").FontSize(10);
            }); 
        });
    }
}
