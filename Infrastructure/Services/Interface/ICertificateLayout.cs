using Core.Entities;
using QuestPDF.Infrastructure;

namespace Infrastructure.Services.Interface;

public interface ICertificateLayout
{
    void Compose(
        IDocumentContainer container, 
        BarangayCertificate certificate, 
        string logoFolder,
        IReadOnlyList<BarangayOfficial> officials);
}
